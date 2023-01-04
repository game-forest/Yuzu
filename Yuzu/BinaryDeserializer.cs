using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Yuzu.Deserializer;
using Yuzu.Metadata;
using Yuzu.Util;

namespace Yuzu.Binary
{
	public class BinaryDeserializer : AbstractReaderDeserializer
	{
		public static BinaryDeserializer Instance = new BinaryDeserializer();

		public BinarySerializeOptions BinaryOptions = new BinarySerializeOptions();

		public BinaryDeserializer() { InitReaders(); }

		public override void Initialize() {}

		private static object ReadSByte(BinaryDeserializer d) => d.Reader.ReadSByte();
		private static object ReadByte(BinaryDeserializer d) => d.Reader.ReadByte();
		private static object ReadShort(BinaryDeserializer d) => d.Reader.ReadInt16();
		private static object ReadUShort(BinaryDeserializer d) => d.Reader.ReadUInt16();
		private static object ReadInt(BinaryDeserializer d) => d.Reader.ReadInt32();
		private static object ReadUInt(BinaryDeserializer d) => d.Reader.ReadUInt32();
		private static object ReadLong(BinaryDeserializer d) => d.Reader.ReadInt64();
		private static object ReadULong(BinaryDeserializer d) => d.Reader.ReadUInt64();
		private static object ReadBool(BinaryDeserializer d) => d.Reader.ReadBoolean();
		private static object ReadChar(BinaryDeserializer d) => d.Reader.ReadChar();
		private static object ReadFloat(BinaryDeserializer d) => d.Reader.ReadSingle();
		private static object ReadDouble(BinaryDeserializer d) => d.Reader.ReadDouble();
		private static object ReadDecimal(BinaryDeserializer d) => d.Reader.ReadDecimal();

		private static DateTime ReadDateTime(BinaryDeserializer d) => DateTime.FromBinary(d.Reader.ReadInt64());
		protected static DateTimeOffset ReadDateTimeOffset(BinaryDeserializer d)
		{
			var dt = DateTime.FromBinary(d.Reader.ReadInt64());
			var t = new TimeSpan(d.Reader.ReadInt64());
			return new DateTimeOffset(dt, t);
		}
		private static TimeSpan ReadTimeSpan(BinaryDeserializer d) => new TimeSpan(d.Reader.ReadInt64());

		private static Guid ReadGuid(BinaryDeserializer d) => new Guid(d.Reader.ReadBytes(16));

		private static object ReadString(BinaryDeserializer d)
		{
			var s = d.Reader.ReadString();
			return s != "" ? s : d.Reader.ReadBoolean() ? null : "";
		}

		private static Type ReadType(BinaryDeserializer d)
		{
			var rt = (RoughType)d.Reader.ReadByte();
			if (RoughType.FirstAtom <= rt && rt <= RoughType.LastAtom) {
				var t = RT.roughTypeToType[(int)rt];
				if (t != null) return t;
			}
			switch (rt) {
				case RoughType.Sequence:
					return typeof(List<>).MakeGenericType(ReadType(d));
				case RoughType.Mapping:
					var k = ReadType(d);
					var v = ReadType(d);
					return typeof(Dictionary<,>).MakeGenericType(k, v);
				case RoughType.Record:
					return typeof(Record);
				case RoughType.Nullable:
					return typeof(Nullable<>).MakeGenericType(ReadType(d));
				default:
					throw d.Error("Unknown rough type {0}", rt);
			}
		}

		private static bool ReadCompatibleType(BinaryDeserializer d, Type expectedType)
		{
			if (expectedType.IsEnum)
				return ReadCompatibleType(d, Enum.GetUnderlyingType(expectedType));
			if (!expectedType.IsArray && expectedType.IsRecord() && (!expectedType.Namespace?.StartsWith("System") ?? true)) {
				var sg = Meta.Get(expectedType, d.Options).Surrogate;
				if (sg.SurrogateType != null && sg.FuncFrom != null)
					return ReadCompatibleType(d, sg.SurrogateType);
			}

			var rt = (RoughType)d.Reader.ReadByte();
			if (RoughType.FirstAtom <= rt && rt <= RoughType.LastAtom) {
				var t = RT.roughTypeToType[(int)rt];
				if (t != null) return t == expectedType;
			}
			if (expectedType.IsArray) {
				var r = expectedType.GetArrayRank();
				return
					(r == 1 ? rt == RoughType.Sequence :
						rt == RoughType.NDimArray && d.Reader.ReadByte() == r) &&
					ReadCompatibleType(d, expectedType.GetElementType());
			}

			var idict = Utils.GetIDictionary(expectedType);
			if (idict != null) {
				if (rt != RoughType.Mapping)
					return false;
				var g = expectedType.GetGenericArguments();
				return ReadCompatibleType(d, g[0]) && ReadCompatibleType(d, g[1]);
			}
			if (expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == typeof(Nullable<>))
				return rt == RoughType.Nullable && ReadCompatibleType(d, expectedType.GetGenericArguments()[0]);
			var icoll = Utils.GetICollection(expectedType);
			if (icoll != null)
				return rt == RoughType.Sequence && ReadCompatibleType(d, icoll.GetGenericArguments()[0]);
			if (rt == RoughType.Record)
				return expectedType.IsRecord();
			throw d.Error("Unknown rough type {0}", rt);
		}

