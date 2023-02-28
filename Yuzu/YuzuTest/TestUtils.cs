using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Yuzu;
using Yuzu.Metadata;

namespace YuzuTest
{
	[TestClass]
	public class TestUtils
	{
		public class A<T1, T2>
		{
			public enum Cat
			{
				Meow,
				Purr,
			}

			public class B<T3, T4>
			{
				public class D
				{
					public enum Dog
					{
						Woof,
						Bark,
					}
				}
			}

			public class C
			{
				public enum Human
				{
					MakeWar,
					MakeLove,
				}
			}
		}

		public class S<T>
		{
		}

		[TestMethod]
		public void TestGetTypeSpec00()
		{
			var t = typeof(A<int, string>.B<double, bool>.D);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.B<double, bool>.D", s);
		}

		[TestMethod]
		public void TestGetTypeSpec01()
		{
			var t = typeof(A<int, string>.C);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.C", s);
		}

		[TestMethod]
		public void TestGetTypeSpec02()
		{
			var t = typeof(A<int, string>);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>", s);
		}

		[TestMethod]
		public void TestGetTypeSpec03()
		{
			var t = typeof(A<int, string>.B<double, bool>);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.B<double, bool>", s);
		}

		[TestMethod]
		public void TestGetTypeSpec04()
		{
			var t = typeof(A<int, string>.Cat);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.Cat", s);
		}

		[TestMethod]
		public void TestGetTypeSpec05()
		{
			var t = typeof(A<int, string>.B<double, bool>.D.Dog);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.B<double, bool>.D.Dog", s);
		}

		[TestMethod]
		public void TestGetTypeSpec06()
		{
			var t = typeof(A<int, string>.C.Human);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.C.Human", s);
		}

		[TestMethod]
		public void TestGetTypeSpec07()
		{
			var t = typeof(List<A<int, string>.B<double, bool>>);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual(
				"global::System.Collections.Generic.List<global::YuzuTest.TestUtils.A<int, string>.B<double, bool>>",
				s
			);
		}

		[TestMethod]
		public void TestGetTypeSpec08()
		{
			var t = typeof(Dictionary<A<int, string>.B<double, bool>, A<int, string>.C>);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual(
				"global::System.Collections.Generic.Dictionary<global::YuzuTest.TestUtils.A<int, string>" +
				".B<double, bool>, global::YuzuTest.TestUtils.A<int, string>.C>",
				s
			);
		}

		[TestMethod]
		public void TestGetTypeSpec09()
		{
			var t = typeof(A<A<int, string>, A<int, string>.B<double, bool>>);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual(
				"global::YuzuTest.TestUtils.A<global::YuzuTest.TestUtils.A<int, string>," +
				" global::YuzuTest.TestUtils.A<int, string>.B<double, bool>>",
				s
			);
		}

		[TestMethod]
		public void TestGetTypeSpec10()
		{
			var t = typeof(A<A<int, string>, A<int, string>.B<double, bool>>.B<double, bool>);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual(
				"global::YuzuTest.TestUtils.A<global::YuzuTest.TestUtils.A<int, string>," +
				" global::YuzuTest.TestUtils.A<int, string>.B<double, bool>>.B<double, bool>",
				s
			);
		}

		[TestMethod]
		public void TestGetTypeSpec11()
		{
			var t = typeof(S<S<S<int>>>);
			var s = Yuzu.Util.Utils.GetTypeSpec(t);
			Assert.AreEqual(
				"global::YuzuTest.TestUtils.S<global::YuzuTest.TestUtils.S<global::YuzuTest.TestUtils.S<int>>>",
				s
			);
		}

		public class NGA
		{
			public enum NGDog
			{
				Woof,
				Bark,
			}

			public class NGB
			{
				public enum NGHuman
				{
					MakeWar,
					MakeLove,
				}
			}
		}

