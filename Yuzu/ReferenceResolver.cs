using System;
using System.Collections.Generic;

namespace Yuzu
{
	public interface IReferenceResolver
	{
		Type ReferenceType();
		object GetObject(object reference);
		bool TryGetObject(object reference, out object obj);
		void AddObject(object reference, object obj);
		bool TryGetReference(object obj, out object reference, out bool referenceGenerated);
	}

	public class ReferenceResolver : IReferenceResolver
	{
		private readonly Dictionary<int, object> referenceToObjects = new Dictionary<int, object>();
		private readonly Dictionary<object, int> objectsToReferences = new Dictionary<object, int>();
		private int currentId = 1;

		public Type ReferenceType() => typeof(int);

		public object GetObject(object reference)
		{
			if (!referenceToObjects.TryGetValue((int)reference, out var obj)) {
				throw new Yuzu.YuzuException($"Unresolved reference: {reference}");
			}
			return obj;
		}

		public bool TryGetObject(object reference, out object obj)
		{
			return referenceToObjects.TryGetValue((int)reference, out obj);
		}

		public void AddObject(object reference, object obj) => referenceToObjects.Add((int)reference, obj);

		public bool TryGetReference(object obj, out object reference, out bool referenceGenerated)
		{
			if (objectsToReferences.TryGetValue(obj, out var key)) {
				reference = key;
				referenceGenerated = false;
			} else {
				reference = currentId++;
				objectsToReferences.Add(obj, (int)reference);
				referenceGenerated = true;
			}
			return true;
		}
	}
}
