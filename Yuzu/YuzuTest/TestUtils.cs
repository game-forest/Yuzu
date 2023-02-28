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
		public void TestGetTypeName0()
		{
			var t0 = typeof(S<S<S<int>>>);
			var t0s = Yuzu.Util.Utils.GetTypeSpec(t0);
			Assert.AreEqual(
				"global::YuzuTest.TestUtils.S<global::YuzuTest.TestUtils.S<global::YuzuTest.TestUtils.S<int>>>",
				t0s
			);
		}

		[TestMethod]
		public void TestGetTypeSpec0()
		{
			var t0 = typeof(A<int, string>.B<double, bool>.D);
			var t0s = Yuzu.Util.Utils.GetTypeSpec(t0);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.B<double, bool>.D", t0s);
		}

		[TestMethod]
		public void TestGetTypeSpec1()
		{
			var t1 = typeof(A<int, string>.C);
			var t1s = Yuzu.Util.Utils.GetTypeSpec(t1);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.C", t1s);
		}

		[TestMethod]
		public void TestGetTypeSpec2()
		{
			var t2 = typeof(A<int, string>);
			var t2s = Yuzu.Util.Utils.GetTypeSpec(t2);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>", t2s);
		}

		[TestMethod]
		public void TestGetTypeSpec3()
		{
			var t3 = typeof(A<int, string>.B<double, bool>);
			var t3s = Yuzu.Util.Utils.GetTypeSpec(t3);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.B<double, bool>", t3s);
		}

		[TestMethod]
		public void TestGetTypeSpec4()
		{
			var t4 = typeof(A<int, string>.Cat);
			var t4s = Yuzu.Util.Utils.GetTypeSpec(t4);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.Cat", t4s);
		}

		[TestMethod]
		public void TestGetTypeSpec5()
		{
			var t5 = typeof(A<int, string>.B<double, bool>.D.Dog);
			var t5s = Yuzu.Util.Utils.GetTypeSpec(t5);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.B<double, bool>.D.Dog", t5s);
		}

		[TestMethod]
		public void TestGetTypeSpec6()
		{
			var t6 = typeof(A<int, string>.C.Human);
			var t6s = Yuzu.Util.Utils.GetTypeSpec(t6);
			Assert.AreEqual("global::YuzuTest.TestUtils.A<int, string>.C.Human", t6s);
		}

		[TestMethod]
		public void TestGetTypeSpec7()
		{
			var t7 = typeof(List<A<int, string>.B<double, bool>>);
			var t7s = Yuzu.Util.Utils.GetTypeSpec(t7);
			Assert.AreEqual(
				"global::System.Collections.Generic.List<global::YuzuTest.TestUtils.A<int, string>.B<double, bool>>",
				t7s
			);
		}

		[TestMethod]
		public void TestGetTypeSpec8()
		{
			var t8 = typeof(Dictionary<A<int, string>.B<double, bool>, A<int, string>.C>);
			var t8s = Yuzu.Util.Utils.GetTypeSpec(t8);
			Assert.AreEqual(
				"global::System.Collections.Generic.Dictionary<global::YuzuTest.TestUtils.A<int, string>" +
				".B<double, bool>, global::YuzuTest.TestUtils.A<int, string>.C>",
				t8s
			);
		}

		[TestMethod]
		public void TestGetTypeSpec9()
		{
			var t9 = typeof(A<A<int, string>, A<int, string>.B<double, bool>>);
			var t9s = Yuzu.Util.Utils.GetTypeSpec(t9);
			Assert.AreEqual(
				"global::YuzuTest.TestUtils.A<global::YuzuTest.TestUtils.A<int, string>," +
				" global::YuzuTest.TestUtils.A<int, string>.B<double, bool>>",
				t9s
			);
		}

		[TestMethod]
		public void TestGetTypeSpec10()
		{
			var t10 = typeof(A<A<int, string>, A<int, string>.B<double, bool>>.B<double, bool>);
			var t10s = Yuzu.Util.Utils.GetTypeSpec(t10);
			Assert.AreEqual(
				"global::YuzuTest.TestUtils.A<global::YuzuTest.TestUtils.A<int, string>," +
				" global::YuzuTest.TestUtils.A<int, string>.B<double, bool>>.B<double, bool>",
				t10s
			);
		}
	}
}