		protected static object ReadAny(BinaryDeserializer d)
		{
			var t = ReadType(d);
			return t == typeof(object) ? null : d.ReadValueFunc(t)(d);
		}

		private void InitReaders()
		{
			if (readerCache != null) return;
			readerCache = new Dictionary<Type, Func<BinaryDeserializer, object>>() {
				{ typeof(sbyte), ReadSByte },
				{ typeof(byte), ReadByte },
				{ typeof(short), ReadShort },
				{ typeof(ushort), ReadUShort },
				{ typeof(int), ReadInt },
				{ typeof(uint), ReadUInt },
				{ typeof(long), ReadLong },
				{ typeof(ulong), ReadULong },
				{ typeof(bool), ReadBool },
				{ typeof(char), ReadChar },
				{ typeof(float), ReadFloat },
				{ typeof(double), ReadDouble },
				{ typeof(decimal), ReadDecimal },
				{ typeof(DateTime), ReadDateTimeObj },
				{ typeof(DateTimeOffset), ReadDateTimeOffsetObj },
				{ typeof(TimeSpan), ReadTimeSpanObj },
				{ typeof(Guid), ReadGuidObj },
				{ typeof(string), ReadString },
				{ typeof(object), ReadAny },
				{ typeof(Record), ReadObject<object> },
			};
		}

		private static object ReadDateTimeObj(BinaryDeserializer d) => ReadDateTime(d);
		private static object ReadDateTimeOffsetObj(BinaryDeserializer d) => ReadDateTimeOffset(d);
		private static object ReadTimeSpanObj(BinaryDeserializer d) => ReadTimeSpan(d);
		private static object ReadGuidObj(BinaryDeserializer d) => ReadGuid(d);

		protected static void ReadIntoCollection<T>(BinaryDeserializer d, ICollection<T> list)
		{
			var rf = d.ReadValueFunc(typeof(T));
			var count = d.Reader.ReadInt32();
			for (int i = 0; i < count; ++i)
				list.Add((T)rf(d));
		}

		protected static void ReadIntoCollectionNG<T>(BinaryDeserializer d, object list)
		{
			ReadIntoCollection(d, (ICollection<T>)list);
		}

		protected static I ReadCollection<I, E>(BinaryDeserializer d) where I : class, ICollection<E>, new()
		{
			var count = d.Reader.ReadInt32();
			if (count == -1)
				return null;
			var list = new I();
			var rf = d.ReadValueFunc(typeof(E));
			for (int i = 0; i < count; ++i)
				list.Add((E)rf(d));
			return list;
		}

		protected static List<T> ReadList<T>(BinaryDeserializer d)
		{
			var count = d.Reader.ReadInt32();
			if (count == -1) return null;
			var list = new List<T>();
			var rf = d.ReadValueFunc(typeof(T));
			for (int i = 0; i < count; ++i)
				list.Add((T)rf(d));
			return list;
		}

		protected static List<object> ReadListRecord(BinaryDeserializer d, Func<BinaryDeserializer, object> readValue)
		{
			var count = d.Reader.ReadInt32();
			if (count == -1)
				return null;
			var list = new List<object>();
			for (int i = 0; i < count; ++i)
				list.Add(readValue(d));
			return list;
		}

		protected static void ReadIntoDictionary<K, V>(BinaryDeserializer d, IDictionary<K, V> dict)
		{
			var rk = d.ReadValueFunc(typeof(K));
			var rv = d.ReadValueFunc(typeof(V));
			var count = d.Reader.ReadInt32();
			for (int i = 0; i < count; ++i)
				dict.Add((K)rk(d), (V)rv(d));
		}

		protected static void ReadIntoDictionaryNG<K, V>(BinaryDeserializer d, object dict)
		{
			ReadIntoDictionary(d, (IDictionary<K, V>)dict);
		}

