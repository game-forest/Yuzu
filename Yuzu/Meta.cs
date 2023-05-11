using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Yuzu.Util;

using AliasCacheType = System.Collections.Concurrent.ConcurrentDictionary<string, System.Type>;

namespace Yuzu.Metadata
{
	public abstract class GeneratedMeta
	{
		public abstract Yuzu.CommonOptions Options { get; set; }

		public abstract Dictionary<Type, Func<Yuzu.Metadata.Meta>> GetMetaMakers();
	}

	public class Meta
	{
		public struct TypeOptions : IEquatable<TypeOptions>
		{
			public Type Type;
			public CommonOptions Options;

			public bool Equals(TypeOptions other)
			{
				return Type == other.Type && Options.Equals(other.Options);
			}

			public override bool Equals(object obj)
			{
				return obj is TypeOptions other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked {
					return (Type.GetHashCode() * 397) ^ Options.GetHashCode();
				}
			}
		}

		private static readonly ConcurrentDictionary<TypeOptions, Meta> cache =
			new ConcurrentDictionary<TypeOptions, Meta>();
		public static ConcurrentDictionary<CommonOptions, AliasCacheType> ReadAliasCache =
			new ConcurrentDictionary<CommonOptions, AliasCacheType>();

		public class Item : IComparable<Item>
		{
			private string id;

			public string Name;
			public string Alias;
			public string Id
			{
				get
				{
					id ??= IdGenerator.GetNextId();

					return id;
				}
			}

			internal ItemAttrs Ia { get; set; }
			public MemberInfo Member { get; internal set; }

			public bool IsOptional;
			public bool IsCompact;
			public bool IsCopyable;
			public bool IsMember;
			public object DefaultValue;
			public Func<object, object, bool> SerializeCond;
			public MethodInfo SerializeIfMethod;
			public Type Type;
			public Func<object, object> GetValue;
			public Action<object, object> SetValue;
			public FieldInfo FieldInfo;
			public PropertyInfo PropertyInfo;

			public int CompareTo(Item yi) => string.CompareOrdinal(Alias, yi.Alias);

			public string Tag(CommonOptions options)
			{
				return options.TagMode switch {
					TagMode.Names => Name,
					TagMode.Aliases => Alias,
					TagMode.Ids => Id,
					_ => throw new YuzuAssert(),
				};
			}
			public string NameTagged(CommonOptions options)
			{
				var tag = Tag(options);
				return Name + (tag == Name ? string.Empty : " (" + tag + ")");
			}
		}

		public Type Type;
		public MetaOptions MetaOptions;
		public readonly List<Item> Items = new List<Item>();
		public bool IsCompact;
		public bool IsCopyable;
		public object Default { get; private set; }
		public YuzuItemKind Must = YuzuItemKind.None;
		public YuzuItemKind AllKind = YuzuItemKind.None;
		public YuzuItemOptionality AllOptionality = YuzuItemOptionality.None;
		public bool AllowReadingFromAncestor;
		public Surrogate Surrogate;
		public string WriteAlias;
		public int RequiredCount { get; set; }
		public Func<object, int, object, bool> SerializeItemIf;
		public MethodInfo SerializeItemIfMethod;

		public object DefaultFactory() => Activator.CreateInstance(Type);
		public MethodInfo FactoryMethod;
		public Func<object> Factory;

		public Dictionary<string, Item> TagToItem = new Dictionary<string, Item>();
		public Func<object, YuzuUnknownStorage> GetUnknownStorage;

		public ActionList BeforeSerialization = new ActionList();
		public ActionList AfterSerialization = new ActionList();
		public ActionList BeforeDeserialization = new ActionList();
		public ActionList AfterDeserialization = new ActionList();

#if !iOS // Apple forbids code generation.
		private static Action<object, object> SetterGenericHelper<TTarget, TParam>(MethodInfo m)
		{
			var action = (Action<TTarget, TParam>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam>), m);
			return (object target, object param) => action((TTarget)target, (TParam)param);
		}

		private static Func<object, object> GetterGenericHelper<TTarget, TReturn>(MethodInfo m)
		{
			var func = (Func<TTarget, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TReturn>), m);
			return (object target) => (object)func((TTarget)target);
		}

		private static Action<object, object> BuildSetter(MethodInfo m)
		{
			var helper = typeof(Meta)
				.GetMethod(nameof(SetterGenericHelper), BindingFlags.Static | BindingFlags.NonPublic)
				.MakeGenericMethod(m.DeclaringType, m.GetParameters()[0].ParameterType);
			return (Action<object, object>)helper.Invoke(null, new object[] { m });
		}

		private static Func<object, object> BuildGetter(MethodInfo m)
		{
			var helper = typeof(Meta)
				.GetMethod(nameof(GetterGenericHelper), BindingFlags.Static | BindingFlags.NonPublic)
				.MakeGenericMethod(m.DeclaringType, m.ReturnType);
			return (Func<object, object>)helper.Invoke(null, new object[] { m });
		}
