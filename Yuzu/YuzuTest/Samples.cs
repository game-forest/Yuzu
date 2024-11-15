using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;

using Yuzu;
using YuzuTestAssembly;

namespace YuzuTest
{
	public class SampleBase
	{
		[YuzuRequired("0_FBase")]
		public int FBase;
	}

	public class SampleDerivedA : SampleBase
	{
		[YuzuRequired]
		public int FA;
	}

	public class SampleDerivedB : SampleBase
	{
		[YuzuRequired]
		public int FB;
	}

	public sealed class SampleSealed
	{
		[YuzuRequired]
		public int FB;
	}

	public class Empty
	{
	}

	public class SampleEmptyDerivied: Empty
	{
		[YuzuRequired]
		public int D;
	}

	[ProtoContract]
	public class Sample1
	{
		[YuzuRequired]
		[ProtoMember(1)]
		public int X;

		[YuzuOptional]
		[YuzuDefault("ttt")]
		[ProtoMember(2)]
		public string Y = "zzz";
	}

	public partial class Sample2
	{
		[YuzuRequired]
		public int X { get; set; }

		[YuzuOptional]
		[YuzuSerializeIf(nameof(SaveYIf))]
		public string Y { get; set; }

		public bool SaveYIf() => X.ToString() != Y;
	}

	public partial class SampleSerializeIf
	{
		[YuzuRequired]
		public int X { get; set; }

		[YuzuOptional]
		[YuzuSerializeIf(nameof(SaveYIf))]
		public Sample1 Y { get; set; }

		public bool SaveYIf() => X != Y.X;
	}

	public partial class SampleSerializeIfOnField
	{
		[YuzuRequired]
		public int X { get; set; }

		[YuzuOptional]
		[YuzuSerializeIf(nameof(SaveYIf))]
		public Sample1 Y;

		[YuzuOptional]
		[YuzuSerializeIf(nameof(SaveYIf))]
		public Sample1 Z, W;

		public bool SaveYIf() => X != Y.X;
	}

	public class SampleNoGen
	{
		[YuzuRequired]
		public int Z;
	}

	public class SampleGenNoGen
	{
		[YuzuRequired]
		public SampleNoGen NG;
	}

	[YuzuCopyable]
	public class SampleCopyable {
		[YuzuRequired]
		public int X;
	}

	public class SampleWithCopyable
	{
		[YuzuRequired]
		public SampleCopyable P = new();
	}

	public class SampleWithCopyableItems
	{
		[YuzuRequired, YuzuCopyable]
		public Sample1 P = new();
		[YuzuRequired, YuzuCopyable]
		public List<int> L = [];
	}

	[YuzuAllowReadingFromAncestor]
	public class Sample2Allow : Sample2
	{
		public int Extra = 0;
	}

	public class Sample3
	{
		[YuzuRequired]
		public Sample1 S1 { get; set; }
		[YuzuOptional("S11")]
		public int F;
		[YuzuOptional]
		public Sample2 S2;
	}

	public enum SampleEnum { E1, E2, E3 };
	public class Sample4
	{
		[YuzuOptional]
		public SampleEnum E;
	}

	public enum SampleEnumByte : byte { EB1, EB2, EB3 };
	public enum SampleEnumLong : long { EL1, EL2, Large = 1L << 50 };
	public class SampleEnumMemberTyped
	{
		[YuzuOptional]
		public SampleEnumByte Eb;
		[YuzuOptional]
		public SampleEnumLong El;
	}

	public class SampleLong
	{
		[YuzuRequired]
		public long S;
		[YuzuRequired]
		public ulong U;
	}

	public class SampleBoolBase { }

	public class SampleBool : SampleBoolBase
	{
		[YuzuRequired]
		public bool B;
	}

	[YuzuMust]
	public class SampleSmallTypes
	{
		[YuzuRequired]
		public char Ch;
		[YuzuRequired]
		public short Sh;
		[YuzuRequired]
		public ushort USh;
		[YuzuRequired]
		public byte B;
		[YuzuRequired]
		public sbyte Sb;
	}

