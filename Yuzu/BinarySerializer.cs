using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yuzu.Metadata;
using Yuzu.Util;
using static Yuzu.Binary.BinarySerializer;

namespace Yuzu.Binary
{
	public class BinarySerializer : AbstractWriterSerializer
	{
		public BinarySerializeOptions BinaryOptions = new BinarySerializeOptions();

		protected static void WriteSByte(BinarySerializer s, object obj) => s.writer.Write((sbyte)obj);
		protected static void WriteByte(BinarySerializer s, object obj) => s.writer.Write((byte)obj);
		protected static void WriteShort(BinarySerializer s, object obj) => s.writer.Write((short)obj);
		protected static void WriteUShort(BinarySerializer s, object obj) => s.writer.Write((ushort)obj);
		protected static void WriteInt(BinarySerializer s, object obj) => s.writer.Write((int)obj);
		protected static void WriteUInt(BinarySerializer s, object obj) => s.writer.Write((uint)obj);
		protected static void WriteLong(BinarySerializer s, object obj) => s.writer.Write((long)obj);
		protected static void WriteULong(BinarySerializer s, object obj) => s.writer.Write((ulong)obj);
		protected static void WriteBool(BinarySerializer s, object obj) => s.writer.Write((bool)obj);
		protected static void WriteChar(BinarySerializer s, object obj) => s.writer.Write((char)obj);
		protected static void WriteFloat(BinarySerializer s, object obj) => s.writer.Write((float)obj);
		protected static void WriteDouble(BinarySerializer s, object obj) => s.writer.Write((double)obj);
		protected static void WriteDecimal(BinarySerializer s, object obj) => s.writer.Write((decimal)obj);
		protected static void WriteDateTime(BinarySerializer s, object obj)
		{
			s.writer.Write(((DateTime)obj).ToBinary());
		}

		protected static void WriteDateTimeOffset(BinarySerializer s, object obj)
		{
			var d = (DateTimeOffset)obj;
			s.writer.Write(d.DateTime.ToBinary());
			s.writer.Write(d.Offset.Ticks);
		}
		protected static void WriteTimeSpan(BinarySerializer s, object obj) => s.writer.Write(((TimeSpan)obj).Ticks);
		protected static void WriteGuid(BinarySerializer s, object obj) => s.writer.Write(((Guid)obj).ToByteArray());

		protected static void WriteString(BinarySerializer s, object obj)
		{
			if (obj == null) {
				s.writer.Write(string.Empty);
				s.writer.Write(true);
				return;
			}
			s.writer.Write((string)obj);
			if ((string)obj == string.Empty) {
				s.writer.Write(false);
			}
		}

		protected static void WriteAny(BinarySerializer s, object obj)
		{
			if (obj == null) {
				s.writer.Write((byte)RoughType.Any);
				return;
			}
			var t = obj.GetType();
			if (t == typeof(object)) {
				throw new YuzuException("WriteAny of unknown type");
			}
			WriteRoughType(s, t);
			s.GetWriteFunc(t)(s, obj);
		}

		protected static void WriteRecord(BinarySerializer s, object obj) => s.GetWriteFunc(obj.GetType())(s, obj);

		private Dictionary<Type, Action<BinarySerializer, object>> writerCache;

		private Action<BinarySerializer, object> GetWriteFunc(Type t)
		{
			if (writerCache.TryGetValue(t, out Action<BinarySerializer, object> result)) {
				return result;
			}
			result = MakeWriteFunc(this, t);
			writerCache[t] = result;
			return result;
		}

		public BinarySerializer()
		{
			InitWriters();
		}

