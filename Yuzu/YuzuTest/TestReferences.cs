using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Yuzu;
using Yuzu.Json;
using Yuzu.Util;
using YuzuTestAssembly;
using YuzuGen.YuzuTest;
using System.Dynamic;

namespace YuzuTest
{
	[TestClass]
	public class TestReferences
	{
		private class IndexReference : IReference
		{
			private static int staticIndex;

			[YuzuRequired]
			public int Index = 0;

			public IndexReference()
			{
				Index = staticIndex++;
			}
		}

		private class GuidReference : IReference
		{
			[YuzuRequired]
			public Guid Guid;

			public GuidReference()
			{

			}
		}

		public interface IReference
		{

		}

		class ReferenceResolver<TReference> : IReferenceResolver, IReferenceResolver<TReference> where TReference : class, IReference, new()
		{
			//System.Runtime.CompilerServices.ConditionalWeakTable<object, TReference>
			private static Dictionary<object, TReference> valueToReference = new Dictionary<object, TReference>();
			private static Dictionary<TReference, object> referenceToValue = new Dictionary<TReference, object>();

			public void AddReference(TReference reference, object value)
			{
				valueToReference.Add(value, reference);
				referenceToValue.Add(reference, value);
			}

			public void AddReference(object reference, object value)
			{
				AddReference((TReference)reference, value);
			}

			public TReference GetReference(object value)
			{
				if (value is TReference) {
					throw new InvalidOperationException();
				}
				if (!valueToReference.TryGetValue(value, out TReference reference)) {
					valueToReference.Add(value, reference = new TReference());
					referenceToValue.Add(reference, value);
				}
				return reference;
			}

			public bool IsReferenced(object value)
			{
				if (value is TReference) {
					return false;
				}
				return valueToReference.TryGetValue(value, out _);
			}

			public Type ReferenceType()
			{
				return typeof(TReference);
			}

			public object ResolveReference(TReference reference)
			{
				return referenceToValue[reference];
			}

			public object ResolveReference(object reference)
			{
				return ResolveReference((TReference)reference);
			}

			object IReferenceResolver.GetReference(object value)
			{
				return GetReference(value);
			}
		}

		public class Node
		{
			[YuzuRequired]
			public string name;

			[YuzuRequired]
			public List<Node> Nodes { get; set; }

			[YuzuRequired]
			public Node Pes;
		}

		public class TestReferences1
		{
			[YuzuRequired]
			public Node A;

			[YuzuRequired]
			public Node B;

			[YuzuRequired]
			public Node C;
		}

		private class Pes
		{
			[YuzuRequired]
			public object Sobaka;
		}

		[TestMethod]
		public void Foo2()
		{
			JsonSerializer js = new JsonSerializer
			{
				ReferenceResolver = new ReferenceResolver<IndexReference>(),
				JsonOptions = new JsonSerializeOptions
				{
					SaveClass = JsonSaveClass.UnknownPrimitive
				}
			};

			var t = new Pes {  Sobaka = Guid.NewGuid() };

			var s = js.ToString(t);

			//Assert.AreEqual(2, 2);
		}

		[TestMethod]
		public void Foo()
		{
			JsonSerializer js = new JsonSerializer {
				ReferenceResolver = new ReferenceResolver<IndexReference>()
			};
			var node_b = new Node
			{
				name = "BBB"
			};
			var node_c = new Node
			{
				name = "CCC"
			};
			var node_a = new Node {
				name = "AAA",
				Nodes = new List<Node>
				{
					node_b,
					node_c
				},

			};
			var t = new TestReferences1
			{
				A = node_a,
				B = node_a,
				C = node_a,
			};
			//var tt = TopoSort(js.Options, t);
			var s = js.ToString(t);

			var jd = new JsonDeserializer {
				ReferenceResolver = new ReferenceResolver<IndexReference>()
			};

			var t2 = jd.FromString<TestReferences1>(s);

			Assert.IsTrue(s != null);
		}

		public List<object> TopoSort(Yuzu.CommonOptions options, object o)
		{
			var r = new List<object>();
			var g = new Dictionary<object, List<object>>();
			var stack = new Stack<object>();
			//g.Add(o, new List<object>());
			//stack.Push(o);
			//while (stack.Count != 0) {
			dfs(o);
			void dfs(object s) {
				//var s = stack.Pop();
				var meta = Yuzu.Metadata.Meta.Get(s.GetType(), options);
				if (g.TryGetValue(s, out var l)) {
					throw new InvalidOperationException("cyclic reference");
				} else {
					g.Add(s, l = new List<object>());
				}
				foreach (var yi in meta.Items) {
					var t = yi.Type;
					if (t.IsGenericType)
					{
						//if (Yuzu.Util.Utils.GetICollection)
						// if (Yuzu.Util.Utils.GetIDictionary)
					}
					if (!JsonSerializer.IsUserObject(t)) {
						continue;
					}
					var v = yi.GetValue(s);
					if (v == null) {
						continue;
					}
					l.Add(v);
					//stack.Push(v);
					dfs(v);
				}
				r.Add(s);
			}
			return r;
		}

	}
}
