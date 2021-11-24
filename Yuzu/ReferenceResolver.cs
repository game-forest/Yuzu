using System;
using System.Collections.Generic;

namespace Yuzu
{
	public interface IDeserializerReferenceResolver
	{
		Type ReferenceType();
		object GetObject(object reference);
		bool TryGetObject(object reference, out object obj);
		void AddObject(object reference, object obj);
	}

	public interface ISerializerReferenceResolver
	{
		Type ReferenceType();
		bool TryGetReference(object obj, out object reference, out bool referenceGenerated);
	}
}