		private void InitWriters()
		{
			writerCache = new Dictionary<Type, Action<BinarySerializer, object>>() {
				{ typeof(sbyte), WriteSByte },
				{ typeof(byte), WriteByte },
				{ typeof(short), WriteShort },
				{ typeof(ushort), WriteUShort },
				{ typeof(int), WriteInt },
				{ typeof(uint), WriteUInt },
				{ typeof(long), WriteLong },
				{ typeof(ulong), WriteULong },
				{ typeof(bool), WriteBool },
				{ typeof(char), WriteChar },
				{ typeof(float), WriteFloat },
				{ typeof(double), WriteDouble },
				{ typeof(decimal), WriteDecimal },
				{ typeof(DateTime), WriteDateTime },
				{ typeof(DateTimeOffset), WriteDateTimeOffset },
				{ typeof(TimeSpan), WriteTimeSpan },
				{ typeof(Guid), WriteGuid },
				{ typeof(string), WriteString },
				{ typeof(object), WriteAny },

				{ typeof(Record), WriteRecord },
				{ typeof(YuzuUnknown), WriteUnknown },
				{ typeof(YuzuUnknownBinary), WriteUnknownBinary },
			};
		}

		private static void WriteRoughType(BinarySerializer s, Type t)
		{
			for (var result = RoughType.FirstAtom; result <= RoughType.LastAtom; ++result) {
				if (t == RT.RoughTypeToType[(int)result]) {
					s.writer.Write((byte)result);
					return;
				}
			}
			if (t.IsEnum) {
				WriteRoughType(s, Enum.GetUnderlyingType(t));
				return;
			}
			if (t.IsGenericType) {
				var g = t.GetGenericTypeDefinition();
				if (g == typeof(Nullable<>)) {
					s.writer.Write((byte)RoughType.Nullable);
					WriteRoughType(s, t.GetGenericArguments()[0]);
					return;
				}
				if (g == typeof(Action<>)) {
					s.writer.Write((byte)RoughType.Record);
					return;
				}
			}
			if (t.IsArray) {
				if (t.GetArrayRank() > 1) {
					s.writer.Write((byte)RoughType.NDimArray);
					s.writer.Write((byte)t.GetArrayRank());
				} else {
					s.writer.Write((byte)RoughType.Sequence);
				}
				WriteRoughType(s, t.GetElementType());
				return;
			}

			if (t != typeof(YuzuUnknown) && !t.IsSubclassOf(typeof(YuzuUnknown)) && t != typeof(Record)) {
				var sg = Meta.Get(t, s.Options).Surrogate;
				if (sg.SurrogateType != null && sg.FuncTo != null) {
					WriteRoughType(s, sg.SurrogateType);
					return;
				}
			}

			var idict = Utils.GetIDictionary(t);
			if (idict != null) {
				s.writer.Write((byte)RoughType.Mapping);
				var a = idict.GetGenericArguments();
				WriteRoughType(s, a[0]);
				WriteRoughType(s, a[1]);
				return;
			}

			var ienum = Utils.GetIEnumerable(t);
			if (ienum != null) {
				s.writer.Write((byte)RoughType.Sequence);
				WriteRoughType(s, ienum.GetGenericArguments()[0]);
				return;
			}
			if (t.IsRecord()) {
				s.writer.Write((byte)RoughType.Record);
				return;
			}
			throw new NotImplementedException();
		}

		private static void WriteIDictionary<K, V>(
			BinarySerializer s,
			object obj,
			Action<BinarySerializer, object> writeKey,
			Action<BinarySerializer, object> writeValue
		) {
			if (obj == null) {
				s.writer.Write(-1);
				return;
			}
			var dict = (IDictionary<K, V>)obj;
			s.writer.Write(dict.Count);
			foreach (var elem in dict) {
				writeKey(s, elem.Key);
				writeValue(s, elem.Value);
			}
		}

		private readonly Stack<object> objStack = new Stack<object>();

		private static void WriteAction(BinarySerializer s, object obj)
		{
			if (obj == null) {
				s.writer.Write(string.Empty);
				return;
			}
			var a = obj as MulticastDelegate;
			if (a.Target != s.objStack.Peek()) {
				throw new NotImplementedException();
			}

			s.writer.Write(a.Method.Name);
		}

