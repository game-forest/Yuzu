using System;
using System.Collections;
using System.Collections.Generic;
using Yuzu.Metadata;
using Yuzu.Util;

namespace Yuzu.Code
{
	public class CodeConstructSerializeOptions
	{
		public string VarName = "x";
		public string Indent = "\t";
		public string NewLine = "\n";
	}

	public class CodeConstructSerializer : AbstractStringSerializer
	{
		public CodeConstructSerializeOptions CodeConstructOptions = new CodeConstructSerializeOptions();

		private bool IsPrimitive(object value)
		{
			if (value == null) {
				return true;
			}
			var t = value.GetType();
			return t.IsPrimitive
				|| t.IsEnum
				|| t == typeof(string)
				|| t == typeof(decimal)
				|| t == typeof(DateTime)
				|| t == typeof(DateTimeOffset)
				|| t == typeof(TimeSpan)
				|| t == typeof(Guid);
		}

		protected override void ToBuilder(object value)
		{
			var o = CodeConstructOptions;
			builder.Append($"var {o.VarName} = ");
			AppendValue(value);
			builder.Append($";{o.NewLine}");
		}

		private void AppendValue(object value)
		{
			var t = value?.GetType();
			if (IsPrimitive(value)) {
				AppendPrimitiveValue(value);
			} else if (Utils.GetICollection(t) != null) {
				AppendCollection(value);
			} else if (Utils.GetIDictionary(t) != null) {
				AppendDictionary(value);
			} else {
				AppendCompositeValue(value);
			}
		}

		private void AppendDictionary(object value)
		{
			var o = CodeConstructOptions;
			builder.Append($"new {Utils.GetTypeSpec(value.GetType())} {{{o.NewLine}");
			var first = true;
			foreach (DictionaryEntry e in (IDictionary)value) {
				if (!first) {
					builder.Append($",{o.NewLine}");
				}
				first = false;
				builder.Append(CodeConstructOptions.Indent);
				builder.Append("{{");
				AppendValue(e.Key);
				builder.Append(", ");
				AppendValue(e.Value);
				builder.Append("}}");
			}
			builder.Append($"{o.NewLine}}}");
		}

		private void AppendCollection(object value)
		{
			var o = CodeConstructOptions;
			builder.Append($"new {Utils.GetTypeSpec(value.GetType())} {{{o.NewLine}");
			var first = true;
			foreach (var e in (ICollection)value) {
				if (!first) {
					builder.Append($",{o.NewLine}");
				}
				first = false;
				builder.Append(CodeConstructOptions.Indent);
				AppendValue(e);
			}
			builder.Append($"{o.NewLine}}}");
		}

		private void AppendPrimitiveValue(object value)
		{
			var v = Utils.CodeValueFormat(value);
			builder.Append(v);
		}

		private void AppendCompositeValue(object value)
		{
			var o = CodeConstructOptions;
			builder.Append($"new {Utils.GetTypeSpec(value.GetType())} {{{o.NewLine}");
			var first = true;
			foreach (var yi in Meta.Get(value.GetType(), Options).Items) {
				if (!first) {
					builder.Append($",{o.NewLine}");
				}
				if (yi.SetValue == null) {
					continue;
				}
				first = false;
				builder.Append(CodeConstructOptions.Indent);
				builder.Append(yi.Name);
				builder.Append(" = ");
				AppendValue(yi.GetValue(value));
			}
			builder.Append($"{o.NewLine}}}");
		}
	}

	public class CodeAssignSerializeOptions
	{
		public string FuncName = "Init";
		public string Indent = "\t";
	}

	public class CodeAssignSerializer : AbstractStringSerializer
	{
		public CodeAssignSerializeOptions CodeAssignOptions = new CodeAssignSerializeOptions();

		protected override void ToBuilder(object obj)
		{
			builder.AppendFormat("void {0}({1} obj) {{\n", CodeAssignOptions.FuncName, obj.GetType().Name);
			foreach (var yi in Meta.Get(obj.GetType(), Options).Items) {
				string valueStr = Utils.CodeValueFormat(yi.GetValue(obj));
				if (valueStr == string.Empty) {
					throw new NotImplementedException(yi.Type.Name);
				}

				builder.AppendFormat("{0}obj.{1} = {2};\n", CodeAssignOptions.Indent, yi.Name, valueStr);
			}
			builder.Append("}\n");
		}
	}
}