	public class SampleFloat
	{
		[YuzuRequired("1")]
		public float F;
		[YuzuRequired("2")]
		public double D;
	}

	public class SampleDecimal
	{
		[YuzuRequired]
		public decimal N;
	}

	public class SampleNullable
	{
		[YuzuRequired]
		public int? N;
	}

	public class SampleMethodOrder
	{
		[YuzuRequired("4")]
		public int P2 { get; set; }
		[YuzuRequired("2")]
		public int P1 { get; set; }
		public int F_no;
		[YuzuRequired("1")]
		public int F1;
		[YuzuRequired("3")]
		public int F2;
		public int Func() { return 0; }
	}

	[ProtoContract]
	public class SampleList
	{
		[YuzuRequired]
		[ProtoMember(1)]
		public List<string> E;
	}

	public class SampleEmptyList
	{
		[YuzuMember]
		public List<string> E = [];
	}

	public class SampleArray
	{
		[YuzuRequired]
		public string[] A;
	}

	public class SampleArrayOfArray
	{
		[YuzuRequired]
		public int[][] A;
	}

	public class SampleArrayNDim
	{
		[YuzuRequired]
		public int[,] A;
		[YuzuRequired]
		public string[,,] B;

		public static void AssertBoundsAreEqual(Array expected, Array actual)
		{
			for (int dim = 0; dim < expected.Rank; ++dim) {
				Assert.AreEqual(expected.GetLowerBound(dim), actual.GetLowerBound(dim));
				Assert.AreEqual(expected.GetUpperBound(dim), actual.GetUpperBound(dim));
			}
		}

		public void AssertAreEqual(SampleArrayNDim actual)
		{
			AssertBoundsAreEqual(A, actual.A);
			if (A.Length > 0)
				Assert.AreEqual(
					A[A.GetLowerBound(0), A.GetLowerBound(1)],
					actual.A[A.GetLowerBound(0), A.GetLowerBound(1)]);
			if (A.Length > 1)
				Assert.AreEqual(
					A[A.GetUpperBound(0), A.GetUpperBound(1)],
					actual.A[A.GetUpperBound(0), A.GetUpperBound(1)]);

			if (B == null) {
				Assert.IsNull(actual.B);
				return;
			}
			AssertBoundsAreEqual(B, actual.B);
			if (B.Length > 0)
				Assert.AreEqual(
					B[B.GetLowerBound(0), B.GetLowerBound(1), B.GetLowerBound(2)],
					actual.B[B.GetLowerBound(0), B.GetLowerBound(1), B.GetLowerBound(2)]);
			if (B.Length > 1)
				Assert.AreEqual(
					B[B.GetUpperBound(0), B.GetUpperBound(1), B.GetUpperBound(2)],
					actual.B[B.GetUpperBound(0), B.GetUpperBound(1), B.GetUpperBound(2)]);
		}
	}

	public class SampleArrayNDimOfClass
	{
		[YuzuRequired]
		public Sample1[,,] A;
	}

	public class SampleArrayOfClass
	{
		[YuzuRequired]
		public Sample1[] A;
	}

	public class SampleRec
	{
		[YuzuRequired]
		public SampleRec Child;
		[YuzuRequired]
		public string S;
	}

	public class SampleTree
	{
		[YuzuRequired("a")]
		public int Value;
		[YuzuOptional("b")]
		public List<SampleTree> Children;
	}

	public class SampleClassList
	{
		[YuzuRequired]
		public List<SampleBase> E;
	}

	public class SampleDict
	{
		[YuzuRequired("a")]
		public int Value;
		[YuzuOptional("b")]
		public Dictionary<string, SampleDict> Children;
	}

	public class SampleSortedDict
	{
		[YuzuMember]
		public SortedDictionary<string, int> d;
	}

	public class SampleSortedDictOfClass
	{
		[YuzuMember]
		public SortedDictionary<string, Sample1> d;
	}

