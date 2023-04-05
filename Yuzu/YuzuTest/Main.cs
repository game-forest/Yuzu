using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Yuzu;
using Yuzu.Binary;
using Yuzu.Clone;
using Yuzu.Code;
using Yuzu.DictOfObjects;
using Yuzu.Json;
using Yuzu.Metadata;
using Yuzu.Util;
using YuzuGenClone;

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

		private void TestTypeSerializerHelper(Type t, string s)
		{
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

	public class BenchmarkClone
	{
		public BenchmarkClone()
		{
			sampleNoCollectionsNoChildren = new NodeForCloneBench();
			FillObjectWithRandomData(sampleNoCollectionsNoChildren, 0);

			sampleLotsOfDataInCollectionsNoChildren = new NodeForCloneBench();
			FillObjectWithRandomData(sampleLotsOfDataInCollectionsNoChildren, 10000);

			sampleNoCollectionsLotsOfChildren = new NodeForCloneBench();
			FillObjectWithRandomData(sampleNoCollectionsLotsOfChildren, 0);
			AddChildren(sampleNoCollectionsLotsOfChildren, 2, 0, 13);

			sampleLotsOfDataInCollectionsLotsOfChildren = new NodeForCloneBench();
			FillObjectWithRandomData(sampleLotsOfDataInCollectionsLotsOfChildren, 100);
			AddChildren(sampleLotsOfDataInCollectionsLotsOfChildren, 2, 100, 13);

			void AddChildren(NodeForCloneBench n, int childCount, int numDataInLists, int depthLimit)
			{
				if (depthLimit == 0) {
					return;
				}
				n.Children = new List<NodeForCloneBench>();
				for (int i = 0; i < childCount; i++) {
					var child = new NodeForCloneBench();
					FillObjectWithRandomData(child, numDataInLists);
					n.Children.Add(child);
					AddChildren(child, childCount, numDataInLists, depthLimit - 1);
				}
			}
		}

		public static void FillObjectWithRandomData(object obj, int numDataInLists)
		{
			var random = new Random();

			foreach (var field in obj.GetType().GetFields()) {
				var fieldType = field.FieldType;
				if (fieldType == typeof(int)) {
					field.SetValue(obj, random.Next());
				} else if (fieldType == typeof(double)) {
					field.SetValue(obj, random.NextDouble());
				} else if (fieldType == typeof(string)) {
					field.SetValue(obj, Guid.NewGuid().ToString());
				} else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>)) {
					if (numDataInLists > 0) {
						var list = Activator.CreateInstance(fieldType) as IList<object>;
						var listType = fieldType.GetGenericArguments()[0];
						var count = numDataInLists;
						for (var i = 0; i < count; i++) {
							if (listType == typeof(int)) {
								list.Add(random.Next());
							} else if (listType == typeof(double)) {
								list.Add(random.NextDouble());
							} else if (listType == typeof(string)) {
								list.Add(Guid.NewGuid().ToString());
							}
						}
						field.SetValue(obj, list);
					}
				}
			}
			foreach (var property in obj.GetType().GetProperties()) {
				var propertyType = property.PropertyType;
				if (propertyType == typeof(int)) {
					property.SetValue(obj, random.Next());
				} else if (propertyType == typeof(double)) {
					property.SetValue(obj, random.NextDouble());
				} else if (propertyType == typeof(string)) {
					property.SetValue(obj, Guid.NewGuid().ToString());
				} else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>)) {
					if (numDataInLists > 0) {
						var list = Activator.CreateInstance(propertyType);
						var listType = propertyType.GetGenericArguments()[0];
						var count = numDataInLists;
						for (var i = 0; i < count; i++) {
							if (listType == typeof(int)) {
								var ll = (List<int>)list;
								ll.Add(random.Next());
							} else if (listType == typeof(double)) {
								var ll = (List<double>)list;
								ll.Add(random.NextDouble());
							} else if (listType == typeof(string)) {
								var ll = (List<string>)list;
								ll.Add(Guid.NewGuid().ToString());
							}
						}
						property.SetValue(obj, list);
					}
				}
			}
		}

		private NodeForCloneBench sampleNoCollectionsNoChildren;
		private NodeForCloneBench sampleLotsOfDataInCollectionsNoChildren;
		private NodeForCloneBench sampleNoCollectionsLotsOfChildren;
		private NodeForCloneBench sampleLotsOfDataInCollectionsLotsOfChildren;

		private YuzuGenClone.ClonerGen yuzuGenCloner = new YuzuGenClone.ClonerGen();

		[Benchmark]
		public void CloneWithMemberwiseClone_NoCollectionsNoChildren()
		{
			var s = sampleNoCollectionsNoChildren.Clone();
		}

		[Benchmark]
		public void CloneWithYuzuGeneratedClone_NoCollectionsNoChildren()
		{
			var s = yuzuGenCloner.Deep(sampleNoCollectionsNoChildren);
		}

		[Benchmark]
		public void CloneWithYuzuClone_NoCollectionsNoChildren()
		{
			var s = Yuzu.Clone.Cloner.Instance.Deep(sampleNoCollectionsNoChildren);
		}

		[Benchmark]
		public void CloneWithMemberwiseClone_LotsOfDataInCollectionsNoChildren()
		{
			var s = sampleLotsOfDataInCollectionsNoChildren.Clone();
		}

		[Benchmark]
		public void CloneWithYuzuGeneratedClone_LotsOfDataInCollectionsNoChildren()
		{
			var s = yuzuGenCloner.Deep(sampleLotsOfDataInCollectionsNoChildren);
		}

		[Benchmark]
		public void CloneWithYuzuClone_LotsOfDataInCollectionsNoChildren()
		{
			var s = Yuzu.Clone.Cloner.Instance.Deep(sampleLotsOfDataInCollectionsNoChildren);
		}

		[Benchmark]
		public void CloneWithMemberwiseClone_NoCollectionsLotsOfChildren()
		{
			var s = sampleNoCollectionsLotsOfChildren.Clone();
		}

		[Benchmark]
		public void CloneWithYuzuGeneratedClone_NoCollectionsLotsOfChildren()
		{
			var s = yuzuGenCloner.Deep(sampleNoCollectionsLotsOfChildren);
		}

		[Benchmark]
		public void CloneWithYuzuClone_NoCollectionsLotsOfChildren()
		{
			var s = Yuzu.Clone.Cloner.Instance.Deep(sampleNoCollectionsLotsOfChildren);
		}

		[Benchmark]
		public void CloneWithMemberwiseClone_LotsOfDataInCollectionsLotsOfChildren()
		{
			var s = sampleLotsOfDataInCollectionsLotsOfChildren.Clone();
		}

		[Benchmark]
		public void CloneWithYuzuGeneratedClone_LotsOfDataInCollectionsLotsOfChildren()
		{
			var s = yuzuGenCloner.Deep(sampleLotsOfDataInCollectionsLotsOfChildren);
		}

		[Benchmark]
		public void CloneWithYuzuClone_LotsOfDataInCollectionsLotsOfChildren()
		{
			var s = Yuzu.Clone.Cloner.Instance.Deep(sampleLotsOfDataInCollectionsLotsOfChildren);
		}

		[Benchmark]
		public void CloneWithMemberwiseClone_Matrix44Required()
		{
			var m = new Matrix44Required();
			var m1 = m.Clone();
		}

		[Benchmark]
		public void CloneWithMemberwiseClone_Matrix44Member()
		{
			var m = new Matrix44Member();
			var m1 = m.Clone();
		}

		[Benchmark]
		public void CloneWithYuzuClone_Matrix44Required()
		{
			var m = new Matrix44Required();
			var m1 = Yuzu.Clone.Cloner.Instance.Deep(m);
		}

		[Benchmark]
		public void CloneWithYuzuClone_Matrix44Member()
		{
			var m = new Matrix44Member();
			var m1 = Yuzu.Clone.Cloner.Instance.Deep(m);
		}

		[Benchmark]
		public void CloneWithYuzuGenClone_Matrix44Required()
		{
			var m = new Matrix44Required();
			var m1 = yuzuGenCloner.Deep(m);
		}

		[Benchmark]
		public void CloneWithYuzuGenClone_Matrix44Member()
		{
			var m = new Matrix44Member();
			var m1 = yuzuGenCloner.Deep(m);
		}
	}

	public class Program
	{
		private static void Gen(string fileName, IGenerator g, Action<IGenerator> fill)
		{
			using var ms = new MemoryStream();
			using var sw = new StreamWriter(ms);
			sw.WriteLine("//------------------------------------------------------------------------------");
			sw.WriteLine("// <auto-generated>");
			sw.WriteLine("// Yuzu generated.");
			sw.WriteLine("// </auto-generated>");
			sw.WriteLine("//------------------------------------------------------------------------------");
			g.GenWriter = sw;
			g.GenerateHeader();
			fill(g);
			g.GenerateFooter();
			sw.Flush();
			using var fs = new FileStream(fileName, FileMode.Create);
			ms.WriteTo(fs);
		}

		public static void Main()
		{
			var jd = JsonDeserializerGenerator.Instance;
			jd.Options.TagMode = TagMode.Names;
			Gen(@"..\..\..\GeneratedJson.cs", jd, g => {
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
			});

			var bdg = new BinaryDeserializerGenerator();
			bdg.SafetyChecks = true;
			Gen(@"..\..\..\GeneratedBinary.cs", bdg, bd => {
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
			});
			var bdg1 = new BinaryDeserializerGenerator(
				className: "BinaryDeserializerGenDerived", baseClassName: "BinaryDeserializerGen"
			);
			Gen(@"..\..\..\GeneratedBinaryDerived.cs", bdg1, bd => {
				bd.Generate<SampleMergeNonPrimitive>();
			});
			var cg = new ClonerGenerator();
			bdg.SafetyChecks = true;
			Gen(@"..\..\..\GeneratedCloner.cs", cg, cd => {
				cd.Generate<NodeForCloneBench>();
				cd.Generate<Matrix44Member>();
				cd.Generate<Matrix44Required>();
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
				cd.Generate<SampleCollection<int>>();
				cd.Generate<SampleCollection<Sample1>>();
				cd.Generate<SampleWithCollectionMerge>();
				cd.Generate<SampleExplicitCollection<int>>();
			});
			var cg1 = new ClonerGenerator(
				className: "ClonerGenDerived", baseClassName: "ClonerGen", parentGen: cg
			);
			Gen(@"..\..\..\GeneratedClonerDerived.cs", cg1, cd => {
				cd.Generate<SampleClonerGenDerived>();
			});
			var summary = BenchmarkRunner.Run<BenchmarkClone>();
		}
	}
}
