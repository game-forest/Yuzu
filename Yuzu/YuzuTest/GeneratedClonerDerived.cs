using System;

using Yuzu.Clone;

namespace YuzuGenClone
{
	public class ClonerGenDerived: ClonerGen
	{
		protected static global::YuzuTest.SampleClonerGenDerived Clone_YuzuTest__SampleClonerGenDerived(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleClonerGenDerived))
				return (global::YuzuTest.SampleClonerGenDerived)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleClonerGenDerived)src;
			object r = null;
			if (cl.ReferenceResolver != null && cl.ReferenceResolver.TryGetReference(src, out r, out bool nr) && !nr)
				return (global::YuzuTest.SampleClonerGenDerived)cl.ReferenceResolver.GetObject(r);
			var result = new global::YuzuTest.SampleClonerGenDerived();
			if (r != null)
				cl.ReferenceResolver.AddObject(r, result);
			result.S = Clone_YuzuTest__Sample1(cl, s.S);
			return result;
		}

		static ClonerGenDerived()
		{
			clonerCache[typeof(global::YuzuTest.SampleClonerGenDerived)] = Clone_YuzuTest__SampleClonerGenDerived;
		}
	}
}
