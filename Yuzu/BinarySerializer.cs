using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Yuzu.Metadata;
using Yuzu.Util;

namespace Yuzu.Binary
{
	public class BinarySerializer : AbstractWriterSerializer
	{
		public BinarySerializeOptions BinaryOptions = new ();

		private Action<object> referenceWriteFunc;

		protected override void Initialize()
		{
			if (ReferenceResolver != null) {
				referenceWriteFunc = GetWriteFunc(ReferenceResolver.ReferenceType);
			}
		}

		protected void WriteSByte(object obj) => writer.Write((sbyte)obj);
		protected void WriteByte(object obj) => writer.Write((byte)obj);
		protected void WriteShort(object obj) => writer.Write((short)obj);
		protected void WriteUShort(object obj) => writer.Write((ushort)obj);
		protected void WriteInt(object obj) => writer.Write((int)obj);
		protected void WriteUInt(object obj) => writer.Write((uint)obj);
		protected void WriteLong(object obj) => writer.Write((long)obj);
		protected void WriteULong(object obj) => writer.Write((ulong)obj);
		protected void WriteBool(object obj) => writer.Write((bool)obj);
		protected void WriteChar(object obj) => writer.Write((char)obj);
		protected void WriteFloat(object obj) => writer.Write((float)obj);
		protected void WriteDouble(object obj) => writer.Write((double)obj);
		protected void WriteDecimal(object obj) => writer.Write((decimal)obj);

		protected void WriteDateTime(object obj) => writer.Write(((DateTime)obj).ToBinary());
		protected void WriteDateTimeOffset(object obj)
		{
			var d = (DateTimeOffset)obj;
			writer.Write(d.DateTime.ToBinary());
			writer.Write(d.Offset.Ticks);
		}
		protected void WriteTimeSpan(object obj) => writer.Write(((TimeSpan)obj).Ticks);

		protected void WriteGuid(object obj) => writer.Write(((Guid)obj).ToByteArray());

		protected void WriteString(object obj)
		{
			if (obj == null) {
				writer.Write("");
				writer.Write(true);
				return;
			}
			writer.Write((string)obj);
			if ((string)obj == "") {
				writer.Write(false);
			}
		}

		protected void WriteAny(object obj)
		{
			if (obj == null) {
				writer.Write((byte)RoughType.Any);
				return;
			}
			var t = obj.GetType();
			if (t == typeof(object))
				throw new YuzuException("WriteAny of unknown type");
			WriteRoughType(t);
			GetWriteFunc(t)(obj);
		}

		protected void WriteRecord(object obj) => GetWriteFunc(obj.GetType())(obj);

		private Dictionary<Type, Action<object>> writerCache;

		private Action<object> GetWriteFunc(Type t)
		{
			if (writerCache.TryGetValue(t, out Action<object> result))
				return result;
			result = MakeWriteFunc(t);
			writerCache[t] = result;
			return result;
		}

		public BinarySerializer()
		{
			InitWriters();
		}

