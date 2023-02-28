using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Yuzu.Binary;

namespace Yuzu.Util
{
	public static class Utils
	{
		public static object[] ZeroObjects = new object[] { };

		public static string QuoteCSharpStringLiteral(string s)
		{
			return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\t", "\\t");
		}

		public static string CodeValueFormat(object value)
		{
			if (value == null) {
				return string.Empty;
			}

			var t = value.GetType();
			if (t == typeof(int) || t == typeof(uint) || t == typeof(float) || t == typeof(double)) {
				return value.ToString();
			}

			if (t == typeof(bool)) {
				return value.ToString().ToLower();
			}

			if (t == typeof(string)) {
				return '"' + QuoteCSharpStringLiteral(value.ToString()) + '"';
			}

			if (t.IsEnum) {
				return t.Name + "." + value.ToString();
			}

			return string.Empty;
			//throw new NotImplementedException();
		}

		public static bool IsStruct(Type t)
		{
			return t.IsValueType && !t.IsPrimitive && !t.IsEnum && !t.IsPointer;
		}

		public static bool? IsCopyable(Type t)
		{
			return t.IsPrimitive || t.IsEnum || t == typeof(string)
				? true
				: t.Namespace == "System"
					? t.IsValueType
					: t.IsClass || t.IsValueType
						? null
						: (bool?)false;
		}

		public static Type GetICollection(Type t)
		{
			if (t.Name == "ICollection`1") {
				return t;
			}

			try {
				return t.GetInterface("ICollection`1");
			} catch (AmbiguousMatchException) {
				throw new YuzuException("Multiple ICollection interfaces for type " + t.Name);
			}
		}

		public static Type GetIEnumerable(Type t)
		{
			if (t.Name == "IEnumerable`1") {
				return t;
			}

			try {
				return t.GetInterface("IEnumerable`1");
			} catch (AmbiguousMatchException) {
				throw new YuzuException("Multiple IEnumerable interfaces for type " + t.Name);
			}
		}

		public static Type GetICollectionNG(Type t)
		{
			try {
				return t.GetInterface("ICollection");
			} catch (AmbiguousMatchException) {
				throw new YuzuException("Multiple ICollection interfaces for type " + t.Name);
			}
		}

		public static Type GetIDictionary(Type t)
		{
			if (t.Name == "IDictionary`2") {
				return t;
			}

			try {
				return t.GetInterface("IDictionary`2");
			} catch (AmbiguousMatchException) {
				throw new YuzuException("Multiple IDictionary interfaces for type " + t.Name);
			}
		}

		public static MethodInfo GetPrivateGeneric(
			Type callerType, string name, params Type[] parameters
		) {
			return callerType
				.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)
				.MakeGenericMethod(parameters);
		}

		public static MethodInfo GetPrivateGenericStatic(
			Type callerType, string name, params Type[] parameters
		) {
			return callerType
				.GetMethod(
					name,
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy
				).MakeGenericMethod(parameters);
		}

		public static MethodInfo GetPrivateCovariantGeneric(Type callerType, string name, Type container)
		{
			var t = container.HasElementType ? container.GetElementType() : container.GetGenericArguments()[0];
			return callerType.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(t);
		}

		public static MethodInfo GetPrivateCovariantGenericStatic(Type callerType, string name, Type container)
		{
			var t = container.HasElementType ? container.GetElementType() : container.GetGenericArguments()[0];
			return callerType.GetMethod(
				name,
				BindingFlags.Instance
					| BindingFlags.NonPublic
					| BindingFlags.Static
					| BindingFlags.FlattenHierarchy
			).MakeGenericMethod(t);
		}