		protected static Dictionary<K, V> ReadDictionary<K, V>(BinaryDeserializer d)
		{
			var count = d.Reader.ReadInt32();
			if (count == -1)
				return null;
			var dict = new Dictionary<K, V>();
			var rk = d.ReadValueFunc(typeof(K));
			var rv = d.ReadValueFunc(typeof(V));
			for (int i = 0; i < count; ++i) {
				var key = (K)rk(d);
				var value = rv(d);
				if (value != null && !(value is V))
					throw d.Error("Incompatible type for key {0}, expected: {1} but got {2}",
						key.ToString(), typeof(V), value.GetType());
				dict.Add(key, (V)value);
			}
			return dict;
		}

		protected static I ReadIDictionary<I, K, V>(BinaryDeserializer d) where I : class, IDictionary<K, V>, new()
		{
			var count = d.Reader.ReadInt32();
			if (count == -1)
				return null;
			var dict = new I();
			var rk = d.ReadValueFunc(typeof(K));
			var rv = d.ReadValueFunc(typeof(V));
			for (int i = 0; i < count; ++i) {
				var key = (K)rk(d);
				var value = rv(d);
				if (!(value is V))
					throw d.Error("Incompatible type for key {0}, expected: {1} but got {2}",
						key.ToString(), typeof(V), value.GetType());
				dict.Add(key, (V)value);
			}
			return dict;
		}

		protected static Dictionary<K, object> ReadDictionaryRecord<K>(BinaryDeserializer d, Func<BinaryDeserializer, object> readValue)
		{
			var count = d.Reader.ReadInt32();
			if (count == -1)
				return null;
			var dict = new Dictionary<K, object>();
			var rk = d.ReadValueFunc(typeof(K));
			for (int i = 0; i < count; ++i)
				dict.Add((K)rk(d), readValue(d));
			return dict;
		}

		protected static T[] ReadArray<T>(BinaryDeserializer d)
		{
			var count = d.Reader.ReadInt32();
			if (count == -1)
				return null;
			var rf = d.ReadValueFunc(typeof(T));
			var array = new T[count];
			for (int i = 0; i < count; ++i)
				array[i] = (T)rf(d);
			return array;
		}

		protected static Array ReadArrayNDim(BinaryDeserializer d, Type elementType, int rank)
		{
			return ReadArrayNDim(d, elementType, rank, d.ReadValueFunc(elementType));
		}

		protected static Array ReadArrayNDim(BinaryDeserializer d, Type elementType, int rank, Func<BinaryDeserializer, object> readElemFunc)
		{
			int lengthOrNull = d.Reader.ReadInt32();
			if (lengthOrNull == -1)
				return null;
			var lbs = new int[rank];
			var ubs = new int[rank];
			var lengths = new int[rank];
			lengths[0] = lengthOrNull;
			for (int dim = 1; dim < rank; ++dim)
				lengths[dim] = d.Reader.ReadInt32();
			if (d.Reader.ReadBoolean()) {
				for (int dim = 0; dim < rank; ++dim)
					lbs[dim] = d.Reader.ReadInt32();
			}
			for (int dim = 0; dim < rank; ++dim)
				ubs[dim] = lengths[dim] + lbs[dim] - 1;
			var array = Array.CreateInstance(elementType, lengths, lbs);
			if (array.Length == 0)
				return array;

			var indices = (int[])lbs.Clone();
			for (int dim = rank - 1; ;) {
				array.SetValue(readElemFunc(d), indices);
				if (indices[dim] == ubs[dim]) {
					for (; dim >= 0 && indices[dim] == ubs[dim]; --dim)
						indices[dim] = lbs[dim];
					if (dim < 0)
						break;
					++indices[dim];
					dim = rank - 1;
				}
				else
					++indices[dim];
			}
			return array;
		}

		protected static Action<T> ReadAction<T>(BinaryDeserializer d)
		{
			return GetActionStatic<T>(d, d.Reader.ReadString());
		}

		// Zeroth element corresponds to 'null'.
		private List<ReaderClassDef> classDefs = new List<ReaderClassDef> { new ReaderClassDef() };
		private List<ReaderClassDef> internalClassDefs = new List<ReaderClassDef>();

		protected virtual void PrepareReaders(ReaderClassDef def) => def.ReadFields = ReadFields;

		public void ClearClassIds()
		{
			classDefs = new List<ReaderClassDef> { new ReaderClassDef() };
			internalClassDefs = new List<ReaderClassDef>();
		}

