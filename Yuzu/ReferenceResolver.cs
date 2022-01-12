using System;
using System.Collections.Generic;

namespace Yuzu
{
	public interface IDeserializerReferenceResolver
	{
		/// <summary>
		/// Returns a reference instance type.
		/// </summary>
		Type ReferenceType();

		/// <summary>
		/// Returns an object for the given reference or throws an exception.
		/// </summary>
		object GetObject(object reference);

		/// <summary>
		/// Returns an object for the given reference if it is already registered with AddObject().
		/// Called from JsonDeserializer since Json supports forward references.
		/// </summary>
		bool TryGetObject(object reference, out object obj);

		/// <summary>
		/// Registers a newly deserialized object.
		/// Called right after the object factory method.
		/// </summary>
		void AddObject(object reference, object obj);

		/// <summary>
		/// Called when deserialization is done.
		/// </summary>
		void Clear();
	}

	public interface ISerializerReferenceResolver
	{
		/// <summary>
		/// Returns a reference instance type.
		/// </summary>
		Type ReferenceType();

		/// <summary>
		/// Returns a reference instance for the given object.
		/// owner is the object that contains a reference to the given object.
		/// This context is necessary to implement a flat Json hierarchy.
		/// writeObject denotes that the object must be serialized, otherwise the reference is serialized.
		/// </summary>
		bool TryGetReference(object obj, object owner, out object reference, out bool writeObject);

		/// <summary>
		/// Called when serialization is done.
		/// </summary>
		void Clear();
	}

	public interface IClonerReferenceResolver
	{
		/// <summary>
		/// Returns a reference instance type.
		/// </summary>
		Type ReferenceType();

		/// <summary>
		/// Returns a reference instance for the given object.
		/// newReference denotes that the reference has just been created.
		/// </summary>
		bool TryGetReference(object obj, out object reference, out bool newReference);

		/// <summary>
		/// Returns an object for the given reference or throws an exception.
		/// </summary>
		object GetObject(object reference);

		// <summary>
		/// Registers a newly deserialized object.
		/// Called right after the object factory method.
		/// </summary>
		void AddObject(object reference, object obj);

		/// <summary>
		/// Called when the clone is done.
		/// </summary>
		void Clear();
	}
}