		private static void WriteArray<T>(BinarySerializer s, object obj, Action<BinarySerializer, object> wf)
		{
			if (obj == null) {
				s.writer.Write(-1);
				return;
			}
			var arr = (T[])obj;
			s.writer.Write(arr.Length);
			foreach (var a in arr) {
				wf(s, a);
			}
		}

		private static void WriteArrayNDim(
			BinarySerializer s,
			object obj,
			Action<BinarySerializer, object> writeElemFunc
		) {
			if (obj == null) {
				s.writer.Write(-1);
				return;
			}
			var arr = (Array)obj;
			var lbs = new int[arr.Rank];
			var ubs = new int[arr.Rank];
			var hasNonZeroLB = false;
			for (int dim = 0; dim < arr.Rank; ++dim) {
				lbs[dim] = arr.GetLowerBound(dim);
				ubs[dim] = arr.GetUpperBound(dim);
				s.writer.Write(ubs[dim] - lbs[dim] + 1);
				if (lbs[dim] != 0) {
					hasNonZeroLB = true;
				}
			}

			s.writer.Write(hasNonZeroLB);
			if (hasNonZeroLB) {
				for (int dim = 0; dim < arr.Rank; ++dim) {
					s.writer.Write(lbs[dim]);
				}
			}

			if (arr.Length == 0) {
				return;
			}

			var indices = (int[])lbs.Clone();
			for (int dim = arr.Rank - 1; ;) {
				writeElemFunc(s, arr.GetValue(indices));
				if (indices[dim] == ubs[dim]) {
					for (; dim >= 0 && indices[dim] == ubs[dim]; --dim) {
						indices[dim] = lbs[dim];
					}
					if (dim < 0) {
						break;
					}
					++indices[dim];
					dim = arr.Rank - 1;
				} else {
					++indices[dim];
				}
			}
		}

		private static void WriteIEnumerable<T>(BinarySerializer s, object list, Action<BinarySerializer, object> wf)
		{
			if (list == null) {
				s.writer.Write(-1);
				return;
			}
			var ienum = (IEnumerable<T>)list;
			s.writer.Write(ienum.Count());
			foreach (var a in ienum) {
				wf(s, a);
			}
		}

		private static void WriteIEnumerableIf<T>(
			BinarySerializer s,
			object list,
			Action<BinarySerializer, object> wf,
			Func<object, int, object, bool> cond
		) {
			if (list == null) {
				s.writer.Write(-1);
				return;
			}
			var ienum = (IEnumerable<T>)list;
			int index = 0;
			s.writer.Write(ienum.Count(a => cond(list, index++, a)));
			index = 0;
			foreach (var a in ienum) {
				if (cond(list, index++, a)) {
					wf(s, a);
				}
			}
		}

		// Duplicate WriteIEnumerable to optimize Count.
		private static void WriteCollection<T>(BinarySerializer s, object list, Action<BinarySerializer, object> wf)
		{
			if (list == null) {
				s.writer.Write(-1);
				return;
			}
			var icoll = (ICollection<T>)list;
			s.writer.Write(icoll.Count);
			foreach (var a in icoll) {
				wf(s, a);
			}
		}

		private static void WriteCollectionNG(BinarySerializer s, object obj, Action<BinarySerializer, object> wf)
		{
			if (obj == null) {
				s.writer.Write(-1);
				return;
			}
			var list = (ICollection)obj;
			s.writer.Write(list.Count);
			foreach (var a in list) {
				wf(s, a);
			}
		}

