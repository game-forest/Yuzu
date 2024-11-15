using System;
using System.Collections.Generic;
using System.IO;

using Yuzu.Metadata;
using Yuzu.Util;

namespace Yuzu.Binary
{
	using ReadCacheAction = Action<BinaryDeserializer, ReaderClassDef, object>;
	using MakeCacheAction = Func<BinaryDeserializer, ReaderClassDef, object>;

	public class BinaryDeserializerGenBase: BinaryDeserializer
	{
		protected static Dictionary<Type, ReadCacheAction> readCache = [];
		protected static Dictionary<Type, MakeCacheAction> makeCache = [];

		protected override void PrepareReaders(ReaderClassDef def)
		{
			base.PrepareReaders(def);
			if (readCache.TryGetValue(def.Meta.Type, out ReadCacheAction r))
				def.ReadFields = r;
			if (makeCache.TryGetValue(def.Meta.Type, out MakeCacheAction m))
				def.Make = m;
		}

	}

	public class BinaryDeserializerGenerator : IGenerator
	{
		private CodeWriter cw = new();
		private string wrapperNameSpace;
		private CommonOptions options;
		private string className;
		private string baseClassName;
		private Dictionary<Type, string> generatedReaders = [];
		private Dictionary<Type, string> generatedMakers = [];
		private string classDefName = typeof(ReaderClassDef).Name;

		public string LineSeparator { get { return cw.LineSeparator; } set { cw.LineSeparator = value; } }

		public StreamWriter GenWriter
		{
			get { return cw.Output; }
			set { cw.Output = value; }
		}

		// Turn off for 5% speedup in exchange for potentially missing broken data.
		public bool SafetyChecks = true;

		public BinaryDeserializerGenerator(
			string wrapperNameSpace = "YuzuGenBin",
			CommonOptions? options = null,
			string className = "BinaryDeserializerGen",
			string baseClassName = "BinaryDeserializerGenBase"
		) {
			this.wrapperNameSpace = wrapperNameSpace;
			this.options = options ?? new CommonOptions();
			this.className = className;
			this.baseClassName = baseClassName;
		}

		public void GenerateHeader()
		{
			cw.Put("using System;\n");
			cw.Put("\n");
			cw.Put("using Yuzu.Binary;\n");
			cw.Put("\n");
			cw.Put("namespace {0}\n", wrapperNameSpace);
			cw.Put("{\n");
			cw.Put("public class {0}: {1}\n", className, baseClassName);
			cw.Put("{\n");
		}

		public void GenerateFooter()
		{
			cw.Put("static {0}()\n", className);
			cw.Put("{\n");
			foreach (var r in generatedReaders)
				cw.Put("readCache[typeof({0})] = {1};\n", Utils.GetTypeSpec(r.Key), r.Value);
			foreach (var r in generatedMakers)
				cw.Put("makeCache[typeof({0})] = {1};\n", Utils.GetTypeSpec(r.Key), r.Value);
			cw.PutEndBlock();
			cw.PutEndBlock(); // Close class.
			cw.PutEndBlock(); // Close namespace.
		}

		private static Dictionary<Type, string> simpleValueReader = new() {
			{ typeof(sbyte), "d.Reader.ReadSByte()" },
			{ typeof(byte), "d.Reader.ReadByte()" },
			{ typeof(short), "d.Reader.ReadInt16()" },
			{ typeof(ushort), "d.Reader.ReadUInt16()" },
			{ typeof(int), "d.Reader.ReadInt32()" },
			{ typeof(uint), "d.Reader.ReadUInt32()" },
			{ typeof(long), "d.Reader.ReadInt64()" },
			{ typeof(ulong), "d.Reader.ReadUInt64()" },
			{ typeof(bool), "d.Reader.ReadBoolean()" },
			{ typeof(char), "d.Reader.ReadChar()" },
			{ typeof(float), "d.Reader.ReadSingle()" },
			{ typeof(double), "d.Reader.ReadDouble()" },
			{ typeof(decimal), "d.Reader.ReadDecimal()" },
			{ typeof(DateTime), "DateTime.FromBinary(d.Reader.ReadInt64())" },
			{ typeof(DateTimeOffset), "dg.ReadDateTimeOffset()" },
			{ typeof(TimeSpan), "new TimeSpan(d.Reader.ReadInt64())" },
			{ typeof(Guid), "new Guid(d.Reader.ReadBytes(16))" },
			{ typeof(object), "dg.ReadAny()" },
		};