	public class SampleKey : IEquatable<SampleKey>
	{
		[YuzuRequired]
		public int V;
		public override string ToString() { return V.ToString() + "!"; }
		public bool Equals(SampleKey other) { return V == other.V; }
		public override int GetHashCode() { return V; }
	}

	public class SampleDictKeys
	{
		[YuzuRequired]
		public Dictionary<int, int> I;
		[YuzuRequired]
		public Dictionary<SampleEnum, int> E;
		[YuzuRequired]
		public Dictionary<SampleKey, int> K;
	}

	public class SampleMatrix
	{
		[YuzuRequired]
		public List<List<int>> M;
	}

	[YuzuCompact]
	public struct SamplePoint
	{
		[YuzuRequired]
		public int X;
		[YuzuRequired]
		public int Y;

		public override bool Equals(object obj)
		{
			return ((SamplePoint)obj).X == X && ((SamplePoint)obj).Y == Y;
		}

		public override int GetHashCode() { return X ^ Y; }
	}

	[YuzuCompact]
	public class SampleOnelineRect
	{
		[YuzuRequired]
		public SamplePoint A;
		[YuzuRequired]
		public SamplePoint B;
	}

	[YuzuCompact, YuzuAll(YuzuItemOptionality.Required)]
	public class SampleOneline
	{
		public SamplePoint Point0;
		public SampleOnelineRect Rect;
		public string Name;
		public SampleEnum Type;
		public SamplePoint Point1;
	}

	public class SampleRect
	{
		[YuzuRequired]
		public SamplePoint A;
		[YuzuRequired]
		public SamplePoint B;
	}

	public struct SampleStructWithClass
	{
		[YuzuRequired]
		public Sample1 A;
	}

	[YuzuAll]
	public class SampleDefault
	{
		public int A = 3;
		public string B = "default";
		public SamplePoint P;

		public SampleDefault() { P = new SamplePoint { X = 7, Y = 2 }; }
	}

	public class SampleObj
	{
		[YuzuRequired]
		public object F;
	}

	public class SampleItemObj
	{
		[YuzuRequired]
		public List<object> L;
		[YuzuRequired]
		public Dictionary<string, object> D;
	}

	public class SampleDate
	{
		[YuzuRequired]
		public DateTime D;
		[YuzuRequired]
		public DateTimeOffset DOfs;
		[YuzuRequired]
		public TimeSpan T;
	}

	internal class SampleGuid
	{
		[YuzuRequired]
		public Guid G;
	}

	public class SampleWithNullField
	{
		[YuzuRequired]
		public string About = null;
	}

	[YuzuCompact]
	public class SampleWithNullFieldCompact
	{
		[YuzuRequired]
		public Sample1 N;
	}

	[YuzuCompact]
	[ProtoContract]
	public class Color
	{
		[YuzuRequired]
		[ProtoMember(1)]
		public byte R;

		[YuzuRequired]
		[ProtoMember(2)]
		public byte G;

		[YuzuRequired]
		[ProtoMember(3)]
		public byte B;
	}

	[ProtoContract]
	public class SamplePerson
	{
		public static int Counter = 0;

		[YuzuRequired("1")]
		[ProtoMember(1)]
		public string Name { get; set; }

		[YuzuRequired("2")]
		[ProtoMember(2)]
		public DateTime Birth;

		[YuzuRequired("3")]
		[ProtoMember(3)]
		public List<SamplePerson> Children;

		[YuzuRequired("4")]
		[ProtoMember(4)]
		public Color EyeColor;

		public SamplePerson() { }