		[TestMethod]
		public void TestGetMangledTypeName00()
		{
			var t = typeof(NGA);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹNGA", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName01()
		{
			var t = typeof(NGA.NGB);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹNGAᱹNGB", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName02()
		{
			var t = typeof(NGA.NGDog);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹNGAᱹNGDog", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName03()
		{
			var t = typeof(NGA.NGB.NGHuman);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹNGAᱹNGBᱹNGHuman", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName04()
		{
			var t = typeof(List<NGA.NGB.NGHuman>);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("ListʳTestUtilsᱹNGAᱹNGBᱹNGHumanʴ", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName05()
		{
			var t = typeof(Dictionary<NGA.NGB.NGHuman, NGA.NGDog>);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual(
				"DictionaryʳTestUtilsᱹNGAᱹNGBᱹNGHumanˎTestUtilsᱹNGAᱹNGDogʴ",
				ts
			);
		}

		[TestMethod]
		public void TestGetMangledTypeName06()
		{
			var t = typeof(A<NGA.NGB.NGHuman, NGA.NGDog>);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹAʳTestUtilsᱹNGAᱹNGBᱹNGHumanˎTestUtilsᱹNGAᱹNGDogʴ", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName07()
		{
			var t = typeof(S<S<S<int>>>);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹSʳTestUtilsᱹSʳTestUtilsᱹSʳintʴʴʴ", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName08()
		{
			var t = typeof(A<int, string>.B<double, bool>.D);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹAʳintˎstringʴᱹBʳdoubleˎboolʴᱹD", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName09()
		{
			var t = typeof(A<int, string>.B<double, bool>.D);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹAʳintˎstringʴᱹBʳdoubleˎboolʴᱹD", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName10()
		{
			var t = typeof(A<int, string>.C);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹAʳintˎstringʴᱹC", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName11()
		{
			var t = typeof(A<int, string>);
			var ts = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹAʳintˎstringʴ", ts);
		}

		[TestMethod]
		public void TestGetMangledTypeName12()
		{
			var t = typeof(A<int, string>.B<double, bool>);
			var s = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹAʳintˎstringʴᱹBʳdoubleˎboolʴ", s);
		}

		[TestMethod]
		public void TestGetMangledTypeName13()
		{
			var t = typeof(A<int, string>.Cat);
			var s = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹAʳintˎstringʴᱹCat", s);
		}

		[TestMethod]
		public void TestGetMangledTypeName14()
		{
			var t = typeof(A<int, string>.B<double, bool>.D.Dog);
			var s = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹAʳintˎstringʴᱹBʳdoubleˎboolʴᱹDᱹDog", s);
		}

		[TestMethod]
		public void TestGetMangledTypeName15()
		{
			var t = typeof(A<int, string>.C.Human);
			var s = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("TestUtilsᱹAʳintˎstringʴᱹCᱹHuman", s);
		}

		[TestMethod]
		public void TestGetMangledTypeName16()
		{
			var t = typeof(List<A<int, string>.B<double, bool>>);
			var s = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual("ListʳTestUtilsᱹAʳintˎstringʴᱹBʳdoubleˎboolʴʴ", s);
		}

		[TestMethod]
		public void TestGetMangledTypeName17()
		{
			var t = typeof(Dictionary<A<int, string>.B<double, bool>, A<int, string>.C>);
			var s = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual(
				"DictionaryʳTestUtilsᱹAʳintˎstringʴᱹBʳdoubleˎboolʴˎTestUtilsᱹAʳintˎstringʴᱹCʴ", s
			);
		}

		[TestMethod]
		public void TestGetMangledTypeName18()
		{
			var t = typeof(A<A<int, string>, A<int, string>.B<double, bool>>);
			var s = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual(
				"TestUtilsᱹAʳTestUtilsᱹAʳintˎstringʴˎTestUtilsᱹAʳintˎstringʴᱹBʳdoubleˎboolʴʴ",
				s
			);
		}

		[TestMethod]
		public void TestGetMangledTypeName19()
		{
			var t = typeof(A<A<int, string>, A<int, string>.B<double, bool>>.B<double, bool>);
			var s = Yuzu.Util.Utils.GetMangledTypeName(t);
			Assert.AreEqual(
				"TestUtilsᱹAʳTestUtilsᱹAʳintˎstringʴˎTestUtilsᱹAʳintˎstringʴᱹBʳdoubleˎboolʴʴᱹBʳdoubleˎboolʴ", s
			);
		}
	}
}
