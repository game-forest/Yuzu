using System;

using Yuzu.Clone;

namespace YuzuGenClone
{
	public class ClonerGen: ClonerGenBase
	{
		protected static global::YuzuTest.Color Clone_YuzuTest__Color(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.Color))
				return (global::YuzuTest.Color)cl.DeepObject(src);
			var s = (global::YuzuTest.Color)src;
			var result = new global::YuzuTest.Color();
			result.B = s.B;
			result.G = s.G;
			result.R = s.R;
			return result;
		}

		protected static global::YuzuTest.TestReferences.Node Clone_YuzuTest__TestReferences__Node(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.TestReferences.Node))
				return (global::YuzuTest.TestReferences.Node)cl.DeepObject(src);
			var s = (global::YuzuTest.TestReferences.Node)src;
			var result = new global::YuzuTest.TestReferences.Node();
			result.Id = s.Id;
			if (s.Nodes != null && result.Nodes != null) {
				foreach (var tmp1 in s.Nodes)
					result.Nodes.Add(Clone_YuzuTest__TestReferences__Node(cl, tmp1));
			}
			return result;
		}

		protected static global::YuzuTest.Sample1 Clone_YuzuTest__Sample1(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.Sample1))
				return (global::YuzuTest.Sample1)cl.DeepObject(src);
			var s = (global::YuzuTest.Sample1)src;
			var result = new global::YuzuTest.Sample1();
			result.X = s.X;
			if (s.Y != "ttt") {
				result.Y = s.Y;
			}
			return result;
		}

		protected static global::YuzuTest.Sample2 Clone_YuzuTest__Sample2(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.Sample2))
				return (global::YuzuTest.Sample2)cl.DeepObject(src);
			var s = (global::YuzuTest.Sample2)src;
			var result = new global::YuzuTest.Sample2();
			result.X = s.X;
			if (s.SaveYIf()) {
				result.Y = s.Y;
			}
			return result;
		}

		private static global::YuzuTest.A.B.C.D.E.Sample2Struct Clone_YuzuTest__A__B__C__D__E__Sample2Struct(Cloner cl, object src) =>
			(global::YuzuTest.A.B.C.D.E.Sample2Struct)src;

		protected static global::YuzuTest.Sample3 Clone_YuzuTest__Sample3(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.Sample3))
				return (global::YuzuTest.Sample3)cl.DeepObject(src);
			var s = (global::YuzuTest.Sample3)src;
			var result = new global::YuzuTest.Sample3();
			result.S1 = Clone_YuzuTest__Sample1(cl, s.S1);
			result.F = s.F;
			result.S2 = Clone_YuzuTest__Sample2(cl, s.S2);
			return result;
		}

		protected static global::YuzuTest.SampleAfter2 Clone_YuzuTest__SampleAfter2(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleAfter2))
				return (global::YuzuTest.SampleAfter2)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleAfter2)src;
			var result = new global::YuzuTest.SampleAfter2();
			result.X = s.X;
			result.After2();
			result.After3();
			result.After();
			return result;
		}

		protected static global::YuzuTest.SampleAfterDeserialization Clone_YuzuTest__SampleAfterDeserialization(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleAfterDeserialization))
				return (global::YuzuTest.SampleAfterDeserialization)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleAfterDeserialization)src;
			var result = new global::YuzuTest.SampleAfterDeserialization();
			result.X = s.X;
			result.After();
			return result;
		}

		protected static global::YuzuTest.SampleAfterSerialization Clone_YuzuTest__SampleAfterSerialization(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleAfterSerialization))
				return (global::YuzuTest.SampleAfterSerialization)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleAfterSerialization)src;
			var result = new global::YuzuTest.SampleAfterSerialization();
			result.X = s.X;
			s.After();
			return result;
		}

		protected static global::YuzuTest.SampleArray Clone_YuzuTest__SampleArray(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleArray))
				return (global::YuzuTest.SampleArray)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleArray)src;
			var result = new global::YuzuTest.SampleArray();
			if (s.A != null) {
				result.A = (string[])s.A.Clone();
			}
			return result;
		}

		protected static global::YuzuTest.SampleArrayNDim Clone_YuzuTest__SampleArrayNDim(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleArrayNDim))
				return (global::YuzuTest.SampleArrayNDim)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleArrayNDim)src;
			var result = new global::YuzuTest.SampleArrayNDim();
			if (s.A != null) {
				result.A = (int[,])s.A.Clone();
			}
			if (s.B != null) {
				result.B = (string[,,])s.B.Clone();
			}
			return result;
		}

		protected static global::YuzuTest.SampleArrayNDimOfClass Clone_YuzuTest__SampleArrayNDimOfClass(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleArrayNDimOfClass))
				return (global::YuzuTest.SampleArrayNDimOfClass)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleArrayNDimOfClass)src;
			var result = new global::YuzuTest.SampleArrayNDimOfClass();
			if (s.A != null) {
				result.A = (global::YuzuTest.Sample1[,,])Array.CreateInstance(typeof(global::YuzuTest.Sample1),
					new int[] { s.A.GetLength(0), s.A.GetLength(1), s.A.GetLength(2) },
					new int[] { s.A.GetLowerBound(0), s.A.GetLowerBound(1), s.A.GetLowerBound(2) });
				for(int tmp1 = s.A.GetLowerBound(0); tmp1 <= s.A.GetLowerBound(0); ++tmp1) {
					for(int tmp2 = s.A.GetLowerBound(1); tmp2 <= s.A.GetLowerBound(1); ++tmp2) {
						for(int tmp3 = s.A.GetLowerBound(2); tmp3 <= s.A.GetLowerBound(2); ++tmp3) {
							result.A[tmp1, tmp2, tmp3] = Clone_YuzuTest__Sample1(cl, s.A[tmp1, tmp2, tmp3]);
						}
					}
				}
			}
			return result;
		}

		protected static global::YuzuTest.SampleArrayOfClass Clone_YuzuTest__SampleArrayOfClass(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleArrayOfClass))
				return (global::YuzuTest.SampleArrayOfClass)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleArrayOfClass)src;
			var result = new global::YuzuTest.SampleArrayOfClass();
			if (s.A != null) {
				result.A = new global::YuzuTest.Sample1[s.A.Length];
				for(int tmp1 = 0; tmp1 < s.A.Length; ++tmp1)
					result.A[tmp1] = Clone_YuzuTest__Sample1(cl, s.A[tmp1]);
			}
			return result;
		}

		protected static global::YuzuTest.SampleBase Clone_YuzuTest__SampleBase(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleBase))
				return (global::YuzuTest.SampleBase)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleBase)src;
			var result = new global::YuzuTest.SampleBase();
			result.FBase = s.FBase;
			return result;
		}

		protected static global::YuzuTest.SampleBefore2 Clone_YuzuTest__SampleBefore2(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleBefore2))
				return (global::YuzuTest.SampleBefore2)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleBefore2)src;
			s.Before2();
			s.Before3();
			s.Before();
			var result = new global::YuzuTest.SampleBefore2();
			result.X = s.X;
			return result;
		}

		protected static global::YuzuTest.SampleBeforeDeserialization Clone_YuzuTest__SampleBeforeDeserialization(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleBeforeDeserialization))
				return (global::YuzuTest.SampleBeforeDeserialization)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleBeforeDeserialization)src;
			var result = new global::YuzuTest.SampleBeforeDeserialization();
			result.Before();
			result.X = s.X;
			return result;
		}

		protected static global::YuzuTest.SampleBeforeSerialization Clone_YuzuTest__SampleBeforeSerialization(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleBeforeSerialization))
				return (global::YuzuTest.SampleBeforeSerialization)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleBeforeSerialization)src;
			s.Before();
			var result = new global::YuzuTest.SampleBeforeSerialization();
			result.X = s.X;
			return result;
		}

		protected static global::YuzuTest.SampleCollection<int> Clone_YuzuTest__SampleCollection_Int32(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleCollection<int>))
				return (global::YuzuTest.SampleCollection<int>)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleCollection<int>)src;
			var result = new global::YuzuTest.SampleCollection<int>();
			int tmp2 = 0;
			foreach (var tmp1 in s) {
				if (s.SaveItemIf(tmp2++, tmp1))
					result.Add(tmp1);
			}
			return result;
		}

		protected static global::YuzuTest.SampleCollection<global::YuzuTest.Sample1> Clone_YuzuTest__SampleCollection_Sample1(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleCollection<global::YuzuTest.Sample1>))
				return (global::YuzuTest.SampleCollection<global::YuzuTest.Sample1>)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleCollection<global::YuzuTest.Sample1>)src;
			var result = new global::YuzuTest.SampleCollection<global::YuzuTest.Sample1>();
			int tmp2 = 0;
			foreach (var tmp1 in s) {
				if (s.SaveItemIf(tmp2++, tmp1))
					result.Add(Clone_YuzuTest__Sample1(cl, tmp1));
			}
			return result;
		}

		private static global::YuzuTest.SampleCopyable Clone_YuzuTest__SampleCopyable(Cloner cl, object src) =>
			(global::YuzuTest.SampleCopyable)src;

		protected static global::YuzuTest.SampleDerivedA Clone_YuzuTest__SampleDerivedA(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleDerivedA))
				return (global::YuzuTest.SampleDerivedA)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleDerivedA)src;
			var result = new global::YuzuTest.SampleDerivedA();
			result.FBase = s.FBase;
			result.FA = s.FA;
			return result;
		}

		protected static global::YuzuTest.SampleDerivedB Clone_YuzuTest__SampleDerivedB(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleDerivedB))
				return (global::YuzuTest.SampleDerivedB)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleDerivedB)src;
			var result = new global::YuzuTest.SampleDerivedB();
			result.FBase = s.FBase;
			result.FB = s.FB;
			return result;
		}

		protected static global::YuzuTest.SampleDict Clone_YuzuTest__SampleDict(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleDict))
				return (global::YuzuTest.SampleDict)cl.DeepObject(src);
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

		protected static global::YuzuTest.SampleDictKeys Clone_YuzuTest__SampleDictKeys(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleDictKeys))
				return (global::YuzuTest.SampleDictKeys)cl.DeepObject(src);
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

		protected static global::YuzuTest.SampleExplicitCollection<int> Clone_YuzuTest__SampleExplicitCollection_Int32(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleExplicitCollection<int>))
				return (global::YuzuTest.SampleExplicitCollection<int>)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleExplicitCollection<int>)src;
			var result = new global::YuzuTest.SampleExplicitCollection<int>();
			foreach (var tmp1 in s)
				((global::System.Collections.Generic.ICollection<int>)result).Add(tmp1);
			return result;
		}

		protected static global::YuzuTest.SampleGenNoGen Clone_YuzuTest__SampleGenNoGen(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleGenNoGen))
				return (global::YuzuTest.SampleGenNoGen)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleGenNoGen)src;
			var result = new global::YuzuTest.SampleGenNoGen();
			result.NG = (global::YuzuTest.SampleNoGen)cl.DeepObject(s.NG);
			return result;
		}

		protected static global::YuzuTest.SampleItemObj Clone_YuzuTest__SampleItemObj(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleItemObj))
				return (global::YuzuTest.SampleItemObj)cl.DeepObject(src);
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

		protected static global::YuzuTest.SampleList Clone_YuzuTest__SampleList(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleList))
				return (global::YuzuTest.SampleList)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleList)src;
			var result = new global::YuzuTest.SampleList();
			if (s.E != null) {
				result.E = new global::System.Collections.Generic.List<string>();
				foreach (var tmp1 in s.E)
					result.E.Add(tmp1);
			}
			return result;
		}

		protected static global::YuzuTest.SampleMatrix Clone_YuzuTest__SampleMatrix(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleMatrix))
				return (global::YuzuTest.SampleMatrix)cl.DeepObject(src);
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

		protected static global::YuzuTest.SampleMerge Clone_YuzuTest__SampleMerge(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleMerge))
				return (global::YuzuTest.SampleMerge)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleMerge)src;
			var result = new global::YuzuTest.SampleMerge();
			if (s.DI != null) {
				foreach (var tmp1 in s.DI)
					result.DI.Add(tmp1.Key, tmp1.Value);
			}
			if (s.LI != null && result.LI != null) {
				foreach (var tmp2 in s.LI)
					result.LI.Add(tmp2);
			}
			cl.GetMerger<global::YuzuTest.Sample1>()(result.M, s.M);
			return result;
		}

		protected static global::YuzuTest.SampleMergeNonPrimitive Clone_YuzuTest__SampleMergeNonPrimitive(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleMergeNonPrimitive))
				return (global::YuzuTest.SampleMergeNonPrimitive)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleMergeNonPrimitive)src;
			var result = new global::YuzuTest.SampleMergeNonPrimitive();
			if (s.DI != null) {
				foreach (var tmp1 in s.DI)
					result.DI.Add(tmp1.Key, Clone_YuzuTest__Sample1(cl, tmp1.Value));
			}
			if (s.LI != null && result.LI != null) {
				foreach (var tmp2 in s.LI)
					result.LI.Add(Clone_YuzuTest__Sample1(cl, tmp2));
			}
			cl.GetMerger<global::YuzuTest.Sample1>()(result.M, s.M);
			return result;
		}

		protected static global::YuzuTest.SampleNullable Clone_YuzuTest__SampleNullable(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleNullable))
				return (global::YuzuTest.SampleNullable)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleNullable)src;
			var result = new global::YuzuTest.SampleNullable();
			result.N = s.N;
			return result;
		}

		protected static global::YuzuTest.SampleObj Clone_YuzuTest__SampleObj(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleObj))
				return (global::YuzuTest.SampleObj)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleObj)src;
			var result = new global::YuzuTest.SampleObj();
			result.F = cl.DeepObject(s.F);
			return result;
		}

		protected static global::YuzuTest.SamplePerson Clone_YuzuTest__SamplePerson(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SamplePerson))
				return (global::YuzuTest.SamplePerson)cl.DeepObject(src);
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

		protected static global::YuzuTest.SamplePrivateConstructor Clone_YuzuTest__SamplePrivateConstructor(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SamplePrivateConstructor))
				return (global::YuzuTest.SamplePrivateConstructor)cl.DeepObject(src);
			var s = (global::YuzuTest.SamplePrivateConstructor)src;
			var result = global::YuzuTest.SamplePrivateConstructor.Make();
			result.X = s.X;
			return result;
		}

		protected static global::YuzuTest.SampleRect Clone_YuzuTest__SampleRect(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleRect))
				return (global::YuzuTest.SampleRect)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleRect)src;
			var result = new global::YuzuTest.SampleRect();
			result.A = s.A;
			result.B = s.B;
			return result;
		}

		protected static global::YuzuTest.SampleSealed Clone_YuzuTest__SampleSealed(Cloner cl, object src)
		{
			if (src == null) return null;
			var s = (global::YuzuTest.SampleSealed)src;
			var result = new global::YuzuTest.SampleSealed();
			result.FB = s.FB;
			return result;
		}

		protected static global::YuzuTest.SampleSerializeIf Clone_YuzuTest__SampleSerializeIf(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleSerializeIf))
				return (global::YuzuTest.SampleSerializeIf)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleSerializeIf)src;
			var result = new global::YuzuTest.SampleSerializeIf();
			result.X = s.X;
			if (s.SaveYIf()) {
				result.Y = Clone_YuzuTest__Sample1(cl, s.Y);
			}
			return result;
		}

		protected static global::YuzuTest.SampleSerializeIfOnField Clone_YuzuTest__SampleSerializeIfOnField(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleSerializeIfOnField))
				return (global::YuzuTest.SampleSerializeIfOnField)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleSerializeIfOnField)src;
			var result = new global::YuzuTest.SampleSerializeIfOnField();
			if (s.SaveYIf()) {
				result.W = Clone_YuzuTest__Sample1(cl, s.W);
			}
			result.X = s.X;
			if (s.SaveYIf()) {
				result.Y = Clone_YuzuTest__Sample1(cl, s.Y);
			}
			if (s.SaveYIf()) {
				result.Z = Clone_YuzuTest__Sample1(cl, s.Z);
			}
			return result;
		}

		protected static global::YuzuTest.A.B.C.D.E.SampleSerializeIfOnFieldStruct Clone_YuzuTest__A__B__C__D__E__SampleSerializeIfOnFieldStruct(Cloner cl, object src)
		{
			var s = (global::YuzuTest.A.B.C.D.E.SampleSerializeIfOnFieldStruct)src;
			var result = new global::YuzuTest.A.B.C.D.E.SampleSerializeIfOnFieldStruct();
			if (s.SaveYIf()) {
				result.W = Clone_YuzuTest__Sample1(cl, s.W);
			}
			result.X = s.X;
			if (s.SaveYIf()) {
				result.Y = Clone_YuzuTest__Sample1(cl, s.Y);
			}
			if (s.SaveYIf()) {
				result.Z = Clone_YuzuTest__Sample1(cl, s.Z);
			}
			return result;
		}

		private static object Clone_YuzuTest__A__B__C__D__E__SampleSerializeIfOnFieldStruct_obj(Cloner cl, object src) =>
			Clone_YuzuTest__A__B__C__D__E__SampleSerializeIfOnFieldStruct(cl, src);

		protected static global::YuzuTest.A.B.C.D.E.SampleSerializeIfStruct Clone_YuzuTest__A__B__C__D__E__SampleSerializeIfStruct(Cloner cl, object src)
		{
			var s = (global::YuzuTest.A.B.C.D.E.SampleSerializeIfStruct)src;
			var result = new global::YuzuTest.A.B.C.D.E.SampleSerializeIfStruct();
			result.X = s.X;
			if (s.SaveYIf()) {
				result.Y = Clone_YuzuTest__Sample1(cl, s.Y);
			}
			return result;
		}

		private static object Clone_YuzuTest__A__B__C__D__E__SampleSerializeIfStruct_obj(Cloner cl, object src) =>
			Clone_YuzuTest__A__B__C__D__E__SampleSerializeIfStruct(cl, src);

		protected static global::YuzuTest.SampleStructWithClass Clone_YuzuTest__SampleStructWithClass(Cloner cl, object src)
		{
			var s = (global::YuzuTest.SampleStructWithClass)src;
			var result = new global::YuzuTest.SampleStructWithClass();
			result.A = Clone_YuzuTest__Sample1(cl, s.A);
			return result;
		}

		private static object Clone_YuzuTest__SampleStructWithClass_obj(Cloner cl, object src) =>
			Clone_YuzuTest__SampleStructWithClass(cl, src);

		protected static global::YuzuTest.SampleSurrogateColor Clone_YuzuTest__SampleSurrogateColor(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleSurrogateColor))
				return (global::YuzuTest.SampleSurrogateColor)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleSurrogateColor)src;
			var tmp1 = s.ToSurrogate();
			var result = global::YuzuTest.SampleSurrogateColor.FromSurrogate(tmp1);
			return result;
		}

		protected static global::YuzuTest.SampleWithCollectionMerge Clone_YuzuTest__SampleWithCollectionMerge(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleWithCollectionMerge))
				return (global::YuzuTest.SampleWithCollectionMerge)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleWithCollectionMerge)src;
			var result = new global::YuzuTest.SampleWithCollectionMerge();
			if (s.A != null && result.A != null) {
				int tmp2 = 0;
				foreach (var tmp1 in s.A) {
					if (s.A.SaveItemIf(tmp2++, tmp1))
						result.A.Add(tmp1);
				}
			}
			return result;
		}

		protected static global::YuzuTest.SampleWithCopyable Clone_YuzuTest__SampleWithCopyable(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleWithCopyable))
				return (global::YuzuTest.SampleWithCopyable)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleWithCopyable)src;
			var result = new global::YuzuTest.SampleWithCopyable();
			result.P = s.P;
			return result;
		}

		protected static global::YuzuTest.SampleWithCopyableItems Clone_YuzuTest__SampleWithCopyableItems(Cloner cl, object src)
		{
			if (src == null) return null;
			if (src.GetType() != typeof(global::YuzuTest.SampleWithCopyableItems))
				return (global::YuzuTest.SampleWithCopyableItems)cl.DeepObject(src);
			var s = (global::YuzuTest.SampleWithCopyableItems)src;
			var result = new global::YuzuTest.SampleWithCopyableItems();
			result.L = s.L;
			result.P = s.P;
			return result;
		}

		static ClonerGen()
		{
			clonerCache[typeof(global::YuzuTest.Color)] = Clone_YuzuTest__Color;
			clonerCache[typeof(global::YuzuTest.TestReferences.Node)] = Clone_YuzuTest__TestReferences__Node;
			clonerCache[typeof(global::YuzuTest.Sample1)] = Clone_YuzuTest__Sample1;
			clonerCache[typeof(global::YuzuTest.Sample2)] = Clone_YuzuTest__Sample2;
			clonerCache[typeof(global::YuzuTest.A.B.C.D.E.Sample2Struct)] = ValueCopyCloner;
			clonerCache[typeof(global::YuzuTest.Sample3)] = Clone_YuzuTest__Sample3;
			clonerCache[typeof(global::YuzuTest.SampleAfter2)] = Clone_YuzuTest__SampleAfter2;
			clonerCache[typeof(global::YuzuTest.SampleAfterDeserialization)] = Clone_YuzuTest__SampleAfterDeserialization;
			clonerCache[typeof(global::YuzuTest.SampleAfterSerialization)] = Clone_YuzuTest__SampleAfterSerialization;
			clonerCache[typeof(global::YuzuTest.SampleArray)] = Clone_YuzuTest__SampleArray;
			clonerCache[typeof(global::YuzuTest.SampleArrayNDim)] = Clone_YuzuTest__SampleArrayNDim;
			clonerCache[typeof(global::YuzuTest.SampleArrayNDimOfClass)] = Clone_YuzuTest__SampleArrayNDimOfClass;
			clonerCache[typeof(global::YuzuTest.SampleArrayOfClass)] = Clone_YuzuTest__SampleArrayOfClass;
			clonerCache[typeof(global::YuzuTest.SampleBase)] = Clone_YuzuTest__SampleBase;
			clonerCache[typeof(global::YuzuTest.SampleBefore2)] = Clone_YuzuTest__SampleBefore2;
			clonerCache[typeof(global::YuzuTest.SampleBeforeDeserialization)] = Clone_YuzuTest__SampleBeforeDeserialization;
			clonerCache[typeof(global::YuzuTest.SampleBeforeSerialization)] = Clone_YuzuTest__SampleBeforeSerialization;
			clonerCache[typeof(global::YuzuTest.SampleCollection<int>)] = Clone_YuzuTest__SampleCollection_Int32;
			clonerCache[typeof(global::YuzuTest.SampleCollection<global::YuzuTest.Sample1>)] = Clone_YuzuTest__SampleCollection_Sample1;
			clonerCache[typeof(global::YuzuTest.SampleCopyable)] = ValueCopyCloner;
			clonerCache[typeof(global::YuzuTest.SampleDerivedA)] = Clone_YuzuTest__SampleDerivedA;
			clonerCache[typeof(global::YuzuTest.SampleDerivedB)] = Clone_YuzuTest__SampleDerivedB;
			clonerCache[typeof(global::YuzuTest.SampleDict)] = Clone_YuzuTest__SampleDict;
			clonerCache[typeof(global::YuzuTest.SampleDictKeys)] = Clone_YuzuTest__SampleDictKeys;
			clonerCache[typeof(global::YuzuTest.SampleExplicitCollection<int>)] = Clone_YuzuTest__SampleExplicitCollection_Int32;
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
			clonerCache[typeof(global::YuzuTest.SampleSealed)] = Clone_YuzuTest__SampleSealed;
			clonerCache[typeof(global::YuzuTest.SampleSerializeIf)] = Clone_YuzuTest__SampleSerializeIf;
			clonerCache[typeof(global::YuzuTest.SampleSerializeIfOnField)] = Clone_YuzuTest__SampleSerializeIfOnField;
			clonerCache[typeof(global::YuzuTest.A.B.C.D.E.SampleSerializeIfOnFieldStruct)] = Clone_YuzuTest__A__B__C__D__E__SampleSerializeIfOnFieldStruct_obj;
			clonerCache[typeof(global::YuzuTest.A.B.C.D.E.SampleSerializeIfStruct)] = Clone_YuzuTest__A__B__C__D__E__SampleSerializeIfStruct_obj;
			clonerCache[typeof(global::YuzuTest.SampleStructWithClass)] = Clone_YuzuTest__SampleStructWithClass_obj;
			clonerCache[typeof(global::YuzuTest.SampleSurrogateColor)] = Clone_YuzuTest__SampleSurrogateColor;
			clonerCache[typeof(global::YuzuTest.SampleWithCollectionMerge)] = Clone_YuzuTest__SampleWithCollectionMerge;
			clonerCache[typeof(global::YuzuTest.SampleWithCopyable)] = Clone_YuzuTest__SampleWithCopyable;
			clonerCache[typeof(global::YuzuTest.SampleWithCopyableItems)] = Clone_YuzuTest__SampleWithCopyableItems;
		}
	}
}