		public class ClassDef
		{
			public struct FieldDef
			{
				public string Name;
				public Type Type;
				public Action<BinarySerializer, object> WriteFunc;
				public Action<BinarySerializer, object> WriteFuncCompact;
				internal Action<BinarySerializer, object, YuzuUnknownStorage, BoxedInt> WriteFuncUnknown;
			}
			public short Id;
			public Meta Meta;
			internal ReaderClassDef ReaderDef;
			public List<FieldDef> Fields = new List<FieldDef>();
		}
		private Dictionary<Type, ClassDef> externalClassIdCache = null;
		private Dictionary<Type, ClassDef> internalClassIdCache = new Dictionary<Type, ClassDef>();
		private Dictionary<string, ClassDef> unknownClassIdCache = new Dictionary<string, ClassDef>();

		public void ClearClassIds() {
			externalClassIdCache = null; // ???
			internalClassIdCache.Clear();
		}

		private static void PrepareClassDefFields(BinarySerializer s, ClassDef result)
		{
			for (short i = 0; i < result.Meta.Items.Count; ++i) {
				var yi = result.Meta.Items[i];
				// Capture.
				short j = (short)(i + 1);
				var wf = s.GetWriteFunc(yi.Type);
				var fd = new ClassDef.FieldDef {
					Name = yi.Tag(s.Options),
					Type = yi.Type,
				};
				if (yi.SerializeCond != null) {
					fd.WriteFunc = (s, obj) => {
						var value = yi.GetValue(obj);
						if (!yi.SerializeCond(obj, value)) {
							return;
						}

						s.writer.Write(j);
						wf(s, value);
					};
				} else {
					fd.WriteFunc = (s, obj) => {
						s.writer.Write(j);
						wf(s, yi.GetValue(obj));
					};
				}

				fd.WriteFuncCompact = (s, obj) => wf(s, yi.GetValue(obj));
				result.Fields.Add(fd);
			}
		}

		private static void PrepareClassDefFieldsUnknown(BinarySerializer s, ClassDef result)
		{
			for (int ourIndex = 0, theirIndex = 1, i = 0; ; ++i) {
				var yi = ourIndex < result.Meta.Items.Count ? result.Meta.Items[ourIndex] : null;
				var their = theirIndex < result.ReaderDef.Fields.Count ? result.ReaderDef.Fields[theirIndex] : null;
				if (yi == null && their == null) {
					break;
				}
				// Capture.
				short j = (short)(i + 1);
				var ourName = yi?.Tag(s.Options);
				var cmp = their == null ? -1 : yi == null ? 1 : string.CompareOrdinal(ourName, their.Name);
				if (cmp <= 0) {
					var wf = s.GetWriteFunc(yi.Type);
					var fd = new ClassDef.FieldDef { Name = ourName, Type = yi.Type };
					if (yi.SerializeCond != null) {
						fd.WriteFuncUnknown = (s, obj, storage, storageIndex) => {
							var value = yi.GetValue(obj);
							if (!yi.SerializeCond(obj, value)) {
								return;
							}
							s.writer.Write(j);
							wf(s, value);
						};
					} else {
						fd.WriteFuncUnknown = (s, obj, storage, storageIndex) => {
							s.writer.Write(j);
							wf(s, yi.GetValue(obj));
						};
					}

					result.Fields.Add(fd);
					++ourIndex;
					if (cmp == 0) {
						++theirIndex;
					}
				} else {
					var theirType = result.ReaderDef.Fields[theirIndex].Type;
					var wf = s.GetWriteFunc(theirType);
					result.Fields.Add(new ClassDef.FieldDef {
						Name = their.Name,
						Type = theirType,
						WriteFuncUnknown = (s, obj, storage, storageIndex) => {
							var si = storageIndex.Value;
							if (si < storage.Fields.Count && storage.Fields[si].Name == their.Name) {
								s.writer.Write(j);
								wf(s, storage.Fields[si].Value);
								++storageIndex.Value;
							}
						},
					});
					++theirIndex;
				}
			}
		}

		private static void WriteClassDefFields(BinarySerializer s, ClassDef def, string className)
		{
			s.writer.Write(def.Id);
			s.writer.Write(className);
			s.writer.Write((short)def.Fields.Count);
			foreach (var fd in def.Fields) {
				s.writer.Write(fd.Name);
				WriteRoughType(s, fd.Type);
			}
		}

