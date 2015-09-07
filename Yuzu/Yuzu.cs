﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuzu
{
	public class YuzuOrder : Attribute
	{
		public readonly int Order = 0;
		public YuzuOrder(int order) { Order = order; }
	}

	public class YuzuRequired : YuzuOrder
	{
		public YuzuRequired(int order): base(order) { }
	}

	public class YuzuOptional : YuzuOrder
	{
		public YuzuOptional(int order) : base(order) { }
	}

	public class CommonOptions
	{
		public Type RequiredAttribute = typeof(YuzuRequired);
		public Type OptionalAttribute = typeof(YuzuOptional);
		public Func<Attribute, int> GetOrder = attr => (attr as YuzuOrder).Order;
	}

	public class YuzuException: Exception
	{
	}

	public abstract class AbstractSerializer
	{
		public CommonOptions Options = new CommonOptions();
		public abstract void ToWriter(object obj, BinaryWriter writer);
		public abstract string ToString(object obj);
		public abstract byte[] ToBytes(object obj);
		public abstract void ToStream(object obj, Stream target);
	}

	public abstract class AbstractWriterSerializer: AbstractSerializer
	{
		protected BinaryWriter writer;

		protected abstract void ToWriter(object obj);

		public override void ToWriter(object obj, BinaryWriter writer)
		{
			this.writer = writer;
			ToWriter(obj);
		}

		protected void WriteStr(string s)
		{
			writer.Write(Encoding.UTF8.GetBytes(s));
		}

		public override string ToString(object obj)
		{
			var ms = new MemoryStream();
			ToStream(obj, ms);
			return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
		}

		public override byte[] ToBytes(object obj)
		{
			var ms = new MemoryStream();
			ToStream(obj, ms);
			var result = ms.GetBuffer();
			Array.Resize(ref result, (int)ms.Length);
			return result;
		}

		public override void ToStream(object obj, Stream target)
		{
			ToWriter(obj, new BinaryWriter(target));
		}
	}

	public abstract class AbstractStringSerializer : AbstractSerializer
	{
		protected StringBuilder builder;

		protected abstract void ToBuilder(object obj);

		public override void ToWriter(object obj, BinaryWriter writer)
		{
			writer.Write(ToBytes(obj));
		}

		public override string ToString(object obj)
		{
			builder = new StringBuilder();
			ToBuilder(obj);
			return builder.ToString();
		}

		public override byte[] ToBytes(object obj)
		{
			return Encoding.UTF8.GetBytes(ToString(obj));
		}

		public override void ToStream(object obj, Stream target)
		{
			var b = ToBytes(obj);
			target.Write(b, 0, b.Length);
		}
	}

	public abstract class AbstractDeserializer
	{
		public CommonOptions Options = new CommonOptions();
		public abstract void FromReader(object obj, BinaryReader reader);
		public abstract void FromString(object obj, string source);
		public abstract void FromStream(object obj, Stream source);
		public abstract void FromBytes(object obj, byte[] bytes);
	}

	public abstract class AbstractReaderDeserializer: AbstractDeserializer
	{
		public BinaryReader Reader;

		public abstract void FromReader(object obj);

		public override void FromReader(object obj, BinaryReader reader)
		{
			Reader = reader;
			FromReader(obj);
		}

		public override void FromString(object obj, string source)
		{
			FromReader(obj, new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes(source), false)));
		}

		public override void FromStream(object obj, Stream source)
		{
			FromReader(obj, new BinaryReader(source));
		}

		public override void FromBytes(object obj, byte[] bytes)
		{
			FromStream(obj, new MemoryStream(bytes, false));
		}

	}

}
