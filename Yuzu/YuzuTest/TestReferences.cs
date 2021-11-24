using System;
using System.Collections.Generic;
using System.Linq;

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
						FlatHierarchy = true
					},
					ReferenceResolver = new ReferenceResolver()
				};
				var d = new JsonDeserializer {
					JsonOptions = new JsonSerializeOptions {
						SaveClass = JsonSaveClass.UnknownOrRoot,
						FlatHierarchy = true
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
					Options = new CommonOptions { ResolveClonedReferences = true }
				};
				return c.DeepObject(i);
			},
			// Cloner generated
			i => {
				var c = new YuzuGenClone.ClonerGenDerived {
					Options = new CommonOptions { ResolveClonedReferences = true }
				};
				return c.DeepObject(i);
			}
		};

		private class ReferenceResolver : IDeserializerReferenceResolver, ISerializerReferenceResolver
		{
			private readonly Dictionary<int, object> referenceToObjects = new Dictionary<int, object>();
			private readonly Dictionary<object, int> objectsToReferences = new Dictionary<object, int>();
			private int currentId = 1;

			public Type ReferenceType() => typeof(int);

			public object GetObject(object reference)
			{
				if (!referenceToObjects.TryGetValue((int)reference, out var obj)) {
					throw new YuzuException($"Unresolved reference: {reference}");
				}
				return obj;
			}

			public bool TryGetObject(object reference, out object obj)
			{
				return referenceToObjects.TryGetValue((int)reference, out obj);
			}

			public void AddObject(object reference, object obj) => referenceToObjects.Add((int)reference, obj);

			public bool TryGetReference(object obj, out object reference, out bool referenceGenerated)
			{
				if (objectsToReferences.TryGetValue(obj, out var key)) {
					reference = key;
					referenceGenerated = false;
				} else {
					reference = currentId++;
					objectsToReferences.Add(obj, (int)reference);
					referenceGenerated = true;
				}
				return true;
			}
		}
	}
}