		private static ReaderClassDef GetClassDefUnknown(BinaryDeserializer d, string typeName)
		{
			var result = new ReaderClassDef {
				Meta = Meta.Unknown,
				Make = (bd, def) => {
					var obj = new YuzuUnknownBinary { ClassTag = typeName, Def = def };
					ReadFields(bd, def, obj);
					return obj;
				},
			};
			var theirCount = d.Reader.ReadInt16();
			for (int theirIndex = 0; theirIndex < theirCount; ++theirIndex) {
				var theirName = d.Reader.ReadString();
				var t = ReadType(d);
				var rf = d.ReadValueFunc(t);
				result.Fields.Add(new ReaderClassDef.FieldDef {
					Name = theirName,
					Type = t,
					OurIndex = -1,
					ReadFunc = (d, obj) => ((YuzuUnknown)obj).Fields[theirName] = rf(d)
				});
			}
			d.internalClassDefs.Add(result);
			return result;
		}

		private static void AddUnknownFieldDef(BinaryDeserializer d, ReaderClassDef def, string fieldName, string typeName)
		{
			if (!d.Options.AllowUnknownFields)
				throw d.Error("New field {0} for class {1}", fieldName, typeName);
			var fd = new ReaderClassDef.FieldDef {
				Name = fieldName,
				OurIndex = -1,
				Type = ReadType(d)
			};
			var rf = d.ReadValueFunc(fd.Type);
			if (def.Meta.GetUnknownStorage == null)
				fd.ReadFunc = (d, obj) => rf(d);
			else
				fd.ReadFunc = (d, obj) => def.Meta.GetUnknownStorage(obj).Add(fieldName, rf(d));
			def.Fields.Add(fd);
		}

		private Action<BinaryDeserializer, object> MakeReadOrMergeFunc(Meta.Item yi)
		{
			if (yi.SetValue != null) {
				var rf = ReadValueFunc(yi.Type);
				return (d, obj) => yi.SetValue(obj, rf(d));
			}
			else {
				var mf = MergeValueFunc(yi.Type);
				return (d, obj) => mf(d, yi.GetValue(obj));
			}
		}

		private static void InitClassDef(BinaryDeserializer d, ReaderClassDef def, string typeName)
		{
			var ourCount = def.Meta.Items.Count;
			var theirCount = d.Reader.ReadInt16();
			int ourIndex = 0, theirIndex = 0;
			var theirName = "";
			while (ourIndex < ourCount && theirIndex < theirCount) {
				var yi = def.Meta.Items[ourIndex];
				var ourName = yi.Tag(d.Options);
				if (theirName == "")
					theirName = d.Reader.ReadString();
				var cmp = String.CompareOrdinal(ourName, theirName);
				if (cmp < 0) {
					if (!yi.IsOptional)
						throw d.Error("Missing required field {0} for class {1}", ourName, typeName);
					ourIndex += 1;
				}
				else if (cmp > 0) {
					AddUnknownFieldDef(d, def, theirName, typeName);
					theirIndex += 1;
					theirName = "";
				}
				else {
					if (!ReadCompatibleType(d, yi.Type))
						throw d.Error(
							"Incompatible type for field {0}, expected {1}", ourName, yi.Type);
					def.Fields.Add(new ReaderClassDef.FieldDef {
						Name = theirName, OurIndex = ourIndex + 1, Type = yi.Type,
						ReadFunc = d.MakeReadOrMergeFunc(yi),
					});
					ourIndex += 1;
					theirIndex += 1;
					theirName = "";
				}
			}
			for (; ourIndex < ourCount; ++ourIndex) {
				var yi = def.Meta.Items[ourIndex];
				var ourName = yi.Tag(d.Options);
				if (!yi.IsOptional)
					throw d.Error("Missing required field {0} for class {1}", ourName, typeName);
			}
			for (; theirIndex < theirCount; ++theirIndex) {
				if (theirName == "")
					theirName = d.Reader.ReadString();
				AddUnknownFieldDef(d, def, theirName, typeName);
				theirName = "";
			}
		}

		private static void InitClassDefUnordered(BinaryDeserializer d, ReaderClassDef def, string typeName)
		{
			var theirCount = d.Reader.ReadInt16();
			int ourIndex = 0, requiredCountActiual = 0;
			for (int theirIndex = 0; theirIndex < theirCount; ++theirIndex) {
				var theirName = d.Reader.ReadString();
				Meta.Item yi = null;
				if (def.Meta.TagToItem.TryGetValue(theirName, out yi)) {
					if (!ReadCompatibleType(d, yi.Type))
						throw d.Error(
							"Incompatible type for field {0}, expected {1}", theirName, yi.Type);
					def.Fields.Add(new ReaderClassDef.FieldDef {
						Name = theirName,
						OurIndex = def.Meta.Items.IndexOf(yi) + 1,
						Type = yi.Type,
						ReadFunc = d.MakeReadOrMergeFunc(yi),
					});
					ourIndex += 1;
					if (!yi.IsOptional)
						requiredCountActiual += 1;
				}
				else
					AddUnknownFieldDef(d, def, theirName, typeName);
			}
			if (requiredCountActiual != def.Meta.RequiredCount)
				throw d.Error(
					"Expected {0} required field(s), but found {1} for class {2}",
					def.Meta.RequiredCount, requiredCountActiual, typeName);
		}