		public static MethodInfo GetPrivateCovariantGenericAll(Type callerType, string name, Type container)
		{
			return callerType
				.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)
				.MakeGenericMethod(container.GetGenericArguments());
		}

		public static MethodInfo GetPrivateCovariantGenericAllStatic(Type callerType, string name, Type container)
		{
			return
				callerType.GetMethod(
					name,
					BindingFlags.Instance
						| BindingFlags.NonPublic
						| BindingFlags.Static
						| BindingFlags.FlattenHierarchy
				).MakeGenericMethod(container.GetGenericArguments());
		}

		private static string DeclaringTypes(Type t, string separator)
		{
			return t.DeclaringType == null
				? string.Empty
				: DeclaringTypes(t.DeclaringType, separator) + t.DeclaringType.Name + separator;
		}

		private static readonly Dictionary<Type, string> knownTypes = new Dictionary<Type, string> {
			{ typeof(byte),  "byte" },
			{ typeof(sbyte), "sbyte" },
			{ typeof(short), "short" },
			{ typeof(ushort), "ushort" },
			{ typeof(int), "int" },
			{ typeof(uint), "uint" },
			{ typeof(long), "long" },
			{ typeof(ulong), "ulong" },
			{ typeof(char), "char" },
			{ typeof(float), "float" },
			{ typeof(double), "double" },
			{ typeof(decimal), "decimal" },
			{ typeof(bool), "bool" },
			{ typeof(object), "object" },
			{ typeof(string), "string" },
			{ typeof(void), "void" },
		};
		public static string GetTypeSpec(Type t, string arraySize = "")
		{
			if (knownTypes.TryGetValue(t, out string result)) {
				return result;
			}
			if (t.IsArray) {
				var suffix = string.Format("[{0}{1}]", arraySize, new string(',', t.GetArrayRank() - 1));
				t = t.GetElementType();
				for (; t.IsArray; t = t.GetElementType()) {
					suffix += "[" + new string(',', t.GetArrayRank() - 1) + "]";
				}
				return GetTypeSpec(t) + suffix;
			}
			var p = "global::" + t.Namespace + ".";
			var n = string.Empty;
			if (!t.IsGenericType) {
				n = DeclaringTypes(t, ".") + t.Name;
			} else {
				var fgargs = t.GetGenericArguments().Select(a => GetTypeSpec(a)).Reverse();
				var dt = t;
				while (dt != null) {
					var name = dt.Name;
					if (dt.Name.Contains("`")) {
						var count = int.Parse(name[(name.IndexOf('`') + 1)..]);
						name = name.Remove(name.IndexOf('`'));
						name = name + "<" + string.Join(", ", fgargs.Take(count).Reverse()) + ">";
						fgargs = fgargs.Skip(count);
					}
					dt = dt.DeclaringType;
					n = n == string.Empty ? name : name + "." + n;
				}
			}
			return p + n;
		}

		// TODO: test
		public static string GetMangledTypeName(Type t)
		{
			var n = DeclaringTypes(t, "__") + t.Name;
			if (!t.IsGenericType) {
				return n;
			}
			var fgargs = t.GetGenericArguments().Select(a => GetMangledTypeName(a)).Reverse();
			var args = string.Join("__", t.GetGenericArguments().Select(a => GetMangledTypeName(a)));
			var dt = t;
			while (dt != null) {
				var name = dt.Name;
				if (dt.Name.Contains("`")) {
					var count = int.Parse(name[(name.IndexOf('`') + 1)..]);
					name = name.Remove(name.IndexOf('`'));
					name = name + "_" + string.Join(", ", fgargs.Take(count).Reverse()) + "_";
					fgargs = fgargs.Skip(count);
				}
				dt = dt.DeclaringType;
				n = n == string.Empty ? name : name + "__" + n;
			}
			return n + "_" + args;
		}

		// TODO: test
		public static string GetMangledTypeNameNS(Type t)
		{
			return t.Namespace.Replace('.', '_') + "__" + GetMangledTypeName(t);
		}
	}

	public static class TypeSerializer
	{
		private static readonly LinkedList<Assembly> assembliesLru = new LinkedList<Assembly>();
		private static readonly ConcurrentDictionary<string, Type> cache = new ConcurrentDictionary<string, Type>();

		static TypeSerializer()
		{
			// TODO: Remove when/if compatibility not needed.
			if (!Compatibility) {
				return;
			}

			var visited = new HashSet<Assembly>();
			var queue = new Queue<Assembly>();

			foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
				queue.Enqueue(a);
				visited.Add(a);
			}

			while (queue.Count() != 0) {
				foreach (var aName in queue.Dequeue().GetReferencedAssemblies()) {
					var a = Assembly.Load(aName);
					if (!visited.Contains(a)) {
						queue.Enqueue(a);
						visited.Add(a);
					}
				}
			}

			foreach (var a in visited) {
				assembliesLru.AddLast(a);
			}
		}

		private static readonly Regex extendedAssemblyInfo = new Regex(
			@", Version=\d+.\d+.\d+.\d+, Culture=neutral, PublicKeyToken=[a-z0-9]+", RegexOptions.Compiled
		);

		public static bool Compatibility;

		public static string Serialize(Type t)
		{
			return extendedAssemblyInfo.Replace(t.AssemblyQualifiedName, string.Empty)
				// Tries to replace multiple system type assembly names
				// because they differ across platforms and frameworks.
				// .NET Framework, Xamarin.Mac
				.Replace(", mscorlib", string.Empty)
				// .NET Core, .NET 5.0+
				.Replace(", System.Private.CoreLib", string.Empty);
		}

		public static Type Deserialize(string typeName)
		{
			if (cache.TryGetValue(typeName, out Type t)) {
				return t;
			}
			t = Type.GetType(typeName);
			if (t != null) {
				cache[typeName] = t;
				return t;
			}
			// TODO: Remove when/if compatibility not needed.
			if (!Compatibility) {
				return null;
			}
			for (var i = assembliesLru.First; i != null; i = i.Next) {
				t = i.Value.GetType(typeName);
				if (t != null) {
					cache[typeName] = t;
					assembliesLru.Remove(i);
					assembliesLru.AddFirst(i);
					return t;
				}
			}
			return null;
		}
	}

	internal class CodeWriter
	{
		public StreamWriter Output;
		private int indentLevel = 0;
		public string IndentString = "\t";
		private int tempCount = 0;
		public string LineSeparator = "\r\n";

		public void PutPart(string format, params object[] p)
		{
			var s = p.Length > 0 ? string.Format(format, p) : format;
			Output.Write(LineSeparator == "\n" ? s : s.Replace("\n", LineSeparator));
		}

		public void Put(string format, params object[] p)
		{
			var s = p.Length > 0 ? string.Format(format, p) : format;
			// "}\n" or "} while"
			if (s.StartsWith("}")) {
				indentLevel -= 1;
			}
			if (s != "\n") {
				for (int i = 0; i < indentLevel; ++i) {
					PutPart(IndentString);
				}
			}
			PutPart(s);
			if (s.EndsWith("{\n")) {
				indentLevel += 1;
			}
		}

		public void PutInd(string format, params object[] p)
		{
			indentLevel += 1;
			Put(format, p);
			indentLevel -= 1;
		}

		public void PutEndBlock() => Put("}\n");

		// Check for explicit vs implicit interface implementation.
		public static string GenAddToCollection(Type t, Type icoll, string collName, string elementName)
		{
			var imap = t.GetInterfaceMap(icoll);
			var addIndex = Array.FindIndex(imap.InterfaceMethods, m => m.Name == "Add");
			return string.Format(
				imap.TargetMethods[addIndex].Name == "Add" ? "{0}.Add({1});\n" : "(({2}){0}).Add({1});\n",
				collName,
				elementName,
				Utils.GetTypeSpec(icoll)
			);
		}

		public void PutAddToCollection(Type t, Type icoll, string collName, string elementName)
		{
			Put(GenAddToCollection(t, icoll, collName, elementName));
		}

		public void ResetTempNames() => tempCount = 0;

		public string GetTempName()
		{
			tempCount += 1;
			return "tmp" + tempCount.ToString();
		}

		public void GenerateActionList(ActionList actions, string name = "result")
		{
			foreach (var a in actions.Actions) {
				Put("{0}.{1}();\n", name, a.Info.Name);
			}
		}
	}

	internal class NullYuzuUnknownStorage : YuzuUnknownStorage
	{
		internal static NullYuzuUnknownStorage Instance = new NullYuzuUnknownStorage();
		public override void Add(string name, object value) { }
	}

	internal class BoxedInt
	{
		public int Value = 0;
	}

	public class ActionList
	{
		public struct MethodAction
		{
			public MethodInfo Info;
			public Action<object> Run;
		}

		public List<MethodAction> Actions = new List<MethodAction>();

		public void MaybeAdd(MethodInfo m, Type attr)
		{
			if (m.IsDefined(attr, false)) {
				Actions.Add(new MethodAction { Info = m, Run = obj => m.Invoke(obj, null) });
			}
		}

		public void Run(object obj)
		{
			foreach (var a in Actions) {
				a.Run(obj);
			}
		}
	}

	public static class IdGenerator
	{
		private static readonly char[] lastId = new char[] { 'A', 'A', 'A', 'A' };

		private static void NextId()
		{
			var i = lastId.Length - 1;
			do {
				switch (lastId[i]) {
					case 'Z':
						lastId[i] = 'a';
						return;
					case 'z':
						lastId[i] = 'A';
						break;
					default:
						lastId[i] = (char)((int)lastId[i] + 1);
						return;
				}
				i--;
			} while (lastId[i] != 'A');
			lastId[i] = 'B';
		}

		public static string GetNextId()
		{
			NextId();
			return new string(lastId);
		}
	}
}
