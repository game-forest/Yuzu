using System;

using Yuzu.Binary;

namespace YuzuGenBin
{
	public class BinaryDeserializerGen: BinaryDeserializerGenBase
	{
		private static void Read_YuzuTest__Sample1(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.Sample1)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.X = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 == fd.OurIndex) {
				result.Y = d.Reader.ReadString();
				if (result.Y == "" && d.Reader.ReadBoolean()) result.Y = null;
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__Sample1(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.Sample1();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__Sample1(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__Sample2(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.Sample2)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.X = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 == fd.OurIndex) {
				result.Y = d.Reader.ReadString();
				if (result.Y == "" && d.Reader.ReadBoolean()) result.Y = null;
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__Sample2(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.Sample2();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__Sample2(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__Sample3(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.Sample3)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.S1 = (global::YuzuTest.Sample1)dg.ReadObject<global::YuzuTest.Sample1>();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 == fd.OurIndex) {
				result.F = d.Reader.ReadInt32();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (3 == fd.OurIndex) {
				result.S2 = (global::YuzuTest.Sample2)dg.ReadObject<global::YuzuTest.Sample2>();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__Sample3(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.Sample3();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__Sample3(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleEnumMemberTyped(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleEnumMemberTyped)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.Eb = (global::YuzuTest.SampleEnumByte)d.Reader.ReadByte();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (2 == fd.OurIndex) {
				result.El = (global::YuzuTest.SampleEnumLong)d.Reader.ReadInt64();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleEnumMemberTyped(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleEnumMemberTyped();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleEnumMemberTyped(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__Sample4(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.Sample4)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.E = (global::YuzuTest.SampleEnum)d.Reader.ReadInt32();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__Sample4(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.Sample4();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__Sample4(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleDecimal(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleDecimal)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.N = d.Reader.ReadDecimal();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleDecimal(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleDecimal();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleDecimal(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleNullable(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleNullable)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.N = d.Reader.ReadBoolean() ? (int?)null : d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleNullable(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleNullable();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleNullable(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleObj(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleObj)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.F = dg.ReadAny();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleObj(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleObj();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleObj(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleDict(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleDict)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.Value = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 == fd.OurIndex) {
				result.Children = (global::System.Collections.Generic.Dictionary<string, global::YuzuTest.SampleDict>)null;
				var tmp1 = d.Reader.ReadInt32();
				if (tmp1 >= 0) {
					result.Children = [];
					while (--tmp1 >= 0) {
						var tmp2 = d.Reader.ReadString();
						if (tmp2 == "" && d.Reader.ReadBoolean()) tmp2 = null;
						var tmp3 = (global::YuzuTest.SampleDict)dg.ReadObject<global::YuzuTest.SampleDict>();
						result.Children.Add(tmp2, tmp3);
					}
				}
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleDict(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleDict();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleDict(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleSortedDict(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleSortedDict)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.d = (global::System.Collections.Generic.SortedDictionary<string, int>)null;
				var tmp1 = d.Reader.ReadInt32();
				if (tmp1 >= 0) {
					result.d = [];
					while (--tmp1 >= 0) {
						var tmp2 = d.Reader.ReadString();
						if (tmp2 == "" && d.Reader.ReadBoolean()) tmp2 = null;
						var tmp3 = d.Reader.ReadInt32();
						result.d.Add(tmp2, tmp3);
					}
				}
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleSortedDict(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleSortedDict();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleSortedDict(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleDictKeys(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleDictKeys)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.E = (global::System.Collections.Generic.Dictionary<global::YuzuTest.SampleEnum, int>)null;
			var tmp1 = d.Reader.ReadInt32();
			if (tmp1 >= 0) {
				result.E = [];
				while (--tmp1 >= 0) {
					var tmp2 = (global::YuzuTest.SampleEnum)d.Reader.ReadInt32();
					var tmp3 = d.Reader.ReadInt32();
					result.E.Add(tmp2, tmp3);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			result.I = (global::System.Collections.Generic.Dictionary<int, int>)null;
			var tmp4 = d.Reader.ReadInt32();
			if (tmp4 >= 0) {
				result.I = [];
				while (--tmp4 >= 0) {
					var tmp5 = d.Reader.ReadInt32();
					var tmp6 = d.Reader.ReadInt32();
					result.I.Add(tmp5, tmp6);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (3 != fd.OurIndex) throw dg.Error("3!=" + fd.OurIndex);
			result.K = (global::System.Collections.Generic.Dictionary<global::YuzuTest.SampleKey, int>)null;
			var tmp7 = d.Reader.ReadInt32();
			if (tmp7 >= 0) {
				result.K = [];
				while (--tmp7 >= 0) {
					var tmp8 = (global::YuzuTest.SampleKey)dg.ReadObject<global::YuzuTest.SampleKey>();
					var tmp9 = d.Reader.ReadInt32();
					result.K.Add(tmp8, tmp9);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleDictKeys(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleDictKeys();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleDictKeys(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleMemberI(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleMemberI)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.X = d.Reader.ReadInt32();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleMemberI(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleMemberI();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleMemberI(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleArray(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleArray)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.A = (string[])null;
			var tmp1 = d.Reader.ReadInt32();
			if (tmp1 >= 0) {
				var tmp2 = new string[tmp1];
				for(tmp1 = 0; tmp1 < tmp2.Length; ++tmp1) {
					tmp2[tmp1] = d.Reader.ReadString();
					if (tmp2[tmp1] == "" && d.Reader.ReadBoolean()) tmp2[tmp1] = null;
				}
				result.A = tmp2;
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleArray(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleArray();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleArray(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleArrayOfArray(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleArrayOfArray)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.A = (int[][])null;
			var tmp1 = d.Reader.ReadInt32();
			if (tmp1 >= 0) {
				var tmp2 = new int[tmp1][];
				for(tmp1 = 0; tmp1 < tmp2.Length; ++tmp1) {
					tmp2[tmp1] = (int[])null;
					var tmp3 = d.Reader.ReadInt32();
					if (tmp3 >= 0) {
						var tmp4 = new int[tmp3];
						for(tmp3 = 0; tmp3 < tmp4.Length; ++tmp3) {
							tmp4[tmp3] = d.Reader.ReadInt32();
						}
						tmp2[tmp1] = tmp4;
					}
				}
				result.A = tmp2;
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleArrayOfArray(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleArrayOfArray();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleArrayOfArray(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleArrayNDim(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleArrayNDim)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.A = (int[,])dg.ReadArrayNDim(typeof(int), 2);
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			result.B = (string[,,])dg.ReadArrayNDim(typeof(string), 3);
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleArrayNDim(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleArrayNDim();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleArrayNDim(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleBase(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleBase)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.FBase = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleBase(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleBase();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleBase(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleDerivedA(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleDerivedA)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.FBase = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			result.FA = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleDerivedA(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleDerivedA();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleDerivedA(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleDerivedB(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleDerivedB)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.FBase = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			result.FB = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleDerivedB(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleDerivedB();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleDerivedB(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleMatrix(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleMatrix)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.M = (global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>>)null;
			var tmp1 = d.Reader.ReadInt32();
			if (tmp1 >= 0) {
				result.M = [];
				result.M.Capacity += tmp1;
				while (--tmp1 >= 0) {
					var tmp2 = (global::System.Collections.Generic.List<int>)null;
					var tmp3 = d.Reader.ReadInt32();
					if (tmp3 >= 0) {
						tmp2 = [];
						tmp2.Capacity += tmp3;
						while (--tmp3 >= 0) {
							var tmp4 = d.Reader.ReadInt32();
							tmp2.Add(tmp4);
						}
					}
					result.M.Add(tmp2);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleMatrix(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleMatrix();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleMatrix(d, def, result);
			return result;
		}

		private static object Make_YuzuTest__SamplePoint(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SamplePoint();
			result.X = d.Reader.ReadInt32();
			result.Y = d.Reader.ReadInt32();
			return result;
		}

		private static void Read_YuzuTest__SampleRect(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleRect)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			dg.EnsureClassDef(typeof(global::YuzuTest.SamplePoint));
			result.A.X = d.Reader.ReadInt32();
			result.A.Y = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			dg.EnsureClassDef(typeof(global::YuzuTest.SamplePoint));
			result.B.X = d.Reader.ReadInt32();
			result.B.Y = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleRect(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleRect();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleRect(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleGuid(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleGuid)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.G = new Guid(d.Reader.ReadBytes(16));
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleGuid(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleGuid();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleGuid(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleDefault(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleDefault)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.A = d.Reader.ReadInt32();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (2 == fd.OurIndex) {
				result.B = d.Reader.ReadString();
				if (result.B == "" && d.Reader.ReadBoolean()) result.B = null;
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (3 == fd.OurIndex) {
				dg.EnsureClassDef(typeof(global::YuzuTest.SamplePoint));
				result.P.X = d.Reader.ReadInt32();
				result.P.Y = d.Reader.ReadInt32();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleDefault(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleDefault();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleDefault(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__Color(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.Color)obj;
			result.B = d.Reader.ReadByte();
			result.G = d.Reader.ReadByte();
			result.R = d.Reader.ReadByte();
		}

		private static object Make_YuzuTest__Color(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.Color();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__Color(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleClassList(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleClassList)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.E = (global::System.Collections.Generic.List<global::YuzuTest.SampleBase>)null;
			var tmp1 = d.Reader.ReadInt32();
			if (tmp1 >= 0) {
				result.E = [];
				result.E.Capacity += tmp1;
				while (--tmp1 >= 0) {
					var tmp2 = (global::YuzuTest.SampleBase)dg.ReadObject<global::YuzuTest.SampleBase>();
					result.E.Add(tmp2);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleClassList(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleClassList();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleClassList(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleSmallTypes(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleSmallTypes)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.B = d.Reader.ReadByte();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			result.Ch = d.Reader.ReadChar();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (3 != fd.OurIndex) throw dg.Error("3!=" + fd.OurIndex);
			result.Sb = d.Reader.ReadSByte();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (4 != fd.OurIndex) throw dg.Error("4!=" + fd.OurIndex);
			result.Sh = d.Reader.ReadInt16();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (5 != fd.OurIndex) throw dg.Error("5!=" + fd.OurIndex);
			result.USh = d.Reader.ReadUInt16();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleSmallTypes(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleSmallTypes();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleSmallTypes(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleWithNullFieldCompact(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleWithNullFieldCompact)obj;
			var dg = (BinaryDeserializerGen)d;
			result.N = (global::YuzuTest.Sample1)dg.ReadObject<global::YuzuTest.Sample1>();
		}

		private static object Make_YuzuTest__SampleWithNullFieldCompact(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleWithNullFieldCompact();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleWithNullFieldCompact(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleNested__NestedClass(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleNested.NestedClass)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.Z = d.Reader.ReadInt32();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleNested__NestedClass(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleNested.NestedClass();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleNested__NestedClass(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleNested(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleNested)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.C = (global::YuzuTest.SampleNested.NestedClass)dg.ReadObject<global::YuzuTest.SampleNested.NestedClass>();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			result.E = (global::YuzuTest.SampleNested.NestedEnum)d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (3 == fd.OurIndex) {
				result.Z = (global::YuzuTest.SampleNested.NestedEnum[])null;
				var tmp1 = d.Reader.ReadInt32();
				if (tmp1 >= 0) {
					var tmp2 = new global::YuzuTest.SampleNested.NestedEnum[tmp1];
					for(tmp1 = 0; tmp1 < tmp2.Length; ++tmp1) {
						tmp2[tmp1] = (global::YuzuTest.SampleNested.NestedEnum)d.Reader.ReadInt32();
					}
					result.Z = tmp2;
				}
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleNested(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleNested();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleNested(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SamplePerson(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SamplePerson)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.Name = d.Reader.ReadString();
			if (result.Name == "" && d.Reader.ReadBoolean()) result.Name = null;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			result.Birth = DateTime.FromBinary(d.Reader.ReadInt64());
			fd = def.Fields[d.Reader.ReadInt16()];
			if (3 != fd.OurIndex) throw dg.Error("3!=" + fd.OurIndex);
			result.Children = (global::System.Collections.Generic.List<global::YuzuTest.SamplePerson>)null;
			var tmp1 = d.Reader.ReadInt32();
			if (tmp1 >= 0) {
				result.Children = [];
				result.Children.Capacity += tmp1;
				while (--tmp1 >= 0) {
					var tmp2 = (global::YuzuTest.SamplePerson)dg.ReadObject<global::YuzuTest.SamplePerson>();
					result.Children.Add(tmp2);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (4 != fd.OurIndex) throw dg.Error("4!=" + fd.OurIndex);
			result.EyeColor = (global::YuzuTest.Color)dg.ReadObject<global::YuzuTest.Color>();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SamplePerson(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SamplePerson();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SamplePerson(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleInterfaceField(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleInterfaceField)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.I = (global::YuzuTest.ISample)dg.ReadObject<global::YuzuTest.ISample>();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleInterfaceField(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleInterfaceField();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleInterfaceField(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleInterfacedGeneric_String(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleInterfacedGeneric<string>)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.G = d.Reader.ReadString();
			if (result.G == "" && d.Reader.ReadBoolean()) result.G = null;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			result.X = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleInterfacedGeneric_String(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleInterfacedGeneric<string>();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleInterfacedGeneric_String(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleConcrete(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleConcrete)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.XX = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleConcrete(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleConcrete();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleConcrete(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleWithCollection(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleWithCollection)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.A = (global::YuzuTest.SampleCollection<global::YuzuTest.ISample>)null;
			var tmp1 = d.Reader.ReadInt32();
			if (tmp1 >= 0) {
				result.A = [];
				while (--tmp1 >= 0) {
					var tmp2 = (global::YuzuTest.ISample)dg.ReadObject<global::YuzuTest.ISample>();
					result.A.Add(tmp2);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 == fd.OurIndex) {
				result.B = (global::YuzuTest.SampleCollection<int>)null;
				var tmp3 = d.Reader.ReadInt32();
				if (tmp3 >= 0) {
					result.B = [];
					while (--tmp3 >= 0) {
						var tmp4 = d.Reader.ReadInt32();
						result.B.Add(tmp4);
					}
				}
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (3 == fd.OurIndex) {
				result.C = (global::YuzuTest.SampleCollection<global::YuzuTest.SamplePoint>)null;
				var tmp5 = d.Reader.ReadInt32();
				if (tmp5 >= 0) {
					result.C = [];
					while (--tmp5 >= 0) {
						var tmp6 = new global::YuzuTest.SamplePoint();
						dg.EnsureClassDef(typeof(global::YuzuTest.SamplePoint));
						tmp6.X = d.Reader.ReadInt32();
						tmp6.Y = d.Reader.ReadInt32();
						result.C.Add(tmp6);
					}
				}
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleWithCollection(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleWithCollection();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleWithCollection(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleAfter2(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleAfter2)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.X = d.Reader.ReadString();
			if (result.X == "" && d.Reader.ReadBoolean()) result.X = null;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
			result.After2();
			result.After3();
			result.After();
		}

		private static object Make_YuzuTest__SampleAfter2(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleAfter2();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleAfter2(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleAfterSerialization(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleAfterSerialization)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.X = d.Reader.ReadString();
			if (result.X == "" && d.Reader.ReadBoolean()) result.X = null;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleAfterSerialization(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleAfterSerialization();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleAfterSerialization(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleBeforeDeserialization(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleBeforeDeserialization)obj;
			result.Before();
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.X = d.Reader.ReadString();
			if (result.X == "" && d.Reader.ReadBoolean()) result.X = null;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleBeforeDeserialization(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleBeforeDeserialization();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleBeforeDeserialization(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleMerge(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleMerge)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			var tmp1 = d.Reader.ReadInt32();
			if (tmp1 >= 0) {
				while (--tmp1 >= 0) {
					var tmp2 = d.Reader.ReadInt32();
					var tmp3 = d.Reader.ReadInt32();
					result.DI.Add(tmp2, tmp3);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			var tmp4 = d.Reader.ReadInt32();
			if (tmp4 >= 0) {
				result.LI.Capacity += tmp4;
				while (--tmp4 >= 0) {
					var tmp5 = d.Reader.ReadInt32();
					result.LI.Add(tmp5);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (3 == fd.OurIndex) {
				dg.ReadIntoObject<global::YuzuTest.Sample1>(result.M);
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleMerge(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleMerge();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleMerge(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleAssemblyDerivedR(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleAssemblyDerivedR)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.P = d.Reader.ReadInt16();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (2 == fd.OurIndex) {
				result.R = d.Reader.ReadString();
				if (result.R == "" && d.Reader.ReadBoolean()) result.R = null;
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleAssemblyDerivedR(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleAssemblyDerivedR();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleAssemblyDerivedR(d, def, result);
			return result;
		}

		private static object Make_YuzuTest__SampleAoS__Color(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleAoS.Color();
			result.B = d.Reader.ReadByte();
			result.G = d.Reader.ReadByte();
			result.R = d.Reader.ReadByte();
			return result;
		}

		private static object Make_YuzuTest__SampleAoS__Vertex(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleAoS.Vertex();
			result.X = d.Reader.ReadSingle();
			result.Y = d.Reader.ReadSingle();
			result.Z = d.Reader.ReadSingle();
			return result;
		}

		private static void Read_YuzuTest__SampleAoS__S(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleAoS.S)obj;
			var dg = (BinaryDeserializerGen)d;
			dg.EnsureClassDef(typeof(global::YuzuTest.SampleAoS.Color));
			result.C.B = d.Reader.ReadByte();
			result.C.G = d.Reader.ReadByte();
			result.C.R = d.Reader.ReadByte();
			dg.EnsureClassDef(typeof(global::YuzuTest.SampleAoS.Vertex));
			result.V.X = d.Reader.ReadSingle();
			result.V.Y = d.Reader.ReadSingle();
			result.V.Z = d.Reader.ReadSingle();
		}

		private static object Make_YuzuTest__SampleAoS__S(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleAoS.S();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleAoS__S(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleAoS(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleAoS)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.A = (global::System.Collections.Generic.List<global::YuzuTest.SampleAoS.S>)null;
			var tmp1 = d.Reader.ReadInt32();
			if (tmp1 >= 0) {
				result.A = [];
				result.A.Capacity += tmp1;
				while (--tmp1 >= 0) {
					var tmp2 = (global::YuzuTest.SampleAoS.S)dg.ReadObject<global::YuzuTest.SampleAoS.S>();
					result.A.Add(tmp2);
				}
			}
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleAoS(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleAoS();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleAoS(d, def, result);
			return result;
		}

		private static object Make_YuzuTest__SampleStructWithProps(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleStructWithProps();
			var dg = (BinaryDeserializerGen)d;
			result.A = d.Reader.ReadInt32();
			dg.EnsureClassDef(typeof(global::YuzuTest.SamplePoint));
			var tmp1 = new global::YuzuTest.SamplePoint();
			tmp1.X = d.Reader.ReadInt32();
			tmp1.Y = d.Reader.ReadInt32();
			result.P = tmp1;
			return result;
		}

		private static void Read_YuzuTest__SampleAliasMany(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleAliasMany)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.X = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleAliasMany(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleAliasMany();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleAliasMany(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SamplePrivateConstructor(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SamplePrivateConstructor)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.X = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SamplePrivateConstructor(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = global::YuzuTest.SamplePrivateConstructor.Make();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SamplePrivateConstructor(d, def, result);
			return result;
		}

		private static void Read_YuzuTestAssembly__SampleAssemblyBase(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTestAssembly.SampleAssemblyBase)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.P = d.Reader.ReadInt16();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTestAssembly__SampleAssemblyBase(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTestAssembly.SampleAssemblyBase();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTestAssembly__SampleAssemblyBase(d, def, result);
			return result;
		}

		private static void Read_YuzuTestAssembly__SampleAssemblyDerivedQ(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTestAssembly.SampleAssemblyDerivedQ)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.P = d.Reader.ReadInt16();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (2 == fd.OurIndex) {
				result.Q = d.Reader.ReadInt16();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTestAssembly__SampleAssemblyDerivedQ(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTestAssembly.SampleAssemblyDerivedQ();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTestAssembly__SampleAssemblyDerivedQ(d, def, result);
			return result;
		}

		private static void Read_YuzuTest2__SampleNamespace(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest2.SampleNamespace)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.B = (global::YuzuTest.SampleBase)dg.ReadObject<global::YuzuTest.SampleBase>();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest2__SampleNamespace(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest2.SampleNamespace();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest2__SampleNamespace(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__SampleExplicitCollection_Int32(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.SampleExplicitCollection<int>)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__SampleExplicitCollection_Int32(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.SampleExplicitCollection<int>();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__SampleExplicitCollection_Int32(d, def, result);
			return result;
		}

		private static object Make_YuzuTest__A__B__C__D__E__Sample2Struct(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.A.B.C.D.E.Sample2Struct();
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.X = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 == fd.OurIndex) {
				result.Y = d.Reader.ReadString();
				if (result.Y == "" && d.Reader.ReadBoolean()) result.Y = null;
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
			return result;
		}

		private static object Make_YuzuTest__A__B__C__D__E__SampleSerializeIfStruct(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.A.B.C.D.E.SampleSerializeIfStruct();
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 != fd.OurIndex) throw dg.Error("1!=" + fd.OurIndex);
			result.X = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (2 == fd.OurIndex) {
				result.Y = (global::YuzuTest.Sample1)dg.ReadObject<global::YuzuTest.Sample1>();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
			return result;
		}

		private static object Make_YuzuTest__A__B__C__D__E__SampleSerializeIfOnFieldStruct(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.A.B.C.D.E.SampleSerializeIfOnFieldStruct();
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.W = (global::YuzuTest.Sample1)dg.ReadObject<global::YuzuTest.Sample1>();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (2 != fd.OurIndex) throw dg.Error("2!=" + fd.OurIndex);
			result.X = d.Reader.ReadInt32();
			fd = def.Fields[d.Reader.ReadInt16()];
			if (3 == fd.OurIndex) {
				result.Y = (global::YuzuTest.Sample1)dg.ReadObject<global::YuzuTest.Sample1>();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (4 == fd.OurIndex) {
				result.Z = (global::YuzuTest.Sample1)dg.ReadObject<global::YuzuTest.Sample1>();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
			return result;
		}

		private static void Read_YuzuTest__TestReferences__Node(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.TestReferences.Node)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.Id = d.Reader.ReadInt32();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (2 == fd.OurIndex) {
				var tmp1 = d.Reader.ReadInt32();
				if (tmp1 >= 0) {
					while (--tmp1 >= 0) {
						var tmp2 = (global::YuzuTest.TestReferences.Node)dg.ReadObject<global::YuzuTest.TestReferences.Node>();
						result.Nodes.Add(tmp2);
					}
				}
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__TestReferences__Node(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.TestReferences.Node();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__TestReferences__Node(d, def, result);
			return result;
		}

		private static void Read_YuzuTest__TestReferences__Employee(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			var result = (global::YuzuTest.TestReferences.Employee)obj;
			var dg = (BinaryDeserializerGen)d;
			ReaderClassDef.FieldDef fd;
			fd = def.Fields[d.Reader.ReadInt16()];
			if (1 == fd.OurIndex) {
				result.DirectReports = (global::System.Collections.Generic.List<global::YuzuTest.TestReferences.IEmployee>)null;
				var tmp1 = d.Reader.ReadInt32();
				if (tmp1 >= 0) {
					result.DirectReports = new global::System.Collections.Generic.List<global::YuzuTest.TestReferences.IEmployee>();
					while (--tmp1 >= 0) {
						var tmp2 = (global::YuzuTest.TestReferences.IEmployee)dg.ReadObject<global::YuzuTest.TestReferences.IEmployee>();
						result.DirectReports.Add(tmp2);
					}
				}
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (2 == fd.OurIndex) {
				result.Manager = (global::YuzuTest.TestReferences.IEmployee)dg.ReadObject<global::YuzuTest.TestReferences.IEmployee>();
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (3 == fd.OurIndex) {
				result.Name = d.Reader.ReadString();
				if (result.Name == "" && d.Reader.ReadBoolean()) result.Name = null;
				fd = def.Fields[d.Reader.ReadInt16()];
			}
			if (fd.OurIndex != ReaderClassDef.EOF) throw dg.Error("Unfinished object");
		}

		private static object Make_YuzuTest__TestReferences__Employee(BinaryDeserializer d, ReaderClassDef def, object id)
		{
			var result = new global::YuzuTest.TestReferences.Employee();
			if (id != null) {
				d.ReferenceResolver?.AddReference(id, result);
			}
			Read_YuzuTest__TestReferences__Employee(d, def, result);
			return result;
		}

		static BinaryDeserializerGen()
		{
			readCache[typeof(global::YuzuTest.Sample1)] = Read_YuzuTest__Sample1;
			readCache[typeof(global::YuzuTest.Sample2)] = Read_YuzuTest__Sample2;
			readCache[typeof(global::YuzuTest.Sample3)] = Read_YuzuTest__Sample3;
			readCache[typeof(global::YuzuTest.SampleEnumMemberTyped)] = Read_YuzuTest__SampleEnumMemberTyped;
			readCache[typeof(global::YuzuTest.Sample4)] = Read_YuzuTest__Sample4;
			readCache[typeof(global::YuzuTest.SampleDecimal)] = Read_YuzuTest__SampleDecimal;
			readCache[typeof(global::YuzuTest.SampleNullable)] = Read_YuzuTest__SampleNullable;
			readCache[typeof(global::YuzuTest.SampleObj)] = Read_YuzuTest__SampleObj;
			readCache[typeof(global::YuzuTest.SampleDict)] = Read_YuzuTest__SampleDict;
			readCache[typeof(global::YuzuTest.SampleSortedDict)] = Read_YuzuTest__SampleSortedDict;
			readCache[typeof(global::YuzuTest.SampleDictKeys)] = Read_YuzuTest__SampleDictKeys;
			readCache[typeof(global::YuzuTest.SampleMemberI)] = Read_YuzuTest__SampleMemberI;
			readCache[typeof(global::YuzuTest.SampleArray)] = Read_YuzuTest__SampleArray;
			readCache[typeof(global::YuzuTest.SampleArrayOfArray)] = Read_YuzuTest__SampleArrayOfArray;
			readCache[typeof(global::YuzuTest.SampleArrayNDim)] = Read_YuzuTest__SampleArrayNDim;
			readCache[typeof(global::YuzuTest.SampleBase)] = Read_YuzuTest__SampleBase;
			readCache[typeof(global::YuzuTest.SampleDerivedA)] = Read_YuzuTest__SampleDerivedA;
			readCache[typeof(global::YuzuTest.SampleDerivedB)] = Read_YuzuTest__SampleDerivedB;
			readCache[typeof(global::YuzuTest.SampleMatrix)] = Read_YuzuTest__SampleMatrix;
			readCache[typeof(global::YuzuTest.SampleRect)] = Read_YuzuTest__SampleRect;
			readCache[typeof(global::YuzuTest.SampleGuid)] = Read_YuzuTest__SampleGuid;
			readCache[typeof(global::YuzuTest.SampleDefault)] = Read_YuzuTest__SampleDefault;
			readCache[typeof(global::YuzuTest.Color)] = Read_YuzuTest__Color;
			readCache[typeof(global::YuzuTest.SampleClassList)] = Read_YuzuTest__SampleClassList;
			readCache[typeof(global::YuzuTest.SampleSmallTypes)] = Read_YuzuTest__SampleSmallTypes;
			readCache[typeof(global::YuzuTest.SampleWithNullFieldCompact)] = Read_YuzuTest__SampleWithNullFieldCompact;
			readCache[typeof(global::YuzuTest.SampleNested.NestedClass)] = Read_YuzuTest__SampleNested__NestedClass;
			readCache[typeof(global::YuzuTest.SampleNested)] = Read_YuzuTest__SampleNested;
			readCache[typeof(global::YuzuTest.SamplePerson)] = Read_YuzuTest__SamplePerson;
			readCache[typeof(global::YuzuTest.SampleInterfaceField)] = Read_YuzuTest__SampleInterfaceField;
			readCache[typeof(global::YuzuTest.SampleInterfacedGeneric<string>)] = Read_YuzuTest__SampleInterfacedGeneric_String;
			readCache[typeof(global::YuzuTest.SampleConcrete)] = Read_YuzuTest__SampleConcrete;
			readCache[typeof(global::YuzuTest.SampleWithCollection)] = Read_YuzuTest__SampleWithCollection;
			readCache[typeof(global::YuzuTest.SampleAfter2)] = Read_YuzuTest__SampleAfter2;
			readCache[typeof(global::YuzuTest.SampleAfterSerialization)] = Read_YuzuTest__SampleAfterSerialization;
			readCache[typeof(global::YuzuTest.SampleBeforeDeserialization)] = Read_YuzuTest__SampleBeforeDeserialization;
			readCache[typeof(global::YuzuTest.SampleMerge)] = Read_YuzuTest__SampleMerge;
			readCache[typeof(global::YuzuTest.SampleAssemblyDerivedR)] = Read_YuzuTest__SampleAssemblyDerivedR;
			readCache[typeof(global::YuzuTest.SampleAoS.S)] = Read_YuzuTest__SampleAoS__S;
			readCache[typeof(global::YuzuTest.SampleAoS)] = Read_YuzuTest__SampleAoS;
			readCache[typeof(global::YuzuTest.SampleAliasMany)] = Read_YuzuTest__SampleAliasMany;
			readCache[typeof(global::YuzuTest.SamplePrivateConstructor)] = Read_YuzuTest__SamplePrivateConstructor;
			readCache[typeof(global::YuzuTestAssembly.SampleAssemblyBase)] = Read_YuzuTestAssembly__SampleAssemblyBase;
			readCache[typeof(global::YuzuTestAssembly.SampleAssemblyDerivedQ)] = Read_YuzuTestAssembly__SampleAssemblyDerivedQ;
			readCache[typeof(global::YuzuTest2.SampleNamespace)] = Read_YuzuTest2__SampleNamespace;
			readCache[typeof(global::YuzuTest.SampleExplicitCollection<int>)] = Read_YuzuTest__SampleExplicitCollection_Int32;
			readCache[typeof(global::YuzuTest.TestReferences.Node)] = Read_YuzuTest__TestReferences__Node;
			readCache[typeof(global::YuzuTest.TestReferences.Employee)] = Read_YuzuTest__TestReferences__Employee;
			makeCache[typeof(global::YuzuTest.Sample1)] = Make_YuzuTest__Sample1;
			makeCache[typeof(global::YuzuTest.Sample2)] = Make_YuzuTest__Sample2;
			makeCache[typeof(global::YuzuTest.Sample3)] = Make_YuzuTest__Sample3;
			makeCache[typeof(global::YuzuTest.SampleEnumMemberTyped)] = Make_YuzuTest__SampleEnumMemberTyped;
			makeCache[typeof(global::YuzuTest.Sample4)] = Make_YuzuTest__Sample4;
			makeCache[typeof(global::YuzuTest.SampleDecimal)] = Make_YuzuTest__SampleDecimal;
			makeCache[typeof(global::YuzuTest.SampleNullable)] = Make_YuzuTest__SampleNullable;
			makeCache[typeof(global::YuzuTest.SampleObj)] = Make_YuzuTest__SampleObj;
			makeCache[typeof(global::YuzuTest.SampleDict)] = Make_YuzuTest__SampleDict;
			makeCache[typeof(global::YuzuTest.SampleSortedDict)] = Make_YuzuTest__SampleSortedDict;
			makeCache[typeof(global::YuzuTest.SampleDictKeys)] = Make_YuzuTest__SampleDictKeys;
			makeCache[typeof(global::YuzuTest.SampleMemberI)] = Make_YuzuTest__SampleMemberI;
			makeCache[typeof(global::YuzuTest.SampleArray)] = Make_YuzuTest__SampleArray;
			makeCache[typeof(global::YuzuTest.SampleArrayOfArray)] = Make_YuzuTest__SampleArrayOfArray;
			makeCache[typeof(global::YuzuTest.SampleArrayNDim)] = Make_YuzuTest__SampleArrayNDim;
			makeCache[typeof(global::YuzuTest.SampleBase)] = Make_YuzuTest__SampleBase;
			makeCache[typeof(global::YuzuTest.SampleDerivedA)] = Make_YuzuTest__SampleDerivedA;
			makeCache[typeof(global::YuzuTest.SampleDerivedB)] = Make_YuzuTest__SampleDerivedB;
			makeCache[typeof(global::YuzuTest.SampleMatrix)] = Make_YuzuTest__SampleMatrix;
			makeCache[typeof(global::YuzuTest.SamplePoint)] = Make_YuzuTest__SamplePoint;
			makeCache[typeof(global::YuzuTest.SampleRect)] = Make_YuzuTest__SampleRect;
			makeCache[typeof(global::YuzuTest.SampleGuid)] = Make_YuzuTest__SampleGuid;
			makeCache[typeof(global::YuzuTest.SampleDefault)] = Make_YuzuTest__SampleDefault;
			makeCache[typeof(global::YuzuTest.Color)] = Make_YuzuTest__Color;
			makeCache[typeof(global::YuzuTest.SampleClassList)] = Make_YuzuTest__SampleClassList;
			makeCache[typeof(global::YuzuTest.SampleSmallTypes)] = Make_YuzuTest__SampleSmallTypes;
			makeCache[typeof(global::YuzuTest.SampleWithNullFieldCompact)] = Make_YuzuTest__SampleWithNullFieldCompact;
			makeCache[typeof(global::YuzuTest.SampleNested.NestedClass)] = Make_YuzuTest__SampleNested__NestedClass;
			makeCache[typeof(global::YuzuTest.SampleNested)] = Make_YuzuTest__SampleNested;
			makeCache[typeof(global::YuzuTest.SamplePerson)] = Make_YuzuTest__SamplePerson;
			makeCache[typeof(global::YuzuTest.SampleInterfaceField)] = Make_YuzuTest__SampleInterfaceField;
			makeCache[typeof(global::YuzuTest.SampleInterfacedGeneric<string>)] = Make_YuzuTest__SampleInterfacedGeneric_String;
			makeCache[typeof(global::YuzuTest.SampleConcrete)] = Make_YuzuTest__SampleConcrete;
			makeCache[typeof(global::YuzuTest.SampleWithCollection)] = Make_YuzuTest__SampleWithCollection;
			makeCache[typeof(global::YuzuTest.SampleAfter2)] = Make_YuzuTest__SampleAfter2;
			makeCache[typeof(global::YuzuTest.SampleAfterSerialization)] = Make_YuzuTest__SampleAfterSerialization;
			makeCache[typeof(global::YuzuTest.SampleBeforeDeserialization)] = Make_YuzuTest__SampleBeforeDeserialization;
			makeCache[typeof(global::YuzuTest.SampleMerge)] = Make_YuzuTest__SampleMerge;
			makeCache[typeof(global::YuzuTest.SampleAssemblyDerivedR)] = Make_YuzuTest__SampleAssemblyDerivedR;
			makeCache[typeof(global::YuzuTest.SampleAoS.Color)] = Make_YuzuTest__SampleAoS__Color;
			makeCache[typeof(global::YuzuTest.SampleAoS.Vertex)] = Make_YuzuTest__SampleAoS__Vertex;
			makeCache[typeof(global::YuzuTest.SampleAoS.S)] = Make_YuzuTest__SampleAoS__S;
			makeCache[typeof(global::YuzuTest.SampleAoS)] = Make_YuzuTest__SampleAoS;
			makeCache[typeof(global::YuzuTest.SampleStructWithProps)] = Make_YuzuTest__SampleStructWithProps;
			makeCache[typeof(global::YuzuTest.SampleAliasMany)] = Make_YuzuTest__SampleAliasMany;
			makeCache[typeof(global::YuzuTest.SamplePrivateConstructor)] = Make_YuzuTest__SamplePrivateConstructor;
			makeCache[typeof(global::YuzuTestAssembly.SampleAssemblyBase)] = Make_YuzuTestAssembly__SampleAssemblyBase;
			makeCache[typeof(global::YuzuTestAssembly.SampleAssemblyDerivedQ)] = Make_YuzuTestAssembly__SampleAssemblyDerivedQ;
			makeCache[typeof(global::YuzuTest2.SampleNamespace)] = Make_YuzuTest2__SampleNamespace;
			makeCache[typeof(global::YuzuTest.SampleExplicitCollection<int>)] = Make_YuzuTest__SampleExplicitCollection_Int32;
			makeCache[typeof(global::YuzuTest.A.B.C.D.E.Sample2Struct)] = Make_YuzuTest__A__B__C__D__E__Sample2Struct;
			makeCache[typeof(global::YuzuTest.A.B.C.D.E.SampleSerializeIfStruct)] = Make_YuzuTest__A__B__C__D__E__SampleSerializeIfStruct;
			makeCache[typeof(global::YuzuTest.A.B.C.D.E.SampleSerializeIfOnFieldStruct)] = Make_YuzuTest__A__B__C__D__E__SampleSerializeIfOnFieldStruct;
			makeCache[typeof(global::YuzuTest.TestReferences.Node)] = Make_YuzuTest__TestReferences__Node;
			makeCache[typeof(global::YuzuTest.TestReferences.Employee)] = Make_YuzuTest__TestReferences__Employee;
		}
	}
}