		private static ReaderClassDef GetClassDef(BinaryDeserializer d, short classId)
		{
			if (classId < d.classDefs.Count) {
				var r = d.classDefs[classId];
				if (r.Incomplete) {
					CompleteClassDef(d, r);
				}
				return r;
			}
			if (classId - short.MaxValue / 2 >= 0 && classId - short.MaxValue/2 < d.internalClassDefs.Count) {
				return d.internalClassDefs[classId - short.MaxValue/2];
			}
			if (classId - short.MaxValue/2 > d.internalClassDefs.Count)
				throw d.Error("Bad classId: {0}", classId);
			var typeName = d.Reader.ReadString();
			var classType = Meta.GetTypeByReadAlias(typeName, d.Options) ?? TypeSerializer.Deserialize(typeName);
			if (classType == null)
				return GetClassDefUnknown(d, typeName);
			var result = new ReaderClassDef { Meta = Meta.Get(classType, d.Options) };
			d.PrepareReaders(result);
			if (d.BinaryOptions.Unordered)
				InitClassDefUnordered(d, result, typeName);
			else
				InitClassDef(d, result, typeName);
			d.internalClassDefs.Add(result);
			return result;
		}

		private static void CompleteClassDef(BinaryDeserializer d, ReaderClassDef classDef)
		{
			var previousReader = d.Reader;
			((System.IO.MemoryStream)classDef.BufferStream).Position = classDef.StreamPosition;
			/*using */var reader = new System.IO.BinaryReader(classDef.BufferStream);
			d.Reader = reader;
			try {
				var typeName = d.Reader.ReadString();
				var classType = Meta.GetTypeByReadAlias(typeName, d.Options) ?? TypeSerializer.Deserialize(typeName);
				if (classType == null)
					// return GetClassDefUnknown(d, typeName);
					throw d.Error("Zalupa");
				classDef.Meta = Meta.Get(classType, d.Options);
				d.PrepareReaders(classDef);
				if (d.BinaryOptions.Unordered)
					InitClassDefUnordered(d, classDef, typeName);
				else
					InitClassDef(d, classDef, typeName);
			} finally {
				d.Reader = previousReader;
			}
			classDef.Incomplete = false;
			classDef.BufferStream = null;
			classDef.StreamPosition = -1;
		}

		private static ReaderClassDef GetClassDefIncomplete(BinaryDeserializer d, short classId)
		{
			if (classId < d.classDefs.Count)
				throw d.Error("Unexpected class def.");
			if (classId - short.MaxValue / 2 >= 0 && classId - short.MaxValue / 2 < d.internalClassDefs.Count) {
				// return d.internalClassDefs[classId - short.MaxValue / 2];
				throw d.Error("Unexpected class def.");
			}
			if (classId - short.MaxValue / 2 > d.internalClassDefs.Count)
				throw d.Error("Bad classId: {0}", classId);
			var result = new ReaderClassDef {
				Incomplete = true,
				BufferStream = d.Reader.BaseStream,
				StreamPosition = ((System.IO.MemoryStream)d.Reader.BaseStream).Position,
			};
			////var typeName = d.Reader.ReadString();
			////var classType = Meta.GetTypeByReadAlias(typeName, d.Options) ?? TypeSerializer.Deserialize(typeName);
			////if (classType == null)
			////	return GetClassDefUnknown(d, typeName);
			////var result = new ReaderClassDef { Meta = Meta.Get(classType, d.Options) };
			////d.PrepareReaders(result);
			////if (d.BinaryOptions.Unordered)
			////	InitClassDefUnordered(d, result, typeName);
			////else
			////	InitClassDef(d, result, typeName);
			////d.internalClassDefs.Add(result);
			return result;
		}

		private static void ReadFields(BinaryDeserializer d, ReaderClassDef def, object obj)
		{
			def.Meta.BeforeDeserialization.Run(obj);
			d.objStack.Push(obj);
			try {
				if (def.Meta.IsCompact) {
					for (int i = 1; i < def.Fields.Count; ++i)
						def.Fields[i].ReadFunc(d, obj);
				}
				else {
					if (def.Meta.GetUnknownStorage != null) {
						var storage = def.Meta.GetUnknownStorage(obj);
						storage.Clear();
						storage.Internal = def;
					}
					var actualIndex = d.Reader.ReadInt16();
					for (int i = 1; i < def.Fields.Count; ++i) {
						var fd = def.Fields[i];
						if (i < actualIndex || actualIndex == 0) {
							if (fd.OurIndex < 0 || def.Meta.Items[fd.OurIndex - 1].IsOptional)
								continue;
							throw d.Error("Expected field '{0}' ({1}), but found '{2}'",
								i, fd.Name, actualIndex);
						}
						fd.ReadFunc(d, obj);
						actualIndex = d.Reader.ReadInt16();
					}
					if (actualIndex != 0)
						throw d.Error("Unfinished object, expected zero, but got {0}", actualIndex);
				}
			}
			finally {
				d.objStack.Pop();
			}
			def.Meta.AfterDeserialization.Run(obj);
		}

