using System;
using System.Reflection;

using Yuzu;
using Yuzu.Clone;

namespace YuzuGenClone
{
	public class ClonerGen: ClonerGenBase
	{
		private static global::YuzuTest.Color Clone_YuzuTest__Color(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.Color)src;
			var result = new global::YuzuTest.Color();
			result.B = s.B;
			result.G = s.G;
			result.R = s.R;
			return result;
		}

		private static global::YuzuTest.Sample1 Clone_YuzuTest__Sample1(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.Sample1)src;
			var result = new global::YuzuTest.Sample1();
			result.X = s.X;
			result.Y = s.Y;
			return result;
		}

		private static global::YuzuTest.Sample2 Clone_YuzuTest__Sample2(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.Sample2)src;
			var result = new global::YuzuTest.Sample2();
			result.X = s.X;
			result.Y = s.Y;
			return result;
		}

		private static global::YuzuTest.Sample3 Clone_YuzuTest__Sample3(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.Sample3)src;
			var result = new global::YuzuTest.Sample3();
			result.S1 = Clone_YuzuTest__Sample1(cl, s.S1);
			result.F = s.F;
			result.S2 = Clone_YuzuTest__Sample2(cl, s.S2);
			return result;
		}

		private static global::YuzuTest.SampleAfter2 Clone_YuzuTest__SampleAfter2(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleAfter2)src;
			var result = new global::YuzuTest.SampleAfter2();
			result.X = s.X;
			result.After2();
			result.After3();
			result.After();
			return result;
		}

		private static global::YuzuTest.SampleAfterDeserialization Clone_YuzuTest__SampleAfterDeserialization(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleAfterDeserialization)src;
			var result = new global::YuzuTest.SampleAfterDeserialization();
			result.X = s.X;
			result.After();
			return result;
		}

		private static global::YuzuTest.SampleAfterSerialization Clone_YuzuTest__SampleAfterSerialization(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleAfterSerialization)src;
			var result = new global::YuzuTest.SampleAfterSerialization();
			result.X = s.X;
			s.After();
			return result;
		}

		private static global::YuzuTest.SampleArray Clone_YuzuTest__SampleArray(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleArray)src;
			var result = new global::YuzuTest.SampleArray();
			if (s.A != null) {
				result.A = new string[s.A.Length];
				Array.Copy(s.A, result.A, s.A.Length);
			}
			return result;
		}

		private static global::YuzuTest.SampleArrayOfClass Clone_YuzuTest__SampleArrayOfClass(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleArrayOfClass)src;
			var result = new global::YuzuTest.SampleArrayOfClass();
			if (s.A != null) {
				result.A = new global::YuzuTest.Sample1[s.A.Length];
				for(int tmp1 = 0; tmp1 < s.A.Length; ++tmp1)
					result.A[tmp1] = Clone_YuzuTest__Sample1(cl, s.A[tmp1]);
			}
			return result;
		}

		private static global::YuzuTest.SampleBefore2 Clone_YuzuTest__SampleBefore2(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleBefore2)src;
			s.Before2();
			s.Before3();
			s.Before();
			var result = new global::YuzuTest.SampleBefore2();
			result.X = s.X;
			return result;
		}

		private static global::YuzuTest.SampleBeforeDeserialization Clone_YuzuTest__SampleBeforeDeserialization(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleBeforeDeserialization)src;
			var result = new global::YuzuTest.SampleBeforeDeserialization();
			result.Before();
			result.X = s.X;
			return result;
		}

		private static global::YuzuTest.SampleBeforeSerialization Clone_YuzuTest__SampleBeforeSerialization(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleBeforeSerialization)src;
			s.Before();
			var result = new global::YuzuTest.SampleBeforeSerialization();
			result.X = s.X;
			return result;
		}

		private static global::YuzuTest.SampleCopyable Clone_YuzuTest__SampleCopyable(Cloner cl, object src) =>
			(global::YuzuTest.SampleCopyable)src;

		private static global::YuzuTest.SampleDict Clone_YuzuTest__SampleDict(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleDict)src;
			var result = new global::YuzuTest.SampleDict();
			result.Value = s.Value;
			if (s.Children != null) {
				result.Children = new global::System.Collections.Generic.Dictionary<string, global::YuzuTest.SampleDict>();
				foreach (var tmp1 in s.Children)
					result.Children.Add(tmp1.Key, Clone_YuzuTest__SampleDict(cl, tmp1.Value));
			}
			return result;
		}

		private static global::YuzuTest.SampleDictKeys Clone_YuzuTest__SampleDictKeys(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleDictKeys)src;
			var result = new global::YuzuTest.SampleDictKeys();
			if (s.E != null) {
				result.E = new global::System.Collections.Generic.Dictionary<global::YuzuTest.SampleEnum, int>();
				foreach (var tmp1 in s.E)
					result.E.Add(tmp1.Key, tmp1.Value);
			}
			if (s.I != null) {
				result.I = new global::System.Collections.Generic.Dictionary<int, int>();
				foreach (var tmp2 in s.I)
					result.I.Add(tmp2.Key, tmp2.Value);
			}
			if (s.K != null) {
				result.K = new global::System.Collections.Generic.Dictionary<global::YuzuTest.SampleKey, int>();
				var tmp4 = cl.GetCloner<global::YuzuTest.SampleKey>();
				foreach (var tmp3 in s.K)
					result.K.Add((global::YuzuTest.SampleKey)tmp4(tmp3.Key), tmp3.Value);
			}
			return result;
		}

		private static global::YuzuTest.SampleGenNoGen Clone_YuzuTest__SampleGenNoGen(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleGenNoGen)src;
			var result = new global::YuzuTest.SampleGenNoGen();
			result.NG = (global::YuzuTest.SampleNoGen)cl.DeepObject(s.NG);
			return result;
		}

		private static global::YuzuTest.SampleItemObj Clone_YuzuTest__SampleItemObj(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleItemObj)src;
			var result = new global::YuzuTest.SampleItemObj();
			if (s.D != null) {
				result.D = new global::System.Collections.Generic.Dictionary<string, object>();
				foreach (var tmp1 in s.D)
					result.D.Add(tmp1.Key, cl.DeepObject(tmp1.Value));
			}
			if (s.L != null) {
				result.L = new global::System.Collections.Generic.List<object>();
				foreach (var tmp2 in s.L)
					result.L.Add(cl.DeepObject(tmp2));
			}
			return result;
		}

		private static global::YuzuTest.SampleList Clone_YuzuTest__SampleList(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleList)src;
			var result = new global::YuzuTest.SampleList();
			if (s.E != null) {
				result.E = new global::System.Collections.Generic.List<string>();
				foreach (var tmp1 in s.E)
					result.E.Add(tmp1);
			}
			return result;
		}

		private static global::YuzuTest.SampleMatrix Clone_YuzuTest__SampleMatrix(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleMatrix)src;
			var result = new global::YuzuTest.SampleMatrix();
			if (s.M != null) {
				result.M = new global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>>();
				var tmp2 = cl.GetCloner<global::System.Collections.Generic.List<int>>();
				foreach (var tmp1 in s.M)
					result.M.Add((global::System.Collections.Generic.List<int>)tmp2(tmp1));
			}
			return result;
		}

		private static global::YuzuTest.SampleMerge Clone_YuzuTest__SampleMerge(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleMerge)src;
			var result = new global::YuzuTest.SampleMerge();
			if (s.DI != null) {
				foreach (var tmp1 in s.DI)
					result.DI.Add(tmp1.Key, tmp1.Value);
			}
			if (s.LI != null) {
				foreach (var tmp2 in s.LI)
					result.LI.Add(tmp2);
			}
			cl.GetMerger<global::YuzuTest.Sample1>()(result.M, s.M);
			return result;
		}

		private static global::YuzuTest.SampleMergeNonPrimitive Clone_YuzuTest__SampleMergeNonPrimitive(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleMergeNonPrimitive)src;
			var result = new global::YuzuTest.SampleMergeNonPrimitive();
			if (s.DI != null) {
				foreach (var tmp1 in s.DI)
					result.DI.Add(tmp1.Key, Clone_YuzuTest__Sample1(cl, tmp1.Value));
			}
			if (s.LI != null) {
				foreach (var tmp2 in s.LI)
					result.LI.Add(Clone_YuzuTest__Sample1(cl, tmp2));
			}
			cl.GetMerger<global::YuzuTest.Sample1>()(result.M, s.M);
			return result;
		}

		private static global::YuzuTest.SampleNullable Clone_YuzuTest__SampleNullable(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleNullable)src;
			var result = new global::YuzuTest.SampleNullable();
			result.N = s.N;
			return result;
		}

		private static global::YuzuTest.SampleObj Clone_YuzuTest__SampleObj(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleObj)src;
			var result = new global::YuzuTest.SampleObj();
			result.F = cl.DeepObject(s.F);
			return result;
		}

		private static global::YuzuTest.SamplePerson Clone_YuzuTest__SamplePerson(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SamplePerson)src;
			var result = new global::YuzuTest.SamplePerson();
			result.Name = s.Name;
			result.Birth = s.Birth;
			if (s.Children != null) {
				result.Children = new global::System.Collections.Generic.List<global::YuzuTest.SamplePerson>();
				foreach (var tmp1 in s.Children)
					result.Children.Add(Clone_YuzuTest__SamplePerson(cl, tmp1));
			}
			result.EyeColor = Clone_YuzuTest__Color(cl, s.EyeColor);
			return result;
		}

		private static global::YuzuTest.SamplePoint Clone_YuzuTest__SamplePoint(Cloner cl, object src) =>
			(global::YuzuTest.SamplePoint)src;

		private static global::YuzuTest.SamplePrivateConstructor Clone_YuzuTest__SamplePrivateConstructor(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SamplePrivateConstructor)src;
			var result = global::YuzuTest.SamplePrivateConstructor.Make();
			result.X = s.X;
			return result;
		}

		private static global::YuzuTest.SampleRect Clone_YuzuTest__SampleRect(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleRect)src;
			var result = new global::YuzuTest.SampleRect();
			result.A = s.A;
			result.B = s.B;
			return result;
		}

		private static global::YuzuTest.SampleStructWithClass Clone_YuzuTest__SampleStructWithClass(Cloner cl, object src)
		{
			var s = (global::YuzuTest.SampleStructWithClass)src;
			var result = new global::YuzuTest.SampleStructWithClass();
			result.A = Clone_YuzuTest__Sample1(cl, s.A);
			return result;
		}

		private static object Clone_YuzuTest__SampleStructWithClass_obj(Cloner cl, object src) =>
			Clone_YuzuTest__SampleStructWithClass(cl, src);

		private static global::YuzuTest.SampleSurrogateColor Clone_YuzuTest__SampleSurrogateColor(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleSurrogateColor)src;
			var tmp1 = s.ToSurrogate();
			var result = global::YuzuTest.SampleSurrogateColor.FromSurrogate(tmp1);
			return result;
		}

		private static global::YuzuTest.SampleWithCopyable Clone_YuzuTest__SampleWithCopyable(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleWithCopyable)src;
			var result = new global::YuzuTest.SampleWithCopyable();
			result.P = s.P;
			return result;
		}

		static ClonerGen()
		{
			clonerCache[typeof(global::YuzuTest.Color)] = Clone_YuzuTest__Color;
			clonerCache[typeof(global::YuzuTest.Sample1)] = Clone_YuzuTest__Sample1;
			clonerCache[typeof(global::YuzuTest.Sample2)] = Clone_YuzuTest__Sample2;
			clonerCache[typeof(global::YuzuTest.Sample3)] = Clone_YuzuTest__Sample3;
			clonerCache[typeof(global::YuzuTest.SampleAfter2)] = Clone_YuzuTest__SampleAfter2;
			clonerCache[typeof(global::YuzuTest.SampleAfterDeserialization)] = Clone_YuzuTest__SampleAfterDeserialization;
			clonerCache[typeof(global::YuzuTest.SampleAfterSerialization)] = Clone_YuzuTest__SampleAfterSerialization;
			clonerCache[typeof(global::YuzuTest.SampleArray)] = Clone_YuzuTest__SampleArray;
			clonerCache[typeof(global::YuzuTest.SampleArrayOfClass)] = Clone_YuzuTest__SampleArrayOfClass;
			clonerCache[typeof(global::YuzuTest.SampleBefore2)] = Clone_YuzuTest__SampleBefore2;
			clonerCache[typeof(global::YuzuTest.SampleBeforeDeserialization)] = Clone_YuzuTest__SampleBeforeDeserialization;
			clonerCache[typeof(global::YuzuTest.SampleBeforeSerialization)] = Clone_YuzuTest__SampleBeforeSerialization;
			clonerCache[typeof(global::YuzuTest.SampleCopyable)] = ValueCopyCloner;
			clonerCache[typeof(global::YuzuTest.SampleDict)] = Clone_YuzuTest__SampleDict;
			clonerCache[typeof(global::YuzuTest.SampleDictKeys)] = Clone_YuzuTest__SampleDictKeys;
			clonerCache[typeof(global::YuzuTest.SampleGenNoGen)] = Clone_YuzuTest__SampleGenNoGen;
			clonerCache[typeof(global::YuzuTest.SampleItemObj)] = Clone_YuzuTest__SampleItemObj;
			clonerCache[typeof(global::YuzuTest.SampleList)] = Clone_YuzuTest__SampleList;
			clonerCache[typeof(global::YuzuTest.SampleMatrix)] = Clone_YuzuTest__SampleMatrix;
			clonerCache[typeof(global::YuzuTest.SampleMerge)] = Clone_YuzuTest__SampleMerge;
			clonerCache[typeof(global::YuzuTest.SampleMergeNonPrimitive)] = Clone_YuzuTest__SampleMergeNonPrimitive;
			clonerCache[typeof(global::YuzuTest.SampleNullable)] = Clone_YuzuTest__SampleNullable;
			clonerCache[typeof(global::YuzuTest.SampleObj)] = Clone_YuzuTest__SampleObj;
			clonerCache[typeof(global::YuzuTest.SamplePerson)] = Clone_YuzuTest__SamplePerson;
			clonerCache[typeof(global::YuzuTest.SamplePoint)] = ValueCopyCloner;
			clonerCache[typeof(global::YuzuTest.SamplePrivateConstructor)] = Clone_YuzuTest__SamplePrivateConstructor;
			clonerCache[typeof(global::YuzuTest.SampleRect)] = Clone_YuzuTest__SampleRect;
			clonerCache[typeof(global::YuzuTest.SampleStructWithClass)] = Clone_YuzuTest__SampleStructWithClass_obj;
			clonerCache[typeof(global::YuzuTest.SampleSurrogateColor)] = Clone_YuzuTest__SampleSurrogateColor;
			clonerCache[typeof(global::YuzuTest.SampleWithCopyable)] = Clone_YuzuTest__SampleWithCopyable;
		}
	}
}
