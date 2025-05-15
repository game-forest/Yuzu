using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Yuzu.CloneUtil;
using Yuzu.Metadata;
using Yuzu.Util;

namespace Yuzu.Clone
{
	using serializeItemIfType = Func<object, int, object, bool>;

	public abstract class AbstractCloner
	{
		public CommonOptions Options;

		public IReferenceResolver ReferenceResolver;

		public abstract object ShallowObject(object src);
		public virtual T Shallow<T>(T src) => (T)ShallowObject(src);

		public abstract object DeepObject(object src);
		public virtual T Deep<T>(T obj) => (T)DeepObject(obj);

		public abstract void MergeObject(object dst, object src);
		public virtual void Merge<T>(T dst, T src) => MergeObject(dst, src);
	}

	public class Cloner : AbstractCloner
	{
		public static Cloner Instance = new();

		private readonly ConcurrentDictionary<Type, Func<object, object>> clonerCache = [];
		private readonly ConcurrentDictionary<Type, Action<object, object>> mergerCache = [];

		public Cloner() { }

		protected Cloner(IDictionary<Type, Func<Cloner, object, object>> initClonerCache)
		{
			foreach (var kv in initClonerCache)
				clonerCache.TryAdd(kv.Key, src => kv.Value(this, src));
		}

		public override object ShallowObject(object src)
		{
			var meta = Meta.Get(src.GetType(), Options);
			meta.BeforeSerialization.Run(src);
			var result = meta.Factory();
			meta.BeforeDeserialization.Run(result);
			foreach (var item in meta.Items)
				item.SetValue(result, item.GetValue(src));
			meta.AfterSerialization.Run(src);
			meta.AfterDeserialization.Run(result);
			return result;
		}

		public override T Deep<T>(T obj) => (T)DeepObject(obj);

		public static bool IsCopyable(Type t, CommonOptions options) =>
			Utils.IsCopyable(t) ?? Meta.Get(t, options).IsCopyable;

		private bool IsCopyable(Type t) => IsCopyable(t, Options);
		internal bool IsCopyable(Meta.Item yi) => yi.IsCopyable || IsCopyable(yi.Type, Options);

		public Func<object, object> GetCloner(Type t)
		{
			if (!clonerCache.TryGetValue(t, out Func<object, object> cloner)) {
				cloner = MakeCloner(t);
				clonerCache.TryAdd(t, cloner);
			}
			return cloner;
		}
		public Func<object, object> GetCloner<T>() => GetCloner(typeof(T));

		public Action<object, object> GetMerger(Type t)
		{
			if (!mergerCache.TryGetValue(t, out Action<object, object> merger)) {
				merger = MakeMerger(t);
				mergerCache.TryAdd(t, merger);
			}
			return merger;
		}
		public Action<object, object> GetMerger<T>() => GetMerger(typeof(T));

		private Func<object, object> MakeSurrogateCloner(Meta meta)
		{
			var sg = meta.Surrogate;
			if (sg.SurrogateType == null)
				return null;
			if (sg.FuncFrom == null || sg.FuncTo == null)
				throw new YuzuException("Both FromSurrogate and ToSurrogate must be defined for cloning");
			var surrogateCloner = GetCloner(sg.SurrogateType);
			return src => sg.FuncFrom(surrogateCloner(sg.FuncTo(src)));
		}

		private Func<object, object> MakeCloner(Type t)
		{
			if (t.IsGenericType) {
				var g = t.GetGenericTypeDefinition();
				if (g == typeof(Nullable<>)) {
					var c = GetCloner(t.GetGenericArguments()[0]);
					return src => src ?? c(src);
				}
			}
			if (t.IsArray) {
				var e = t.GetElementType();
				if (IsCopyable(e)) {
					if (t.GetArrayRank() > 1)
						return CloneUtils.CloneArrayPrimitiveNDim;
					var m = CloneUtils.GetGeneric(nameof(CloneUtils.CloneArrayPrimitive), e);
					return CloneUtils.MakeDelegate<Func<object, object>>(m);
				}
				else {
					var cloneElem = GetCloner(e);
					if (t.GetArrayRank() > 1)
						return src => CloneUtils.CloneArrayNDim(src, cloneElem);
					var m = CloneUtils.GetGeneric(nameof(CloneUtils.CloneArray), e);
					var d = CloneUtils.MakeDelegate<Func<object, Func<object, object>, object>>(m);
					return src => d(src, cloneElem);
				}
			}
			{
				var idict = Utils.GetIDictionary(t);
				if (idict != null) {
					var a = idict.GetGenericArguments();
					if (!IsCopyable(a[0])) {
						var ck = GetCloner(a[0]);
						var cv = GetCloner(a[1]);
						var m = CloneUtils.GetGeneric(nameof(CloneUtils.CloneIDictionary), t, a[0], a[1]);
						var d = CloneUtils.MakeDelegate<
							Func<object, Func<object, object>, Func<object, object>, object>>(m);
						return obj => d(obj, ck, cv);
					}
					else if (!IsCopyable(a[1])) {
						var cv = GetCloner(a[1]);
						var m = CloneUtils.GetGeneric(
							nameof(CloneUtils.CloneIDictionaryPrimiviteKey), t, a[0], a[1]);
						var d = CloneUtils.MakeDelegate<Func<object, Func<object, object>, object>>(m);
						return obj => d(obj, cv);
					}
					else {
						var m = CloneUtils.GetGeneric(
							nameof(CloneUtils.CloneIDictionaryPrimivite), t, a[0], a[1]);
						return CloneUtils.MakeDelegate<Func<object, object>>(m);
					}
				}
			}
			if (t == typeof(object))
				return DeepObject;
			if (IsCopyable(t)) return CloneUtils.ValueCopy;
			var meta = Meta.Get(t, Options);
			{
				var icoll = Utils.GetICollection(t);
				if (icoll != null) {
					var a = icoll.GetGenericArguments();
					if (!IsCopyable(a[0])) {
						var ce = GetCloner(a[0]);
						if (meta.SerializeItemIf != null) {
							var m = CloneUtils.GetGeneric(nameof(CloneUtils.CloneCollectionIf), t, a[0]);
							var d = CloneUtils.MakeDelegate<
								Func<object, Func<object, object>, serializeItemIfType, object>>(m);
							return obj => d(obj, ce, meta.SerializeItemIf);
						}
						else {
							var m = CloneUtils.GetGeneric(nameof(CloneUtils.CloneCollection), t, a[0]);
							var d = CloneUtils.MakeDelegate<Func<object, Func<object, object>, object>>(m);
							return obj => d(obj, ce);
						}
					}
					else {
						if (meta.SerializeItemIf != null) {
							var m = CloneUtils.GetGeneric(
								nameof(CloneUtils.CloneCollectionPrimitiveIf), t, a[0]);
							var d = CloneUtils.MakeDelegate<Func<object, serializeItemIfType, object>>(m);
							return src => d(src, meta.SerializeItemIf);
						}
						else {
							var m = CloneUtils.GetGeneric(
								nameof(CloneUtils.CloneCollectionPrimitive), t, a[0]);
							return CloneUtils.MakeDelegate<Func<object, object>>(m);
						}
					}
				}
			}
			if (t.IsClass || t.IsInterface || Utils.IsStruct(t)) {
				var surrogateCloner = MakeSurrogateCloner(meta);
				if (surrogateCloner != null)
					return surrogateCloner;
				var oc = new ObjectCloner(this, meta);
				return oc.Get();
			}
			throw new NotImplementedException("Unable to clone type: " + t.FullName);
		}