		protected static void ReadIntoObject<T>(BinaryDeserializer d, object obj)
		{
			var classId = d.Reader.ReadInt16();
			if (classId == 0)
				throw d.Error("Unable to read null into object");
			var def = GetClassDef(d, classId);
			var expectedType = obj.GetType();
			if (
				expectedType != def.Meta.Type &&
				(!Meta.Get(expectedType, d.Options).AllowReadingFromAncestor || expectedType.BaseType != def.Meta.Type)
			)
				throw d.Error("Unable to read type {0} into {1}", def.Meta.Type, expectedType);
			def.ReadFields(d, def, obj);
		}

		protected static void ReadIntoObjectUnchecked<T>(BinaryDeserializer d, object obj)
		{
			var classId = d.Reader.ReadInt16();
			var def = GetClassDef(d, classId);
			def.ReadFields(d, def, obj);
		}

		private static object MakeAndCheckAssignable<T>(BinaryDeserializer d, ReaderClassDef def)
		{
			var srcType = def.Meta.Type;
			var dstType = typeof(T);
			if (srcType != typeof(YuzuUnknown) && !dstType.IsAssignableFrom(srcType))
				throw d.Error("Unable to assign type \"{0}\" to \"{1}\"", srcType.ToString(), dstType);
			var result = def.Make?.Invoke(d, def);
			if (srcType == typeof(YuzuUnknown) && !dstType.IsInstanceOfType(result))
				throw d.Error("Unable to assign type \"{0}\" to \"{1}\"", ((YuzuUnknownBinary)result).ClassTag, dstType);
			return result;
		}

		protected static object ReadObject<T>(BinaryDeserializer d) where T : class
		{
			var classId = d.Reader.ReadInt16();
			if (classId == 0)
				return null;
			var def = GetClassDef(d, classId);
			var result = MakeAndCheckAssignable<T>(d, def);
			if (result == null) {
				result = def.Meta.Factory();
				def.ReadFields(d, def, result);
			}
			return result;
		}

		protected static object ReadObjectUnchecked<T>(BinaryDeserializer d) where T : class
		{
			var classId = d.Reader.ReadInt16();
			if (classId == 0)
				return null;
			var def = GetClassDef(d, classId);
			if (def.Make != null)
				return def.Make(d, def);
			var result = def.Meta.Factory();
			def.ReadFields(d, def, result);
			return result;
		}

		protected static void EnsureClassDef(BinaryDeserializer d, Type t)
		{
			var def = GetClassDef(d, d.Reader.ReadInt16());
			if (def.Meta.Type != t)
				throw d.Error("Expected type {0}, but found {1}", def.Meta.Type, t);
		}

		protected static object ReadStruct<T>(BinaryDeserializer d) where T : struct
		{
			var classId = d.Reader.ReadInt16();
			if (classId == 0)
				return null;
			var def = GetClassDef(d, classId);
			var result = MakeAndCheckAssignable<T>(d, def);
			if (result == null) {
				result = def.Meta.Factory();
				def.ReadFields(d, def, result);
			}
			return result;
		}

		protected static void ReadIntoStruct<T>(BinaryDeserializer d, ref T s) where T : struct
		{
			var classId = d.Reader.ReadInt16();
			if (classId == 0)
				return;
			var def = GetClassDef(d, classId);
			var result = MakeAndCheckAssignable<T>(d, def);
			if (result == null) {
				result = def.Meta.Factory();
				def.ReadFields(d, def, result);
			}
			s = (T)result;
		}

		protected static object ReadStructUnchecked<T>(BinaryDeserializer d) where T : struct
		{
			var classId = d.Reader.ReadInt16();
			if (classId == 0)
				return null;
			var def = GetClassDef(d, classId);
			if (def.Make != null)
				return def.Make(d, def);
			var result = def.Meta.Factory();
			def.ReadFields(d, def, result);
			return result;
		}

		private static Dictionary<Type, Func<BinaryDeserializer, object>> readerCache;
		private static Dictionary<Type, Action<BinaryDeserializer, object>> mergerCache = new Dictionary<Type, Action<BinaryDeserializer, object>>();

