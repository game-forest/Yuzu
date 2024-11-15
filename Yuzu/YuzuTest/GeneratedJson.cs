using System;
using System.Collections.Generic;

using Yuzu;
using Yuzu.Json;

namespace YuzuGen.YuzuTest
{
	class Sample1_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new Sample1_JsonDeserializer Instance = new();

		public Sample1_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.Sample1>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.Sample1(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.Sample1)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			if ("Y" == name) {
				result.Y = RequireString();
				name = GetNextName(false);
			}
			return result;
		}
	}

	class Sample2_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new Sample2_JsonDeserializer Instance = new();

		public Sample2_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.Sample2>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.Sample2(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.Sample2)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			if ("Y" == name) {
				result.Y = RequireString();
				name = GetNextName(false);
			}
			return result;
		}
	}

	class Sample3_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new Sample3_JsonDeserializer Instance = new();

		public Sample3_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.Sample3>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.Sample3(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.Sample3)obj;
			if ("S1" != name) throw new YuzuException("S1!=" + name);
			result.S1 = YuzuGen.YuzuTest.Sample1_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Sample1>(Reader);
			name = GetNextName(false);
			if ("F" == name) {
				result.F = RequireInt();
				name = GetNextName(false);
			}
			if ("S2" == name) {
				result.S2 = YuzuGen.YuzuTest.Sample2_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Sample2>(Reader);
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleEnumMemberTyped_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleEnumMemberTyped_JsonDeserializer Instance = new();

		public SampleEnumMemberTyped_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleEnumMemberTyped>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleEnumMemberTyped(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleEnumMemberTyped)obj;
			if ("Eb" == name) {
				result.Eb = (global::YuzuTest.SampleEnumByte)checked((byte)RequireUInt());
				name = GetNextName(false);
			}
			if ("El" == name) {
				result.El = (global::YuzuTest.SampleEnumLong)RequireLong();
				name = GetNextName(false);
			}
			return result;
		}
	}

	class Sample4_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new Sample4_JsonDeserializer Instance = new();

		public Sample4_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.Sample4>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.Sample4(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.Sample4)obj;
			if ("E" == name) {
				result.E = (global::YuzuTest.SampleEnum)Enum.Parse(typeof(global::YuzuTest.SampleEnum), RequireString());
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleDecimal_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleDecimal_JsonDeserializer Instance = new();

		public SampleDecimal_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleDecimal>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleDecimal(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleDecimal)obj;
			if ("N" != name) throw new YuzuException("N!=" + name);
			result.N = RequireDecimal();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleNullable_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleNullable_JsonDeserializer Instance = new();

		public SampleNullable_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleNullable>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleNullable(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleNullable)obj;
			if ("N" != name) throw new YuzuException("N!=" + name);
			result.N = null;
			if (SkipSpacesCarefully() == 'n') {
				Require("null");
			}
			else {
				result.N = RequireInt();
			}
			name = GetNextName(false);
			return result;
		}
	}

	class SampleBool_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleBool_JsonDeserializer Instance = new();

		public SampleBool_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleBool>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleBool(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleBool)obj;
			if ("B" != name) throw new YuzuException("B!=" + name);
			result.B = RequireBool();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleList_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleList_JsonDeserializer Instance = new();

		public SampleList_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
			JsonOptions.Comments = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleList>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleList(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleList)obj;
			if ("E" != name) throw new YuzuException("E!=" + name);
			result.E = RequireOrNull('[') ? null : new global::System.Collections.Generic.List<string>();
			if (result.E != null) {
				if (SkipSpacesCarefully() == ']') {
					Require(']');
				}
				else {
					do {
						var tmp1 = RequireString();
						result.E.Add(tmp1);
					} while (Require(']', ',') == ',');
				}
			}
			name = GetNextName(false);
			return result;
		}
	}

	class SampleObj_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleObj_JsonDeserializer Instance = new();

		public SampleObj_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleObj>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleObj(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleObj)obj;
			if ("F" != name) throw new YuzuException("F!=" + name);
			result.F = ReadAnyObject();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleDict_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleDict_JsonDeserializer Instance = new();

		public SampleDict_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleDict>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleDict(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleDict)obj;
			if ("Value" != name) throw new YuzuException("Value!=" + name);
			result.Value = RequireInt();
			name = GetNextName(false);
			if ("Children" == name) {
				result.Children = RequireOrNull('{') ? null : new global::System.Collections.Generic.Dictionary<string, global::YuzuTest.SampleDict>();
				if (result.Children != null) {
					if (SkipSpacesCarefully() == '}') {
						Require('}');
					}
					else {
						do {
							var tmp1 = RequireString();
							Require(':');
							var tmp2 = YuzuGen.YuzuTest.SampleDict_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.SampleDict>(Reader);
							result.Children.Add(tmp1, tmp2);
						} while (Require('}', ',') == ',');
					}
				}
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleSortedDict_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleSortedDict_JsonDeserializer Instance = new();

		public SampleSortedDict_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleSortedDict>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleSortedDict(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleSortedDict)obj;
			if ("d" == name) {
				result.d = RequireOrNull('{') ? null : new global::System.Collections.Generic.SortedDictionary<string, int>();
				if (result.d != null) {
					if (SkipSpacesCarefully() == '}') {
						Require('}');
					}
					else {
						do {
							var tmp1 = RequireString();
							Require(':');
							var tmp2 = RequireInt();
							result.d.Add(tmp1, tmp2);
						} while (Require('}', ',') == ',');
					}
				}
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleSortedDictOfClass_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleSortedDictOfClass_JsonDeserializer Instance = new();

		public SampleSortedDictOfClass_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleSortedDictOfClass>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleSortedDictOfClass(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleSortedDictOfClass)obj;
			if ("d" == name) {
				result.d = RequireOrNull('{') ? null : new global::System.Collections.Generic.SortedDictionary<string, global::YuzuTest.Sample1>();
				if (result.d != null) {
					if (SkipSpacesCarefully() == '}') {
						Require('}');
					}
					else {
						do {
							var tmp1 = RequireString();
							Require(':');
							var tmp2 = YuzuGen.YuzuTest.Sample1_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Sample1>(Reader);
							result.d.Add(tmp1, tmp2);
						} while (Require('}', ',') == ',');
					}
				}
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleDictKeys_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleDictKeys_JsonDeserializer Instance = new();

		public SampleDictKeys_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleDictKeys>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleDictKeys(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleDictKeys)obj;
			if ("E" != name) throw new YuzuException("E!=" + name);
			result.E = RequireOrNull('{') ? null : new global::System.Collections.Generic.Dictionary<global::YuzuTest.SampleEnum, int>();
			if (result.E != null) {
				if (SkipSpacesCarefully() == '}') {
					Require('}');
				}
				else {
					do {
						var tmp1 = RequireString();
						Require(':');
						var tmp2 = RequireInt();
						result.E.Add((global::YuzuTest.SampleEnum)Enum.Parse(typeof(global::YuzuTest.SampleEnum), tmp1), tmp2);
					} while (Require('}', ',') == ',');
				}
			}
			name = GetNextName(false);
			if ("I" != name) throw new YuzuException("I!=" + name);
			result.I = RequireOrNull('{') ? null : new global::System.Collections.Generic.Dictionary<int, int>();
			if (result.I != null) {
				if (SkipSpacesCarefully() == '}') {
					Require('}');
				}
				else {
					do {
						var tmp3 = RequireString();
						Require(':');
						var tmp4 = RequireInt();
						result.I.Add(int.Parse(tmp3), tmp4);
					} while (Require('}', ',') == ',');
				}
			}
			name = GetNextName(false);
			if ("K" != name) throw new YuzuException("K!=" + name);
			result.K = RequireOrNull('{') ? null : new global::System.Collections.Generic.Dictionary<global::YuzuTest.SampleKey, int>();
			if (result.K != null) {
				if (SkipSpacesCarefully() == '}') {
					Require('}');
				}
				else {
					do {
						var tmp5 = RequireString();
						Require(':');
						var tmp6 = RequireInt();
						result.K.Add((global::YuzuTest.SampleKey)keyParsers[typeof(global::YuzuTest.SampleKey)](tmp5), tmp6);
					} while (Require('}', ',') == ',');
				}
			}
			name = GetNextName(false);
			return result;
		}
	}

	class ISampleMember_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new ISampleMember_JsonDeserializer Instance = new();

		public ISampleMember_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderInterface<global::YuzuTest.ISampleMember>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return null;
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.ISampleMember)obj;
			if ("X" == name) {
				result.X = RequireInt();
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleMemberI_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleMemberI_JsonDeserializer Instance = new();

		public SampleMemberI_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleMemberI>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleMemberI(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleMemberI)obj;
			if ("X" == name) {
				result.X = RequireInt();
				name = GetNextName(false);
			}
			return result;
		}
	}

}

