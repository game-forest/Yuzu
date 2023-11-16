using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ProtoBuf;

using Yuzu;
using Yuzu.Metadata;

namespace YuzuTest.Metadata
{
	[TestClass]
	public partial class TestMeta
	{
		[TestMethod]
		public void TestAttributes()
		{
			var opt1 = new CommonOptions {
				Meta = new MetaOptions { }
			};
			var opt2 = new CommonOptions {
				Meta = new MetaOptions {
					RequiredAttribute = typeof(ProtoMemberAttribute),
					OptionalAttribute = null,
					MemberAttribute = null,
					GetAlias = attr => (attr as ProtoMemberAttribute).Tag.ToString(),
				}
			};
			var m1 = Meta.Get(typeof(Sample1), opt1);
			var m2 = Meta.Get(typeof(Sample1), opt2);
			Assert.AreNotEqual(m1, m2);

			Assert.AreEqual(2, m1.Items.Count);
			Assert.AreEqual("X", m1.Items[0].Tag(opt1));
			Assert.AreEqual("Y", m1.Items[1].Tag(opt1));

			Assert.AreEqual(2, m2.Items.Count);
			Assert.AreEqual("1", m2.Items[0].Tag(opt2));
			Assert.AreEqual("2", m2.Items[1].Tag(opt2));
		}

		[YuzuMust(YuzuItemKind.Field)]
		internal class MustField
		{
			[YuzuOptional]
			public int F1 = 0;
			public int P1 { get; set; }
			public int F2 = 0;
		}

		[YuzuMust(YuzuItemKind.Property)]
		internal class MustProperty
		{
			public int F1 = 0;
			[YuzuOptional]
			public int P1 { get; set; }
			public int P2 { get; set; }
		}

		[YuzuMust]
		internal class MustPrivate
		{
			[YuzuRequired]
			public int F1 = 0;
			[YuzuRequired]
			public int P1 { get; set; }
#pragma warning disable CS0414
			private int F2 = 0;
#pragma warning restore CS0414
			private int P2 { get; set; }
		}