		private Func<BinaryDeserializer, object> ReadValueFunc(Type t)
		{
			lock (readerCache) {
				if (readerCache.TryGetValue(t, out Func<BinaryDeserializer, object> f))
					return f;
				System.Diagnostics.Trace.WriteLine($"NICEDOG reader cache: new count = {readerCache.Count}; added: '{t.FullName}'");
				return readerCache[t] = MakeReaderFunc(this, t);
			}
		}

		private Action<BinaryDeserializer, object> MergeValueFunc(Type t)
		{
			lock (mergerCache) {
				if (mergerCache.TryGetValue(t, out Action<BinaryDeserializer, object> f))
					return f;
				System.Diagnostics.Trace.WriteLine($"NICEDOG merger cache: new count = {mergerCache.Count}; added: '{t.FullName}'");
				return mergerCache[t] = MakeMergerFunc(this, t);
			}
		}

		private static Func<BinaryDeserializer, object> MakeEnumReaderFunc(Type t)
		{
			var ut = Enum.GetUnderlyingType(t);
			if (ut == typeof(sbyte))
				return d => Enum.ToObject(t, d.Reader.ReadSByte());
			if (ut == typeof(byte))
				return d => Enum.ToObject(t, d.Reader.ReadByte());
			if (ut == typeof(short))
				return d => Enum.ToObject(t, d.Reader.ReadInt16());
			if (ut == typeof(ushort))
				return d => Enum.ToObject(t, d.Reader.ReadUInt16());
			if (ut == typeof(int))
				return d => Enum.ToObject(t, d.Reader.ReadInt32());
			if (ut == typeof(uint))
				return d => Enum.ToObject(t, d.Reader.ReadUInt32());
			if (ut == typeof(long))
				return d => Enum.ToObject(t, d.Reader.ReadInt64());
			if (ut == typeof(ulong))
				return d => Enum.ToObject(t, d.Reader.ReadUInt64());
			throw new YuzuException();
		}

		private static Func<BinaryDeserializer, object> ReadDataStructureOfRecord(BinaryDeserializer d, Type t)
		{
			if (t == typeof(Record))
				return ReadObject<object>;
			if (!t.IsGenericType)
				return null;
			var g = t.GetGenericTypeDefinition();
			if (g == typeof(List<>)) {
				var readValue = ReadDataStructureOfRecord(d, t.GetGenericArguments()[0]);
				if (readValue == null) return null;
				return d => ReadListRecord(d, readValue);
			}
			if (g == typeof(Dictionary<,>)) {
				var readValue = ReadDataStructureOfRecord(d, t.GetGenericArguments()[1]);
				if (readValue == null) return null;
				var rdrd = (Func<BinaryDeserializer, Func<BinaryDeserializer, object>, object>)Delegate.CreateDelegate(
					typeof(Func<BinaryDeserializer, Func<BinaryDeserializer, object>, object>),
					Utils.GetPrivateCovariantGenericStatic(d.GetType(), nameof(ReadDictionaryRecord), t)
				);
				return d => rdrd(d, readValue);
			}
			return null;
		}

		private static Func<BinaryDeserializer, object> MakeReaderFunc(BinaryDeserializer d, Type t)
		{
			if (t.IsEnum)
				return MakeEnumReaderFunc(t);
			if (t.IsGenericType) {
				var readRecord = ReadDataStructureOfRecord(d, t);
				if (readRecord != null)
					return readRecord;
				var g = t.GetGenericTypeDefinition();
				if (g == typeof(List<>))
					return MakeDelegateStatic(
						Utils.GetPrivateCovariantGenericStatic(d.GetType(), nameof(ReadList), t));
				if (g == typeof(Dictionary<,>))
					return MakeDelegateStatic(
						Utils.GetPrivateCovariantGenericAllStatic(d.GetType(), nameof(ReadDictionary), t));
				if (g == typeof(Action<>))
					return MakeDelegateStatic(Utils.GetPrivateCovariantGenericStatic(d.GetType(), nameof(ReadAction), t));
				if (g == typeof(Nullable<>)) {
					var r = d.ReadValueFunc(t.GetGenericArguments()[0]);
					return d => d.Reader.ReadBoolean() ? null : r(d);
				}
			}
			if (t.IsArray) {
				if (t.GetArrayRank() > 1) {
					var et = t.GetElementType();
					var rf = d.ReadValueFunc(et);
					var rank = t.GetArrayRank();
					return d => ReadArrayNDim(d, et, rank, rf);
				}
				return MakeDelegateStatic(Utils.GetPrivateCovariantGenericStatic(d.GetType(), nameof(ReadArray), t));
			}

			var meta = Meta.Get(t, d.Options);
			var sg = meta.Surrogate;
			if (sg.SurrogateType != null && sg.FuncFrom != null) {
				var rf = d.ReadValueFunc(sg.SurrogateType);
				return d => sg.FuncFrom(rf(d));
			}

			var idict = Utils.GetIDictionary(t);
			if (idict != null) {
				var kv = idict.GetGenericArguments();
				return MakeDelegateStatic(Utils.GetPrivateGenericStatic(d.GetType(), nameof(ReadIDictionary), t, kv[0], kv[1]));
			}

			var icoll = Utils.GetICollection(t);
			if (icoll != null) {
				var elemType = icoll.GetGenericArguments()[0];
				return MakeDelegateStatic(Utils.GetPrivateGenericStatic(d.GetType(), nameof(ReadCollection), t, elemType));
			}

			if (t.IsClass || t.IsInterface) {
				return MakeDelegateStatic(Utils.GetPrivateGenericStatic(d.GetType(), nameof(ReadObject), t));
			}
			if (Utils.IsStruct(t))
				return MakeDelegateStatic(Utils.GetPrivateGenericStatic(d.GetType(), nameof(ReadStruct), t));
			throw new NotImplementedException(t.Name);
		}