		private static void WriteFields(BinarySerializer s, ClassDef def, object obj)
		{
			def.Meta.BeforeSerialization.Run(obj);
			s.objStack.Push(obj);
			try {
				foreach (var d in def.Fields) {
					d.WriteFunc(s, obj);
				}
				s.writer.Write((short)0);
			} finally {
				s.objStack.Pop();
			}
			def.Meta.AfterSerialization.Run(obj);
		}

		private static ClassDef WriteClassId(BinarySerializer s, object obj)
		{
			var t = obj.GetType();
			if (s.externalClassIdCache?.TryGetValue(t, out ClassDef result) ?? false) {
				s.writer.Write(result.Id);
				var g = result.Meta.GetUnknownStorage;
				if (g == null) {
					return result;
				}
				var i = g(obj).Internal;
				// If we have unknown fields, their definition must be present in the first serialized object,
				// but not necessariliy in subsequent ones.
				if (i != null && i != result.ReaderDef) {
					throw new YuzuException("Conflicting reader class definitions for unknown storage of " + t.Name);
				}
				return result;
			}
			if (s.internalClassIdCache.TryGetValue(t, out result)) {
				s.writer.Write(result.Id);
				var g = result.Meta.GetUnknownStorage;
				if (g == null) {
					return result;
				}
				var i = g(obj).Internal;
				// If we have unknown fields, their definition must be present in the first serialized object,
				// but not necessariliy in subsequent ones.
				if (i != null && i != result.ReaderDef) {
					throw new YuzuException("Conflicting reader class definitions for unknown storage of " + t.Name);
				}
				return result;
			}
			Console.WriteLine($"New Pes: `{t.FullName}`");
			result = new ClassDef {
				Id = (short)(s.internalClassIdCache.Count + s.unknownClassIdCache.Count + 1),
				Meta = Meta.Get(t, s.Options),
			};
			s.internalClassIdCache[t] = result;
			if (result.Meta.GetUnknownStorage == null) {
				PrepareClassDefFields(s, result);
			} else {
				result.ReaderDef = result.Meta.GetUnknownStorage(obj).Internal as ReaderClassDef;
				if (result.ReaderDef == null) {
					PrepareClassDefFields(s, result);
				} else {
					PrepareClassDefFieldsUnknown(s, result);
				}
			}
			WriteClassDefFields(s, result, result.Meta.WriteAlias ?? TypeSerializer.Serialize(result.Meta.Type));
			return result;
		}

		internal static void WriteClassDef(BinarySerializer s, System.IO.BinaryWriter externalWriter, ClassDef classDef)
		{
			var previousWriter = s.writer;
			s.writer = externalWriter;
			try {
				if (classDef.Meta.GetUnknownStorage != null) {
					throw new YuzuException($"Cached class defs do not support unknown storage.");
				}
				PrepareClassDefFields(s, classDef);
				WriteClassDefFields(
					s, classDef, classDef.Meta.WriteAlias ?? TypeSerializer.Serialize(classDef.Meta.Type)
				);
				return;
			} finally {
				s.writer = previousWriter;
			}
		}

		// Unknown class lacking binary-specific field descriptions.
		protected static void WriteUnknown(BinarySerializer s, object obj)
		{
			if (obj == null) {
				s.writer.Write((short)0);
				return;
			}
			var u = (YuzuUnknown)obj;
			if (s.unknownClassIdCache.TryGetValue(u.ClassTag, out ClassDef def)) {
				s.writer.Write(def.Id);
			} else {
				def = new ClassDef {
					Id = (short)(s.internalClassIdCache.Count + s.unknownClassIdCache.Count + 1),
					Meta = Meta.Unknown,
				};
				s.unknownClassIdCache[u.ClassTag] = def;
				short i = 0;
				foreach (var f in u.Fields) {
					// Capture.
					short j = (short)(i + 1);
					var t = f.Value.GetType();
					var wf = s.GetWriteFunc(t);
					// Capture.
					var name = f.Key;
					def.Fields.Add(new ClassDef.FieldDef {
						Name = name,
						Type = t,
						WriteFunc = (s, obj1) => {
							if ((obj1 as YuzuUnknown).Fields.TryGetValue(name, out object value)) {
								s.writer.Write(j);
								wf(s, value);
							}
						},
					});
					++i;
				}
				WriteClassDefFields(s, def, u.ClassTag);
			}
			WriteFields(s, def, obj);
		}