		private string PutCount()
		{
			var tempCountName = cw.GetTempName();
			cw.Put("var {0} = d.Reader.ReadInt32();\n", tempCountName);
			cw.Put("if ({0} >= 0) {{\n", tempCountName);
			return tempCountName;
		}

		private string PutNullOrCount(Type t)
		{
			cw.PutPart("({0})null;\n", Utils.GetTypeSpec(t));
			return PutCount();
		}

		private void GenerateCollection(Type t, Type icoll, string name, string tempIndexName)
		{
			var capacityProperty = t.GetProperty("Capacity");
			if (capacityProperty != null && capacityProperty.GetSetMethod() != null) {
				cw.Put($"{name}.Capacity += {tempIndexName};\n");
			}
			cw.Put("while (--{0} >= 0) {{\n", tempIndexName);
			var tempElementName = cw.GetTempName();
			cw.Put("var {0} = ", tempElementName);
			GenerateValue(icoll.GetGenericArguments()[0], tempElementName);
			cw.PutAddToCollection(t, icoll, name, tempElementName);
			cw.PutEndBlock(); // while
		}

		private void GenerateDictionary(Type t, Type idict, string name, string tempIndexName)
		{
			cw.Put("while (--{0} >= 0) {{\n", tempIndexName);
			var tempKeyName = cw.GetTempName();
			cw.Put("var {0} = ", tempKeyName);
			GenerateValue(idict.GetGenericArguments()[0], tempKeyName);
			var tempValueName = cw.GetTempName();
			cw.Put("var {0} = ", tempValueName);
			GenerateValue(idict.GetGenericArguments()[1], tempValueName);
			cw.Put("{0}.Add({1}, {2});\n", name, tempKeyName, tempValueName);
			cw.PutEndBlock(); // while
		}

		private string MaybeUnchecked() { return SafetyChecks ? "" : "Unchecked"; }

