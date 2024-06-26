using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Yuzu;
using Yuzu.Binary;
using Yuzu.Clone;
using Yuzu.Code;
using Yuzu.DictOfObjects;
using Yuzu.Json;
using Yuzu.Metadata;
using Yuzu.Util;

namespace YuzuTest
{
	[TestClass]
	public class TestEtc
	{

		[TestMethod]
		public void TestCodeAssignSimple()
		{
			var v1 = new Sample1 { X = 150, Y = "test" };
			var cs = new CodeAssignSerializer();
			var result1 = cs.ToString(v1);
			Assert.AreEqual("void Init(Sample1 obj) {\n\tobj.X = 150;\n\tobj.Y = \"test\";\n}\n", result1);

			var v2 = new Sample2 { X = 150, Y = "test" };
			var result2 = cs.ToString(v2);
			Assert.AreEqual("void Init(Sample2 obj) {\n\tobj.X = 150;\n\tobj.Y = \"test\";\n}\n", result2);
		}

		private void TestTypeSerializerHelper(Type t, string s) {
			Assert.AreEqual(s, TypeSerializer.Serialize(t));
			Assert.AreEqual(t, TypeSerializer.Deserialize(s));
		}

		[TestMethod]
		public void TestTypeSerializer()
		{
			TestTypeSerializerHelper(typeof(int), "System.Int32");
			TestTypeSerializerHelper(typeof(Sample1), "YuzuTest.Sample1, YuzuTest");
			TestTypeSerializerHelper(
				typeof(List<string>),
				"System.Collections.Generic.List`1[[System.String]]");
			TestTypeSerializerHelper(
				typeof(SampleInterfacedGeneric<YuzuTestAssembly.SampleAssemblyBase>),
				"YuzuTest.SampleInterfacedGeneric`1[[YuzuTestAssembly.SampleAssemblyBase, AssemblyTest]], YuzuTest");
			TestTypeSerializerHelper(
				typeof(Dictionary<Sample1, string>),
				"System.Collections.Generic.Dictionary`2[[YuzuTest.Sample1, YuzuTest],[System.String]]");
			TestTypeSerializerHelper(
				typeof(Dictionary<Sample1, List<Sample2>>),
				"System.Collections.Generic.Dictionary`2[[YuzuTest.Sample1, YuzuTest]," +
				"[System.Collections.Generic.List`1[[YuzuTest.Sample2, YuzuTest]]]]");
		}

		[TestMethod]
		public void TestMetaCollect()
		{
			var t = Meta.Collect(GetType().Assembly, MetaOptions.Default);
			Assert.IsTrue(t.Contains(typeof(Sample1)));
			Assert.IsFalse(t.Contains(typeof(SampleInterfacedGeneric<>)));
			Assert.IsTrue(t.Contains(typeof(Metadata.TestMeta.AllDefault)));
		}

		[TestMethod]
		public void TestDictObjObjects()
		{
			var src = new Sample3 { F = 7, S1 = new Sample1 { X = 3 }, S2 = null };
			var d = DictOfObjects.Pack(src);
			Assert.AreEqual(3, d.Count);
			Assert.AreEqual(src.F, d["F"]);
			Assert.AreEqual(src.S1, d["S1"]);
			Assert.AreEqual(src.S2, d["S2"]);
			var dst = DictOfObjects.Unpack<Sample3>(d);
			Assert.AreEqual(src.F, dst.F);
			Assert.AreEqual(src.S1, dst.S1);
			Assert.AreEqual(src.S2, dst.S2);
		}
	}

	public class Program
	{
		private static void Gen(string fileName, IGenerator g, Action<IGenerator> fill)
		{
			using var ms = new MemoryStream();
			using var sw = new StreamWriter(ms);
			g.GenWriter = sw;
			g.GenerateHeader();
			fill(g);
			g.GenerateFooter();
			sw.Flush();
			using var file = new FileStream(fileName, FileMode.Create);
			ms.WriteTo(file);
		}