		public SamplePerson(Random rnd, int depth)
		{
			Counter++;
			StringBuilder sb = new();
			var len = rnd.Next(1, 40);
			for (int i = 0; i < len; ++i)
				sb.Append((char)rnd.Next((int)'a', (int)'z' + 1));
			Name = sb.ToString();
			Birth = new DateTime(1999, rnd.Next(10) + 1, 13);
			var childCount = rnd.Next(28 / depth);
			Children = [];
			for (int i = 0; i < childCount; ++i)
				Children.Add(new SamplePerson(rnd, depth + 1));
			EyeColor = new Color { R = (byte)rnd.Next(256), G = (byte)rnd.Next(256), B = (byte)rnd.Next(256) };
		}
	}

	public class SampleSelfDelegate
	{
		[YuzuRequired]
		public int x;
		[YuzuRequired]
		public Action<int> OnSomething;

		public void Handler1(int v) { x += v; }
		public void Handler2(int v) { x *= v; }
	}

	public interface ISample
	{
		int X { get; set; }
	}

	public interface ISampleMember
	{
		[YuzuMember]
		int X { get; set; }
	}

	public abstract class SampleMemberAbstract
	{
		[YuzuMember]
		public int X = 72;
	}

	public class SampleMemberConcrete : SampleMemberAbstract { }

	public class SampleMemberI : ISampleMember
	{
		public int X { get; set; }
		public SampleMemberI() { X = 71; }
	}

	public class SampleInterfaced : ISample
	{
		[YuzuRequired]
		public int X { get; set; }
	}

	public class SampleInterfacedGeneric<T> : ISample
	{
		[YuzuRequired]
		public int X { get; set; }

		[YuzuRequired]
		public T G;
	}

	public class SampleInterfaceField
	{
		[YuzuRequired]
		public ISample I { get; set; }
	}

	public interface ISampleField
	{
		[YuzuRequired]
		int X { get; set; }
	}

	public class SampleInterfacedField : ISampleField
	{
		public int X { get; set; }
	}

	public class SampleInterfacedFieldDup : ISampleField
	{
		[YuzuOptional]
		public int X { get; set; }
	}

	public abstract class SampleAbstract { }

	public class SampleConcrete : SampleAbstract
	{
		[YuzuRequired]
		public int XX;
	}

	public partial class SampleCollection<T> : ICollection<T>
	{
		private List<T> impl = [];
		public int Count { get { return impl.Count; } }
		public bool IsReadOnly { get { return false; } }
		public void Add(T item) { impl.Add(item); }
		public void Clear() { impl.Clear(); }
		public bool Contains(T item) { return impl.Contains(item); }
		public void CopyTo(T[] array, int arrayIndex) { impl.CopyTo(array, arrayIndex); }
		public bool Remove(T item) { return impl.Remove(item); }
		IEnumerator<T> IEnumerable<T>.GetEnumerator() { return impl.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return impl.GetEnumerator(); }

		public int Filter = 0;
		[YuzuSerializeItemIf]
		public bool SaveItemIf(int index, object item) => Filter switch
		{
			1 => index % 2 == 0,
			2 => (int)item % 2 == 0,
			_ => Filter != 3
		};
	}

	public readonly partial record struct A
	{
		public partial record class B
		{
			public readonly partial record struct C
			{
				public static partial class D
				{
					public partial interface E {
						public partial struct Sample2Struct
						{
							[YuzuRequired]
							public int X { get; set; }

							[YuzuOptional]
							[YuzuSerializeIf(nameof(SaveYIf))]
							public string Y { get; set; }

							public bool SaveYIf() => X.ToString() != Y;
						}

						public partial struct SampleSerializeIfStruct
						{
							[YuzuRequired]
							public int X { get; set; }

							[YuzuOptional]
							[YuzuSerializeIf(nameof(SaveYIf))]
							public Sample1 Y { get; set; }

							public bool SaveYIf() => X != Y.X;
						}

						public partial struct SampleSerializeIfOnFieldStruct
						{
							[YuzuRequired]
							public int X { get; set; }

							[YuzuOptional]
							[YuzuSerializeIf(nameof(SaveYIf))]
							public Sample1 Y;

							[YuzuOptional]
							[YuzuSerializeIf(nameof(SaveYIf))]
							public Sample1 Z, W;

							public bool SaveYIf() => X != Y.X;
						}
					}
				}
			}
		}
	}