namespace YuzuGen.System.Collections.Generic
{
	class List_ISampleMember_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new List_ISampleMember_JsonDeserializer Instance = new();

		public List_ISampleMember_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderInt(new global::System.Collections.Generic.List<global::YuzuTest.ISampleMember>());
		}

		public override object FromReaderInt(object obj)
		{
			var result = (global::System.Collections.Generic.List<global::YuzuTest.ISampleMember>)obj;
			Require('[');
			if (SkipSpacesCarefully() == ']') {
				Require(']');
			}
			else {
				do {
					var tmp1 = YuzuGen.YuzuTest.ISampleMember_JsonDeserializer.Instance.FromReaderInterface<global::YuzuTest.ISampleMember>(Reader);
					result.Add(tmp1);
				} while (Require(']', ',') == ',');
			}
			return result;
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::System.Collections.Generic.List<global::YuzuTest.ISampleMember>(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::System.Collections.Generic.List<global::YuzuTest.ISampleMember>)obj;
			return result;
		}
	}

}

namespace YuzuGen.YuzuTest
{
	class SampleArray_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleArray_JsonDeserializer Instance = new();

		public SampleArray_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
			JsonOptions.ArrayLengthPrefix = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleArray>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleArray(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleArray)obj;
			if ("A" != name) throw new YuzuException("A!=" + name);
			result.A = RequireOrNull('[') ? null : new string[0];
			if (result.A != null) {
				if (SkipSpacesCarefully() != ']') {
					var tmp1 = new string[RequireUInt()];
					for(int tmp2 = 0; tmp2 < tmp1.Length; ++tmp2) {
						Require(',');
						tmp1[tmp2] = RequireString();
					}
					result.A = tmp1;
				}
				Require(']');
			}
			name = GetNextName(false);
			return result;
		}
	}

	class SampleArrayOfArray_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleArrayOfArray_JsonDeserializer Instance = new();

		public SampleArrayOfArray_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleArrayOfArray>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleArrayOfArray(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleArrayOfArray)obj;
			if ("A" != name) throw new YuzuException("A!=" + name);
			result.A = RequireOrNull('[') ? null : new int[0][];
			if (result.A != null) {
				if (SkipSpacesCarefully() == ']') {
					Require(']');
				}
				else {
					var tmp1 = new List<int[]>();
					do {
						var tmp2 = RequireOrNull('[') ? null : new int[0];
						if (tmp2 != null) {
							if (SkipSpacesCarefully() == ']') {
								Require(']');
							}
							else {
								var tmp3 = new List<int>();
								do {
									var tmp4 = RequireInt();
									tmp3.Add(tmp4);
								} while (Require(']', ',') == ',');
								tmp2 = tmp3.ToArray();
							}
						}
						tmp1.Add(tmp2);
					} while (Require(']', ',') == ',');
					result.A = tmp1.ToArray();
				}
			}
			name = GetNextName(false);
			return result;
		}
	}

	class SampleArrayNDim_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleArrayNDim_JsonDeserializer Instance = new();

		public SampleArrayNDim_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleArrayNDim>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleArrayNDim(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleArrayNDim)obj;
			if ("A" != name) throw new YuzuException("A!=" + name);
			result.A = (int[,])ReadArrayNDim(typeof(int[,]));
			name = GetNextName(false);
			if ("B" != name) throw new YuzuException("B!=" + name);
			result.B = (string[,,])ReadArrayNDim(typeof(string[,,]));
			name = GetNextName(false);
			return result;
		}
	}

	class SampleBase_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleBase_JsonDeserializer Instance = new();

		public SampleBase_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleBase>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleBase(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleBase)obj;
			if ("FBase" != name) throw new YuzuException("FBase!=" + name);
			result.FBase = RequireInt();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleDerivedA_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleDerivedA_JsonDeserializer Instance = new();

		public SampleDerivedA_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleDerivedA>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleDerivedA(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleDerivedA)obj;
			if ("FBase" != name) throw new YuzuException("FBase!=" + name);
			result.FBase = RequireInt();
			name = GetNextName(false);
			if ("FA" != name) throw new YuzuException("FA!=" + name);
			result.FA = RequireInt();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleDerivedB_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleDerivedB_JsonDeserializer Instance = new();

		public SampleDerivedB_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleDerivedB>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleDerivedB(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleDerivedB)obj;
			if ("FBase" != name) throw new YuzuException("FBase!=" + name);
			result.FBase = RequireInt();
			name = GetNextName(false);
			if ("FB" != name) throw new YuzuException("FB!=" + name);
			result.FB = RequireInt();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleMatrix_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleMatrix_JsonDeserializer Instance = new();

		public SampleMatrix_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleMatrix>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleMatrix(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleMatrix)obj;
			if ("M" != name) throw new YuzuException("M!=" + name);
			result.M = RequireOrNull('[') ? null : new global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>>();
			if (result.M != null) {
				if (SkipSpacesCarefully() == ']') {
					Require(']');
				}
				else {
					do {
						var tmp1 = RequireOrNull('[') ? null : new global::System.Collections.Generic.List<int>();
						if (tmp1 != null) {
							if (SkipSpacesCarefully() == ']') {
								Require(']');
							}
							else {
								do {
									var tmp2 = RequireInt();
									tmp1.Add(tmp2);
								} while (Require(']', ',') == ',');
							}
						}
						result.M.Add(tmp1);
					} while (Require(']', ',') == ',');
				}
			}
			name = GetNextName(false);
			return result;
		}
	}

	class SamplePoint_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SamplePoint_JsonDeserializer Instance = new();

		public SamplePoint_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SamplePoint>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SamplePoint(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SamplePoint)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			if ("Y" != name) throw new YuzuException("Y!=" + name);
			result.Y = RequireInt();
			name = GetNextName(false);
			return result;
		}

		protected override object ReadFieldsCompact(object obj)
		{
			var result = (global::YuzuTest.SamplePoint)obj;
			result.X = RequireInt();
			Require(',');
			result.Y = RequireInt();
			Require(']');
			return result;
		}
	}

	class SampleRect_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleRect_JsonDeserializer Instance = new();

		public SampleRect_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleRect>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleRect(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleRect)obj;
			if ("A" != name) throw new YuzuException("A!=" + name);
			result.A = YuzuGen.YuzuTest.SamplePoint_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.SamplePoint>(Reader);
			name = GetNextName(false);
			if ("B" != name) throw new YuzuException("B!=" + name);
			result.B = YuzuGen.YuzuTest.SamplePoint_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.SamplePoint>(Reader);
			name = GetNextName(false);
			return result;
		}
	}

	class SampleDate_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleDate_JsonDeserializer Instance = new();

		public SampleDate_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleDate>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleDate(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleDate)obj;
			if ("D" != name) throw new YuzuException("D!=" + name);
			result.D = RequireDateTime();
			name = GetNextName(false);
			if ("DOfs" != name) throw new YuzuException("DOfs!=" + name);
			result.DOfs = RequireDateTimeOffset();
			name = GetNextName(false);
			if ("T" != name) throw new YuzuException("T!=" + name);
			result.T = RequireTimeSpan();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleGuid_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleGuid_JsonDeserializer Instance = new();

		public SampleGuid_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleGuid>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleGuid(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleGuid)obj;
			if ("G" != name) throw new YuzuException("G!=" + name);
			result.G = RequireGuid();
			name = GetNextName(false);
			return result;
		}
	}

	class Color_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new Color_JsonDeserializer Instance = new();

		public Color_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.Color>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.Color(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.Color)obj;
			if ("B" != name) throw new YuzuException("B!=" + name);
			result.B = checked((byte)RequireUInt());
			name = GetNextName(false);
			if ("G" != name) throw new YuzuException("G!=" + name);
			result.G = checked((byte)RequireUInt());
			name = GetNextName(false);
			if ("R" != name) throw new YuzuException("R!=" + name);
			result.R = checked((byte)RequireUInt());
			name = GetNextName(false);
			return result;
		}

		protected override object ReadFieldsCompact(object obj)
		{
			var result = (global::YuzuTest.Color)obj;
			result.B = checked((byte)RequireUInt());
			Require(',');
			result.G = checked((byte)RequireUInt());
			Require(',');
			result.R = checked((byte)RequireUInt());
			Require(']');
			return result;
		}
	}

}

