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
	public class TestJsonFlattenHierarchy
	{
		[TestMethod]
		public void TestObjectGraph()
		{
			foreach (var roundtrip in roundtrips) {
				var root = new Node();
				for (int i = 0; i < 10; i++) {
					root.Nodes.Add(new Node());
				}
				var rnd = new Random(123);
				for (int i = 0; i < 50; i++) {
					root.Nodes[rnd.Next(root.Nodes.Count)].Nodes.Add(root.Nodes[rnd.Next(root.Nodes.Count)]);
				}
				var objList = BuildObjectList(root);
				var output = (Node)((List<object>)roundtrip(objList))[0];
				for (int i = 0; i < root.Nodes.Count; i++) {
					Assert.IsTrue(
						output.Nodes[i].Nodes.Select(j => j.Guid).SequenceEqual(root.Nodes[i].Nodes.Select(j => j.Guid))
					);
				}
			}
		}

		[TestMethod]
		public void TestNonReferenceableNode()
		{
			foreach (var roundtrip in roundtrips) {
				var n1 = new Node();
				var n2 = new Node();
				n1.NonReferenceable = new NonReferenceableNode { Node = n2 };
				var objList = BuildObjectList(n1);
				var output = (Node)((List<object>)roundtrip(objList))[0];
				Assert.AreEqual(n1.Guid, output.Guid);
				Assert.AreEqual(n1.NonReferenceable.Node.Guid, output.NonReferenceable.Node.Guid);
			}
		}

		private ObjectList BuildObjectList(object obj)
		{
			var visitedObjects = new HashSet<object>();
			var objectList = new ObjectList();
			TraverseObjectGraph(visitedObjects, objectList, obj);
			// Sort by Guid to provide a stable json, but keep the root object the first.
			objectList.Sort(1, objectList.Count - 1, new ReferenceableComparer());
			return objectList;
		}

		class ReferenceableComparer : IComparer<IReferenceable>
		{
			public int Compare(IReferenceable x, IReferenceable y) => x.Guid.CompareTo(y.Guid);
		}

		class ObjectList : List<IReferenceable> { }

		readonly CommonOptions commonOptions = new CommonOptions();

		private void TraverseObjectGraph(HashSet<object> visitedObjects, ObjectList objectList, object obj)
		{
			if (!visitedObjects.Add(obj)) {
				return;
			}
			if (obj is IEnumerable<object> ie) {
				foreach (var o in ie) {
					TraverseObjectGraph(visitedObjects, objectList, o);
				}
				return;
			}
			if (obj is IReferenceable referenceable) {
				objectList.Add(referenceable);
			}
			var meta = Yuzu.Metadata.Meta.Get(obj.GetType(), commonOptions);
			foreach (var i in meta.Items) {
				if (i.Type.IsClass || i.Type.IsInterface) {
					if (i.GetValue(obj) is { } o) {
						TraverseObjectGraph(visitedObjects, objectList, i.GetValue(obj));
					}
				}
			}
		}

		public interface IReferenceable
		{
			Guid Guid { get; }
		}

		public class Node : IReferenceable
		{
			[YuzuMember]
			public Guid Guid { get; set; }

			public Node()
			{
				Guid = Guid.NewGuid();
			}

			[YuzuMember]
			[YuzuMerge]
			public readonly List<Node> Nodes = new List<Node>();

			[YuzuMember]
			public NonReferenceableNode NonReferenceable;
		}

		public class NonReferenceableNode
		{
			[YuzuMember]
			public Node Node;
		}

		List<Func<object, object>> roundtrips = new List<Func<object, object>> {
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
			}
		};

		private class ReferenceResolver : IDeserializerReferenceResolver, ISerializerReferenceResolver
		{
			private readonly Dictionary<Guid, object> referenceToObjects = new Dictionary<Guid, object>();

			public Type ReferenceType() => typeof(Guid);

			public object GetObject(object reference)
			{
				if (!referenceToObjects.TryGetValue((Guid)reference, out var obj)) {
					throw new YuzuException($"Unresolved reference: {reference}");
				}
				return obj;
			}

			public bool TryGetObject(object reference, out object obj)
			{
				return referenceToObjects.TryGetValue((Guid)reference, out obj);
			}

			public void AddObject(object reference, object obj) => referenceToObjects.Add((Guid)reference, obj);

			public bool TryGetReference(object obj, object owner, out object reference, out bool writeObject)
			{
				if (obj is IReferenceable referenceable) {
					reference = referenceable.Guid;
					writeObject = owner is ObjectList;
					return true;
				}
				writeObject = false;
				reference = default;
				return false;
			}

			public void Clear()
			{
				referenceToObjects.Clear();
			}

			public bool IsEmpty() => referenceToObjects.Count == 0;
		}
	}
}
