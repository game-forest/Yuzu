﻿using System.Collections.Generic;

using Yuzu;

namespace YuzuTest
{
	[YuzuAlias("SampleOrder")]
	class SampleOrder
	{
		[YuzuMember]
		public int StarterPackOfferEndTime;
		[YuzuMember]
		public bool StartGoldInitialized;
	}

	[YuzuAlias("SampleOrder")]
	class SampleOrderExt : SampleOrder
	{
		[YuzuMember]
#pragma warning disable CS0649
		public bool StarterPackOffered;
#pragma warning restore CS0649
	}

	[YuzuAlias("NewNameForAliasField")]
	class SampleAliasField
	{
		[YuzuMember]
		public int X;
	}

	class SampleWithAliasedField
	{
		[YuzuMember]
		public SampleAliasField F;
	}

	[YuzuAlias("DifferentName")]
	public class SampleAlias
	{
		[YuzuRequired]
		public int X;
	}

	[YuzuAlias(read: ["Name1", "Name2"])]
	public class SampleAliasMany
	{
		[YuzuRequired]
		public int X;
	}

	public class RenameDictionaryValue
	{
		[YuzuAlias("YuzuTest.RenameDictionaryValue+Sample, YuzuTest")]
		public class Sample_Renamed
		{
			[YuzuMember]
			public int F;
		}
		[YuzuMember]
		public Dictionary<int, Sample_Renamed> Samples = [];

		public static RenameDictionaryValue Data = new() {
			Samples = new Dictionary<int, Sample_Renamed> { { 1, new Sample_Renamed { F = 1 } } }
		};
	}

	public class RenameDictionaryKey
	{
		[YuzuAlias("YuzuTest.RenameDictionaryKey+Sample, YuzuTest")]
		public struct Sample_Renamed
		{
			[YuzuMember]
			public int F;
		}
		[YuzuMember]
		public Dictionary<Sample_Renamed, int> Samples = [];

		public static RenameDictionaryKey Data = new() {
			Samples = new Dictionary<Sample_Renamed, int> { { new Sample_Renamed { F = 2 }, 2 } }
		};
	}

	public class RenameListType
	{
		[YuzuAlias("YuzuTest.RenameListType+Sample, YuzuTest")]
		public class Sample_Renamed
		{
			[YuzuMember]
			public int F;
		}
		[YuzuMember]
		public List<Sample_Renamed> Samples = [];

		public static RenameListType Data = new() {
			Samples = [new Sample_Renamed { F = 3 }]
		};
	}

	public class RenameHashSetType
	{
		[YuzuAlias("YuzuTest.RenameHashSetType+Sample, YuzuTest")]
		public class Sample_Renamed
		{
			[YuzuMember]
			public int F;
		}
		[YuzuMember]
		public HashSet<Sample_Renamed> Samples = [];

		public static RenameHashSetType Data = new() {
			Samples = [new Sample_Renamed { F = 4 }]
		};
	}

	public class RenameCustomGenericType
	{
		public class Sample
		{
			[YuzuMember]
			public int F;
		}

		[YuzuAlias("YuzuTest.RenameCustomGenericType+GenericSample`1[[YuzuTest.RenameCustomGenericType+Sample, YuzuTest]], YuzuTest")]
		public class GenericSample_Renamed<T>
		{
			[YuzuMember]
			public T Type;

			public static string TypeFieldName => nameof(Type);
		}
		[YuzuMember]
		public GenericSample_Renamed<Sample> Samples = new();

		public static RenameCustomGenericType Data = new() {
			Samples = new GenericSample_Renamed<Sample>() { Type = new Sample { F = 5 } }
		};
	}

	public class RenameCustomGenericTypeGenericArgumentType
	{
		[YuzuAlias("YuzuTest.RenameCustomGenericTypeGenericArgumentType+Sample, YuzuTest")]
		public class Sample_Renamed
		{
			[YuzuMember]
			public int F;
		}
		[YuzuAlias("YuzuTest.RenameCustomGenericTypeGenericArgumentType+GenericSample`1[[YuzuTest.RenameCustomGenericTypeGenericArgumentType+Sample, YuzuTest]], YuzuTest")]
		public class GenericSample<T>
		{
			[YuzuMember]
			public T Type;
		}
		[YuzuMember]
		public GenericSample<Sample_Renamed> Samples = new();

		public static RenameCustomGenericTypeGenericArgumentType Data =
			new() {
				Samples = new GenericSample<Sample_Renamed>() { Type = new Sample_Renamed { F = 6 } }
			};
	}

	public class EnclosingClassForEnclosingClass
	{
		[YuzuMember]
		public SampleAliasForNestedClassWhenEnclosingClassRenamed_Renamed F;

		public static EnclosingClassForEnclosingClass Sample = new() {
			F = new SampleAliasForNestedClassWhenEnclosingClassRenamed_Renamed {
				NestedClassField = new SampleAliasForNestedClassWhenEnclosingClassRenamed_Renamed.NestedClass { F = 666 }
			}
		};
	}

	[YuzuAlias("YuzuTest.SampleAliasForNestedClassWhenEnclosingClassRenamed, YuzuTest")]
	public class SampleAliasForNestedClassWhenEnclosingClassRenamed_Renamed
	{
		[YuzuAlias("YuzuTest.SampleAliasForNestedClassWhenEnclosingClassRenamed+NestedClass, YuzuTest")]
		public class NestedClass
		{
			[YuzuMember]
			public int F;
		}
		[YuzuMember]
		public NestedClass NestedClassField;
	}

	[YuzuAlias("YuzuTest.SampleAliasClassToBeRenamed, YuzuTest")]
	public class SampleAliasClassToBeRenamed_Renamed
	{
		[YuzuMember]
		public int Foo;

		[YuzuMember]
		public SampleAliasClassToBeRenamed_Renamed Bar_Renamed;

		public YuzuUnknownStorage UnknownStorage = new();
	}

	public class SampleAliasWithinUnknownContainer
	{
		[YuzuMember]
		public SampleAliasClassToBeRenamed_Renamed Foo_Renamed;

		public YuzuUnknownStorage UnknownStorage = new();
	}
}