#endif

		internal struct ItemAttrs
		{
			private readonly Attribute[] attrs;
			public Attribute Optional => attrs[0];
			public Attribute Required => attrs[1];
			public Attribute Member => attrs[2];
			public int Count;
			public Attribute Any() => Optional ?? Required ?? Member;

			public ItemAttrs(MemberInfo m, MetaOptions options, YuzuItemOptionality opt)
			{
				var attrTypes = new Type[] {
					options.OptionalAttribute,
					options.RequiredAttribute,
					options.MemberAttribute,
				};
				var over = options.GetItem(m);
				attrs = attrTypes.Select(t => over.Attr(t)).ToArray();
				Count = attrs.Count(a => a != null);
				if (Count == 0 && opt > 0 && attrTypes[(int)opt - 1] != null) {
					attrs[(int)opt - 1] = Activator.CreateInstance(attrTypes[(int)opt - 1]) as Attribute;
					Count = 1;
				}
			}
		}

		private bool IsNonEmptyCollection<T>(object obj, object value)
		{
			return value == null || ((ICollection<T>)value).Any();
		}

		private static bool IsNonEmptyCollectionConditional(object obj, object value, Meta collMeta)
		{
			if (value == null) {
				return false;
			}

			int index = 0;
			// Use non-generic IEnumerable to avoid boxing/unboxing.
			foreach (var i in (IEnumerable)value) {
				if (collMeta.SerializeItemIf(value, index++, i)) {
					return true;
				}
			}

			return false;
		}

		private bool IsEqualCollections<T>(object value, IEnumerable defColl)
		{
			return !Enumerable.SequenceEqual((IEnumerable<T>)value, (IEnumerable<T>)defColl);
		}

		public Func<object, object, bool> GetSerializeIf(Item item, CommonOptions options)
		{
			Default ??= Factory();

			var d = item.GetValue(Default);
			var icoll = Utils.GetICollection(item.Type);
			if (d == null || icoll == null) {
				return (object obj, object value) => !object.Equals(value, d);
			}

			var defColl = (IEnumerable)d;
			var collMeta = Get(item.Type, options);
			bool checkForEmpty = options.CheckForEmptyCollections && collMeta.SerializeItemIf != null;
			if (
				defColl.GetEnumerator().MoveNext() &&
				(!checkForEmpty || IsNonEmptyCollectionConditional(Default, defColl, collMeta))
			) {
				var m = Utils.GetPrivateCovariantGeneric(GetType(), nameof(IsEqualCollections), icoll);
				var eq = (Func<object, IEnumerable, bool>)Delegate.CreateDelegate(
					typeof(Func<object, IEnumerable, bool>), this, m
				);
				return (object obj, object value) => eq(value, defColl);
			}
			if (checkForEmpty) {
				return (object obj, object value) => IsNonEmptyCollectionConditional(obj, value, collMeta);
			}

			var mi = Utils.GetPrivateGeneric(
				GetType(), nameof(IsNonEmptyCollection), icoll.GetGenericArguments()[0]
			);
			return (Func<object, object, bool>)Delegate.CreateDelegate(typeof(Func<object, object, bool>), this, mi);
		}

		private void CheckCopyable(Type itemType, CommonOptions options)
		{
			var isCopyable = Utils.IsCopyable(itemType);
			if (isCopyable.HasValue) {
				if (!isCopyable.Value) {
					IsCopyable = false;
				}
			} else {
				if (Utils.IsStruct(itemType)) {
					var meta = Get(itemType, options);
					if (!meta.IsCopyable) {
						IsCopyable = false;
					}
				} else {
					IsCopyable = false;
				}
			}
		}

		private void AddItem(MemberInfo m, CommonOptions options, bool must, bool all)
		{
			var ia = new ItemAttrs(m, this.MetaOptions, all ? AllOptionality : YuzuItemOptionality.None);
			if (ia.Count == 0) {
				if (must) {
					throw Error("Item {0} must be serialized", m.Name);
				}

				return;
			}
			if (ia.Count != 1) {
				throw Error("More than one of optional, required and member attributes for field '{0}'", m.Name);
			}

			var attrs = this.MetaOptions.GetItem(m);
			var serializeCond = attrs.Attr(this.MetaOptions.SerializeConditionAttribute);
			var item = new Item {
				Alias = this.MetaOptions.GetAlias(ia.Any()) ?? m.Name,
				IsOptional = ia.Required == null,
				IsCompact = attrs.HasAttr(this.MetaOptions.CompactAttribute),
				IsCopyable = attrs.HasAttr(this.MetaOptions.CopyableAttribute),
				IsMember = ia.Member != null,
				SerializeCond = serializeCond != null
					? this.MetaOptions.GetSerializeCondition(serializeCond, Type)
					: null,
				SerializeIfMethod = serializeCond != null
					? this.MetaOptions.GetSerializeMethod(serializeCond, Type)
					: null,
				DefaultValue = serializeCond != null
					? this.MetaOptions.GetDefault(serializeCond)
					: YuzuNoDefault.NoDefault,
				Name = m.Name,
			};
			item.Ia = ia;
			item.Member = m;
			if (!item.IsOptional) {
				RequiredCount += 1;
			}

			var merge = attrs.HasAttr(this.MetaOptions.MergeAttribute);

			switch (m.MemberType) {
				case MemberTypes.Field:
					var f = m as FieldInfo;
					if (!f.IsPublic) {
						throw Error("Non-public item '{0}'", f.Name);
					}

					item.Type = f.FieldType;
					item.GetValue = f.GetValue;
					if (!merge) {
						item.SetValue = f.SetValue;
					}

					item.FieldInfo = f;
					break;
				case MemberTypes.Property:
					var p = m as PropertyInfo;
					var getter = p.GetGetMethod();
					if (getter == null) {
						throw Error("No getter for item '{0}'", p.Name);
					}

					item.Type = p.PropertyType;
					var setter = p.GetSetMethod();
#if iOS // Apple forbids code generation.
					item.GetValue = obj => p.GetValue(obj, Utils.ZeroObjects);
					if (!merge && setter != null) {
						item.SetValue = (obj, value) => p.SetValue(obj, value, Utils.ZeroObjects);
					}
#else
					if (Utils.IsStruct(Type)) {
						item.GetValue = obj => p.GetValue(obj, Utils.ZeroObjects);
						if (!merge && setter != null) {
							item.SetValue = (obj, value) => p.SetValue(obj, value, Utils.ZeroObjects);
						}
					} else {
						item.GetValue = BuildGetter(getter);
						if (!merge && setter != null) {
							item.SetValue = BuildSetter(setter);
						}
					}
#endif
					item.PropertyInfo = p;
					break;
				default:
					throw Error("Member type {0} not supported", m.MemberType);
			}
			if (item.SetValue == null) {
				if (!item.Type.IsClass && !item.Type.IsInterface || item.Type == typeof(object)) {
					throw Error("Unable to either set or merge item {0}", item.Name);
				}
			}
			var over = this.MetaOptions.GetOverride(item.Type);
			if (over.HasAttr(this.MetaOptions.CompactAttribute)) {
				item.IsCompact = true;
			}

			if (!over.HasAttr(this.MetaOptions.CopyableAttribute)) {
				CheckCopyable(item.Type, options);
			}

			if (ia.Member != null && item.SerializeCond == null && !Type.IsAbstract && !Type.IsInterface) {
				item.SerializeCond = GetSerializeIf(item, options);
			}

			Items.Add(item);
		}

		private void AddMethod(MethodInfo m)
		{
			var attrs = MetaOptions.GetItem(m);
			if (attrs.HasAttr(MetaOptions.SerializeItemIfAttribute)) {
				if (SerializeItemIf != null) {
					throw Error("Duplicate SerializeItemIf");
				}

				if (Utils.GetIEnumerable(Type) == null) {
					throw Error("SerializeItemIf may only be used inside of IEnumerable");
				}

				SerializeItemIf = MetaOptions.GetSerializeItemCondition(m);
				SerializeItemIfMethod = m;
			}
			BeforeSerialization.MaybeAdd(m, MetaOptions.BeforeSerializationAttribute);
			AfterSerialization.MaybeAdd(m, MetaOptions.AfterSerializationAttribute);
			BeforeDeserialization.MaybeAdd(m, MetaOptions.BeforeDeserializationAttribute);
			AfterDeserialization.MaybeAdd(m, MetaOptions.AfterDeserializationAttribute);

			if (attrs.HasAttr(MetaOptions.FactoryAttribute)) {
				if (FactoryMethod != null) {
					throw Error("Duplicate Factory: '{0}' and '{1}'", FactoryMethod.Name, m.Name);
				}

				if (!m.IsStatic || m.GetParameters().Length > 0) {
					throw Error("Factory '{0}' must be a static method without parameters", m.Name);
				}

				FactoryMethod = m;
				Factory = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>), m);
			}

			Surrogate.ProcessMethod(m);
		}

		public void ExploreType(Type t, CommonOptions options)
		{
			const BindingFlags bindingFlags =
				BindingFlags.Static
				| BindingFlags.Instance
				| BindingFlags.Public
				| BindingFlags.NonPublic
				| BindingFlags.FlattenHierarchy;
			foreach (var m in t.GetMembers(bindingFlags)) {
				var attrs = this.MetaOptions.GetItem(m);
				if (attrs.HasAttr(this.MetaOptions.ExcludeAttribute)) {
					continue;
				}

				switch (m.MemberType) {
					case MemberTypes.Field:
						var f = m as FieldInfo;
						if (f.FieldType == typeof(YuzuUnknownStorage)) {
							if (GetUnknownStorage != null) {
								throw Error("Duplicated unknown storage in field {0}", m.Name);
							}

							GetUnknownStorage = obj => (YuzuUnknownStorage)f.GetValue(obj);
						} else {
							AddItem(
								m: m,
								options: options,
								must: Must.HasFlag(YuzuItemKind.Field) && f.IsPublic,
								all: AllKind.HasFlag(YuzuItemKind.Field) && f.IsPublic
							);
						}

						break;
					case MemberTypes.Property:
						var p = m as PropertyInfo;
						var g = p.GetGetMethod();
						if (p.PropertyType == typeof(YuzuUnknownStorage)) {
							if (GetUnknownStorage != null) {
								throw Error("Duplicated unknown storage in field {0}", m.Name);
							}
#if iOS // Apple forbids code generation.
							GetUnknownStorage = obj => (YuzuUnknownStorage)p.GetValue(obj, Utils.ZeroObjects);
#else
							var getter = BuildGetter(g);
							GetUnknownStorage = obj => (YuzuUnknownStorage)getter(obj);
#endif
						} else {
							AddItem(
								m: m,
								options: options,
								must: Must.HasFlag(YuzuItemKind.Property) && g != null,
								all: AllKind.HasFlag(YuzuItemKind.Property) && g != null
							);
						}

						break;
					case MemberTypes.Method:
						AddMethod(m as MethodInfo);
						break;
				}
			}
		}

		private Meta(Type t)
		{
			Type = t;
			MetaOptions = MetaOptions.Default;
		}

		public static readonly Func<CommonOptions, AliasCacheType> MakeReadAliases =
			commonOptions => new AliasCacheType();

		public void CheckForNoFields(CommonOptions options)
		{
			if (Surrogate.SurrogateType != null || Type.IsArray) {
				return;
			}
			if (Utils.GetIEnumerable(Type) != null) {
				if (Items.Count > 0) {
					throw Error("Serializable fields in collection are not supported");
				}
			} else if (
				!options.AllowEmptyTypes && Items.Count == 0 && !(Type.IsInterface || Type.IsAbstract)
			) {
				throw Error("No serializable fields");
			}
		}

		public bool HasAnyTrigger()
		{
			return BeforeSerialization.Actions.Any()
				|| AfterSerialization.Actions.Any()
				|| BeforeDeserialization.Actions.Any()
				|| AfterDeserialization.Actions.Any();
		}

		public Meta()
		{
		}

		public Meta(Type t, CommonOptions options)
		{
			Type = t;
			Factory = DefaultFactory;
			this.MetaOptions = options.Meta ?? MetaOptions.Default;
			IsCopyable = Utils.IsStruct(t);
			var over = this.MetaOptions.GetOverride(t);
			IsCompact = over.HasAttr(this.MetaOptions.CompactAttribute);
			var must = over.Attr(this.MetaOptions.MustAttribute);
			if (must != null) {
				Must = this.MetaOptions.GetItemKind(must);
			}
			var all = over.Attr(this.MetaOptions.AllAttribute);
			if (all != null) {
				var ok = this.MetaOptions.GetItemOptionalityAndKind(all);
				AllOptionality = ok.Item1;
				AllKind = ok.Item2;
			}

			Surrogate = new Surrogate(Type, this.MetaOptions);
			foreach (var i in t.GetInterfaces()) {
				ExploreType(i, options);
			}
			ExploreType(t, options);
			Surrogate.Complete();
			CheckForNoFields(options);

			Items.Sort();
			Item prev = null;
			foreach (var i in Items) {
				if (prev != null && prev.CompareTo(i) == 0) {
					throw Error("Duplicate item {0} / {1}", i.Name, i.Alias);
				}
				prev = i;
			}
			var prevTag = string.Empty;
			foreach (var i in Items) {
				var tag = i.Tag(options);
				if (tag == string.Empty) {
					throw Error("Empty tag for field '{0}'", i.Name);
				}
				foreach (var ch in tag) {
					if (ch <= ' ' || ch >= 127) {
						throw Error("Bad character '{0}' in tag for field '{1}'", ch, i.Name);
					}
				}
				if (tag == prevTag) {
					throw Error("Duplicate tag '{0}' for field '{1}'", tag, i.Name);
				}
				prevTag = tag;
				TagToItem.Add(tag, i);
			}

			AllowReadingFromAncestor = over.HasAttr(this.MetaOptions.AllowReadingFromAncestorAttribute);
			if (AllowReadingFromAncestor) {
				var ancestorMeta = Get(t.BaseType, options);
				if (ancestorMeta.Items.Count != Items.Count) {
					throw Error(
						"Allows reading from ancestor {0}, but has {1} items instead of {2}",
						t.BaseType.Name,
						Items.Count,
						ancestorMeta.Items.Count
					);
				}
			}

			var alias = over.Attr(this.MetaOptions.AliasAttribute);
			if (alias != null) {
				var aliases = this.MetaOptions.GetReadAliases(alias);
				if (aliases != null) {
					AliasCacheType readAliases = ReadAliasCache.GetOrAdd(options, MakeReadAliases);
					foreach (var a in aliases) {
						if (string.IsNullOrWhiteSpace(a)) {
							throw Error("Empty read alias");
						}
						if (readAliases.TryGetValue(a, out Type duplicate)) {
							throw Error("Read alias '{0}' was already defined for '{1}'", a, duplicate.Name);
						}
						readAliases.TryAdd(a, t);
					}
				}
				WriteAlias = this.MetaOptions.GetWriteAlias(alias);
				if (WriteAlias != null && WriteAlias == string.Empty) {
					throw Error("Empty write alias");
				}
			}

			if (over.HasAttr(this.MetaOptions.CopyableAttribute)) {
				IsCopyable = true;
			} else if (HasAnyTrigger()) {
				IsCopyable = false;
			}
		}

		private static readonly Func<TypeOptions, Meta> makeMeta = key => {
			if (generatedMetaCache.TryGetValue(key.Options, out var makeCache)) {
				if (makeCache.TryGetValue(key.Type, out var metaMaker)) {
					var sw = System.Diagnostics.Stopwatch.StartNew();
					var r = metaMaker();
					sw.Stop();
					System.Console.WriteLine(
						$"[new_gen_meta] " +
						$"'{Utils.GetTypeSpec(key.Type)}' {sw.Elapsed.TotalMilliseconds} ms"
					);
					return r;
				}
			}
			{
				var sw = System.Diagnostics.Stopwatch.StartNew();
				var r = new Meta(key.Type, key.Options);
				sw.Stop();
				System.Console.WriteLine(
					$"[new_meta] " +
					$"'{Utils.GetTypeSpec(key.Type)}' {sw.Elapsed.TotalMilliseconds} ms"
				);
				return r;
			}
		};

		private static volatile int msInGet = 0;
		public static Meta Get(Type t, CommonOptions options)
		{
			return cache.GetOrAdd(new TypeOptions { Type = t, Options = options, }, makeMeta);
		}

		public static Type GetTypeByReadAlias(string alias, CommonOptions options)
		{
			if (!ReadAliasCache.TryGetValue(options, out AliasCacheType readAliases)) {
				return null;
			}

			return readAliases.TryGetValue(alias, out Type result) ? result : null;
		}

		internal static Meta Unknown = new Meta(typeof(YuzuUnknown));

		public YuzuException Error(string format, params object[] args)
		{
			return new YuzuException("In type '" + Type.FullName + "': " + string.Format(format, args));
		}

		private static bool HasItems(Type t, MetaOptions options)
		{
			const BindingFlags BindingFlags =
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
			var over = options.GetOverride(t);
			var all = over.Attr(options.AllAttribute);
			var k = all != null ? options.GetItemOptionalityAndKind(all).Item2 : YuzuItemKind.None;
			foreach (var m in t.GetMembers(BindingFlags)) {
				var attrs = over.Item(m);
				if (
					m.MemberType != MemberTypes.Field && m.MemberType != MemberTypes.Property
					|| attrs.HasAttr(options.ExcludeAttribute)
				) {
					continue;
				}

				if (
					k.HasFlag(YuzuItemKind.Field) && m.MemberType == MemberTypes.Field
					|| k.HasFlag(YuzuItemKind.Property) && m.MemberType == MemberTypes.Property
					|| new ItemAttrs(m, options, YuzuItemOptionality.None).Any() != null
				) {
					return true;
				}
			}
			return false;
		}

		public static List<Type> Collect(Assembly assembly, MetaOptions options = null)
		{
			var result = new List<Type>();
			var q = new Queue<Type>(assembly.GetTypes());
			while (q.Count > 0) {
				var t = q.Dequeue();
				if (HasItems(t, options ?? MetaOptions.Default) && !t.IsGenericTypeDefinition) {
					result.Add(t);
				}
				foreach (var nt in t.GetNestedTypes()) {
					q.Enqueue(nt);
				}
			}
			return result;
		}

		public const int FoundNonPrimitive = -1;
		public int CountPrimitiveChildren(CommonOptions options)
		{
			int result = 0;
			foreach (var yi in Items) {
				if (yi.Type.IsPrimitive || yi.Type.IsEnum || yi.Type == typeof(string)) {
					result += 1;
				} else if (yi.IsCompact) {
					var c = Get(yi.Type, options).CountPrimitiveChildren(options);
					if (c == FoundNonPrimitive) {
						return FoundNonPrimitive;
					}
					result += c;
				} else {
					return FoundNonPrimitive;
				}
			}
			return result;
		}

		public static void Generate(string path, List<Type> types, CommonOptions options)
		{
			using var fs = new FileStream(
				path, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.None
			);
			using var w = new StreamWriter(fs) { NewLine = "\n" };
			int indent = 0;
			P("//------------------------------------------------------------------------------");
			P("// <auto-generated>");
			P("// Yuzu generated meta.");
			P("// </auto-generated>");
			P("//------------------------------------------------------------------------------");
			P("using Yuzu;");
			P("using Lime;");
			P("using System.Collections.Generic;");
			P("using System;");
			P("using Yuzu.Metadata;");
			P("using System.Reflection;");
			P("using System.Linq;");
			P("using System.Collections;");
			P(string.Empty);
			P();
			P("namespace YuzuGenerated");
			P("{");
			indent++;
			P("public class Meta : GeneratedMeta");
			P("{");
			indent++;
			P("private static Yuzu.CommonOptions options;");
			P();
			P("private const BindingFlags bindingFlags =");
			PP("BindingFlags.Static");
			PP("| BindingFlags.Instance");
			PP("| BindingFlags.Public");
			PP("| BindingFlags.NonPublic");
			PP("| BindingFlags.FlattenHierarchy;");
			P();
			P(
				"private static Dictionary<Type, Func<Yuzu.Metadata.Meta>> makeCache = " +
				"new Dictionary<Type, Func<Yuzu.Metadata.Meta>>();"
			);
			P();
			P("public override Yuzu.CommonOptions Options");
			P("{");
			PP("get");
			PP("{");
			PPP("return options;");
			PP("}");
			PP("set");
			PP("{");
			PPP("options = value;");
			PP("}");
			P("}");
			P(string.Empty);
			P("public override Dictionary<Type, Func<Yuzu.Metadata.Meta>> GetMetaMakers()");
			P("{");
			PP("return makeCache;");
			P("}");

			foreach (var t in types) {
				var name = Utils.GetMangledTypeNameNS(t);
				var meta = Meta.Get(t, options);

				P($"public static Yuzu.Metadata.Meta Make_{name}()");
				P("{");
				indent++;
				P($"var t = typeof({Utils.GetTypeSpec(t)});");
				P($"var meta = new Yuzu.Metadata.Meta();");
				P($"meta.Type = t;");
				P($"meta.Factory = meta.DefaultFactory;");
				P($"var metaOptions = meta.MetaOptions = options.Meta ?? MetaOptions.Default;");
				if (meta.IsCopyable) {
					P($"meta.IsCopyable = {A(meta.IsCopyable)};");
				}
				if (meta.IsCompact) {
					P($"meta.IsCompact = {A(meta.IsCompact)};");
				}
				if (meta.Must != YuzuItemKind.None) {
					P($"meta.Must = {A(meta.Must)};");
				}
				if (meta.AllOptionality != YuzuItemOptionality.None) {
					P($"meta.AllOptionality = {A(meta.AllOptionality)};");
				}
				if (meta.AllKind != YuzuItemKind.None) {
					P($"meta.AllKind = {A(meta.AllKind)};");
				}
				if (meta.AfterDeserialization.Actions.Count > 0) {
					foreach (var a in meta.AfterDeserialization.Actions) {
						P($"meta.AfterDeserialization.Actions.Add(new Yuzu.Util.ActionList.MethodAction {{");
						PP($"Info = typeof({A(a.Info.DeclaringType)}).GetMethod({A(a.Info.Name)}),");
						PP($"Run = o => (({A(a.Info.DeclaringType)})o).{a.Info.Name}(),");
						P("});");
					}
				}
				if (meta.BeforeDeserialization.Actions.Count > 0) {
					foreach (var a in meta.BeforeDeserialization.Actions) {
						P($"meta.BeforeDeserialization.Actions.Add(new Yuzu.Util.ActionList.MethodAction {{");
						PP($"Info = typeof({A(a.Info.DeclaringType)}).GetMethod({A(a.Info.Name)}),");
						PP($"Run = o => (({A(a.Info.DeclaringType)})o).{a.Info.Name}(),");
						P("});");
					}
				}
				if (meta.AfterSerialization.Actions.Count > 0) {
					foreach (var a in meta.AfterSerialization.Actions) {
						P($"meta.AfterSerialization.Actions.Add(new Yuzu.Util.ActionList.MethodAction {{");
						PP($"Info = typeof({A(a.Info.DeclaringType)}).GetMethod({A(a.Info.Name)}),");
						PP($"Run = o => (({A(a.Info.DeclaringType)})o).{a.Info.Name}(),");
						P("});");
					}
				}
				if (meta.BeforeSerialization.Actions.Count > 0) {
					foreach (var a in meta.BeforeSerialization.Actions) {
						P($"meta.BeforeSerialization.Actions.Add(new Yuzu.Util.ActionList.MethodAction {{");
						PP($"Info = typeof({A(a.Info.DeclaringType)}).GetMethod({A(a.Info.Name)}),");
						PP($"Run = o => (({A(a.Info.DeclaringType)})o).{a.Info.Name}(),");
						P("});");
					}
				}
				if (/*meta.Factory != null || */meta.FactoryMethod != null) {
					throw new NotSupportedException("Factory not supported in meta gen.");
				}
				if (meta.SerializeItemIfMethod != null) {
					PP($"SerializeItemIfMethod = typeof({A(t)}).GetMethod{meta.SerializeItemIfMethod.Name},");
				}
				if (meta.SerializeItemIf != null) {
					PP(
						$"{meta}.SerializeItemIf = " +
						$"MetaOptions.GetSerializeItemCondition(meta.SerializeItemIfMethod);"
					);
				}
				int itemIndex = 0;
				foreach (var i in meta.Items) {
					var itemName = $"item{itemIndex}";
					var itemDeclaringType = i.Member.DeclaringType;
					P($"var {itemName} = new Yuzu.Metadata.Meta.Item {{");
					PP($"Alias = {A(i.Alias)},");
					if (i.IsOptional) {
						PP($"IsOptional = {A(i.IsOptional)},");
					}
					if (i.IsCompact) {
						PP($"IsCompact = {A(i.IsCompact)},");
					}
					if (i.IsCopyable) {
						PP($"IsCopyable = {A(i.IsCopyable)},");
					}
					if (i.IsMember) {
						PP($"IsMember = {A(i.IsMember)},");
					}
					if (!Equals(i.DefaultValue, YuzuNoDefault.NoDefault)) {
						var o = i.DefaultValue;
						var ot = o.GetType();
						string dvs = string.Empty;
						if (ot.IsPrimitive) {
							dvs = o.ToString();
						} else {
							if (Utils.IsStruct(ot)) {
								dvs = ot.FullName;
							} else {
								dvs = ot.FullName;
							}
						}
						PP($"DefaultValue = {dvs},");
					}

					PP($"Name = {A(i.Name)},");
					P("};");

					var merge = meta.MetaOptions.GetItem((MemberInfo)i.FieldInfo ?? i.PropertyInfo)
						.HasAttr(meta.MetaOptions.MergeAttribute);
					if (i.FieldInfo != null) {
						var f = i.FieldInfo;
						// Why do we even need FieldInfo at runtime?
						P(
							$"{itemName}.FieldInfo = " +
							$"typeof({A(itemDeclaringType)}).GetField({A(i.Name)}, bindingFlags);"
						);
						P($"{itemName}.Type = typeof({Utils.GetTypeSpec(i.FieldInfo.FieldType)});");
						P($"{itemName}.GetValue = (o) => (({A(itemDeclaringType)})o).{i.Name};");
						if (!merge) {
							P($"{itemName}.SetValue = (o, v) => {itemName}.FieldInfo.SetValueDirect(__makeref(o), v);");
							////P($"{itemName}.SetValue = (o, v) => {{");
							////PP($"var t = ({A(t)})o;");
							////PP($"t.{i.Name} = ({A(i.FieldInfo.FieldType)})v;");
							////PP($"{itemName}.FieldInfo.SetValue(o, v);");
							////P($"}};");
						}
					} else if (i.PropertyInfo != null) {
						var p = i.PropertyInfo;
						P(
							$"{itemName}.PropertyInfo = " +
							$"typeof({A(itemDeclaringType)}).GetProperty({A(i.Name)}, bindingFlags);"
						);
						P($"{itemName}.Type = typeof({Utils.GetTypeSpec(p.PropertyType)});");
						var setter = p.GetSetMethod();
						if (Utils.IsStruct(t)) {
							P(
								$"{itemName}.GetValue = o => {itemName}.PropertyInfo" +
								$".GetValue(o, Array.Empty<object>());"
							);
							if (!merge && setter != null) {
								P(
									$"{itemName}.SetValue = (o, v) => {itemName}.PropertyInfo"
									+ $".SetValue(o, v, Array.Empty<object>());"
								);
							}
						} else {
							P($"{itemName}.GetValue = (o) => (({A(itemDeclaringType)})o).{i.Name};");
							if (!merge && setter != null) {
								P(
									$"{itemName}.SetValue = " +
									$"(o, v) => (({A(itemDeclaringType)})o).{i.Name} = ({A(p.PropertyType)})v;"
								);
							}
						}
					}

					var attrs = meta.MetaOptions.GetItem(i.Member);
					bool serializeCondSet = false;
					var serializeCond = attrs.Attr(meta.MetaOptions.SerializeConditionAttribute);
					if (serializeCond != null) {
						if (serializeCond is YuzuSerializeIf ysif) {
							P(
								$"{itemName}.SerializeIfMethod " +
								$"= typeof({A(itemDeclaringType)}).GetMethod({A(ysif.Method)}, bindingFlags);"
							);
							P(
								$"{itemName}.SerializeCond = (o, v) => " +
								$"(({A(itemDeclaringType)})o).{ysif.Method}();"
							);
							serializeCondSet = true;
							P($"{itemName}.DefaultValue = YuzuNoDefault.NoDefault;");
						} else if (serializeCond is YuzuDefault yd) {
							throw new NotSupportedException("YuzuDefault not supported in meta gen.");
						} else {
							throw new NotSupportedException(
								$"Unknown SerializeConditionAttribute '{serializeCond.GetType()}'."
							);
						}
					}
					if (i.Ia.Member != null && !serializeCondSet && !t.IsAbstract && !t.IsInterface) {
						var cs = new Yuzu.Code.CodeConstructSerializer {
							CodeConstructOptions = new Code.CodeConstructSerializeOptions {
								IndentLevel = indent,
							},
							Options = options,
						};
						string condText = null;
						var defaultValue = i.GetValue(meta.Default);
						var dt = defaultValue?.GetType();
						var icoll = Utils.GetICollection(i.Type);
						if (defaultValue == null || icoll == null) {
								condText = $"(o, v) => !object.Equals(v, {cs.ToString(defaultValue)[8..^2]});";
						} else {
							var istr = new string('\t', indent);
							var defColl = (IEnumerable)defaultValue;
							var collMeta = Get(i.Type, options);
							bool checkForEmpty = options.CheckForEmptyCollections && collMeta.SerializeItemIf != null;
							if (
								defColl.GetEnumerator().MoveNext()
								&& (!checkForEmpty || IsNonEmptyCollectionConditional(meta.Default, defColl, collMeta))
							) {
								condText = $"(o, v) => {{\n" +
									$"{istr}\tvar defColl = \n{cs.ToString(defaultValue)[8..^2]};\n" +
									$"{istr}\treturn!Enumerable.SequenceEqual(({A(i.Type)})v, defColl);" +
								$"{istr}}};";
							} else {
								if (checkForEmpty) {
									condText = $"(o, v) => {{\n" +
										$"{istr}\tif(v == null) return false;\n" +
										$"{istr}\tint index = 0;\n" +
										// Use non-generic IEnumerable to avoid boxing/unboxing.
										$"{istr}\tforeach (var i in (IEnumerable)v) {{\n" +
											$"{istr}\t\tif (\n" +
											$"{istr}\t\t(({A(collMeta.Type)})v).\n" +
											$"{istr}\t\t\t{collMeta.SerializeItemIfMethod.Name}(index++, i)\n" +
											$"{istr}\t\t) {{\n" +
												$"{istr}\t\t\treturn true;\n" +
											$"{istr}\t\t}}\n" +
										$"{istr}\t}}\n" +
										$"{istr}\treturn false;\n" +
									$"{istr}}};";
								} else {
									var gt = icoll.GetGenericArguments()[0];
									condText = $"(o, v) => v == null || ((ICollection<{A(gt)}>)v).Any();";
								}
							}
						}
						if (string.IsNullOrEmpty(condText)) {
							throw new NotSupportedException("Serialize condition not supported in meta gen.");
						}
						P($"{itemName}.SerializeCond = {condText}");
					}

					P($"meta.Items.Add({itemName});");
					itemIndex++;
				}
				P("foreach (var i in meta.Items) {");
				PP("var tag = i.Tag(options);");
				PP("meta.TagToItem.Add(tag, i);");
				P("}");
				if (meta.AllowReadingFromAncestor) {
					P($"meta.AllowReadingFromAncestor = {A(meta.AllowReadingFromAncestor)};");
				}
				var over = meta.MetaOptions.GetOverride(t);
				var alias = over.Attr(meta.MetaOptions.AliasAttribute);
				if (alias != null) {
					var aliases = meta.MetaOptions.GetReadAliases(alias);
					if (aliases != null) {
						P($"AliasCacheType readAliases = Yuzu.Metadata.Meta.ReadAliasCache");
						PP($".GetOrAdd(options, Meta.MakeReadAliases);");
						AliasCacheType readAliases = Yuzu.Metadata.Meta.ReadAliasCache
							.GetOrAdd(options, Meta.MakeReadAliases);
						foreach (var a in aliases) {
							P($"readAliases.TryAdd({A(a)}, t);");
						}
					}
				}
				if (!string.IsNullOrEmpty(meta.WriteAlias)) {
					P($"meta.WriteAlias = {A(meta.WriteAlias)};");
				}
				if (meta.RequiredCount != 0) {
					P($"meta.RequiredCount = {A(meta.RequiredCount)};");
				}
				P($"meta.Surrogate = new Surrogate(meta.Type, meta.MetaOptions);");
				P($"meta.Surrogate.Complete();");
				P($"return meta;");
				indent--;
				P("}");
			}
			P("public Meta() { }");
			P("static Meta()");
			P("{");
			indent++;
			foreach (var t in types) {
				var name = Utils.GetMangledTypeNameNS(t);
				P($"makeCache[typeof({Utils.GetTypeSpec(t)})] = Make_{name};");
			}
			indent--;
			P("}");
			indent--;
			P("}");
			indent--;
			P("}");

			void P(string text = null)
			{
				if (text == null) {
					w.WriteLine();
					return;
				}
				w.WriteLine($"{new string('\t', indent)}{text}");
			}

			void PP(string text)
			{
				indent++;
				P(text);
				indent--;
			}

			void PPP(string text)
			{
				indent += 2;
				P(text);
				indent -= 2;
			}
			void PPPP(string text)
			{
				indent += 3;
				P(text);
				indent -= 3;
			}

			string A(object @object)
			{
				if (@object == null) {
					return "null";
				} else if (@object is string) {
					return $"\"{@object}\"";
				} else if (@object is bool) {
					return @object.ToString().ToLower();
				} else if (@object is System.Type) {
					return Utils.GetTypeSpec((Type)@object);
				} else if (@object is Enum) {
					return $"{Utils.GetTypeSpec(@object.GetType())}.{@object}";
				} else {
					return @object.ToString();
				}
			}
		}

		public static void InjectMetaCache(Yuzu.Metadata.GeneratedMeta meta)
		{
			generatedMetaCache[meta.Options] = meta.GetMetaMakers();
		}

		private static Dictionary<CommonOptions, Dictionary<Type, Func<Meta>>> generatedMetaCache =
			new Dictionary<CommonOptions, Dictionary<Type, Func<Meta>>>();
	}
}