		protected static void WriteUnknownBinary(BinarySerializer s, object obj)
		{
			if (obj == null) {
				s.writer.Write((short)0);
				return;
			}
			var u = (YuzuUnknownBinary)obj;
			if (s.unknownClassIdCache.TryGetValue(u.ClassTag, out ClassDef def)) {
				s.writer.Write(def.Id);
			} else {
				def = new ClassDef {
					Id = (short)(s.internalClassIdCache.Count + s.unknownClassIdCache.Count + 1),
					Meta = Meta.Unknown,
				};
				s.unknownClassIdCache[u.ClassTag] = def;
				for (short i = 1; i < u.Def.Fields.Count; ++i) {
					var f = u.Def.Fields[i];
					// Capture.
					short j = (short)i;
					var wf = s.GetWriteFunc(f.Type);
					def.Fields.Add(new ClassDef.FieldDef {
						Name = f.Name,
						Type = f.Type,
						WriteFunc = (s, obj1) => {
							if ((obj1 as YuzuUnknown).Fields.TryGetValue(f.Name, out object value)) {
								s.writer.Write(j);
								wf(s, value);
							}
						},
					});
				}
				WriteClassDefFields(s, def, u.ClassTag);
			}
			WriteFields(s, def, obj);
		}

		private static void WriteObject(BinarySerializer s, object obj)
		{
			if (obj == null) {
				s.writer.Write((short)0);
			} else {
				WriteFields(s, WriteClassId(s, obj), obj);
			}
		}

		private static void WriteObjectUnknown(BinarySerializer s, object obj)
		{
			if (obj == null) {
				s.writer.Write((short)0);
				return;
			}
			var def = WriteClassId(s, obj);
			var storage = def.Meta.GetUnknownStorage(obj);
			var storageIndex = new BoxedInt();
			s.objStack.Push(obj);
			try {
				if (def.Fields.Count > 0) {
					if (def.Fields[0].WriteFuncUnknown != null) {
						foreach (var d in def.Fields) {
							d.WriteFuncUnknown(s, obj, storage, storageIndex);
						}
					} else {
						foreach (var d in def.Fields) {
							d.WriteFunc(s, obj);
						}
					}
				}
				s.writer.Write((short)0);
			} finally {
				s.objStack.Pop();
			}
		}

		private static void WriteObjectCompact(BinarySerializer s, object obj)
		{
			if (obj == null) {
				s.writer.Write((short)0);
				return;
			}
			var def = WriteClassId(s, obj);
			def.Meta.BeforeSerialization.Run(obj);
			s.objStack.Push(obj);
			try {
				foreach (var d in def.Fields) {
					d.WriteFuncCompact(s, obj);
				}
			} finally {
				s.objStack.Pop();
			}
			def.Meta.AfterSerialization.Run(obj);
		}

		private static Action<BinarySerializer, object> MakeWriteIEnumerable(BinarySerializer s, Type t)
		{
			var wf = s.GetWriteFunc(t.GetGenericArguments()[0]);
			var m = Utils.GetPrivateCovariantGenericStatic(s.GetType(), nameof(WriteIEnumerable), t);
			var d = MakeDelegateParamStatic<Action<BinarySerializer, object>>(m);
			return (s, obj) => d(s, obj, wf);
		}

