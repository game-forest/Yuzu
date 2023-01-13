// Copyright 2010 the V8 project authors. All rights reserved.
// Copyright 2011-2012, Kevin Ring. All rights reserved.
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
//       copyright notice, this list of conditions and the following
//       disclaimer in the documentation and/or other materials provided
//       with the distribution.
//     * Neither the name of Google Inc. nor the names of its
//       contributors may be used to endorse or promote products derived
//       from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Diagnostics;

namespace Yuzu.Grisu
{
	internal struct GrisuDouble
	{
		private const ulong KSignMask = 0x8000000000000000;
		private const ulong KExponentMask = 0x7FF0000000000000;
		private const ulong KSignificandMask = 0x000FFFFFFFFFFFFF;
		private const ulong KHiddenBit = 0x0010000000000000;
		private const int KPhysicalSignificandSize = 52;  // Excludes the hidden bit.
		private const int KSignificandSize = 53;

		private const int KExponentBias = 0x3FF + KPhysicalSignificandSize;
		private const int KDenormalExponent = -KExponentBias + 1;
		private const int KMaxExponent = 0x7FF - KExponentBias;
		private const ulong KInfinity = 0x7FF0000000000000;

		public GrisuDouble(double d)
		{
			value_ = d;
			d64_ = (ulong)BitConverter.DoubleToInt64Bits(d);
		}

		public GrisuDouble(ulong d64)
		{
			d64_ = d64;
			value_ = BitConverter.Int64BitsToDouble((long)d64);
		}

		public GrisuDouble(DiyFp diy_fp)
		{
			d64_ = DiyFpToUInt64(diy_fp);
			value_ = BitConverter.Int64BitsToDouble((long)d64_);
		}

		// The value encoded by this Double must be greater or equal to +0.0.
		// It must not be special (infinity, or NaN).
		public DiyFp AsDiyFp()
		{
			Debug.Assert(Sign > 0);
			Debug.Assert(!IsSpecial);
			return new DiyFp(Significand, Exponent);
		}

		// The value encoded by this Double must be strictly greater than 0.
		public DiyFp AsNormalizedDiyFp()
		{
			Debug.Assert(Value > 0.0);

			ulong d64 = d64_;
			ulong f;
			int e;
			if (IsDenormal) {
				f = d64 & KSignificandMask;
				e = KDenormalExponent;
			} else {
				f = (d64 & KSignificandMask) + KHiddenBit;
				e = (int)((d64 & KExponentMask) >> KPhysicalSignificandSize) - KExponentBias;
			}

			// The current double could be a denormal.
			while ((f & KHiddenBit) == 0) {
				f <<= 1;
				e--;
			}
			// Do the final shifts in one go.
			f <<= DiyFp.KSignificandSize - KSignificandSize;
			e -= DiyFp.KSignificandSize - KSignificandSize;
			return new DiyFp(f, e);
		}

		// Returns the double's bit as UInt64.
		public ulong AsUInt64()
		{
			return d64_;
		}

		public int Exponent
		{
			get
			{
				if (IsDenormal) {
					return KDenormalExponent;
				}

				int biased_e = (int)((d64_ & KExponentMask) >> KPhysicalSignificandSize);
				return biased_e - KExponentBias;
			}
		}

		public ulong Significand
		{
			get
			{
				ulong significand = d64_ & KSignificandMask;
				if (IsDenormal) {
					return significand;
				} else {
					return significand + KHiddenBit;
				}
			}
		}

		// Returns true if the double is a denormal.
		public bool IsDenormal => (d64_ & KExponentMask) == 0;

		// We consider denormals not to be special.
		// Hence only Infinity and NaN are special.
		public bool IsSpecial => (d64_ & KExponentMask) == KExponentMask;

		public bool IsNaN => ((d64_ & KExponentMask) == KExponentMask) && ((d64_ & KSignificandMask) != 0);

		public bool IsInfinite => ((d64_ & KExponentMask) == KExponentMask) && ((d64_ & KSignificandMask) == 0);

		public int Sign => (d64_ & KSignMask) == 0 ? 1 : -1;