		private void GenerateValue(Type t, string name)
		{
			if (simpleValueReader.TryGetValue(t, out string sr)) {
				cw.PutPart(sr + ";\n");
				return;
			}
			if (t == typeof(string)) {
				cw.PutPart("d.Reader.ReadString();\n");
				cw.Put("if ({0} == \"\" && d.Reader.ReadBoolean()) {0} = null;\n", name);
				return;
			}
			if (t.IsEnum) {
				cw.PutPart("({0}){1};\n", Utils.GetTypeSpec(t), simpleValueReader[Enum.GetUnderlyingType(t)]);
				return;
			}
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)) {
				var arg = t.GetGenericArguments()[0];
				cw.PutPart("d.Reader.ReadBoolean() ? ({0}?)null : ", Utils.GetTypeSpec(arg));
				GenerateValue(arg, name);
				return;
			}
			if (t.IsArray) {
				var r = t.GetArrayRank();
				if (r > 1) {
					cw.PutPart("({0})dg.ReadArrayNDim(typeof({1}), {2});\n",
						Utils.GetTypeSpec(t), Utils.GetTypeSpec(t.GetElementType()), r);
					return;
				}
				var tempIndexName = PutNullOrCount(t);
				var tempArrayName = cw.GetTempName();
				cw.Put("var {0} = new {1};\n", tempArrayName, Utils.GetTypeSpec(t, arraySize: tempIndexName));
				cw.Put("for({0} = 0; {0} < {1}.Length; ++{0}) {{\n", tempIndexName, tempArrayName);
				GenerateSetValue(t.GetElementType(), String.Format("{0}[{1}]", tempArrayName, tempIndexName), null);
				cw.PutEndBlock();
				cw.Put("{0} = {1};\n", name, tempArrayName);
				cw.PutEndBlock(); // if >= 0
				return;
			}
			var idict = Utils.GetIDictionary(t);
			if (idict != null) {
				var tempIndexName = PutNullOrCount(t);
				cw.Put("{0} = [];\n", name);
				GenerateDictionary(t, idict, name, tempIndexName);
				cw.PutEndBlock();
				return;
			}
			var icoll = Utils.GetICollection(t);
			if (icoll != null) {
				var tempIndexName = PutNullOrCount(t);
				cw.Put("{0} = [];\n", name);
				GenerateCollection(t, icoll, name, tempIndexName);
				cw.PutEndBlock();
				return;
			}
			if (t.IsClass || t.IsInterface) {
				cw.PutPart("({0})dg.ReadObject{1}<{0}>();\n", Utils.GetTypeSpec(t), MaybeUnchecked());
				return;
			}
			if (Utils.IsStruct(t)) {
				var meta = Meta.Get(t, options);
				if (meta.IsCompact) {
					cw.PutPart("new {0}();\n", Utils.GetTypeSpec(t));
					cw.Put("dg.EnsureClassDef(typeof({0}));\n", Utils.GetTypeSpec(t));
					foreach (var yi in meta.Items)
						GenerateSetValue(yi.Type, $"{name}." + yi.Name, yi);
				} else {
					cw.PutPart("({0})dg.ReadStruct{1}<{0}>();\n", Utils.GetTypeSpec(t), MaybeUnchecked());
				}
				return;
			}
			throw new NotImplementedException();
		}

		private string GenerateFactoryCall(Meta meta) =>
			meta.FactoryMethod == null ?
				String.Format("new {0}()", Utils.GetTypeSpec(meta.Type)) :
				String.Format("{0}.{1}()", Utils.GetTypeSpec(meta.Type), meta.FactoryMethod.Name);

		private bool CanInline(Meta meta) =>
			meta.IsCompact && meta.Surrogate.FuncFrom == null &&
			meta.BeforeDeserialization.Actions.Count == 0 &&
			meta.AfterDeserialization.Actions.Count == 0;

		private bool GenerateSetValueInline(Type t, string name, Meta.Item item)
		{
			if (t.IsGenericType || !Utils.IsStruct(t) || item == null || simpleValueReader.ContainsKey(t))
				return false;
			var meta = Meta.Get(t, options);
			if (CanInline(meta)) {
				cw.Put("dg.EnsureClassDef(typeof({0}));\n", Utils.GetTypeSpec(t));
				if (item.PropInfo == null) {
					foreach (var yi in meta.Items)
						GenerateSetValue(yi.Type, name + "." + yi.Name, yi);
				}
				else {
					var tempStructName = cw.GetTempName();
					cw.Put("var {0} = {1};\n", tempStructName, GenerateFactoryCall(meta));
					foreach (var yi in meta.Items)
						GenerateSetValue(yi.Type, tempStructName + "." + yi.Name, yi);
					cw.Put("{0} = {1};\n", name, tempStructName);
				}
				return true;
			}
			else if (item.PropInfo == null) {
				cw.Put("dg.ReadIntoStruct(ref {0});\n", name);
				return true;
			}
			return false;
		}

		private void GenerateSetValue(Type t, string name, Meta.Item item)
		{
			if (!GenerateSetValueInline(t, name, item)) {
				cw.Put("{0} = ", name);
				GenerateValue(t, name);
			}
		}

		private void GenerateMerge(Type t, string name)
		{
			var idict = Utils.GetIDictionary(t);
			if (idict != null) {
				GenerateDictionary(t, idict, name, PutCount());
				cw.PutEndBlock();
				return;
			}
			var icoll = Utils.GetICollection(t);
			if (icoll != null) {
				GenerateCollection(t, icoll, name, PutCount());
				cw.PutEndBlock();
				return;
			}
			if ((t.IsClass || t.IsInterface) && t != typeof(object)) {
				cw.Put("dg.ReadIntoObject{0}<{1}>({2});\n", MaybeUnchecked(), Utils.GetTypeSpec(t), name);
				return;
			}
			throw new YuzuException(String.Format("Unable to merge field {1} of type {0}", name, t.Name));
		}

		private bool IsDeserializerGenRequired(Meta meta)
		{
			if (!meta.IsCompact) return true;
			foreach (var yi in meta.Items)
			{
				if (yi.Type == typeof(object)) return true;
				if (simpleValueReader.ContainsKey(yi.Type) || yi.Type.IsEnum || yi.Type == typeof(string))
					continue;
				// TODO: Containers.
				return true;
			}
			return false;
		}

		private void GenerateReaderBody(Meta meta)
		{
			cw.GenerateActionList(meta.BeforeDeserialization);
			cw.ResetTempNames();
			if (IsDeserializerGenRequired(meta))
				cw.Put("var dg = ({0})d;\n", className);
			if (meta.IsCompact) {
				foreach (var yi in meta.Items)
					GenerateSetValue(yi.Type, "result." + yi.Name, yi);
			}
			else {
				cw.Put("{0}.FieldDef fd;\n", classDefName);
				var ourIndex = 0;
				cw.Put("fd = def.Fields[d.Reader.ReadInt16()];\n");
				foreach (var yi in meta.Items) {
					ourIndex += 1;
					if (yi.IsOptional)
						cw.Put("if ({0} == fd.OurIndex) {{\n", ourIndex);
					else if (SafetyChecks)
						cw.Put("if ({0} != fd.OurIndex) throw dg.Error(\"{0}!=\" + fd.OurIndex);\n", ourIndex);
					if (yi.SetValue != null)
						GenerateSetValue(yi.Type, "result." + yi.Name, yi);
					else
						GenerateMerge(yi.Type, "result." + yi.Name);
					cw.Put("fd = def.Fields[d.Reader.ReadInt16()];\n");
					if (yi.IsOptional)
						cw.PutEndBlock();
				}
				if (SafetyChecks)
					cw.Put("if (fd.OurIndex != {0}.EOF) throw dg.Error(\"Unfinished object\");\n", classDefName);
			}
			cw.GenerateActionList(meta.AfterDeserialization);
		}

		public void Generate<T>() { Generate(typeof(T)); }

		public void Generate(Type t)
		{
			if (t.IsInterface)
				throw new YuzuException("Useless BinaryGenerator for interface " + t.FullName);
			if (t.IsAbstract)
				throw new YuzuException("Useless BinaryGenerator for abstract class " + t.FullName);

			var meta = Meta.Get(t, options);

			var readerName = "Read_" + Utils.GetMangledTypeNameNS(t);
			if (!Utils.IsStruct(t)) {
				cw.Put("private static void {0}(BinaryDeserializer d, {1} def, object obj)\n", readerName, classDefName);
				cw.Put("{\n");
				cw.Put("var result = ({0})obj;\n", Utils.GetTypeSpec(t));
				GenerateReaderBody(meta);
				cw.PutEndBlock();
				cw.Put("\n");
				generatedReaders[t] = readerName;
			}

			var makerName = "Make_" + Utils.GetMangledTypeNameNS(t);
			cw.Put("private static object {0}(BinaryDeserializer d, {1} def)\n", makerName, classDefName);
			cw.Put("{\n");
			cw.Put("var result = {0};\n", GenerateFactoryCall(meta));
			if (Utils.IsStruct(t))
				GenerateReaderBody(meta);
			else
				cw.Put("{0}(d, def, result);\n", readerName);
			cw.Put("return result;\n");
			cw.PutEndBlock();
			cw.Put("\n");
			generatedMakers[t] = makerName;
		}

	}
}