namespace YuzuGen.System.Collections.Generic
{
	class List_List_Int32_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new List_List_Int32_JsonDeserializer Instance = new();

		public List_List_Int32_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderInt(new global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>>());
		}

		public override object FromReaderInt(object obj)
		{
			var result = (global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>>)obj;
			Require('[');
			if (SkipSpacesCarefully() == ']') {
				Require(']');
			}
			else {
				do {
					var tmp1 = RequireOrNull('[') ? null : new global::System.Collections.Generic.List<int>();
					if (tmp1 != null) {
						if (SkipSpacesCarefully() == ']') {
							Require(']');
						}
						else {
							do {
								var tmp2 = RequireInt();
								tmp1.Add(tmp2);
							} while (Require(']', ',') == ',');
						}
					}
					result.Add(tmp1);
				} while (Require(']', ',') == ',');
			}
			return result;
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>>(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>>)obj;
			return result;
		}
	}

}

namespace YuzuGen.YuzuTest
{
	class SampleClassList_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleClassList_JsonDeserializer Instance = new();

		public SampleClassList_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleClassList>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleClassList(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleClassList)obj;
			if ("E" != name) throw new YuzuException("E!=" + name);
			result.E = RequireOrNull('[') ? null : new global::System.Collections.Generic.List<global::YuzuTest.SampleBase>();
			if (result.E != null) {
				if (SkipSpacesCarefully() == ']') {
					Require(']');
				}
				else {
					do {
						var tmp1 = YuzuGen.YuzuTest.SampleBase_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.SampleBase>(Reader);
						result.E.Add(tmp1);
					} while (Require(']', ',') == ',');
				}
			}
			name = GetNextName(false);
			return result;
		}
	}

	class SampleSmallTypes_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleSmallTypes_JsonDeserializer Instance = new();

		public SampleSmallTypes_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleSmallTypes>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleSmallTypes(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleSmallTypes)obj;
			if ("B" != name) throw new YuzuException("B!=" + name);
			result.B = checked((byte)RequireUInt());
			name = GetNextName(false);
			if ("Ch" != name) throw new YuzuException("Ch!=" + name);
			result.Ch = RequireChar();
			name = GetNextName(false);
			if ("Sb" != name) throw new YuzuException("Sb!=" + name);
			result.Sb = checked((sbyte)RequireInt());
			name = GetNextName(false);
			if ("Sh" != name) throw new YuzuException("Sh!=" + name);
			result.Sh = checked((short)RequireInt());
			name = GetNextName(false);
			if ("USh" != name) throw new YuzuException("USh!=" + name);
			result.USh = checked((ushort)RequireUInt());
			name = GetNextName(false);
			return result;
		}
	}

	class SampleWithNullFieldCompact_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleWithNullFieldCompact_JsonDeserializer Instance = new();

		public SampleWithNullFieldCompact_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleWithNullFieldCompact>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleWithNullFieldCompact(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleWithNullFieldCompact)obj;
			if ("N" != name) throw new YuzuException("N!=" + name);
			result.N = YuzuGen.YuzuTest.Sample1_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Sample1>(Reader);
			name = GetNextName(false);
			return result;
		}

		protected override object ReadFieldsCompact(object obj)
		{
			var result = (global::YuzuTest.SampleWithNullFieldCompact)obj;
			result.N = YuzuGen.YuzuTest.Sample1_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Sample1>(Reader);
			Require(']');
			return result;
		}
	}

	class SampleNested__NestedClass_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleNested__NestedClass_JsonDeserializer Instance = new();

		public SampleNested__NestedClass_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleNested.NestedClass>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleNested.NestedClass(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleNested.NestedClass)obj;
			if ("Z" == name) {
				result.Z = RequireInt();
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleNested_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleNested_JsonDeserializer Instance = new();

		public SampleNested_JsonDeserializer()
		{
			Options.TagMode = TagMode.Names;
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleNested>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleNested(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleNested)obj;
			if ("C" != name) throw new YuzuException("C!=" + name);
			result.C = YuzuGen.YuzuTest.SampleNested__NestedClass_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.SampleNested.NestedClass>(Reader);
			name = GetNextName(false);
			if ("E" != name) throw new YuzuException("E!=" + name);
			result.E = (global::YuzuTest.SampleNested.NestedEnum)Enum.Parse(typeof(global::YuzuTest.SampleNested.NestedEnum), RequireString());
			name = GetNextName(false);
			if ("Z" == name) {
				result.Z = RequireOrNull('[') ? null : new global::YuzuTest.SampleNested.NestedEnum[0];
				if (result.Z != null) {
					if (SkipSpacesCarefully() == ']') {
						Require(']');
					}
					else {
						var tmp1 = new List<global::YuzuTest.SampleNested.NestedEnum>();
						do {
							var tmp2 = (global::YuzuTest.SampleNested.NestedEnum)Enum.Parse(typeof(global::YuzuTest.SampleNested.NestedEnum), RequireString());
							tmp1.Add(tmp2);
						} while (Require(']', ',') == ',');
						result.Z = tmp1.ToArray();
					}
				}
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SamplePerson_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SamplePerson_JsonDeserializer Instance = new();

		public SamplePerson_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SamplePerson>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SamplePerson(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SamplePerson)obj;
			if ("1" != name) throw new YuzuException("1!=" + name);
			result.Name = RequireString();
			name = GetNextName(false);
			if ("2" != name) throw new YuzuException("2!=" + name);
			result.Birth = RequireDateTime();
			name = GetNextName(false);
			if ("3" != name) throw new YuzuException("3!=" + name);
			result.Children = RequireOrNull('[') ? null : new global::System.Collections.Generic.List<global::YuzuTest.SamplePerson>();
			if (result.Children != null) {
				if (SkipSpacesCarefully() == ']') {
					Require(']');
				}
				else {
					do {
						var tmp1 = YuzuGen.YuzuTest.SamplePerson_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.SamplePerson>(Reader);
						result.Children.Add(tmp1);
					} while (Require(']', ',') == ',');
				}
			}
			name = GetNextName(false);
			if ("4" != name) throw new YuzuException("4!=" + name);
			result.EyeColor = YuzuGen.YuzuTest.Color_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Color>(Reader);
			name = GetNextName(false);
			return result;
		}
	}

	class ISample_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new ISample_JsonDeserializer Instance = new();

		public ISample_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderInterface<global::YuzuTest.ISample>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return null;
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.ISample)obj;
			return result;
		}
	}

	class SampleInterfaced_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleInterfaced_JsonDeserializer Instance = new();

		public SampleInterfaced_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleInterfaced>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleInterfaced(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleInterfaced)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleInterfaceField_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleInterfaceField_JsonDeserializer Instance = new();

		public SampleInterfaceField_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleInterfaceField>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleInterfaceField(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleInterfaceField)obj;
			if ("I" != name) throw new YuzuException("I!=" + name);
			result.I = YuzuGen.YuzuTest.ISample_JsonDeserializer.Instance.FromReaderInterface<global::YuzuTest.ISample>(Reader);
			name = GetNextName(false);
			return result;
		}
	}

	class SampleInterfacedGeneric_String_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleInterfacedGeneric_String_JsonDeserializer Instance = new();

		public SampleInterfacedGeneric_String_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleInterfacedGeneric<string>>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleInterfacedGeneric<string>(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleInterfacedGeneric<string>)obj;
			if ("G" != name) throw new YuzuException("G!=" + name);
			result.G = RequireString();
			name = GetNextName(false);
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleAbstract_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleAbstract_JsonDeserializer Instance = new();

		public SampleAbstract_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderInterface<global::YuzuTest.SampleAbstract>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return null;
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleAbstract)obj;
			return result;
		}
	}

	class SampleConcrete_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleConcrete_JsonDeserializer Instance = new();

		public SampleConcrete_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleConcrete>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleConcrete(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleConcrete)obj;
			if ("XX" != name) throw new YuzuException("XX!=" + name);
			result.XX = RequireInt();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleCollection_Int32_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleCollection_Int32_JsonDeserializer Instance = new();

		public SampleCollection_Int32_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderInt(new global::YuzuTest.SampleCollection<int>());
		}

		public override object FromReaderInt(object obj)
		{
			var result = (global::YuzuTest.SampleCollection<int>)obj;
			Require('[');
			if (SkipSpacesCarefully() == ']') {
				Require(']');
			}
			else {
				do {
					var tmp1 = RequireInt();
					result.Add(tmp1);
				} while (Require(']', ',') == ',');
			}
			return result;
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleCollection<int>(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleCollection<int>)obj;
			return result;
		}
	}

	class SampleExplicitCollection_Int32_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleExplicitCollection_Int32_JsonDeserializer Instance = new();

		public SampleExplicitCollection_Int32_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderInt(new global::YuzuTest.SampleExplicitCollection<int>());
		}

		public override object FromReaderInt(object obj)
		{
			var result = (global::YuzuTest.SampleExplicitCollection<int>)obj;
			Require('[');
			if (SkipSpacesCarefully() == ']') {
				Require(']');
			}
			else {
				do {
					var tmp2 = RequireInt();
					((global::System.Collections.Generic.ICollection<int>)result).Add(tmp2);
				} while (Require(']', ',') == ',');
			}
			return result;
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleExplicitCollection<int>(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleExplicitCollection<int>)obj;
			return result;
		}
	}

	class SampleWithCollection_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleWithCollection_JsonDeserializer Instance = new();

		public SampleWithCollection_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleWithCollection>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleWithCollection(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleWithCollection)obj;
			if ("A" != name) throw new YuzuException("A!=" + name);
			result.A = RequireOrNull('[') ? null : new global::YuzuTest.SampleCollection<global::YuzuTest.ISample>();
			if (result.A != null) {
				if (SkipSpacesCarefully() == ']') {
					Require(']');
				}
				else {
					do {
						var tmp1 = YuzuGen.YuzuTest.ISample_JsonDeserializer.Instance.FromReaderInterface<global::YuzuTest.ISample>(Reader);
						result.A.Add(tmp1);
					} while (Require(']', ',') == ',');
				}
			}
			name = GetNextName(false);
			if ("B" == name) {
				result.B = RequireOrNull('[') ? null : new global::YuzuTest.SampleCollection<int>();
				if (result.B != null) {
					if (SkipSpacesCarefully() == ']') {
						Require(']');
					}
					else {
						do {
							var tmp2 = RequireInt();
							result.B.Add(tmp2);
						} while (Require(']', ',') == ',');
					}
				}
				name = GetNextName(false);
			}
			if ("C" == name) {
				result.C = RequireOrNull('[') ? null : new global::YuzuTest.SampleCollection<global::YuzuTest.SamplePoint>();
				if (result.C != null) {
					if (SkipSpacesCarefully() == ']') {
						Require(']');
					}
					else {
						do {
							var tmp3 = YuzuGen.YuzuTest.SamplePoint_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.SamplePoint>(Reader);
							result.C.Add(tmp3);
						} while (Require(']', ',') == ',');
					}
				}
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleConcreteCollection_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleConcreteCollection_JsonDeserializer Instance = new();

		public SampleConcreteCollection_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderInt(new global::YuzuTest.SampleConcreteCollection());
		}

		public override object FromReaderInt(object obj)
		{
			var result = (global::YuzuTest.SampleConcreteCollection)obj;
			Require('[');
			if (SkipSpacesCarefully() == ']') {
				Require(']');
			}
			else {
				do {
					var tmp4 = RequireInt();
					result.Add(tmp4);
				} while (Require(']', ',') == ',');
			}
			return result;
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleConcreteCollection(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleConcreteCollection)obj;
			return result;
		}
	}

	class SampleAfter2_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleAfter2_JsonDeserializer Instance = new();

		public SampleAfter2_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleAfter2>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleAfter2(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleAfter2)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireString();
			name = GetNextName(false);
			result.After2();
			result.After3();
			result.After();
			return result;
		}
	}

	class SampleAfterSerialization_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleAfterSerialization_JsonDeserializer Instance = new();

		public SampleAfterSerialization_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleAfterSerialization>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleAfterSerialization(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleAfterSerialization)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireString();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleBeforeDeserialization_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleBeforeDeserialization_JsonDeserializer Instance = new();

		public SampleBeforeDeserialization_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleBeforeDeserialization>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleBeforeDeserialization(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleBeforeDeserialization)obj;
			result.Before();
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireString();
			name = GetNextName(false);
			return result;
		}
	}

	class SampleMerge_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleMerge_JsonDeserializer Instance = new();

		public SampleMerge_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleMerge>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleMerge(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleMerge)obj;
			if ("DI" != name) throw new YuzuException("DI!=" + name);
			Require('{');
			if (SkipSpacesCarefully() == '}') {
				Require('}');
			}
			else {
				do {
					var tmp1 = RequireString();
					Require(':');
					var tmp2 = RequireInt();
					result.DI.Add(int.Parse(tmp1), tmp2);
				} while (Require('}', ',') == ',');
			}
			name = GetNextName(false);
			if ("LI" != name) throw new YuzuException("LI!=" + name);
			Require('[');
			if (SkipSpacesCarefully() == ']') {
				Require(']');
			}
			else {
				do {
					var tmp3 = RequireInt();
					result.LI.Add(tmp3);
				} while (Require(']', ',') == ',');
			}
			name = GetNextName(false);
			if ("M" == name) {
				YuzuGen.YuzuTest.Sample1_JsonDeserializer.Instance.FromReader(result.M, Reader);
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleAssemblyDerivedR_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleAssemblyDerivedR_JsonDeserializer Instance = new();

		public SampleAssemblyDerivedR_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleAssemblyDerivedR>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleAssemblyDerivedR(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleAssemblyDerivedR)obj;
			if ("P" == name) {
				result.P = checked((short)RequireInt());
				name = GetNextName(false);
			}
			if ("R" == name) {
				result.R = RequireString();
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleAliasMany_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleAliasMany_JsonDeserializer Instance = new();

		public SampleAliasMany_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.SampleAliasMany>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.SampleAliasMany(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SampleAliasMany)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			return result;
		}
	}

	class SamplePrivateConstructor_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SamplePrivateConstructor_JsonDeserializer Instance = new();

		public SamplePrivateConstructor_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTypedFactory(Reader, global::YuzuTest.SamplePrivateConstructor.Make);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(global::YuzuTest.SamplePrivateConstructor.Make(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.SamplePrivateConstructor)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			return result;
		}
	}

}

