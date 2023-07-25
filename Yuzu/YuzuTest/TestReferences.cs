using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Yuzu;
using Yuzu.Binary;
using Yuzu.Clone;
using Yuzu.Json;

namespace YuzuTest
{
	[TestClass]
	public class TestReferences
	{
		[TestMethod]
		public void TestJsonSerialization()
		{
			var js = new JsonSerializer {
				JsonOptions = new JsonSerializeOptions {
					SaveClass = JsonSaveClass.UnknownOrRoot,
					Indent = ""
				},
				ReferenceResolver = new ReferenceResolver()
			};
			var tyler = new Employee { Name = "Tyler Stein" };
			var adrian = new Employee { Name = "Adrian King" };
			tyler.DirectReports = new List<IEmployee> { adrian };
			adrian.Manager = tyler;
			var s =
				"{\n\"id\":0,\n\"class\":\"YuzuTest.TestReferences+Employee, YuzuTest\",\n" +
				"\"DirectReports\":[\n{\n\"id\":1,\n\"class\":\"YuzuTest.TestReferences+Employee, YuzuTest\",\n" +
				"\"Manager\":{\n\"ref\":0\n},\n\"Name\":\"Adrian King\"\n}\n],\n\"Name\":\"Tyler Stein\"\n}";
			Assert.AreEqual(js.ToString(tyler), s);
		}

		[TestMethod]
		public void TestBinarySerialization()
		{
			var bs = new BinarySerializer {
				ReferenceResolver = new ReferenceResolver()
			};
			var tyler = new Employee { Name = "Tyler Stein" };
			var adrian = new Employee { Name = "Adrian King" };
			tyler.DirectReports = new List<IEmployee> { adrian };
			adrian.Manager = tyler;
			var s = 
				"20-FF-FF-00-00-00-00-01-00-2A-59-75-7A-75-54-65-73-74-2E-54-65-73-74-52-65-66-65-72-65-6E-63-65-73-2B-45-6D-70-" +
				"6C-6F-79-65-65-2C-20-59-75-7A-75-54-65-73-74-00-03-00-0D-44-69-72-65-63-74-52-65-70-6F-72-74-73-21-20-07-4D-61-" +
				"6E-61-67-65-72-20-04-4E-61-6D-65-10-01-00-01-00-00-00-FF-FF-01-00-00-00-01-00-02-00-FE-FF-00-00-00-00-03-00-0B-" +
				"41-64-72-69-61-6E-20-4B-69-6E-67-00-00-03-00-0B-54-79-6C-65-72-20-53-74-65-69-6E-00-00";
			Assert.AreEqual(BitConverter.ToString(bs.ToBytes(tyler)), s);
		}

		[TestMethod]
		public void TestObjectGraph()
		{
			foreach (var roundtrip in roundtrips) {
				var root = new Node();
				for (int i = 0; i < 10; i++) {
					root.Nodes.Add(new Node { Id = i });
				}
				var rnd = new Random(123);
				for (int i = 0; i < 50; i++) {
					root.Nodes[rnd.Next(root.Nodes.Count)].Nodes.Add(root.Nodes[rnd.Next(root.Nodes.Count)]);
				}
				var output = (Node)roundtrip(root);
				for (int i = 0; i < root.Nodes.Count; i++) {
					Assert.IsTrue(
						output.Nodes[i].Nodes.Select(j => j.Id).SequenceEqual(root.Nodes[i].Nodes.Select(j => j.Id))
					);
				}
			}
		}

		[YuzuSerializeByReference]
		public class Node
		{
			[YuzuMember]
			public int Id;

			[YuzuMember]
			[YuzuMerge]
			public readonly List<Node> Nodes = new List<Node>();
		}


		[TestMethod]
		public void TestInterfaces()
		{
			foreach (var roundtrip in roundtrips) {
				var tyler = new Employee { Name = "Tyler Stein" };
				var adrian = new Employee { Name = "Adrian King" };
				tyler.DirectReports = new List<IEmployee> { adrian };
				adrian.Manager = tyler;
				IEmployee input = tyler;
				var output = (IEmployee)roundtrip(input);
				Assert.AreEqual(output.DirectReports[0].Manager, output);
			}
		}

		public interface IEmployee
		{
			[YuzuMember]
			string Name { get; set; }

			[YuzuMember]
			IEmployee Manager { get; set; }

			[YuzuMember]
			public List<IEmployee> DirectReports { get; set; }
		}

		[YuzuSerializeByReference]
		public class Employee : IEmployee
		{
			public string Name { get; set; }

			public IEmployee Manager { get; set; }

			public List<IEmployee> DirectReports { get; set; }
		}

		List<Func<object, object>> roundtrips = new List<Func<object, object>> {
			// Json
			i => {
				var s = new JsonSerializer {
					JsonOptions = new JsonSerializeOptions {
						SaveClass = JsonSaveClass.UnknownOrRoot,
					},
					ReferenceResolver = new ReferenceResolver()
				};
				var d = new JsonDeserializer {
					JsonOptions = new JsonSerializeOptions {
						SaveClass = JsonSaveClass.UnknownOrRoot,
					},
					ReferenceResolver = new ReferenceResolver()
				};
				var buf = s.ToString(i);
				return d.FromString(buf);
			},
			// Binary
			i => {
				var s = new BinarySerializer {
					ReferenceResolver = new ReferenceResolver()
				};
				var d = new BinaryDeserializer {
					ReferenceResolver = new ReferenceResolver()
				};
				var buf = s.ToBytes(i);
				return d.FromBytes(buf);
			},
			// Binary generated
			i => {
				var s = new BinarySerializer {
					ReferenceResolver = new ReferenceResolver()
				};
				var d = new YuzuGenBin.BinaryDeserializerGenDerived {
					ReferenceResolver = new ReferenceResolver()
				};
				var buf = s.ToBytes(i);
				return d.FromBytes(buf);
			},
			// Cloner
			i => {
				var c = new Cloner {
					ReferenceResolver = new ReferenceResolver()
				};
				return c.DeepObject(i);
			},
			// Cloner generated
			i => {
				var c = new YuzuGenClone.ClonerGenDerived {
					ReferenceResolver = new ReferenceResolver()
				};
				return c.DeepObject(i);
			},
		};

		private class ReferenceEqualityComparer : IEqualityComparer<object>
		{
			public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

			public new bool Equals(object x, object y) => ReferenceEquals(x, y);

			public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
		}

		private class ReferenceResolver : ReferenceResolver<int>
		{
			private readonly List<object> referenceToObject = new List<object>();
			private readonly Dictionary<object, int> objectToReference = new Dictionary<object, int>(ReferenceEqualityComparer.Instance);

			public override int GetReference(object obj, out bool alreadyExists)
			{
				alreadyExists = objectToReference.TryGetValue(obj, out var reference);
				if (!alreadyExists) {
					reference = objectToReference.Count;
					objectToReference.Add(obj, reference);
				}
				return reference;
			}

			public override void AddReference(int reference, object obj)
			{
				while (referenceToObject.Count <= reference) {
					referenceToObject.Add(null);
				}
				referenceToObject[reference] = obj;
			}

			public override object ResolveReference(int reference)
			{
				if (reference < 0 || reference >= referenceToObject.Count || referenceToObject[reference] == null) {
					throw new YuzuException($"Unresolved reference: {reference}");
				}
				return referenceToObject[reference];
			}
		}
	}
}