		private Action<object, object> MakeMerger(Type t)
		{
			{
				var idict = Utils.GetIDictionary(t);
				if (idict != null) {
					var a = idict.GetGenericArguments();
					if (!IsCopyable(a[0])) {
						var ck = GetCloner(a[0]);
						var cv = GetCloner(a[1]);
						var m = CloneUtils.GetGeneric(nameof(CloneUtils.MergeIDictionary), t, a[0], a[1]);
						var d = CloneUtils.MakeDelegate<
							Action<object, object, Func<object, object>, Func<object, object>>>(m);
						return (dst, src) => d(dst, src, ck, cv);
					}
					else if (!IsCopyable(a[1])) {
						var cv = GetCloner(a[1]);
						var m = CloneUtils.GetGeneric(
							nameof(CloneUtils.MergeIDictionaryPrimiviteKey), t, a[0], a[1]);
						var d = CloneUtils.MakeDelegate<Action<object, object, Func<object, object>>>(m);
						return (dst, src) => d(dst, src, cv);
					}
					else {
						var m = CloneUtils.GetGeneric(
							nameof(CloneUtils.MergeIDictionaryPrimivite), t, a[0], a[1]);
						return CloneUtils.MakeDelegate<Action<object, object>>(m);
					}
				}
			}
			var meta = Meta.Get(t, Options);
			{
				var icoll = Utils.GetICollection(t);
				if (icoll != null) {
					var a = icoll.GetGenericArguments();
					if (!IsCopyable(a[0])) {
						var ce = GetCloner(a[0]);
						if (meta.SerializeItemIf != null) {
							var m = CloneUtils.GetGeneric(nameof(CloneUtils.MergeCollectionIf), t, a[0]);
							var d = CloneUtils.MakeDelegate<
								Action<object, object, Func<object, object>, serializeItemIfType>>(m);
							return (dst, src) => d(dst, src, ce, meta.SerializeItemIf);
						}
						else {
							var m = CloneUtils.GetGeneric(nameof(CloneUtils.MergeCollection), t, a[0]);
							var d = CloneUtils.MakeDelegate<Action<object, object, Func<object, object>>>(m);
							return (dst, src) => d(dst, src, ce);
						}
					}
					else {
						if (meta.SerializeItemIf != null) {
							var m = CloneUtils.GetGeneric(
								nameof(CloneUtils.MergeCollectionPrimitiveIf), t, a[0]);
							var d = CloneUtils.MakeDelegate<Action<object, object, serializeItemIfType>>(m);
							return (dst, src) => d(dst, src, meta.SerializeItemIf);
						}
						else {
							var m = CloneUtils.GetGeneric(
							nameof(CloneUtils.MergeCollectionPrimitive), t, a[0]);
							return CloneUtils.MakeDelegate<Action<object, object>>(m);
						}
					}
				}
			}
			if (t.IsClass || t.IsInterface || Utils.IsStruct(t)) {
				if (meta.Items.Count == 0)
					return (dst, src) => {};
				var om = new ObjectMerger(this, meta);
				return om.Get();
			}
			throw new NotImplementedException("Unable to merge type: " + t.FullName);
		}

		public override object DeepObject(object src) =>
			src == null ? null : GetCloner(src.GetType())(src);
		public override void MergeObject(object dst, object src) => GetMerger(src.GetType())(dst, src);
	}
}
