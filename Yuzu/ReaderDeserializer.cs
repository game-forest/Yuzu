using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Yuzu.Util;
using Yuzu.Metadata;

namespace Yuzu.Deserializer
{
	public abstract class AbstractReaderDeserializer : AbstractDeserializer
	{
		public BinaryReader Reader;

		protected virtual void Initialize() { }
		public abstract object FromReader(object obj, Type t);

		public override object FromReader(object obj, BinaryReader reader, Type t)
		{
			Reader = reader;
			Initialize();
			return FromReader(obj, t);
		}

		public override object FromString(object obj, string source, Type t)
		{
			return FromReader(obj, new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes(source), false)), t);
		}

		public override object FromStream(object obj, Stream source, Type t)
		{
			return FromReader(obj, new BinaryReader(source), t);
		}

		public override object FromBytes(object obj, byte[] bytes, Type t)
		{
			return FromStream(obj, new MemoryStream(bytes, false), t);
		}

		protected YuzuException Error(string message, params object[] args)
		{
			for (int i = 0; i < args.Length; ++i)
				if (args[i] is Type)
					args[i] = TypeSerializer.Serialize((Type)args[i]);
			return new YuzuException(
				String.Format(message, args),
				Options.ReportErrorPosition ? new YuzuPosition(Reader.BaseStream.Position) : null);
		}

		protected Type FindType(string typeName)
		{
			var t = Meta.GetTypeByReadAlias(typeName, Options) ?? TypeSerializer.Deserialize(typeName);
			if (t == null)
				throw Error("Unknown type '{0}'", typeName);
			return t;
		}

		protected void CheckExpectedType(string typeName, Type expectedType)
		{
			var actualType = FindType(typeName);
			if (
				actualType != expectedType &&
				(!Meta.Get(expectedType, Options).AllowReadingFromAncestor || expectedType.BaseType != actualType)
			)
				throw Error("Expected type '{0}', but got '{1}'", expectedType.Name, typeName);
		}

		protected Stack<object> objStack = new ();

		protected Action<T> GetAction<T>(string name)
		{
			if (String.IsNullOrEmpty(name))
				return null;
			var obj = objStack.Peek();
			var m = obj.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
			if (m == null)
				throw Error("Unknown action '{0}'", name);
			return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), obj, m);
		}

		protected Func<object> MakeDelegate(MethodInfo m) =>
			(Func<object>)Delegate.CreateDelegate(typeof(Func<object>), this, m);

		protected Action<object> MakeDelegateAction(MethodInfo m) =>
			(Action<object>)Delegate.CreateDelegate(typeof(Action<object>), this, m);

	}
}
