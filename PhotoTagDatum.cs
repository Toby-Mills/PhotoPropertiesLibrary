using System;

namespace JSG.PhotoPropertiesLibrary {
	/// <summary>
	/// The PhotoTagDatum class is used to store an image's tag property data</summary>
	public class PhotoTagDatum : IComparable {
		private PhotoTagMetadata _tag;
		private int _id;
		private string _value;

		/// <summary>Used to sort (by Id)</summary>
		public int CompareTo(object obj) {
			PhotoTagDatum tagDatum = (PhotoTagDatum)obj;
			if (this._id == tagDatum.Id)
				return 0;
			else if (this._id < tagDatum.Id)
				return -1;
			else
				return 1;
		}

		/// <summary>Constructor</summary>
		/// <param name="id">The TagMetadata's id</param>
		/// <param name="tag">The particular PhotoTagMetadata instance</param>
		/// <param name="val">The value obtained from the image</param>
		public PhotoTagDatum(int id, PhotoTagMetadata tag, string val) {

			_id = id;
			_tag = tag;
			_value = val;
		}
		/// <summary>Get the Id value.</summary>
		public int Id { 
			get {
				return _id;
			}
		}
		/// <summary>Get the Category value.</summary>
		public string Category {
			get {
				return (_tag == null) ? String.Empty : _tag.Category;
			}
		}
		/// <summary>Get the Name value.</summary>
		public string Name {
			get {
				return (_tag == null) ? String.Empty : _tag.Name;
			}
		}
		/// <summary>Get the Description value.</summary>
		public string Description {
			get {
				return (_tag == null) ? String.Empty : _tag.Description;
			}
		}
		/// <summary>Get the Value as obtained from the image.</summary>
		/// <remarks>Partial formatting may be applied depending upon the 
		/// value of the PhotoTagMetadata's format instruction, FormatInstr.
		/// <newpara></newpara>
		/// For example the ExposureTime tag property (Id=33434) with 
		/// the FRACTION format instruction would format a Rational value 
		/// of (10 300) into "1/30".</remarks>
		public string Value { 
			get { 
				return _value;
			}
		}
		/// <summary>Get the Pretty Print Value.</summary>
		/// <remarks>The pretty print value is determined by ValueOptions
		/// that may exist in the PhotoTagMetadata's XML data.
		/// <newpara></newpara>
		/// For example the Flash tag property (Id=37385) provides options
		/// for raw data that transforms the value 1 to the pretty print
		/// value, "Flash fired".</remarks>
		public string PrettyPrintValue {
			get {
				return _tag.PrettyPrintValue(_value);
			}
		}
	}
}