		private void InitWriters()
		{
			writerCache = new Dictionary<Type, Action<object>>() {
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

		private void WriteRoughType(Type t)
		{
			for (var result = RoughType.FirstAtom; result <= RoughType.LastAtom; ++result)
				if (t == RT.roughTypeToType[(int)result]) {
					writer.Write((byte)result);
					return;
				}
			if (t.IsEnum) {
				WriteRoughType(Enum.GetUnderlyingType(t));
				return;
			}
			if (t.IsGenericType) {
				var g = t.GetGenericTypeDefinition();
				if (g == typeof(Nullable<>)) {
					writer.Write((byte)RoughType.Nullable);
					WriteRoughType(t.GetGenericArguments()[0]);
					return;
				}
				if (g == typeof(Action<>)) {
					writer.Write((byte)RoughType.Record);
					return;
				}
			}
			if (t.IsArray) {
				if (t.GetArrayRank() > 1) {
					writer.Write((byte)RoughType.NDimArray);
					writer.Write((byte)t.GetArrayRank());
				}
				else
					writer.Write((byte)RoughType.Sequence);
				WriteRoughType(t.GetElementType());
				return;
			}

			if (t != typeof(YuzuUnknown) && !t.IsSubclassOf(typeof(YuzuUnknown)) && t != typeof(Record)) {
				var sg = Meta.Get(t, Options).Surrogate;
				if (sg.SurrogateType != null && sg.FuncTo != null) {
					WriteRoughType(sg.SurrogateType);
					return;
				}
			}

			var idict = Utils.GetIDictionary(t);
			if (idict != null) {
				writer.Write((byte)RoughType.Mapping);
				var a = idict.GetGenericArguments();
				WriteRoughType(a[0]);
				WriteRoughType(a[1]);
				return;
			}

			var ienum = Utils.GetIEnumerable(t);
			if (ienum != null) {
				writer.Write((byte)RoughType.Sequence);
				WriteRoughType(ienum.GetGenericArguments()[0]);
				return;
			}
			if (t.IsRecord()) {
				writer.Write((byte)RoughType.Record);
				return;
			}
			throw new NotImplementedException();
		}

		private void WriteIDictionary<K, V>(object obj, Action<object> writeKey, Action<object> writeValue)
		{
			if (obj == null) {
				writer.Write(-1);
				return;
			}
			var dict = (IDictionary<K, V>)obj;
			writer.Write(dict.Count);
			foreach (var elem in dict) {
				writeKey(elem.Key);
				writeValue(elem.Value);
			}
		}

		private Stack<object> objStack = new ();

		private void WriteAction(object obj)
		{
			if (obj == null) {
				writer.Write("");
				return;
			}
			var a = obj as MulticastDelegate;
			if (a.Target != objStack.Peek())
				throw new NotImplementedException();
			writer.Write(a.Method.Name);
		}

		private void WriteArray<T>(object obj, Action<object> wf)
		{
			if (obj == null) {
				writer.Write(-1);
				return;
			}
			var arr = (T[])obj;
			writer.Write(arr.Length);
			foreach (var a in arr)
				wf(a);
		}

		private void WriteArrayNDim(object obj, Action<object> writeElemFunc)
		{
			if (obj == null) {
				writer.Write(-1);
				return;
			}
			var arr = (Array)obj;
			var lbs = new int[arr.Rank];
			var ubs = new int[arr.Rank];
			var hasNonZeroLB = false;
			for (int dim = 0; dim < arr.Rank; ++dim) {
				lbs[dim] = arr.GetLowerBound(dim);
				ubs[dim] = arr.GetUpperBound(dim);
				writer.Write(ubs[dim] - lbs[dim] + 1);
				if (lbs[dim] != 0)
					hasNonZeroLB = true;
			}

			writer.Write(hasNonZeroLB);
			if (hasNonZeroLB)
				for (int dim = 0; dim < arr.Rank; ++dim)
					writer.Write(lbs[dim]);

			if (arr.Length == 0)
				return;
			var indices = (int[])lbs.Clone();
			for (int dim = arr.Rank - 1; ;) {
				writeElemFunc(arr.GetValue(indices));
				if (indices[dim] == ubs[dim]) {
					for (; dim >= 0 && indices[dim] == ubs[dim]; --dim)
						indices[dim] = lbs[dim];
					if (dim < 0)
						break;
					++indices[dim];
					dim = arr.Rank - 1;
				}
				else
					++indices[dim];
			}
		}

		private void WriteIEnumerable<T>(object list, Action<object> wf)
		{
			if (list == null) {
				writer.Write(-1);
				return;
			}
			var ienum = (IEnumerable<T>)list;
			writer.Write(ienum.Count());
			foreach (var a in ienum)
				wf(a);
		}

		private void WriteIEnumerableIf<T>(object list, Action<object> wf, Func<object, int, object, bool> cond)
		{
			if (list == null) {
				writer.Write(-1);
				return;
			}
			var ienum = (IEnumerable<T>)list;
			int index = 0;
			writer.Write(ienum.Count(a => cond(list, index++, a)));
			index = 0;
			foreach (var a in ienum)
				if (cond(list, index++, a))
					wf(a);
		}

		// Duplicate WriteIEnumerable to optimize Count.
		private void WriteCollection<T>(object list, Action<object> wf)
		{
			if (list == null) {
				writer.Write(-1);
				return;
			}
			var icoll = (ICollection<T>)list;
			writer.Write(icoll.Count);
			foreach (var a in icoll)
				wf(a);
		}

		private void WriteCollectionNG(object obj, Action<object> wf)
		{
			if (obj == null) {
				writer.Write(-1);
				return;
			}
			var list = (ICollection)obj;
			writer.Write(list.Count);
			foreach (var a in list)
				wf(a);
		}

		protected class ClassDef
		{
			public struct FieldDef
			{
				public string Name;
				public Type Type;
				public Action<object> WriteFunc;
				public Action<object> WriteFuncCompact;
				internal Action<object, YuzuUnknownStorage, BoxedInt> WriteFuncUnknown;
			}
			public short Id;
			internal Meta Meta;
			internal ReaderClassDef ReaderDef;
			public List<FieldDef> Fields = new ();
		}
		private Dictionary<Type, ClassDef> classIdCache = new ();
		private Dictionary<string, ClassDef> unknownClassIdCache = new ();

		public void ClearClassIds() { classIdCache.Clear(); }

		private void PrepareClassDefFields(ClassDef result)
		{
			for (short i = 0; i < result.Meta.Items.Count; ++i) {
				var yi = result.Meta.Items[i];
				short j = (short)(i + 1); // Capture.
				var wf = GetWriteFunc(yi.Type);
				var fd = new ClassDef.FieldDef { Name = yi.Tag(Options), Type = yi.Type };
				if (yi.SerializeCond != null)
					fd.WriteFunc = obj => {
						var value = yi.GetValue(obj);
						if (!yi.SerializeCond(obj, value))
							return;
						writer.Write(j);
						wf(value);
					};
				else
					fd.WriteFunc = obj => {
						writer.Write(j);
						wf(yi.GetValue(obj));
					};
				fd.WriteFuncCompact = obj => wf(yi.GetValue(obj));
				result.Fields.Add(fd);
			}
		}

		private void PrepareClassDefFieldsUnknown(ClassDef result)
		{
			for (int ourIndex = 0, theirIndex = 1, i = 0; ; ++i) {
				var yi = ourIndex < result.Meta.Items.Count ? result.Meta.Items[ourIndex] : null;
				var their = theirIndex < result.ReaderDef.Fields.Count ? result.ReaderDef.Fields[theirIndex] : null;
				if (yi == null && their == null)
					break;

				short j = (short)(i + 1); // Capture.
				var ourName = yi == null ? null : yi.Tag(Options);
				var cmp = their == null ? -1 : yi == null ? 1 : String.CompareOrdinal(ourName, their.Name);
				if (cmp <= 0) {
					var wf = GetWriteFunc(yi.Type);
					var fd = new ClassDef.FieldDef { Name = ourName, Type = yi.Type };
					if (yi.SerializeCond != null)
						fd.WriteFuncUnknown = (obj, storage, storageIndex) => {
							var value = yi.GetValue(obj);
							if (!yi.SerializeCond(obj, value))
								return;
							writer.Write(j);
							wf(value);
						};
					else
						fd.WriteFuncUnknown = (obj, storage, storageIndex) => {
							writer.Write(j);
							wf(yi.GetValue(obj));
						};
					result.Fields.Add(fd);
					++ourIndex;
					if (cmp == 0)
						++theirIndex;
				}
				else {
					var theirType = result.ReaderDef.Fields[theirIndex].Type;
					var wf = GetWriteFunc(theirType);
					result.Fields.Add(new ClassDef.FieldDef {
						Name = their.Name, Type = theirType,
						WriteFuncUnknown = (obj, storage, storageIndex) => {
							var si = storageIndex.Value;
							if (si < storage.Fields.Count && storage.Fields[si].Name == their.Name) {
								writer.Write(j);
								wf(storage.Fields[si].Value);
								++storageIndex.Value;
							}
						}
					});
					++theirIndex;
				}
			}
		}

		private void WriteClassDefFields(ClassDef def, string className)
		{
			writer.Write(def.Id);
			writer.Write(className);
			writer.Write((short)def.Fields.Count);
			foreach (var fd in def.Fields) {
				writer.Write(fd.Name);
				WriteRoughType(fd.Type);
			}
		}

		private void WriteFields(ClassDef def, object obj)
		{
			def.Meta.BeforeSerialization.Run(obj);
			objStack.Push(obj);
			try {
				foreach (var d in def.Fields)
					d.WriteFunc(obj);
				writer.Write((short)0);
			}
			finally {
				objStack.Pop();
			}
			def.Meta.AfterSerialization.Run(obj);
		}

		private ClassDef GetClassDef(object obj, out bool isNew)
		{
			var t = obj.GetType();
			isNew = !classIdCache.TryGetValue(t, out var result);
			if (isNew) {
				result = new ClassDef { Id = (short)(classIdCache.Count + unknownClassIdCache.Count + 1) };
				result.Meta = Meta.Get(t, Options);
				classIdCache[t] = result;
				if (result.Meta.GetUnknownStorage == null)
					PrepareClassDefFields(result);
				else {
					result.ReaderDef = result.Meta.GetUnknownStorage(obj).Internal as ReaderClassDef;
					if (result.ReaderDef == null)
						PrepareClassDefFields(result);
					else
						PrepareClassDefFieldsUnknown(result);
				}
			} else {
				var g = result.Meta.GetUnknownStorage;
				if (g != null) {
					var i = g(obj).Internal;
					// If we have unknown fields, their definition must be present in the first serialized object,
					// but not necessariliy in subsequent ones.
					if (i != null && i != result.ReaderDef)
						throw new YuzuException("Conflictiing reader class definitions for unknown storage of " + t.Name);
				}
			}
			return result;
		}

		private void WriteClassId(ClassDef d, bool isNew)
		{
			if (isNew) {
				WriteClassDefFields(d, d.Meta.WriteAlias ?? TypeSerializer.Serialize(d.Meta.Type));
			} else {
				writer.Write(d.Id);
			}
		}

		// Unknown class lacking binary-specific field descriptions.
		protected void WriteUnknown(object obj)
		{
			if (obj == null) {
				writer.Write((short)0);
				return;
			}
			var u = (YuzuUnknown)obj;
			if (unknownClassIdCache.TryGetValue(u.ClassTag, out ClassDef def)) {
				writer.Write(def.Id);
			}
			else {
				def = new ClassDef { Id = (short)(classIdCache.Count + unknownClassIdCache.Count + 1) };
				def.Meta = Meta.Unknown;
				unknownClassIdCache[u.ClassTag] = def;
				short i = 0;
				foreach (var f in u.Fields) {
					short j = (short)(i + 1); // Capture.
					var t = f.Value.GetType();
					var wf = GetWriteFunc(t);
					var name = f.Key; // Capture.
					def.Fields.Add(new ClassDef.FieldDef {
						Name = name,
						Type = t,
						WriteFunc = obj1 => {
							if ((obj1 as YuzuUnknown).Fields.TryGetValue(name, out object value)) {
								writer.Write(j);
								wf(value);
							}
						},
					});
					++i;
				}
				WriteClassDefFields(def, u.ClassTag);
			}
			WriteFields(def, obj);
		}

		protected void WriteUnknownBinary(object obj)
		{
			if (obj == null) {
				writer.Write((short)0);
				return;
			}
			var u = (YuzuUnknownBinary)obj;
			if (unknownClassIdCache.TryGetValue(u.ClassTag, out ClassDef def)) {
				writer.Write(def.Id);
			}
			else {
				def = new ClassDef { Id = (short)(classIdCache.Count + unknownClassIdCache.Count + 1) };
				def.Meta = Meta.Unknown;
				unknownClassIdCache[u.ClassTag] = def;
				for (short i = 1; i < u.Def.Fields.Count; ++i) {
					var f = u.Def.Fields[i];
					short j = (short)i; // Capture.
					var wf = GetWriteFunc(f.Type);
					def.Fields.Add(new ClassDef.FieldDef {
						Name = f.Name,
						Type = f.Type,
						WriteFunc = obj1 => {
							if ((obj1 as YuzuUnknown).Fields.TryGetValue(f.Name, out object value)) {
								writer.Write(j);
								wf(value);
							}
						},
					});
				}
				WriteClassDefFields(def, u.ClassTag);
			}
			WriteFields(def, obj);
		}

		private void WriteObject(object obj)
		{
			if (obj == null) {
				writer.Write((short)0);
				return;
			}
			// TODO: Meta.Get()???
			if (Meta.Get(obj.GetType(), Options).SerializeByReference && ReferenceResolver != null) {
				var reference = ReferenceResolver.GetReference(obj, out var alreadyExists);
				if (alreadyExists) {
					writer.Write((short)BinarySerializeOptions.ReferenceTag);
					referenceWriteFunc(reference);
					return;
				}
				writer.Write(BinarySerializeOptions.IdTag);
				referenceWriteFunc(reference);
			}
			var def = GetClassDef(obj, out var isNewDef);
			WriteClassId(def, isNewDef);
			WriteFields(def, obj);
		}

		private void WriteObjectUnknown(object obj)
		{
			if (obj == null) {
				writer.Write((short)0);
				return;
			}
			var def = GetClassDef(obj, out var isNewDef);
			WriteClassId(def, isNewDef);
			var storage = def.Meta.GetUnknownStorage(obj);
			var storageIndex = new BoxedInt();
			objStack.Push(obj);
			try {
				if (def.Fields.Count > 0) {
					if (def.Fields[0].WriteFuncUnknown != null)
						foreach (var d in def.Fields)
							d.WriteFuncUnknown(obj, storage, storageIndex);
					else
						foreach (var d in def.Fields)
							d.WriteFunc(obj);
				}
				writer.Write((short)0);
			}
			finally {
				objStack.Pop();
			}
		}

		private void WriteObjectCompact(object obj)
		{
			if (obj == null) {
				writer.Write((short)0);
				return;
			}
			var def = GetClassDef(obj, out var isNewDef);
			WriteClassId(def, isNewDef);
			def.Meta.BeforeSerialization.Run(obj);
			objStack.Push(obj);
			try {
				foreach (var d in def.Fields)
					d.WriteFuncCompact(obj);
			}
			finally {
				objStack.Pop();
			}
			def.Meta.AfterSerialization.Run(obj);
		}

		private Action<object> MakeWriteIEnumerable(Type t)
		{
			var wf = GetWriteFunc(t.GetGenericArguments()[0]);
			var m = Utils.GetPrivateCovariantGeneric(GetType(), nameof(WriteIEnumerable), t);
			var d = MakeDelegateParam<Action<object>>(m);
			return obj => d(obj, wf);
		}

		private Action<object> MakeObjectWriteFunc(Meta meta)
		{
			if (meta.IsCompact) return WriteObjectCompact;
			if (meta.GetUnknownStorage == null) return WriteObject;
			return WriteObjectUnknown;
		}

		private Action<object> WriteDataStructureOfRecord(Type t)
		{
			if (t == typeof(Record))
				return WriteRecord;
			if (!t.IsGenericType)
				return null;
			var g = t.GetGenericTypeDefinition();
			if (g == typeof(List<>)) {
				var writeValue = WriteDataStructureOfRecord(t.GetGenericArguments()[0]);
				if (writeValue == null) return null;
				return obj => WriteIEnumerable<object>(obj, writeValue);
			}
			if (g == typeof(Dictionary<,>)) {
				var a = t.GetGenericArguments();
				var writeValue = WriteDataStructureOfRecord(a[1]);
				if (writeValue == null) return null;
				var d = (Action<object, Action<object>, Action<object>>)Delegate.CreateDelegate(
					typeof(Action<object, Action<object>, Action<object>>), this,
					Utils.GetPrivateGeneric(GetType(), nameof(WriteIDictionary), a[0], typeof(object))
				);
				return obj => d(obj, GetWriteFunc(a[0]), writeValue);
			}
			return null;
		}

		private Action<object> MakeWriteFunc(Type t)
		{
			if (t.IsEnum)
				return GetWriteFunc(Enum.GetUnderlyingType(t));
			if (t.IsGenericType) {
				var writeRecord = WriteDataStructureOfRecord(t);
				if (writeRecord != null)
					return writeRecord;
				var g = t.GetGenericTypeDefinition();
				if (g == typeof(Action<>))
					return WriteAction;
				if (g == typeof(Nullable<>)) {
					var w = GetWriteFunc(t.GetGenericArguments()[0]);
					return obj => {
						writer.Write(obj == null);
						if (obj != null)
							w(obj);
					};
				}
			}
			if (t.IsArray) {
				var wf = GetWriteFunc(t.GetElementType());
				if (t.GetArrayRank() > 1) {
					return obj => WriteArrayNDim(obj, wf);
				}
				var m = Utils.GetPrivateCovariantGeneric(GetType(), nameof(WriteArray), t);
				var d = MakeDelegateParam<Action<object>>(m);
				return obj => d(obj, wf);
			}
			var meta = Meta.Get(t, Options);
			Action<object> normalWrite = MakeObjectWriteFunc(meta);
			var sg = meta.Surrogate;
			if (sg.SurrogateType != null && sg.FuncTo != null) {
				var sw = GetWriteFunc(sg.SurrogateType);
				if (sg.FuncIf == null)
					return obj => sw(sg.FuncTo(obj));
				return obj => {
					if (sg.FuncIf(obj))
						sw(sg.FuncTo(obj));
					else
						normalWrite(obj);
				};
			}
			{
				var idict = Utils.GetIDictionary(t);
				if (idict != null) {
					var a = idict.GetGenericArguments();
					var wk = GetWriteFunc(a[0]);
					var wv = GetWriteFunc(a[1]);
					var m = Utils.GetPrivateCovariantGenericAll(GetType(), nameof(WriteIDictionary), idict);
					var d = MakeDelegateParam2<Action<object>, Action<object>>(m);
					return obj => d(obj, wk, wv);
				}
			}
			if (meta.SerializeItemIf != null) {
				// Two passes are required anyway, so it is useless to optimize Count.
				var ienum = Utils.GetIEnumerable(t);
				var wf = GetWriteFunc(ienum.GetGenericArguments()[0]);
				var m = Utils.GetPrivateCovariantGeneric(GetType(), nameof(WriteIEnumerableIf), ienum);
				var d = MakeDelegateParam2<Action<object>, Func<object, int, object, bool>>(m);
				return obj => d(obj, wf, meta.SerializeItemIf);
			}
			{
				var icoll = Utils.GetICollection(t);
				if (icoll != null) {
					var wf = GetWriteFunc(icoll.GetGenericArguments()[0]);
					if (Utils.GetICollectionNG(t) != null)
						return obj => WriteCollectionNG(obj, wf);
					var m = Utils.GetPrivateCovariantGeneric(GetType(), nameof(WriteCollection), icoll);
					var d = MakeDelegateParam<Action<object>>(m);
					return obj => d(obj, wf);
				}
			}
			{
				var ienum = Utils.GetIEnumerable(t);
				if (ienum != null)
					return MakeWriteIEnumerable(ienum);
			}
			if (t.IsRecord())
				return normalWrite;
			throw new NotImplementedException(t.Name);
		}

		protected override void ToWriter(object obj)
		{
			if (BinaryOptions.AutoSignature)
				WriteSignature();
			WriteAny(obj);
		}

		public void WriteSignature() { writer.Write(BinaryOptions.Signature); }

		public override string ToString(object obj)
		{
			var ms = new System.IO.MemoryStream();
			ToStream(obj, ms);
			return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
		}
	}
}