namespace YuzuGen.System.Collections.Generic
{
	class List_SampleAssemblyBase_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new List_SampleAssemblyBase_JsonDeserializer Instance = new();

		public List_SampleAssemblyBase_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderInt(new global::System.Collections.Generic.List<global::YuzuTestAssembly.SampleAssemblyBase>());
		}

		public override object FromReaderInt(object obj)
		{
			var result = (global::System.Collections.Generic.List<global::YuzuTestAssembly.SampleAssemblyBase>)obj;
			Require('[');
			if (SkipSpacesCarefully() == ']') {
				Require(']');
			}
			else {
				do {
					var tmp1 = YuzuGen.YuzuTestAssembly.SampleAssemblyBase_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTestAssembly.SampleAssemblyBase>(Reader);
					result.Add(tmp1);
				} while (Require(']', ',') == ',');
			}
			return result;
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::System.Collections.Generic.List<global::YuzuTestAssembly.SampleAssemblyBase>(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::System.Collections.Generic.List<global::YuzuTestAssembly.SampleAssemblyBase>)obj;
			return result;
		}
	}

}

namespace YuzuGen.YuzuTestAssembly
{
	class SampleAssemblyBase_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleAssemblyBase_JsonDeserializer Instance = new();

		public SampleAssemblyBase_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTestAssembly.SampleAssemblyBase>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTestAssembly.SampleAssemblyBase(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTestAssembly.SampleAssemblyBase)obj;
			if ("P" == name) {
				result.P = checked((short)RequireInt());
				name = GetNextName(false);
			}
			return result;
		}
	}

	class SampleAssemblyDerivedQ_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleAssemblyDerivedQ_JsonDeserializer Instance = new();

		public SampleAssemblyDerivedQ_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTestAssembly.SampleAssemblyDerivedQ>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTestAssembly.SampleAssemblyDerivedQ(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTestAssembly.SampleAssemblyDerivedQ)obj;
			if ("P" == name) {
				result.P = checked((short)RequireInt());
				name = GetNextName(false);
			}
			if ("Q" == name) {
				result.Q = checked((short)RequireInt());
				name = GetNextName(false);
			}
			return result;
		}
	}

}

