using System;

namespace JSG.PhotoPropertiesLibrary
{
	// //////////////////////////////////////////////////////////
	// Formatting a Rational into a Fraction
	// -------------------------------------
	// Modified from:
	//		Java Software Solutions: Foundations of Program Design e/3
	//		by John Lewis and William Loftus
	//		Published by Addison Wesley
	//		ISBN: 0-201-78129-8
	//		Copyright 2003
		
	/// <summary>Formatting for a signed fraction.</summary>
	public class Fraction {
		private Int32 _numer;
		private Int32 _denom;

		public Fraction(Int32 numer, Int32 denom) {
			_numer = numer;
			_denom = denom;
		}

		/// <summary>
		/// Converts the signed numerator and denominator values
		/// to its equivalent string representation.
		/// A fraction (x/y) is returned when applicable.</summary>
		public override string ToString() {
			Int32 numer = _numer;
			Int32 denom = (_denom == 0) ? 1 : _denom;

			// Make the numerator "store" the sign
			if (denom < 0) {
				numer = numer * -1;
				denom = denom * -1;
			}

			Reduce(ref numer, ref denom);

			string result;
			if (numer == 0)
				result = "0";
			else if (denom == 1)
				result = numer + "";
			else
				result = numer + "/" + denom;
    
			return result;
		}

		/// <summary>
		/// Reduces the rational number by dividing both the numerator
		/// and the denominator by their greatest common divisor.</summary>
		private static void Reduce(ref Int32 numer, ref Int32 denom) {
			if (numer != 0) {
				Int32 common = GCD(Math.Abs(numer), denom);

				numer = numer / common;
				denom = denom / common;
			}
		}

		/// <summary>
		/// Computes and returns the greatest common divisor of the two
		/// positive parameters. Uses Euclid's algorithm.</summary>
		private static Int32 GCD(Int32 num1, Int32 num2) {
			while (num1 != num2)
				if (num1 > num2)
					num1 = num1 - num2;
				else
					num2 = num2 - num1;

			return num1;
		}
	}

	/// <summary>Formatting for an unsigned fraction.</summary>
	public class UFraction {
		private UInt32 _numer;
		private UInt32 _denom;

		public UFraction(UInt32 numer, UInt32 denom) {
			_numer = numer;
			_denom = denom;
		}

		/// <summary>
		/// Converts the unsigned numerator and denominator values
		/// to its equivalent string representation.
		/// A fraction is used when applicable.</summary>
		public override string ToString() {
			UInt32 numer = _numer;
			UInt32 denom = (_denom == 0) ? 1 : _denom;

			Reduce(ref numer, ref denom);

			string result;
			if (numer == 0)
				result = "0";
			else if (denom == 1)
				result = numer + "";
			else
				result = numer + "/" + denom;
    
			return result;
		}

		/// <summary>
		/// Reduces the rational number by dividing both the numerator
		/// and the denominator by their greatest common divisor.</summary>
		private static void Reduce(ref UInt32 numer, ref UInt32 denom) {
			if (numer != 0) {
				UInt32 common = GCD(numer, denom);

				numer = numer / common;
				denom = denom / common;
			}
		}

		/// <summary>
		/// Computes and returns the greatest common divisor of the two
		/// positive parameters. Uses Euclid's algorithm.</summary>
		private static UInt32 GCD(UInt32 num1, UInt32 num2) {
			while (num1 != num2)
				if (num1 > num2)
					num1 = num1 - num2;
				else
					num2 = num2 - num1;

			return num1;
		}
	}
}