		private static Action<BinarySerializer, object> MakeObjectWriteFunc(Meta meta)
		{
			if (meta.IsCompact) {
				return WriteObjectCompact;
			}
			if (meta.GetUnknownStorage == null) {
				return WriteObject;
			}
			return WriteObjectUnknown;
		}

		private static Action<BinarySerializer, object> WriteDataStructureOfRecord(BinarySerializer s, Type t)
		{
			if (t == typeof(Record)) {
				return WriteRecord;
			}
			if (!t.IsGenericType) {
				return null;
			}
			var g = t.GetGenericTypeDefinition();
			if (g == typeof(List<>)) {
				var writeValue = WriteDataStructureOfRecord(s, t.GetGenericArguments()[0]);
				if (writeValue == null) {
					return null;
				}
				return (s, obj) => WriteIEnumerable<object>(s, obj, writeValue);
			}
			if (g == typeof(Dictionary<,>)) {
				var a = t.GetGenericArguments();
				var writeValue = WriteDataStructureOfRecord(s, a[1]);
				if (writeValue == null) {
					return null;
				}
				var d = (Action<
					BinarySerializer, object, Action<BinarySerializer, object>, Action<BinarySerializer, object>
				>)Delegate.CreateDelegate(
					typeof(Action<
						BinarySerializer, object, Action<BinarySerializer, object>, Action<BinarySerializer, object>
					>),
					Utils.GetPrivateGenericStatic(
						s.GetType(),
						nameof(WriteIDictionary),
						a[0],
						typeof(object)
					)
				);
				return (s, obj) => d(s, obj, s.GetWriteFunc(a[0]), writeValue);
			}
			return null;
		}

		private static Action<BinarySerializer, object> MakeWriteFunc(BinarySerializer s, Type t)
		{
			if (t.IsEnum) {
				return s.GetWriteFunc(Enum.GetUnderlyingType(t));
			}
			if (t.IsGenericType) {
				var writeRecord = WriteDataStructureOfRecord(s, t);
				if (writeRecord != null) {
					return writeRecord;
				}
				var g = t.GetGenericTypeDefinition();
				if (g == typeof(Action<>)) {
					return WriteAction;
				}
				if (g == typeof(Nullable<>)) {
					var w = s.GetWriteFunc(t.GetGenericArguments()[0]);
					return (s, obj) => {
						s.writer.Write(obj == null);
						if (obj != null) {
							w(s, obj);
						}
					};
				}
			}
			if (t.IsArray) {
				var wf = s.GetWriteFunc(t.GetElementType());
				if (t.GetArrayRank() > 1) {
					return (s, obj) => WriteArrayNDim(s, obj, wf);
				}
				var m = Utils.GetPrivateCovariantGenericStatic(s.GetType(), nameof(WriteArray), t);
				var d = MakeDelegateParamStatic<Action<BinarySerializer, object>>(m);
				return (s, obj) => d(s, obj, wf);
			}
			var meta = Meta.Get(t, s.Options);
			Action<BinarySerializer, object> normalWrite = MakeObjectWriteFunc(meta);
			var sg = meta.Surrogate;
			if (sg.SurrogateType != null && sg.FuncTo != null) {
				var sw = s.GetWriteFunc(sg.SurrogateType);
				if (sg.FuncIf == null) {
					return (s, obj) => sw(s, sg.FuncTo(obj));
				}
				return (s, obj) => {
					if (sg.FuncIf(obj)) {
						sw(s, sg.FuncTo(obj));
					} else {
						normalWrite(s, obj);
					}
				};
			}
			{
				var idict = Utils.GetIDictionary(t);
				if (idict != null) {
					var a = idict.GetGenericArguments();
					var wk = s.GetWriteFunc(a[0]);
					var wv = s.GetWriteFunc(a[1]);
					var m = Utils.GetPrivateCovariantGenericAllStatic(s.GetType(), nameof(WriteIDictionary), idict);
					var d = MakeDelegateParam2Static<
						Action<BinarySerializer, object>, Action<BinarySerializer, object>
					>(m);
					return (s, obj) => d(s, obj, wk, wv);
				}
			}
			if (meta.SerializeItemIf != null) {
				// Two passes are required anyway, so it is useless to optimize Count.
				var ienum = Utils.GetIEnumerable(t);
				var wf = s.GetWriteFunc(ienum.GetGenericArguments()[0]);
				var m = Utils.GetPrivateCovariantGenericStatic(s.GetType(), nameof(WriteIEnumerableIf), ienum);
				var d = MakeDelegateParam2Static<Action<BinarySerializer, object>, Func<object, int, object, bool>>(m);
				return (s, obj) => d(s, obj, wf, meta.SerializeItemIf);
			}
			{
				var icoll = Utils.GetICollection(t);
				if (icoll != null) {
					var wf = s.GetWriteFunc(icoll.GetGenericArguments()[0]);
					if (Utils.GetICollectionNG(t) != null) {
						return (s, obj) => WriteCollectionNG(s, obj, wf);
					}
					var m = Utils.GetPrivateCovariantGenericStatic(s.GetType(), nameof(WriteCollection), icoll);
					var d = MakeDelegateParamStatic<Action<BinarySerializer, object>>(m);
					return (s, obj) => d(s, obj, wf);
				}
			}
			{
				var ienum = Utils.GetIEnumerable(t);
				if (ienum != null) {
					return MakeWriteIEnumerable(s, ienum);
				}
			}
			if (t.IsRecord()) {
				return normalWrite;
			}
			throw new NotImplementedException(t.Name);
		}