		// Precondition: the value encoded by this Double must be greater or equal
		// than +0.0.
		public DiyFp UpperBoundary()
		{
			Debug.Assert(Sign > 0);
			return new DiyFp(Significand * 2 + 1, Exponent - 1);
		}

		// Computes the two boundaries of this.
		// The bigger boundary (m_plus) is normalized. The lower boundary has the same
		// exponent as m_plus.
		// Precondition: the value encoded by this Double must be greater than 0.
		public void NormalizedBoundaries(out DiyFp out_m_minus, out DiyFp out_m_plus)
		{
			Debug.Assert(Value > 0.0);

			ulong d64 = d64_;
			ulong vF;
			int vE;
			if (IsDenormal) {
				vF = d64 & KSignificandMask;
				vE = KDenormalExponent;
			} else {
				vF = (d64 & KSignificandMask) + KHiddenBit;
				vE = (int)((d64 & KExponentMask) >> KPhysicalSignificandSize) - KExponentBias;
			}

			ulong plusF = (vF << 1) + 1;
			int plusE = vE - 1;

			// This code is manually inlined from the GrisuDouble.Normalize() method,
			// because the .NET JIT (at least the 64-bit one as of version 4) is too
			// incompetent to do it itself.
			const ulong k10MSBits = 0xFFC0000000000000;
			const ulong kUint64MSB = 0x8000000000000000;
			while ((plusF & k10MSBits) == 0) {
				plusF <<= 10;
				plusE -= 10;
			}
			while ((plusF & kUint64MSB) == 0) {
				plusF <<= 1;
				plusE--;
			}

			ulong minusF;
			int minusE;
			bool significand_is_zero = vF == KHiddenBit;
			if (significand_is_zero && vE != KDenormalExponent) {
				// The boundary is closer. Think of v = 1000e10 and v- = 9999e9.
				// Then the boundary (== (v - v-)/2) is not just at a distance of 1e9 but
				// at a distance of 1e8.
				// The only exception is for the smallest normal: the largest denormal is
				// at the same distance as its successor.
				// Note: denormals have the same exponent as the smallest normals.
				minusF = (vF << 2) - 1;
				minusE = vE - 2;
			} else {
				minusF = (vF << 1) - 1;
				minusE = vE - 1;
			}
			out_m_minus = new DiyFp(minusF << (minusE - plusE), plusE);
			out_m_plus = new DiyFp(plusF, plusE);
		}

		public double Value => value_;

		// Returns the significand size for a given order of magnitude.
		// If v = f*2^e with 2^p-1 <= f <= 2^p then p+e is v's order of magnitude.
		// This function returns the number of significant binary digits v will have
		// once it's encoded into a double. In almost all cases this is equal to
		// kSignificandSize. The only exceptions are denormals. They start with
		// leading zeroes and their effective significand-size is hence smaller.
		public static int SignificandSizeForOrderOfMagnitude(int order)
		{
			if (order >= (KDenormalExponent + KSignificandSize)) {
				return KSignificandSize;
			}
			if (order <= KDenormalExponent) {
				return 0;
			}

			return order - KDenormalExponent;
		}

		public static double Infinity => double.PositiveInfinity;

		public static double NaN => double.NaN;

		private static ulong DiyFpToUInt64(DiyFp diy_fp)
		{
			ulong significand = diy_fp.F;
			int exponent = diy_fp.E;
			while (significand > KHiddenBit + KSignificandMask) {
				significand >>= 1;
				exponent++;
			}
			if (exponent >= KMaxExponent) {
				return KInfinity;
			}
			if (exponent < KDenormalExponent) {
				return 0;
			}
			while (exponent > KDenormalExponent && (significand & KHiddenBit) == 0) {
				significand <<= 1;
				exponent--;
			}
			ulong biased_exponent;
			if (exponent == KDenormalExponent && (significand & KHiddenBit) == 0) {
				biased_exponent = 0;
			} else {
				biased_exponent = (ulong)(exponent + KExponentBias);
			}
			return (significand & KSignificandMask) |
				(biased_exponent << KPhysicalSignificandSize);
		}

		private ulong d64_;
		private double value_;
	}

