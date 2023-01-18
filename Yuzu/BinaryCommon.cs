using System;
using System.Collections.Generic;
using System.Linq;
using Yuzu.Metadata;
using Yuzu.Util;

namespace Yuzu.Binary
{
	public class BinarySerializeOptions
	{
		public byte[] Signature = new byte[] { (byte)'Y', (byte)'B', (byte)'0', (byte)'1' };
		public bool AutoSignature = false;
		public bool Unordered = false;
	}

	// These values are part of format.
	public enum RoughType : byte
	{
		None = 0,
		SByte = 1,
		Byte = 2,
		Short = 3,
		UShort = 4,
		Int = 5,
		UInt = 6,
		Long = 7,
		ULong = 8,
		Bool = 9,
		Char = 10,
		Float = 11,
		Double = 12,
		Decimal = 13,
		DateTime = 14,
		TimeSpan = 15,
		String = 16,
		Any = 17,
		Nullable = 18,
		DateTimeOffset = 19,
		Guid = 20,

		Record = 32,
		Sequence = 33,
		Mapping = 34,
		NDimArray = 35,

		FirstAtom = SByte,
		LastAtom = Guid,
	}

	internal static class RT
	{
		public static Type[] RoughTypeToType = new Type[] {
			null,
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(bool),
			typeof(char),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(string),
			typeof(object),
			null,
			typeof(DateTimeOffset),
			typeof(Guid),
		};

		public static bool IsRecord(this Type t)
		{
			// TODO: when type.IsArray is true type.IsClass is also true, which seems to be a bug in this method
			return t.IsClass || t.IsInterface || Utils.IsStruct(t);
		}
	}

	public class ReaderClassDef
	{
		public class FieldDef
		{
			public string Name;
			public Type Type;
			public int OurIndex; // 1-based
			public Action<BinaryDeserializer, object> ReadFunc;
		}

		public Meta Meta;
		public const int EOF = short.MaxValue;
		public Action<BinaryDeserializer, ReaderClassDef, object> ReadFields;
		public Func<BinaryDeserializer, ReaderClassDef, object> Make;
		public List<FieldDef> Fields = new List<FieldDef> { new FieldDef { OurIndex = EOF } };
		internal CompletionRecord CompletionRecord;
	}

	internal class CompletionRecord
	{
		public string TypeName;
		public byte[] Buffer;
	}

	public class YuzuCache
	{
		internal List<ReaderClassDef> ReaderCache;
		internal Dictionary<Type, BinarySerializer.ClassDef> WriterCache;
		internal const int Version = 0;

		public static YuzuCache ReadFromStream(System.IO.MemoryStream ms)
		{
			using var reader = new System.IO.BinaryReader(ms, System.Text.Encoding.UTF8, leaveOpen: false);
			var version = reader.ReadInt32();
			if (version > Version) {
				throw new YuzuException($"Cache version {version} is greater than current {Version}.");
			}
			var classDefCount = reader.ReadInt32();
			if (classDefCount == 0) {
				return null;
			}
			var readerClassDefCache = new List<ReaderClassDef>();
			for (int i = 0; i < classDefCount; i++) {
				var classDef = BinaryDeserializer.ReadCachedClassDef(reader, i, ms);
				readerClassDefCache.Add(classDef);
			}
			return new YuzuCache {
				ReaderCache = readerClassDefCache,
			};
		}

		internal void CompleteReaderCache(CommonOptions options)
		{
			var yd = new BinaryDeserializer { Options = options };
			foreach (var i in ReaderCache) {
				if (i.CompletionRecord != null) {
					BinaryDeserializer.TryCompleteClassDef(yd, i, out var e);
				}
			}
		}

		public static void WriteToStream(
			System.IO.MemoryStream ms, YuzuCache existingCache, IEnumerable<Type> types, CommonOptions options
		) {
			using var writer = new System.IO.BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true);
			// version
			writer.Write(0);
			var tl = types.ToList();

			var yd = new global::Yuzu.Binary.BinaryDeserializer { Options = options };
			var ys = new global::Yuzu.Binary.BinarySerializer { Options = options };
			short id = -1;
			if (existingCache != null) {
				// TODO:
				//  case A: type no longer exists. For that case
				//      trace full story from when we are parsing it's class def
				//  case B: type exists => we must preserve it's id,
				//      but must regenerate it's class def to reflect current fields
				//  case C: same as B, but we detect some
				//      fields become missing =>? should we alarm?
				foreach (var i in existingCache.ReaderCache) {
					if (i.CompletionRecord != null) {
						if (!BinaryDeserializer.TryCompleteClassDef(yd, i, out var _)) {
							continue;
						}
					}
					tl.Remove(i.Meta.Type);
				}
				writer.Write(tl.Count + existingCache.ReaderCache.Count);
				foreach (var i in existingCache.ReaderCache) {
					if (i.CompletionRecord != null) {
						// in case we failed to deserialize the old record
						// (i.e. complete classdef) write it back as it was.
						writer.Write(id);
						// TypeName should be in buffer already
						// writer.Write(i.CompletionRecord.TypeName);
						writer.Write(i.CompletionRecord.Buffer);
					} else {
						var classDef = new BinarySerializer.ClassDef {
							Id = id,
							Meta = i.Meta,
						};
						Yuzu.Binary.BinarySerializer.WriteClassDef(ys, writer, classDef);
					}
					id--;
				}
			} else {
				writer.Write(tl.Count);
			}
			tl.Sort((a, b) => a.FullName.CompareTo(b.FullName));
			foreach (var type in tl) {
				var classDef = new BinarySerializer.ClassDef {
					Id = id,
					Meta = Meta.Get(type, options),
				};
				Yuzu.Binary.BinarySerializer.WriteClassDef(ys, writer, classDef);
				id--;
			}
		}
	}

	internal class Record { }

	internal class YuzuUnknownBinary : YuzuUnknown
	{
		public ReaderClassDef Def;
	}
}
