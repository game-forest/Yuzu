using System;
using System.Collections;
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
	public class TestReadIntoDictionary
	{
		[TestMethod]
		public void TestObjectGraph()
		{
			var graph = new Widget {
				Id = 1,
				Guid = Guid.NewGuid(),
				Position = new Vector2 { X = 1, Y = 2 }
			};
			graph.Nodes.Add(new Widget { Position = new Vector2 { X = 3, Y = 4 } });
			graph.Nodes.Add(new Widget { Position = new Vector2 { X = 5, Y = 6 } });
			graph.Nodes.Add(new DerivedWidget {
				Dictionary = new Dictionary<int, int> { { 6, 7 } },
				SomeObject = new Vector2 { X = -5, Y = -1 },
				Array = new double[2] { 1.3f, 4.5f }
			});
			var t1 = ObjectToDictionaryTreeUsingJsonSerialization(graph);
			var t2 = ObjectToDictionaryTreeUsingBinarySerialization(graph);
			CompareDictionaryTrees(t1, t2);
		}

		private void CompareDictionaryTrees(object o1, object o2)
		{
			if (o1.GetType().IsPrimitive) {
				Assert.AreEqual(o1, Convert.ChangeType(o2, o1.GetType()));
			} else if (o1 is string s1) {
				Assert.IsTrue(o2 is string);
				Assert.AreEqual(s1, (string)o2);
			} else if (o1 is IDictionary d1) {
				if (d1.Contains("class")) {
					d1.Remove("class");
				}
				var d2 = o2 as IDictionary;
				Assert.IsNotNull(d2);
				Assert.AreEqual(d1.Count, d2.Count);
				foreach (var i in d1) {
					var e = (DictionaryEntry)i;
					Assert.IsTrue(d2.Contains(e.Key));
					CompareDictionaryTrees(e.Value, d2[e.Key.ToString()]);
				}
			} else if (o1 is IList l1) {
				var l2 = o2 as IList;
				Assert.IsNotNull(l2);
				Assert.AreEqual(l1.Count, l2.Count);
				for (int i = 0; i < l1.Count; i++) {
					CompareDictionaryTrees(l1[i], l2[i]);
				}
			} else {
				Assert.Fail();
			}
		}

		[YuzuCompact]
		public struct Vector2
		{
			[YuzuMember]
			public double X;

			[YuzuMember]
			public double Y;
		}

		public class Widget
		{
			[YuzuMember]
			public int Id;

			[YuzuMember]
			public Guid Guid;

			[YuzuMember]
			public Vector2 Position;

			[YuzuMember]
			[YuzuMerge]
			public readonly List<Widget> Nodes = new List<Widget>();
		}

		public class DerivedWidget : Widget
		{
			[YuzuMember]
			public Dictionary<int, int> Dictionary;

			[YuzuMember]
			public double[] Array;

			[YuzuMember]
			public object SomeObject;
		}

		object ObjectToDictionaryTreeUsingJsonSerialization(object obj)
		{
			var s = new JsonSerializer {
				JsonOptions = new JsonSerializeOptions {
					SaveClass = JsonSaveClass.UnknownOrRoot,
				},
			};
			var d = new JsonDeserializer {
				Options = new CommonOptions { DeserializeAsUnknown = true },
				JsonOptions = new JsonSerializeOptions {
					SaveClass = JsonSaveClass.KnownRoot,
					UnknownNumberType = JsonUnknownNumberType.Minimal,
				},
			};
			var buf = s.ToString(obj);
			return d.FromString(buf);
		}

		object ObjectToDictionaryTreeUsingBinarySerialization(object obj)
		{
			var s = new BinarySerializer();
			var d = new BinaryDeserializer {
				Options = new CommonOptions { DeserializeAsUnknown = true }
			};
			var buf = s.ToBytes(obj);
			return YuzuUnknownBinaryToDictionaryTree(d.FromBytes(buf), compactAsDictionary: false);
		}

		object YuzuUnknownBinaryToDictionaryTree(object obj, bool compactAsDictionary)
		{
			if (obj.GetType().IsPrimitive) {
				return obj;
			}
			if (obj is Guid guid) {
				return obj.ToString();
			}
			if (obj is YuzuUnknownBinary unknown) {
				if (unknown.Def.Meta.IsCompact && !compactAsDictionary) {
					var r = new List<object>();
					foreach (var f in unknown.Def.Fields) {
						if (f.OurIndex != ReaderClassDef.EOF) {
							r.Add(YuzuUnknownBinaryToDictionaryTree(unknown.Fields[f.Name], compactAsDictionary: false));
						}
					}
					return r;
				} else {
					var r = new Dictionary<string, object>();
					foreach (var f in unknown.Def.Fields) {
						if (f.OurIndex != ReaderClassDef.EOF && unknown.Fields.TryGetValue(f.Name, out var v)) {
							r.Add(f.Name, YuzuUnknownBinaryToDictionaryTree(v, compactAsDictionary: f.Type == typeof(object)));
						}
					}
					return r;
				}
			} else if (obj is IDictionary dictionary) {
				var r = new Dictionary<object, object>();
				foreach (var i in dictionary) {
					var e = (DictionaryEntry)i;
					r.Add(e.Key.ToString(), YuzuUnknownBinaryToDictionaryTree(e.Value, compactAsDictionary: true));
				}
				return r;
			} else if (obj is ICollection collection) {
				var r = new List<object>();
				foreach (var i in collection) {
					r.Add(YuzuUnknownBinaryToDictionaryTree(i, compactAsDictionary: true));
				}
				return r; 
			} else if (obj is IList l) {
				var r = new List<object>();
				foreach (var i in l) {
					r.Add(YuzuUnknownBinaryToDictionaryTree(i, compactAsDictionary: true));
				}
				return r;
			} else {
				throw new InvalidOperationException($"Unexpected type {obj.GetType()}");
			}
		}
	}
}