namespace YuzuGen.YuzuTest2
{
	class SampleNamespace_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new SampleNamespace_JsonDeserializer Instance = new();

		public SampleNamespace_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest2.SampleNamespace>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest2.SampleNamespace(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest2.SampleNamespace)obj;
			if ("B" != name) throw new YuzuException("B!=" + name);
			result.B = YuzuGen.YuzuTest.SampleBase_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.SampleBase>(Reader);
			name = GetNextName(false);
			return result;
		}
	}

}

namespace YuzuGen.YuzuTest
{
	class A__B__C__D__E__Sample2Struct_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new A__B__C__D__E__Sample2Struct_JsonDeserializer Instance = new();

		public A__B__C__D__E__Sample2Struct_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.A.B.C.D.E.Sample2Struct>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.A.B.C.D.E.Sample2Struct(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.A.B.C.D.E.Sample2Struct)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			if ("Y" == name) {
				result.Y = RequireString();
				name = GetNextName(false);
			}
			return result;
		}
	}

	class A__B__C__D__E__SampleSerializeIfStruct_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new A__B__C__D__E__SampleSerializeIfStruct_JsonDeserializer Instance = new();

		public A__B__C__D__E__SampleSerializeIfStruct_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.A.B.C.D.E.SampleSerializeIfStruct>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.A.B.C.D.E.SampleSerializeIfStruct(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.A.B.C.D.E.SampleSerializeIfStruct)obj;
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			if ("Y" == name) {
				result.Y = YuzuGen.YuzuTest.Sample1_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Sample1>(Reader);
				name = GetNextName(false);
			}
			return result;
		}
	}

	class A__B__C__D__E__SampleSerializeIfOnFieldStruct_JsonDeserializer : JsonDeserializerGenBase
	{
		public static new A__B__C__D__E__SampleSerializeIfOnFieldStruct_JsonDeserializer Instance = new();

		public A__B__C__D__E__SampleSerializeIfOnFieldStruct_JsonDeserializer()
		{
			JsonOptions.EnumAsString = true;
		}

		public override object FromReaderInt()
		{
			return FromReaderTyped<global::YuzuTest.A.B.C.D.E.SampleSerializeIfOnFieldStruct>(Reader);
		}

		public override object FromReaderIntPartial(string name)
		{
			return ReadFields(new global::YuzuTest.A.B.C.D.E.SampleSerializeIfOnFieldStruct(), name);
		}

		protected override object ReadFields(object obj, string name)
		{
			var result = (global::YuzuTest.A.B.C.D.E.SampleSerializeIfOnFieldStruct)obj;
			if ("W" == name) {
				result.W = YuzuGen.YuzuTest.Sample1_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Sample1>(Reader);
				name = GetNextName(false);
			}
			if ("X" != name) throw new YuzuException("X!=" + name);
			result.X = RequireInt();
			name = GetNextName(false);
			if ("Y" == name) {
				result.Y = YuzuGen.YuzuTest.Sample1_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Sample1>(Reader);
				name = GetNextName(false);
			}
			if ("Z" == name) {
				result.Z = YuzuGen.YuzuTest.Sample1_JsonDeserializer.Instance.FromReaderTyped<global::YuzuTest.Sample1>(Reader);
				name = GetNextName(false);
			}
			return result;
		}
	}

}