		protected override void ToWriter(object obj)
		{
			if (BinaryOptions.AutoSignature) {
				WriteSignature();
			}
			WriteAny(this, obj);
		}

		public void WriteSignature() => writer.Write(BinaryOptions.Signature);

		public void UseYuzuCache(YuzuCache yuzuCache)
		{
			if (yuzuCache.WriterCache != null) {
				externalClassIdCache = yuzuCache.WriterCache;
				return;
			}
			yuzuCache.CompleteReaderCache(Options);
			var readerCache = yuzuCache.ReaderCache;
			var writerCache = new Dictionary<Type, ClassDef>();
			for (int i = 0; i < readerCache.Count; i++) {
				var readerClassDef = readerCache[i];
				// var t = Yuzu.Util.TypeSerializer.Deserialize(readerClassDef.CompletionRecord.TypeName);
				var t = readerClassDef.Meta.Type;
				var writerDef = new Yuzu.Binary.BinarySerializer.ClassDef {
					Id = (short)(-i - 1),
					Meta = readerClassDef.Meta,
				};
				if (writerDef.Meta.GetUnknownStorage == null) {
					Yuzu.Binary.BinarySerializer.PrepareClassDefFields(this, writerDef);
				} else {
					throw new NotImplementedException();
				}
				if (writerCache.ContainsKey(t)) {
					Console.WriteLine("pizda konechno, chto esche skazat");
					// throw new InvalidOperationException();
				}
				writerCache[t] = writerDef;
			}
			externalClassIdCache = writerCache;
			yuzuCache.WriterCache = writerCache;
		}

		protected static Action<BinarySerializer, object, TParam> MakeDelegateParamStatic<TParam>(MethodInfo m)
		{
			return (Action<BinarySerializer, object, TParam>)Delegate.CreateDelegate(
				typeof(Action<BinarySerializer, object, TParam>), m
			);
		}

		protected static Action<BinarySerializer, object, TParam1, TParam2>
			MakeDelegateParam2Static<TParam1, TParam2>(MethodInfo m)
		{
			return (Action<BinarySerializer, object, TParam1, TParam2>)Delegate.CreateDelegate(
				typeof(Action<BinarySerializer, object, TParam1, TParam2>), m
			);
		}
	}
}