		private static Action<BinaryDeserializer, object> MakeMergerFunc(BinaryDeserializer d, Type t)
		{
			var idict = Utils.GetIDictionary(t);
			if (idict != null)
				return MakeDelegateActionStatic(
					Utils.GetPrivateCovariantGenericAllStatic(d.GetType(), nameof(ReadIntoDictionaryNG), idict));
			var icoll = Utils.GetICollection(t);
			if (icoll != null)
				return MakeDelegateActionStatic(
					Utils.GetPrivateCovariantGenericStatic(d.GetType(), nameof(ReadIntoCollectionNG), icoll));
			if ((t.IsClass || t.IsInterface || Utils.IsStruct(t)) && t != typeof(object))
				return MakeDelegateActionStatic(Utils.GetPrivateGenericStatic(d.GetType(), nameof(ReadIntoObject), t));
			throw d.Error("Unable to merge field of type {0}", t);
		}

		public override object FromReaderInt()
		{
			if (BinaryOptions.AutoSignature)
				CheckSignature();
			return ReadAny(this);
		}

		public override object FromReaderInt(object obj)
		{
			var expectedType = obj.GetType();
			if (expectedType == typeof(object))
				throw Error("Unable to read into untyped object");
			if (BinaryOptions.AutoSignature)
				CheckSignature();
			if (!ReadCompatibleType(this, expectedType))
				throw Error("Incompatible type to read into {0}", expectedType.Name);
			MergeValueFunc(expectedType)(this, obj);
			return obj;
		}

		public override T FromReaderInt<T>()
		{
			if (BinaryOptions.AutoSignature)
				CheckSignature();
			if (typeof(T) == typeof(object))
				return (T)ReadAny(this);
			if (!ReadCompatibleType(this, typeof(T)))
				throw Error("Incompatible type to read into {0}", typeof(T));
			return (T)ReadValueFunc(typeof(T))(this);
		}

		// If possible, preserves stream position if signature is absent.
		public bool IsValidSignature()
		{
			var s = BinaryOptions.Signature;
			if (s.Length == 0)
				return true;
			if (!Reader.BaseStream.CanSeek)
				return s.Equals(Reader.ReadBytes(s.Length));
			var pos = Reader.BaseStream.Position;
			if (Reader.BaseStream.Length - pos < s.Length)
				return false;
			foreach (var b in s)
				if (b != Reader.ReadByte()) {
					Reader.BaseStream.Position = pos;
					return false;
				}
			return true;
		}

		public void CheckSignature()
		{
			if (!IsValidSignature())
				throw Error("Signature not found");
		}

		public ReaderClassDef ReadClassDef(System.IO.BinaryReader reader, int expectedIndex, System.IO.MemoryStream ms)
		{
			var previousReader = this.Reader;
			this.Reader = reader;
			try {
				var classId = Reader.ReadInt16();
				if (classId != expectedIndex + 1) {
					throw Error("Invalid class id.");
				}
				if (classId == 0)
					throw Error("Unable to read null into object");
				var def = GetClassDefIncomplete(this, classId);
				def.BufferStream = ms;
				// ==
				reader.ReadString();
				var theirCount = reader.ReadInt16();
				while (theirCount-- > 0) {
					var theirName = reader.ReadString();
					var rt = ReadType(this);
				}
				// ==

				return def;
			} finally {
				this.Reader = previousReader;
			}
		}

		public void UseClassDefCache(List<ReaderClassDef> classDefs)
		{
			this.classDefs = classDefs;
		}
	}
}
