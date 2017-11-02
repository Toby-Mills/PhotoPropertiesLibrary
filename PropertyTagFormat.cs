using System;
using System.Drawing.Imaging;

namespace JSG.PhotoPropertiesLibrary {

	/// <summary>Provides formatting methods for property tag values.</summary>
	public sealed class PropertyTagFormat {

		/// <summary>
		/// Defines the DOUBLE format used when formatting Rational and 
		/// SRational Property Tag types.</summary>
		private const string DOUBLETYPE_FORMAT = "0.0####";

		/// <summary>Index byte jump for a PropertyItem's byte array of Shorts.</summary>
		private const int BYTEJUMP_SHORT		= 2;
		/// <summary>Index byte jump for a PropertyItem's byte array of Longs.</summary>
		private const int BYTEJUMP_LONG			= 4;
		/// <summary>Index byte jump for a PropertyItem's byte array of Rationals.</summary>
		private const int BYTEJUMP_RATIONAL		= 8;
		/// <summary>Index byte jump for a PropertyItem's byte array of SLongs.</summary>
		private const int BYTEJUMP_SLONG		= 4;
		/// <summary>Index byte jump for a PropertyItem's byte array of SRationals.</summary>
		private const int BYTEJUMP_SRATIONAL	= 8;

		// //////////////////////////////////////////////////////////
		// Image Property Tag Types 
		// //////////////////////////////////////////////////////////
		/// <summary>BYTE</summary>
		/// <remarks>An 8-bit unsigned integer.</remarks>
		private const int PropertyTagTypeByte        = 1;
		/// <summary>ASCII</summary>
		/// <remarks>An 8-bit byte containing one 7-bit ASCII code. 
		///          The final byte is terminated with NULL.</remarks>
		private const int PropertyTagTypeASCII       = 2;
		/// <summary>SHORT</summary>
		/// <remarks>A 16-bit (2-byte) unsigned integer</remarks>
		private const int PropertyTagTypeShort       = 3;
		/// <summary>LONG</summary>
		/// <remarks>A 32-bit (4-byte) unsigned integer</remarks>
		private const int PropertyTagTypeLong        = 4;
		/// <summary>RATIONAL</summary>
		/// <remarks>Two LONGs. 
		///          The first LONG is the numerator.
		///          The second LONG is the denominator.</remarks>
		private const int PropertyTagTypeRational    = 5;
		/// <summary>UNDEFINED</summary>
		/// <remarks>An 8-bit byte that can take any value 
		///          depending on the field definition</remarks>
		private const int PropertyTagTypeUndefined   = 7;
		/// <summary>SLONG</summary>
		/// <remarks>A 32-bit (4-byte) signed integer (2's complement notation)</remarks>
		private const int PropertyTagTypeSLong       = 9;
		/// <summary>SRATIONAL</summary>
		/// <remarks>Two SLONGs.
		///          The first SLONG is the numerator.
		///          The second SLONG is the denominator.</remarks>
		private const int PropertyTagTypeSRational   = 10;


		/// <summary>
		/// Formats a PropertyItem's byte data.</summary>
		/// <remarks>The format is based upon the data's tag type as well 
		/// as further information included in the item's PhotoTagMetadata.</remarks>
		/// <param name="propItem">A specific PropertyItem.</param>
		/// <param name="tagMetadata">A specific PhotoTagMetadata instance.</param>
		/// <returns>A string representation of the data.</returns>
		public static string FormatValue(PropertyItem propItem, PhotoTagMetadata tagMetadata) {

			if (propItem == null)
				return String.Empty;

			FormatInstr formatInstr;
			if (tagMetadata != null && tagMetadata.FormatInstrSpecified == true)
				formatInstr = tagMetadata.FormatInstr;
			else
				formatInstr = FormatInstr.NO_OP;

			string strRet;

			switch (propItem.Type) {
				case PropertyTagTypeByte:
					strRet = FormatTagByte(propItem, formatInstr);
					break;
				case PropertyTagTypeASCII:
					strRet = FormatTagAscii(propItem, formatInstr);
					break;
				case PropertyTagTypeShort:
					strRet = FormatTagShort(propItem, formatInstr);
					break;
				case PropertyTagTypeLong:
					strRet = FormatTagLong(propItem, formatInstr);
					break;
				case PropertyTagTypeRational:
					strRet = FormatTagRational(propItem, formatInstr);
					break;
				case PropertyTagTypeUndefined:
					strRet = FormatTagUndefined(propItem, formatInstr);
					break;
				case PropertyTagTypeSLong:
					strRet = FormatTagSLong(propItem, formatInstr);
					break;
				case PropertyTagTypeSRational:
					strRet = FormatTagSRational(propItem, formatInstr);
					break;
				default:
					strRet = "";
					break;
			}
			return strRet;
		}