	public class SampleExplicitCollection<T> : ICollection<T>
	{
		private List<T> impl = [];
		int ICollection<T>.Count { get { return impl.Count; } }
		bool ICollection<T>.IsReadOnly { get { return false; } }
		void ICollection<T>.Add(T item) { impl.Add(item); }
		void ICollection<T>.Clear() { impl.Clear(); }
		bool ICollection<T>.Contains(T item) { return impl.Contains(item); }
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) { impl.CopyTo(array, arrayIndex); }
		bool ICollection<T>.Remove(T item) { return impl.Remove(item); }
		IEnumerator<T> IEnumerable<T>.GetEnumerator() { return impl.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return impl.GetEnumerator(); }
	}

	public class SampleMultiCollection : List<string>, ICollection<int>
	{
		private List<int> impl = [];
		int ICollection<int>.Count { get { return impl.Count; } }
		bool ICollection<int>.IsReadOnly { get { return false; } }
		void ICollection<int>.Add(int item) { impl.Add(item); }
		void ICollection<int>.Clear() { impl.Clear(); }
		bool ICollection<int>.Contains(int item) { return impl.Contains(item); }
		void ICollection<int>.CopyTo(int[] array, int arrayIndex) { impl.CopyTo(array, arrayIndex); }
		bool ICollection<int>.Remove(int item) { return impl.Remove(item); }
		IEnumerator<int> IEnumerable<int>.GetEnumerator() { return impl.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return impl.GetEnumerator(); }
	}

	public class SampleCollectionWithField<T> : SampleCollection<T>
	{
		[YuzuRequired]
		public int X;
	}

	public class SampleConcreteCollection : SampleCollection<int> { }

	public class SampleWithCollection
	{
		[YuzuRequired]
		public SampleCollection<ISample> A = [];
		[YuzuMember]
		public SampleCollection<int> B = [];
		[YuzuMember]
		public SampleCollection<SamplePoint> C = [];
	}

	public class SampleWithCollectionMerge
	{
		[YuzuRequired]
		public SampleCollection<int> A { get; } = [];
	}

	public class SampleWithCollectionDefault
	{
		[YuzuMember]
		public List<int> B = [1];
	}

	public class SampleWithCollectionDefaultNonSerializable
	{
		[YuzuMember]
		public SampleCollection<int> B = [];
		public SampleWithCollectionDefaultNonSerializable()
		{
			B.Add(1);
			B.Filter = 3;
		}
	}

	public class SampleIEnumerable
	{
		[YuzuRequired]
		public IEnumerable<int> L = [1, 2, 3];
	}

	public class SampleBeforeSerialization
	{
		[YuzuRequired]
		public string X;
		[YuzuBeforeSerialization]
		public void Before() { X += "1"; }
	}

	public class SampleBefore2 : SampleBeforeSerialization
	{
		[YuzuBeforeSerialization]
		public void Before2() { X += "2"; }
		[YuzuBeforeSerialization]
		public void Before3() { X += "3"; }
	}

	public class SampleAfterSerialization
	{
		[YuzuRequired]
		public string X;
		[YuzuAfterSerialization]
		public void After() { X += "1"; }
	}

	public class SampleBeforeDeserialization
	{
		private string hidden = "X";
		[YuzuRequired]
		public string X { get { return hidden; }  set { hidden += value; } }
		[YuzuBeforeDeserialization]
		public void Before() { hidden = "2"; }
	}

	public class SampleAfterDeserialization
	{
		[YuzuRequired]
		public string X;
		[YuzuAfterDeserialization]
		public void After() { X += "1"; }
	}

	public class SampleAfter2 : SampleAfterDeserialization
	{
		[YuzuAfterDeserialization]
		public void After2() { X += "2"; }
		[YuzuAfterDeserialization]
		public void After3() { X += "3"; }
	}

	public class SampleMerge
	{
		[YuzuRequired]
		public Dictionary<int, int> DI { get; } = [];
		[YuzuRequired]
		public List<int> LI { get; } = [];
		[YuzuOptional, YuzuMerge]
		public Sample1 M;

		public static SampleMerge Make() => new() { M = new Sample1() };
	}

	public class SampleMergeNonPrimitive
	{
		[YuzuRequired]
		public Dictionary<int, Sample1> DI { get; } = [];
		[YuzuRequired]
		public List<Sample1> LI { get; } = [];
		[YuzuOptional, YuzuMerge]
		public Sample1 M = new();
	}

	public class SampleNested
	{
		public enum NestedEnum { One, Two };
		public class NestedClass
		{
			[YuzuOptional]
			public int Z;
		}
		[YuzuRequired]
		public NestedEnum E;
		[YuzuRequired]
		public NestedClass C;
		[YuzuMember]
		public NestedEnum[] Z = null;
	}

	public class SampleAssemblyDerivedR : SampleAssemblyBase
	{
		[YuzuMember]
		public string R = "R";
	}

	public class SampleClonerGenDerived
	{
		[YuzuMember]
		public Sample1 S = new();
	}

	public class SampleUnknown
	{
		[YuzuMember]
		public int X;
		public YuzuUnknownStorage Storage = new();
	}

	public class SampleUnknownDictOfLists
	{
		[YuzuMember]
		public Dictionary<int, List<SampleBoolBase>> F;

		public static MetaOptions Override() =>
			new MetaOptions().AddOverride(
				typeof(SampleUnknownDictOfLists),
				o => o.AddAttr(new YuzuAlias("Something")));

		public static SampleUnknownDictOfLists Sample = new() {
			F = new Dictionary<int, List<SampleBoolBase>> {
				{ 7, [new SampleBool { B = true }] } }
		};
	}

	public class SampleSurrogateColor
	{
		public int R, G, B;
		[YuzuToSurrogate]
		public string ToSurrogate() { return String.Format("#{0:X2}{1:X2}{2:X2}", R, G, B); }
		[YuzuFromSurrogate]
		public static SampleSurrogateColor FromSurrogate(string s) {
			if (s[0] != '#')
				throw new Exception("Bad color: " + s);
			return new SampleSurrogateColor {
				R = int.Parse(s.Substring(1, 2), NumberStyles.AllowHexSpecifier),
				G = int.Parse(s.Substring(3, 2), NumberStyles.AllowHexSpecifier),
				B = int.Parse(s.Substring(5, 2), NumberStyles.AllowHexSpecifier),
			};
		}
	}

	[YuzuCompact]
	public class SampleSurrogateColorIf
	{
		[YuzuRequired]
		public int R, G, B;
		public static bool S;
		[YuzuSurrogateIf]
		public virtual bool SurrogateIf() { return S; }
		[YuzuToSurrogate]
		public static int ToSurrogate(SampleSurrogateColorIf c)
		{
			return c.R * 10000 + c.G * 100 + c.B;
		}
	}

	[YuzuCompact]
	public class SampleSurrogateColorIfDerived : SampleSurrogateColorIf
	{
		public override bool SurrogateIf() { return true; }
		[YuzuFromSurrogate]
		public static SampleSurrogateColorIfDerived FromSurrogate(int x)
		{
			return new SampleSurrogateColorIfDerived {
				R = x / 10000,
				G = (x % 10000) / 100,
				B = x % 100,
			};
		}
	}

	public class SampleSurrogateClass
	{
		public bool FB;
		[YuzuToSurrogate]
		public static SampleBool ToSurrogate(SampleSurrogateClass obj) { return new SampleBool { B = obj.FB }; }
		[YuzuFromSurrogate]
		public static SampleSurrogateClass FromSurrogate(SampleBool obj)
		{
			return new SampleSurrogateClass { FB = obj.B };
		}
	}

	public class SampleCompactSurrogate
	{
		public int X, Y;
		[YuzuToSurrogate]
		public static SamplePoint ToSurrogate(SampleCompactSurrogate obj)
		{
			return new SamplePoint { X = obj.X, Y = obj.Y };
		}
		[YuzuFromSurrogate]
		public static SampleCompactSurrogate FromSurrogate(SamplePoint obj)
		{
			return new SampleCompactSurrogate { X = obj.X, Y = obj.Y };
		}
	}

	public class SampleSurrogateHashSet: HashSet<char>
	{
		[YuzuToSurrogate]
		public static string ToSurrogate(SampleSurrogateHashSet obj) =>
			string.Join("", obj.OrderBy(ch => ch));

		[YuzuFromSurrogate]
		public static SampleSurrogateHashSet FromSurrogate(string s)
		{
			var result = new SampleSurrogateHashSet();
			foreach (var ch in s)
				result.Add(ch);
			return result;
		}
	}

	[YuzuCompact]
	public struct SampleStructWithProps
	{
		[YuzuRequired]
		public int A { get; set; }
		[YuzuRequired]
		public SamplePoint P { get; set; }
	}

	public class SamplePrivateConstructor
	{
		[YuzuRequired]
		public int X;

		private SamplePrivateConstructor() {}

		[YuzuFactory]
		public static SamplePrivateConstructor Make() => new();
	}

	public class SampleConstructorParam
	{
		[YuzuOptional]
		public int X;

		public SampleConstructorParam(int x) { X = x; }

		[YuzuFactory]
		public static SampleConstructorParam Make() => new(72);
	}

	[ProtoContract]
	public class SampleAoS
	{
		[ProtoContract]
		[YuzuCompact]
		public struct Vertex
		{
			[YuzuRequired]
			[ProtoMember(1)]
			public float X;
			[YuzuRequired]
			[ProtoMember(2)]
			public float Y;
			[YuzuRequired]
			[ProtoMember(3)]
			public float Z;
		}
		[ProtoContract]
		[YuzuCompact]
		public struct Color
		{
			[YuzuRequired]
			[ProtoMember(1)]
			public byte R;
			[YuzuRequired]
			[ProtoMember(2)]
			public byte G;
			[YuzuRequired]
			[ProtoMember(3)]
			public byte B;
		}
		[ProtoContract]
		[YuzuCompact]
		public class S {
			[YuzuRequired]
			[ProtoMember(1)]
			public Vertex V;
			[YuzuRequired]
			[ProtoMember(2)]
			public Color C;
		}

		[YuzuRequired]
		[ProtoMember(1)]
		public List<S> A = [];
	}

	public class Bad1
	{
		[YuzuRequired]
		[YuzuOptional]
		public int F;
	}

	public class Bad2
	{
		[YuzuRequired("привет")]
		public int F;
	}

	public class Bad3
	{
		[YuzuRequired("q")]
		public int F;
		[YuzuRequired("q")]
		public int G;
	}

	public class BadPrivate
	{
		[YuzuRequired]
		private int F = 0;
		public int G { get { return F; } }
	}

	public class BadPrivateGetter
	{
		[YuzuRequired]
		public int F { private get; set; }
	}

	public class BadMerge1
	{
		[YuzuRequired]
		public int F { get { return 1; } }
	}

	public class BadMerge2
	{
		[YuzuRequired, YuzuMerge]
		public int F;
	}

	public static class XAssert
	{
		public static void Throws<TExpectedException>(Action exceptionThrower, string expectedExceptionMessage = "")
			where TExpectedException : Exception
		{
			try {
				exceptionThrower();
			}
			catch (TExpectedException ex) {
				StringAssert.Contains(ex.Message, expectedExceptionMessage, "Bad exception message");
				return;
			}
			Assert.Fail("Expected exception:<{0}>. Actual exception: none.", typeof(TExpectedException).Name);
		}
	}
}

namespace YuzuTest2
{
	public class SampleNamespace
	{
		[YuzuRequired]
		public YuzuTest.SampleBase B;
	}
}
