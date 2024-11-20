using System;

namespace Yuzu
{
	public interface IReferenceResolver
	{
		/// <summary>
		/// Returns a reference instance type.
		/// </summary>
		Type ReferenceType { get; }

		/// <summary>
		/// Returns an object for the given reference or throws an exception.
		/// </summary>
		object ResolveReference(object reference, Type memberType);

		/// <summary>
		/// Registers a newly deserialized object.
		/// Called right after the object factory method.
		/// </summary>
		void AddReference(object reference, object obj);

		/// <summary>
		/// Returns a reference instance for the given object.
		/// owner is the object that contains a reference to the given object.
		/// This context is necessary to implement a flat Json hierarchy.
		/// alreadyExists denotes that the reference must be serialized, otherwise the object is serialized.
		/// </summary>
		object GetReference(object obj, out bool alreadyExists);
	}

	public abstract class ReferenceResolver<TReference> : IReferenceResolver
	{
		Type IReferenceResolver.ReferenceType => typeof(TReference);

		/// <summary>
		/// Returns an object for the given reference or throws an exception.
		/// </summary>
		public abstract object ResolveReference(TReference reference, Type memberType);

		/// <summary>
		/// Registers a newly deserialized object.
		/// Called right after the object factory method.
		/// </summary>
		public abstract void AddReference(TReference reference, object obj);

		/// <summary>
		/// Returns a reference instance for the given object.
		/// owner is the object that contains a reference to the given object.
		/// This context is necessary to implement a flat Json hierarchy.
		/// alreadyExists denotes that the reference must be serialized, otherwise the object is serialized.
		/// </summary>
		public abstract TReference GetReference(object obj, out bool alreadyExists);

		object IReferenceResolver.ResolveReference(object reference, Type memberType) =>
			ResolveReference((TReference)reference, memberType);

		void IReferenceResolver.AddReference(object reference, object obj) => AddReference((TReference)reference, obj);

		object IReferenceResolver.GetReference(object obj, out bool alreadyExists) => GetReference(obj, out alreadyExists);
	}
}