		public static void Main()
		{
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			while (!File.Exists(Path.Combine(path, "YuzuTest.csproj"))) {
				path = Path.GetDirectoryName(path);
				if (path == null) {
					throw new InvalidOperationException("Can't find directory with YuzuTest.csproj");
				}
			}
			var jd = JsonDeserializerGenerator.Instance;
			jd.Options.TagMode = TagMode.Names;
			Gen(Path.Combine(path, "GeneratedJson.cs"), jd, g => {
				var js = g as JsonDeserializerGenerator;
				jd.Generate<Sample1>();
				jd.Generate<Sample2>();
				jd.Generate<Sample3>();
				jd.Generate<SampleEnumMemberTyped>();
				jd.JsonOptions.EnumAsString = true;
				jd.Generate<Sample4>();
				jd.Generate<SampleDecimal>();
				jd.Generate<SampleNullable>();
				jd.Generate<SampleBool>();
				jd.JsonOptions.Comments = true;
				jd.Generate<SampleList>();
				jd.JsonOptions.Comments = false;
				jd.Generate<SampleObj>();
				jd.Generate<SampleDict>();
				jd.Generate<SampleSortedDict>();
				jd.Generate<SampleSortedDictOfClass>();
				jd.Generate<SampleDictKeys>();
				jd.Generate<ISampleMember>();
				jd.Generate<SampleMemberI>();
				jd.Generate<List<ISampleMember>>();
				jd.JsonOptions.ArrayLengthPrefix = true;
				jd.Generate<SampleArray>();
				jd.JsonOptions.ArrayLengthPrefix = false;
				jd.Generate<SampleArrayOfArray>();
				jd.Generate<SampleArrayNDim>();
				jd.Generate<SampleBase>();
				jd.Generate<SampleDerivedA>();
				jd.Generate<SampleDerivedB>();
				jd.Generate<SampleMatrix>();
				jd.Generate<SamplePoint>();
				jd.Generate<SampleRect>();
				jd.Generate<SampleDate>();
				jd.Generate<SampleGuid>();
				jd.Generate<Color>();
				jd.Generate<List<List<int>>>();
				jd.Generate<SampleClassList>();
				jd.Generate<SampleSmallTypes>();
				jd.Generate<SampleWithNullFieldCompact>();
				jd.Generate<SampleNested.NestedClass>();
				jd.Generate<SampleNested>();
				jd.Options.TagMode = TagMode.Aliases;
				jd.Generate<SamplePerson>();
				jd.Generate<ISample>();
				jd.Generate<SampleInterfaced>();
				jd.Generate<SampleInterfaceField>();
				jd.Generate<SampleInterfacedGeneric<string>>();
				jd.Generate<SampleAbstract>();
				jd.Generate<SampleConcrete>();
				jd.Generate<SampleCollection<int>>();
				jd.Generate<SampleExplicitCollection<int>>();
				jd.Generate<SampleWithCollection>();
				jd.Generate<SampleConcreteCollection>();
				jd.Generate<SampleAfter2>();
				jd.Generate<SampleAfterSerialization>();
				jd.Generate<SampleBeforeDeserialization>();
				jd.Generate<SampleMerge>();
				jd.Generate<SampleAssemblyDerivedR>();
				jd.Generate<SampleAliasMany>();
				jd.Generate<SamplePrivateConstructor>();
				jd.Generate<List<YuzuTestAssembly.SampleAssemblyBase>>();
				jd.Generate<YuzuTestAssembly.SampleAssemblyBase>();
				jd.Generate<YuzuTestAssembly.SampleAssemblyDerivedQ>();
				jd.Generate<YuzuTest2.SampleNamespace>();
				jd.Generate<A.B.C.D.E.Sample2Struct>();
				jd.Generate<A.B.C.D.E.SampleSerializeIfStruct>();
				jd.Generate<A.B.C.D.E.SampleSerializeIfOnFieldStruct>();
			});

			var bdg = new BinaryDeserializerGenerator();
			bdg.SafetyChecks = true;
			Gen(Path.Combine(path, "GeneratedBinary.cs"), bdg, bd => {
				bd.Generate<Sample1>();
				bd.Generate<Sample2>();
				bd.Generate<Sample3>();
				bd.Generate<SampleEnumMemberTyped>();
				bd.Generate<Sample4>();
				bd.Generate<SampleDecimal>();
				bd.Generate<SampleNullable>();
				bd.Generate<SampleObj>();
				bd.Generate<SampleDict>();
				bd.Generate<SampleSortedDict>();
				bd.Generate<SampleDictKeys>();
				bd.Generate<SampleMemberI>();
				bd.Generate<SampleArray>();
				bd.Generate<SampleArrayOfArray>();
				bd.Generate<SampleArrayNDim>();
				bd.Generate<SampleBase>();
				bd.Generate<SampleDerivedA>();
				bd.Generate<SampleDerivedB>();
				bd.Generate<SampleMatrix>();
				bd.Generate<SamplePoint>();
				bd.Generate<SampleRect>();
				bd.Generate<SampleGuid>();
				bd.Generate<SampleDefault>();
				bd.Generate<Color>();
				bd.Generate<SampleClassList>();
				bd.Generate<SampleSmallTypes>();
				bd.Generate<SampleWithNullFieldCompact>();
				bd.Generate<SampleNested.NestedClass>();
				bd.Generate<SampleNested>();
				bd.Generate<SamplePerson>();
				bd.Generate<SampleInterfaceField>();
				bd.Generate<SampleInterfacedGeneric<string>>();
				bd.Generate<SampleConcrete>();
				bd.Generate<SampleWithCollection>();
				bd.Generate<SampleAfter2>();
				bd.Generate<SampleAfterSerialization>();
				bd.Generate<SampleBeforeDeserialization>();
				bd.Generate<SampleMerge>();
				bd.Generate<SampleAssemblyDerivedR>();
				bd.Generate<SampleAoS.Color>();
				bd.Generate<SampleAoS.Vertex>();
				bd.Generate<SampleAoS.S>();
				bd.Generate<SampleAoS>();
				bd.Generate<SampleStructWithProps>();
				bd.Generate<SampleAliasMany>();
				bd.Generate<SamplePrivateConstructor>();
				bd.Generate<YuzuTestAssembly.SampleAssemblyBase>();
				bd.Generate<YuzuTestAssembly.SampleAssemblyDerivedQ>();
				bd.Generate<YuzuTest2.SampleNamespace>();
				bd.Generate<SampleExplicitCollection<int>>();
				bd.Generate<A.B.C.D.E.Sample2Struct>();
				bd.Generate<A.B.C.D.E.SampleSerializeIfStruct>();
				bd.Generate<A.B.C.D.E.SampleSerializeIfOnFieldStruct>();
			});
			var bdg1 = new BinaryDeserializerGenerator(
				className: "BinaryDeserializerGenDerived",
				baseClassName: "BinaryDeserializerGen"
			);
			Gen(Path.Combine(path, "GeneratedBinaryDerived.cs"), bdg1, bd => {
				bd.Generate<SampleMergeNonPrimitive>();
			});
			var cg = new ClonerGenerator();
			bdg.SafetyChecks = true;
			Gen(Path.Combine(path, "GeneratedCloner.cs"), cg, cd => {
				cd.Generate<Sample1>();
				cd.Generate<Sample2>();
				cd.Generate<Sample3>();
				cd.Generate<SampleGenNoGen>();
				cd.Generate<SampleArray>();
				cd.Generate<SampleArrayOfClass>();
				cd.Generate<SampleArrayNDim>();
				cd.Generate<SampleArrayNDimOfClass>();
				cd.Generate<SampleList>();
				cd.Generate<SampleNullable>();
				cd.Generate<SampleObj>();
				cd.Generate<SampleItemObj>();
				cd.Generate<SampleDict>();
				cd.Generate<SampleDictKeys>();
				cd.Generate<SamplePoint>();
				cd.Generate<SampleRect>();
				cd.Generate<SampleMatrix>();
				cd.Generate<SampleStructWithClass>();
				cd.Generate<SamplePerson>();
				cd.Generate<SamplePrivateConstructor>();
				cd.Generate<SampleBeforeSerialization>();
				cd.Generate<SampleBefore2>();
				cd.Generate<SampleAfterSerialization>();
				cd.Generate<SampleBeforeDeserialization>();
				cd.Generate<SampleAfterDeserialization>();
				cd.Generate<SampleAfter2>();
				cd.Generate<SampleSurrogateColor>();
				cd.Generate<Color>();
				cd.Generate<SampleWithCopyable>();
				cd.Generate<SampleCopyable>();
				cd.Generate<SampleWithCopyableItems>();
				cd.Generate<SampleMerge>();
				cd.Generate<SampleMergeNonPrimitive>();
				cd.Generate<SampleBase>();
				cd.Generate<SampleDerivedA>();
				cd.Generate<SampleDerivedB>();
				cd.Generate<SampleSealed>();
				cd.Generate<SampleSerializeIf>();
				cd.Generate<SampleSerializeIfOnField>();
				cd.Generate<SampleCollection<int>>();
				cd.Generate<SampleCollection<Sample1>>();
				cd.Generate<SampleWithCollectionMerge>();
				cd.Generate<SampleExplicitCollection<int>>();
				cd.Generate<A.B.C.D.E.Sample2Struct>();
				cd.Generate<A.B.C.D.E.SampleSerializeIfStruct>();
				cd.Generate<A.B.C.D.E.SampleSerializeIfOnFieldStruct>();
			});
			var cg1 = new ClonerGenerator(
				className: "ClonerGenDerived",
				baseClassName: "ClonerGen",
				parentGen: cg
			);
			Gen(Path.Combine(path, "GeneratedClonerDerived.cs"), cg1, cd => {
				cd.Generate<SampleClonerGenDerived>();
			});
		}
	}
}