	internal struct GrisuSingle
	{
		private const uint KSignMask = 0x80000000;
		private const uint KExponentMask = 0x7F800000;
		private const uint KSignificandMask = 0x007FFFFF;
		private const uint KHiddenBit = 0x00800000;
		private const int KPhysicalSignificandSize = 23;  // Excludes the hidden bit.
		private const int KSignificandSize = 24;

		public GrisuSingle(float f) => d32_ = BitConverter.ToUInt32(BitConverter.GetBytes(f), 0);
		public GrisuSingle(uint d32) => d32_ = d32;

		// The value encoded by this Single must be greater or equal to +0.0.
		// It must not be special (infinity, or NaN).
		private DiyFp AsDiyFp
		{
			get
			{
				Debug.Assert(Sign > 0);
				Debug.Assert(!IsSpecial);
				return new DiyFp(Significand, Exponent);
			}
		}

		// Returns the single's bits as uint32.
		public uint AsUint32 => d32_;

		public int Exponent
		{
			get
			{
				if (IsDenormal) {
					return KDenormalExponent;
				}

				var biased_e = (int)((d32_ & KExponentMask) >> KPhysicalSignificandSize);
				return biased_e - KExponentBias;
			}
		}

		public uint Significand
		{
			get
			{
				uint significand = d32_ & KSignificandMask;
				return IsDenormal ? significand : significand + KHiddenBit;
			}
		}

		// Returns true if the single is a denormal.
		public bool IsDenormal => (d32_ & KExponentMask) == 0;

		// We consider denormals not to be special.
		// Hence only Infinity and NaN are special.
		public bool IsSpecial => (d32_ & KExponentMask) == KExponentMask;

		public bool IsNan => ((d32_ & KExponentMask) == KExponentMask) && ((d32_ & KSignificandMask) != 0);

		public bool IsInfinite => ((d32_ & KExponentMask) == KExponentMask) && ((d32_ & KSignificandMask) == 0);

		public int Sign => (d32_ & KSignMask) == 0 ? 1 : -1;

		// Computes the two boundaries of this.
		// The bigger boundary (m_plus) is normalized. The lower boundary has the same
		// exponent as m_plus.
		// Precondition: the value encoded by this Single must be greater than 0.
		public void NormalizedBoundaries(out DiyFp out_m_minus, out DiyFp out_m_plus)
		{
			Debug.Assert(Value > 0.0);
			DiyFp v = AsDiyFp;
			DiyFp m_plus = new DiyFp((v.F << 1) + 1, v.E - 1);
			m_plus.Normalize();
			DiyFp m_minus = LowerBoundaryIsCloser() ?
				new DiyFp((v.F << 2) - 1, v.E - 2) :
				new DiyFp((v.F << 1) - 1, v.E - 1);
			m_minus.F = m_minus.F << (m_minus.E - m_plus.E);
			m_minus.E = m_plus.E;
			out_m_plus = m_plus;
			out_m_minus = m_minus;
		}

		// Precondition: the value encoded by this Single must be greater or equal
		// than +0.0.
		private DiyFp UpperBoundary()
		{
			Debug.Assert(Sign > 0);
			return new DiyFp(Significand * 2 + 1, Exponent - 1);
		}

		private bool LowerBoundaryIsCloser()
		{
			// The boundary is closer if the significand is of the form f == 2^p-1 then
			// the lower boundary is closer.
			// Think of v = 1000e10 and v- = 9999e9.
			// Then the boundary (== (v - v-)/2) is not just at a distance of 1e9 but
			// at a distance of 1e8.
			// The only exception is for the smallest normal: the largest denormal is
			// at the same distance as its successor.
			// Note: denormals have the same exponent as the smallest normals.
			bool physical_significand_is_zero = (d32_ & KSignificandMask) == 0;
			return physical_significand_is_zero && (Exponent != KDenormalExponent);
		}

		public float Value { get { return BitConverter.ToSingle(BitConverter.GetBytes(d32_), 0); } }

		private const int KExponentBias = 0x7F + KPhysicalSignificandSize;
		private const int KDenormalExponent = -KExponentBias + 1;
		private const int KMaxExponent = 0xFF - KExponentBias;
		private const uint KInfinity = 0x7F800000;
		private const uint KNaN = 0x7FC00000;

		private uint d32_;
	}
}