		/// <summary>Format a Byte tag.</summary>
		private static string FormatTagByte(PropertyItem propItem, FormatInstr formatInstr) {
			string strRet;
			if (formatInstr == FormatInstr.BASE64)
				strRet = Convert.ToBase64String(propItem.Value);
			else
				strRet = BitConverter.ToString(propItem.Value, 0, propItem.Len);
			return strRet;
		}

		/// <summary>Format an ASCII tag.</summary>
		private static string FormatTagAscii(PropertyItem propItem, FormatInstr formatInstr) {
			string strRet;
			System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
			strRet = encoding.GetString(propItem.Value, 0, propItem.Len - 1);

			return strRet;
		}

		/// <summary>Format a Short tag (unsigned).</summary>
		private static string FormatTagShort(PropertyItem propItem, FormatInstr formatInstr) {
			string strRet = "";
			for (int i = 0; i < propItem.Len; i = i + BYTEJUMP_SHORT) {
				System.UInt16 val = BitConverter.ToUInt16(propItem.Value, i);
				strRet += val.ToString();
				if (i + BYTEJUMP_SHORT < propItem.Len)
					strRet += " ";
			}
			return strRet;
		}

		/// <summary>Format a Long tag (unsigned).</summary>
		private static string FormatTagLong(PropertyItem propItem, FormatInstr formatInstr) {
			string strRet = "";
			for (int i = 0; i < propItem.Len; i = i + BYTEJUMP_LONG) {
				System.UInt32 val = BitConverter.ToUInt32(propItem.Value, i);
				strRet += val.ToString();
				if (i + BYTEJUMP_LONG < propItem.Len)
					strRet += " ";
			}
			return strRet;
		}

		/// <summary>Format a Rational tag (unsigned).</summary>
		private static string FormatTagRational(PropertyItem propItem, FormatInstr formatInstr) {
			string strRet = "";
			for (int i = 0; i < propItem.Len; i = i + BYTEJUMP_RATIONAL) {
				System.UInt32 numer = BitConverter.ToUInt32(propItem.Value, i);
				System.UInt32 denom = BitConverter.ToUInt32(propItem.Value, i + BYTEJUMP_LONG);
				if (formatInstr == FormatInstr.FRACTION) {
					UFraction frac = new UFraction(numer, denom);
					strRet += frac.ToString();
				}
				else {
					double dbl;
					if (denom  == 0)
						dbl = 0.0;
					else
						dbl = (double)numer / (double)denom;
					strRet += dbl.ToString(DOUBLETYPE_FORMAT);
				}
				if (i + BYTEJUMP_RATIONAL < propItem.Len)
					strRet += " ";
			}
			return strRet;
		}

		/// <summary>Format a Undefined tag.</summary>
		private static string FormatTagUndefined(PropertyItem propItem, FormatInstr formatInstr) {
			string strRet;
			if (formatInstr == FormatInstr.ALLCHAR) {
				System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
				strRet = encoding.GetString(propItem.Value, 0, propItem.Len);
			}
			else
				strRet = BitConverter.ToString(propItem.Value, 0, propItem.Len);

			return strRet;
		}

		/// <summary>Format a SLong tag (signed).</summary>
		private static string FormatTagSLong(PropertyItem propItem, FormatInstr formatInstr) {
			string strRet = "";
			for (int i = 0; i < propItem.Len; i = i + BYTEJUMP_SLONG) {
				System.Int32 val = BitConverter.ToInt32(propItem.Value, i);
				strRet += val.ToString();
				if (i + BYTEJUMP_SLONG < propItem.Len)
					strRet += " ";
			}
			return strRet;
		}

		/// <summary>Format a SRational tag (signed).</summary>
		private static string FormatTagSRational(PropertyItem propItem, FormatInstr formatInstr) {
			string strRet = "";
			for (int i = 0; i < propItem.Len; i = i + BYTEJUMP_SRATIONAL) {
				System.Int32 numer = BitConverter.ToInt32(propItem.Value, i);
				System.Int32 denom = BitConverter.ToInt32(propItem.Value, i + BYTEJUMP_SLONG);
				if (formatInstr == FormatInstr.FRACTION) {
					Fraction frac = new Fraction(numer, denom);
					strRet += frac.ToString();
				}
				else {
					double dbl;
					if (denom  == 0)
						dbl = 0.0;
					else
						dbl = (double)numer / (double)denom;
					strRet += dbl.ToString(DOUBLETYPE_FORMAT);
				}
				if (i + BYTEJUMP_SRATIONAL < propItem.Len)
					strRet += " ";
			}
			return strRet;
		}
	}
}