		[TestMethod]
		public void TestMust()
		{
			var opt1 = new CommonOptions();
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(MustField), opt1), "F2");
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(MustProperty), opt1), "P2");
			Assert.AreEqual(2, Meta.Get(typeof(MustPrivate), opt1).Items.Count);
		}

		[YuzuAll]
		internal class AllDefault
		{
			public int F1 = 0;
			public int P1 { get; set; }
#pragma warning disable CS0414
			private int F2 = 0;
#pragma warning restore CS0414
			private int P2 { get; set; }
			[YuzuRequired]
			public int R = 0;
		}

		[YuzuAll(YuzuItemOptionality.Required, YuzuItemKind.Field)]
		internal class AllRequiredFields
		{
			public int F1 = 0;
			public int P1 { get; set; }
#pragma warning disable CS0414
			private int F2 = 0;
#pragma warning restore CS0414
			private int P2 { get; set; }
			[YuzuMember]
			public int M = 0;
		}

		[TestMethod]
		public void TestAll()
		{
			var opt1 = new CommonOptions();
			var m1 = Meta.Get(typeof(AllDefault), opt1);
			Assert.AreEqual(3, m1.Items.Count);
			Assert.IsTrue(m1.Items[0].IsOptional);
			Assert.IsTrue(m1.Items[1].IsOptional);
			Assert.IsFalse(m1.Items[2].IsOptional);

			var m2 = Meta.Get(typeof(AllRequiredFields), opt1);
			Assert.AreEqual(2, m2.Items.Count);
			Assert.IsFalse(m2.Items[0].IsOptional);
			Assert.IsTrue(m2.Items[1].IsOptional);
		}

		[YuzuAll]
		internal class Exclude
		{
			public int F1 = 0;
			public int P1 { get; set; }
			[YuzuExclude]
			public int F2 = 0;
			[YuzuExclude]
			public int P2 { get; set; }
			[YuzuRequired, YuzuExclude]
			public int R = 0;
		}

		[TestMethod]
		public void TestExclude()
		{
			var opt1 = new CommonOptions();
			var m1 = Meta.Get(typeof(Exclude), opt1);
			Assert.AreEqual(2, m1.Items.Count);
			Assert.AreEqual("F1", m1.Items[0].Tag(opt1));
			Assert.AreEqual("P1", m1.Items[1].Tag(opt1));
		}

		[YuzuAllowReadingFromAncestor]
		internal class Sample1Bad : Sample1
		{
			[YuzuMember]
			public int F1 = 0;
		}

		[TestMethod]
		public void TestAllowReadingFromAncestorBad()
		{
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(Sample1Bad), new CommonOptions()), "3");
		}

		internal class ToSurrogateBadParams
		{
			[YuzuToSurrogate]
			int ToS1(int y) { return 0; }
		}

		internal class SurrogateIfBadParams
		{
			[YuzuSurrogateIf]
			bool SIf1(int y) { return false; }
		}

		internal class FromSurrogateBadParams
		{
			[YuzuToSurrogate]
			int ToS1() { return 0; }
			[YuzuFromSurrogate]
			static FromSurrogateBadParams FromS1(string s) { return null; }
		}

		internal class ToSurrogateStaticBadParams
		{
			[YuzuToSurrogate]
			static int ToS2() { return 0; }
		}

		internal class ToSurrogateStaticBadParamType
		{
			[YuzuToSurrogate]
			static int ToS21(int x) { return 0; }
		}

		internal class ToSurrogateVoid
		{
			[YuzuToSurrogate]
			void ToS3() { }
		}

		internal class SurrogateIfInt
		{
			[YuzuSurrogateIf]
			int SIf3() { return 0; }
		}

		internal class FromSurrogateInt
		{
			[YuzuFromSurrogate]
			static int FromS3() { return 0; }
		}

		internal class ToSurrogateDup
		{
			[YuzuToSurrogate]
			int ToS41() { return 0; }
			[YuzuToSurrogate]
			int ToS42() { return 0; }
		}

		internal class ToSurrogateChain
		{
			[YuzuToSurrogate]
			SampleSurrogateColor ToS5() { return new SampleSurrogateColor(); }
		}

		[TestMethod]
		public void TestSurrogateErrors()
		{
			var opt = new CommonOptions();

			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(ToSurrogateBadParams), opt), "ToS1");
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(SurrogateIfBadParams), opt), "SIf1");
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(FromSurrogateBadParams), opt), "FromS1");

			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(ToSurrogateStaticBadParams), opt), "ToS2");
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(ToSurrogateStaticBadParamType), opt), "ToS21");

			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(ToSurrogateVoid), opt), "ToS3");
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(SurrogateIfInt), opt), "SIf3");
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(FromSurrogateInt), opt), "FromS3");

			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(ToSurrogateDup), opt), "ToS41");

			var opt1 = new CommonOptions { Meta = new MetaOptions() };
			Meta.Get(typeof(ToSurrogateChain), opt1);
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(SampleSurrogateColor), opt1), "chain");
			var opt2 = new CommonOptions { Meta = new MetaOptions() };
			Meta.Get(typeof(SampleSurrogateColor), opt2);
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(ToSurrogateChain), opt2), "chain");
		}

		[YuzuAlias(read: new string[] { "Dup", "Dup" })]
		internal class DuplicateReadAlias
		{
			[YuzuRequired]
#pragma warning disable CS0649
			public int X;
#pragma warning restore CS0649
		}

		[YuzuAlias("")]
		internal class EmptyReadAlias
		{
			[YuzuRequired]
#pragma warning disable CS0649
			public int X;
#pragma warning restore CS0649
		}

		[TestMethod]
		public void TestAlias()
		{
			var opt = new CommonOptions();
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(DuplicateReadAlias), opt), "Dup");
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(EmptyReadAlias), opt), "Empty");
		}

		internal partial class SampleItemIfScalar
		{
			[YuzuMember]
			public int X = 1;
			[YuzuSerializeItemIf]
			public bool Func(int index, object item) => true;
		}

		internal partial class SampleItemIfDup : List<int>
		{
			[YuzuSerializeItemIf]
			public bool Func1(int index, object item) => true;
			[YuzuSerializeItemIf]
			public bool Func2(int index, object item) => true;
		}

		[TestMethod]
		public void TestSerializeItemIfErrors()
		{
			var opt = new CommonOptions();
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(SampleItemIfScalar), opt), "IEnumerable");
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(SampleItemIfDup), opt), "Duplicate");
		}

		internal class BadFactory
		{
			[YuzuFactory]
			public object F() => null;
		}

		internal class DuplicateFactory
		{
			[YuzuFactory]
			public static object F() => null;
			[YuzuFactory]
			public static object FDup() => null;
		}

		[TestMethod]
		public void TestFactoryErrors()
		{
			var opt = new CommonOptions();
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(BadFactory), opt), "static");
			XAssert.Throws<YuzuException>(() => Meta.Get(typeof(DuplicateFactory), opt), "FDup");
		}

		internal class Overridden
		{
#pragma warning disable CS0649
			public int X;
			public string Y;
#pragma warning restore CS0649
		}

		[TestMethod]
		public void TestOverride()
		{
			{
				var metaOptions = new MetaOptions();
				var opt = new CommonOptions { Meta = metaOptions };
				metaOptions.AddOverride(typeof(Overridden), over => over.AddAttr(new YuzuAll()));
				var meta = Meta.Get(typeof(Overridden), opt);
				Assert.AreEqual(2, meta.Items.Count);
			}
			{
				var metaOptions = new MetaOptions();
				var opt = new CommonOptions { Meta = metaOptions };
				metaOptions.AddOverride(
					typeof(Overridden),
					over => over.AddItem(nameof(Overridden.X), i => i.AddAttr(new YuzuMember()))
				);
				var meta = Meta.Get(typeof(Overridden), opt);
				Assert.AreEqual(1, meta.Items.Count);
				Assert.AreEqual("X", meta.Items[0].Name);
			}
			{
				var metaOptions = new MetaOptions();
				var opt = new CommonOptions { Meta = metaOptions };
				metaOptions.AddOverride(
					typeof(Overridden),
					over => over.
						AddAttr(new YuzuAll()).
						AddItem(nameof(Overridden.X), i => i.AddAttr(new YuzuExclude()))
				);
				var meta = Meta.Get(typeof(Overridden), opt);
				Assert.AreEqual(1, meta.Items.Count);
				Assert.AreEqual("Y", meta.Items[0].Name);
			}
			{
				var metaOptions = new MetaOptions();
				var opt = new CommonOptions { Meta = metaOptions };
				metaOptions.AddOverride(
					typeof(Sample1),
					over => over.
						AddItem(nameof(Sample1.X), i => i.NegateAttr(typeof(YuzuRequired)))
				);
				var meta = Meta.Get(typeof(Sample1), opt);
				Assert.AreEqual(1, meta.Items.Count);
				Assert.AreEqual("Y", meta.Items[0].Name);
			}
		}

		internal class MutualRec1
		{
#pragma warning disable CS0649
			[YuzuRequired]
			public MutualRec2 R;
#pragma warning restore CS0649
		}

		internal class MutualRec2
		{
#pragma warning disable CS0649
			[YuzuRequired]
			public MutualRec1 R;
#pragma warning restore CS0649
		}

		[TestMethod]
		public void TestMutualRec()
		{
			Meta.Get(typeof(MutualRec1), new CommonOptions());
		}

		internal class DefaultNull
		{
			[YuzuOptional]
			[YuzuDefault(null)]
			public string S = null;
		}

		[TestMethod]
		public void TestDefaultNull ()
		{
			var d = new DefaultNull();
			Assert.IsFalse(
				Meta.Get(typeof(DefaultNull), new CommonOptions()).Items[0].SerializeCond(d, d.S));
		}
	}

}
