using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Yuzu;
using Yuzu.Binary;
using Yuzu.Metadata;
using Yuzu.Unsafe;
using YuzuGenBin;
using YuzuTestAssembly;
using YuzuTest.SampleMigrations;
using Yuzu.Migrations;

namespace YuzuTest.Migrations
{
	internal static class Helper
	{
		internal static Yuzu.Json.JsonDeserializer JD(int fromVersion = 0, int toVersion = 1) =>
			new Yuzu.Json.JsonDeserializer() {
				JsonOptions = new Yuzu.Json.JsonSerializeOptions {
					Unordered = true,
					SaveRootClass = true,
				},
				MigrationContext = new Yuzu.Migrations.MigrationContext(fromVersion,  toVersion),
			};

		internal static Yuzu.Json.JsonSerializer JS() =>
			new Yuzu.Json.JsonSerializer() {
				JsonOptions = new Yuzu.Json.JsonSerializeOptions {
					Unordered = true,
					SaveRootClass = true,
				},
			};

		internal static string TRQ(this string s) => s.Replace("'", "\"");
	}

	[TestClass]
	public class TestTrivialTypeMigrations
	{
		public class Foo
		{
			[YuzuMember]
			public Bad V;
		}
		public class Foo2
		{
			[YuzuMember]
			public Bad V { get; private set; } = new Bad();
		}
		public class Baz
		{
			[YuzuMember]
			public string B;
			[YuzuMember]
			public int V;
		}
		public class Bad
		{
			[YuzuMember]
			public int V;
			[YuzuMember]
			public string B;
		}
		[YuzuTypeMigration(0)]
		public static Bad Migrate(Baz b) => new Bad { B = b.B, V = b.V };
		[TestMethod]
		public void Test1()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigrations(typeof(TestTrivialTypeMigrations));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s = "{'V':{'class':'YuzuTest.Migrations.TestTrivialTypeMigrations+Baz, YuzuTest','B':'foo','V':111}}".TRQ();
			var jd = Helper.JD();
			var o1 = (Dictionary<string, object>)jd.FromString(s);
			Assert.AreEqual(111, ((Bad)o1["V"]).V);
			Assert.AreEqual("foo", ((Bad)o1["V"]).B);
			var o2 = jd.FromString<Foo>(s);
			Assert.AreEqual(111, o2.V.V);
			Assert.AreEqual("foo", o2.V.B);
			var o3 = (Foo)jd.FromString(new Foo(), s);
			Assert.AreEqual(111, o3.V.V);
			Assert.AreEqual("foo", o3.V.B);
			var o4 = jd.FromString<Foo2>(s);
			Assert.AreEqual(111, o4.V.V);
			Assert.AreEqual("foo", o4.V.B);
		}
	}

	[TestClass]
	public class TestMigrateRootObject
	{
		public class Foo
		{
			[YuzuMember]
			public int V;
		}
		public class Bar
		{
			[YuzuMember]
			public int V;
		}
		[YuzuTypeMigration(0)]
		public static Bar Migrate(Foo b) => new Bar { V = b.V };
		[TestMethod]
		public void Test1()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigrations(typeof(TestMigrateRootObject));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s = "{'class':'YuzuTest.Migrations.TestMigrateRootObject+Foo, YuzuTest','V':666}".TRQ();
			var jd = Helper.JD();
			var o1 = jd.FromString(s);
			Assert.AreEqual(666, ((Bar)o1).V);
			var o2 = jd.FromString<Bar>(s);
			Assert.AreEqual(666, (o2).V);
			var o3 = jd.FromString(new Bar(), s);
			Assert.AreEqual(666, ((Bar)o3).V);
		}
	}

	[TestClass]
	public class TestMigrateListItems
	{
		public class Foo
		{
			[YuzuMember]
			public List<Bar> List = new List<Bar>();
		}
		public class Foo2
		{
			[YuzuMember]
			public List<Bar> List { get; private set; } = new List<Bar>();
		}
		public class Bar
		{
			[YuzuMember]
			public int V;
		}
		public class Baz : Bar
		{
			[YuzuMember]
			public string B;
		}
		public class Bad : Bar
		{
			[YuzuMember]
			public string B;
		}
		[YuzuTypeMigration(0)]
		public static Baz Migrate(Bad bad) => new Baz {
			V = bad.V,
			B = bad.B,
		};
		[TestMethod]
		public void Test1()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigrations(typeof(TestMigrateListItems));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s = "{'class':'YuzuTest.Migrations.TestMigrateListItems+Foo, YuzuTest','List':[" +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bar, YuzuTest','V':0}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bad, YuzuTest','V':1,'B':'1'}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bar, YuzuTest','V':2}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bad, YuzuTest','V':3,'B':'3'}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bad, YuzuTest','V':4,'B':'4'}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bar, YuzuTest','V':5}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bad, YuzuTest','V':6,'B':'6'}" +
				"]}";
			s = s.TRQ();
			var jd = Helper.JD();
			var o = jd.FromString<Foo>(s);
			for (int i = 0; i < 7; i++) {
				Assert.AreEqual(i, o.List[i].V);
			}
			foreach (int i in new[] { 1, 3, 4, 6 }) {
				Assert.AreEqual(i.ToString(), ((Baz)o.List[i]).B);
			}
		}
		[TestMethod]
		public void Test2()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigrations(typeof(TestMigrateListItems));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s = "{'class':'YuzuTest.Migrations.TestMigrateListItems+Foo2, YuzuTest','List':[" +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bar, YuzuTest','V':0}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bad, YuzuTest','V':1,'B':'1'}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bar, YuzuTest','V':2}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bad, YuzuTest','V':3,'B':'3'}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bad, YuzuTest','V':4,'B':'4'}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bar, YuzuTest','V':5}," +
				"{'class':'YuzuTest.Migrations.TestMigrateListItems+Bad, YuzuTest','V':6,'B':'6'}" +
				"]}";
			s = s.TRQ();
			var jd = Helper.JD();
			var o = jd.FromString<Foo2>(s);
			for (int i = 0; i < 7; i++) {
				Assert.AreEqual(i, o.List[i].V);
			}
			foreach (int i in new[] { 1, 3, 4, 6 }) {
				Assert.AreEqual(i.ToString(), ((Baz)o.List[i]).B);
			}
		}
	}

	[TestClass]
	public class TestMigrations
	{
		[TestMethod]
		public void TestMigrateFooIntoBar()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigration(typeof(Test.MigrateFooIntoBar));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s = "{\n\t\"FooValue\":{\n\t\t\"FooS\":\"FooBar\"\n\t}\n}";
			var jd = Helper.JD();
			var o = jd.FromString<Test>(s);
			Assert.AreEqual(o.BarValue.BarS, "FooBarFooBar");
			Yuzu.Migrations.Storage.Clear();
		}

		[TestMethod]
		public void TestSimpleInput()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigration(typeof(TestSimpleIntField.MigrateTestSimpleInt_SimpleInput));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s1 = "{\n\t\"Value\": 666\n}";
			var s2 = "{}";
			var jd = Helper.JD();
			var o = jd.FromString<TestSimpleIntField>(s1);
			Console.WriteLine(s1);
			Console.WriteLine(o.Value);
			Console.WriteLine(o.ValueSetByMigration);
			Assert.AreEqual(o.Value, 666);
			Assert.AreEqual(o.ValueSetByMigration, 666 * 666 + 333);
			o = jd.FromString<TestSimpleIntField>(s2);
			Assert.AreEqual(o.Value, 0);
			Assert.AreEqual(o.ValueSetByMigration, 0 * 0 + 333);
		}


		[TestMethod]
		public void TestChangeFieldValue()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigration(typeof(TestSimpleIntField.MigrateTestSimpleInt_ChangeFieldValue));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s1 = "{\n\t\"Value\": 666\n}";
			var s2 = "{}";
			var jd = Helper.JD();
			var o = jd.FromString<TestSimpleIntField>(s1);
			Assert.AreEqual(o.Value, 666 * 666 + 333);
			Assert.AreEqual(o.ValueSetByMigration, 0);
			o = jd.FromString<TestSimpleIntField>(s2);
			Assert.AreEqual(o.Value, 0 * 0 + 333);
			Assert.AreEqual(o.ValueSetByMigration, 0);
		}

		[TestMethod]
		public void TestRenameField()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigration(typeof(TestSimpleIntField.MigrateTestSimpleInt_RenameField));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s1 = "{\n\t\"PreviousFieldName\": 666\n}";
			var s2 = "{}";
			var jd = Helper.JD();
			var o = jd.FromString<TestSimpleIntField>(s1);
			Assert.AreEqual(o.Value, 666);
			o = jd.FromString<TestSimpleIntField>(s2);
			Assert.AreEqual(o.Value, 0);
		}

		[TestMethod]
		public void TestFieldTypeRename()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigration(typeof(TestSimpleClassField.MigrateTestSimpleInt_FieldTypeRename));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s1 = "{\n\t\"Value\":\n\t{\n\t\t\"IValue\": 666,\n\t\t\"SValue\": \"FooBar\"\n\t}\n}";
			var s2 = "{}";
			var s3 = "{\n\t\"Value\":\n\t{\n\t\t\"class\":\"YuzuTest.SampleMigrations.TestSimpleClassField+Bar, YuzuTest\",\n\t\t\"IValue\": 666,\n\t\t\"SValue\": \"FooBar\"\n\t}\n}";
			var jd = Helper.JD();
			var o = jd.FromString<TestSimpleClassField>(s1);
			Assert.AreEqual(o.Value.IValue, 666);
			Assert.AreEqual(o.Value.SValue, "FooBar");
			o = jd.FromString<TestSimpleClassField>(s2);
			Assert.AreEqual(o.Value, null);
			o = jd.FromString<TestSimpleClassField>(s3);
			Assert.AreEqual(o.Value.IValue, 666);
			Assert.AreEqual(o.Value.SValue, "FooBar");
		}

		[TestMethod]
		public void TestIntermediateNullInPropertyPath()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigration(typeof(TestIntermediateNullInPropertyPath.Migration));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s = "{\n\t\"class\":\"YuzuTest.SampleMigrations.TestIntermediateNullInPropertyPath+Foo, YuzuTest\",\n\t\"Bar1\":{\n\t\t\"Moo\":null\n\t},\n\t\"Bar2\":{\n\t}\n}";
			var jd = Helper.JD();
			XAssert.Throws<InvalidOperationException>(() => jd.FromString(s), "Unable to fullfill migration input value");
		}

		[TestMethod]
		public void TestCombinedTypeAndPropertyMigrations1()
		{
			Yuzu.Migrations.Storage.Clear();
			Yuzu.Migrations.Storage.RegisterMigration(typeof(YuzuTest.SampleMigrations.CombinedMigrations.Migrations.MigrateFooProperty));
			Yuzu.Migrations.Storage.RegisterTypeMigration(typeof(YuzuTest.SampleMigrations.CombinedMigrations.Migrations).GetMethod("MigrateFoo"));
			Yuzu.Migrations.Storage.BuildMigrations();
			var s_v0 = "{\n\t\"class\":\"YuzuTest.SampleMigrations.CombinedMigrations.Tar, YuzuTest\",\n\t\"Foo\":{" +
				"\n\t\t\"class\":\"YuzuTest.SampleMigrations.CombinedMigrations.Foo, YuzuTest\",\n\t\t\"Bar\":{" +
				"\n\t\t\t\"class\":\"YuzuTest.SampleMigrations.CombinedMigrations.Bar, YuzuTest\",\n\t\t\t\"V\":666\n\t\t}\n\t}\n}";
			var s_v1 = "{\n\t\"class\":\"YuzuTest.SampleMigrations.CombinedMigrations.Tar, YuzuTest\",\n\t\"Foo\":{" +
				"\n\t\t\"class\":\"YuzuTest.SampleMigrations.CombinedMigrations.Foo, YuzuTest\",\n\t\t\"Bar2\":{" +
				"\n\t\t\t\"class\":\"YuzuTest.SampleMigrations.CombinedMigrations.Bar2, YuzuTest\",\n\t\t\t\"B\":\"666\"\n\t\t}\n\t}\n}";
			var jd = Helper.JD(fromVersion: 0, toVersion: 2);
			var o = jd.FromString<YuzuTest.SampleMigrations.CombinedMigrations.Tar>(s_v0);
			Assert.AreEqual(typeof(SampleMigrations.CombinedMigrations.Foo2), o.Foo.GetType());
			var foo = (SampleMigrations.CombinedMigrations.Foo2)o.Foo;
			Assert.AreEqual(typeof(SampleMigrations.CombinedMigrations.Bar2), foo.Bar.GetType());
			Assert.AreEqual("666", ((SampleMigrations.CombinedMigrations.Bar2)foo.Bar).B);
			jd = Helper.JD(fromVersion: 1, toVersion: 2);
			o = jd.FromString<YuzuTest.SampleMigrations.CombinedMigrations.Tar>(s_v1);
			Assert.AreEqual(typeof(SampleMigrations.CombinedMigrations.Foo2), o.Foo.GetType());
			foo = (SampleMigrations.CombinedMigrations.Foo2)o.Foo;
			Assert.AreEqual(typeof(SampleMigrations.CombinedMigrations.Bar2), foo.Bar.GetType());
			Assert.AreEqual("666", ((SampleMigrations.CombinedMigrations.Bar2)foo.Bar).B);
		}
	}
}
