using System;
using System.Collections;
using System.Runtime.Serialization;

namespace JSG.PhotoPropertiesLibrary {

	/// <summary>
	/// The generic PhotoProperties Exception class.</summary>
	public class PhotoPropertiesException : System.ApplicationException {

		/// <summary>Generic PhotoProperties Exception.</summary>
		public PhotoPropertiesException(String message) 
			: base(message) {
		}
	}

	/// <summary>
	/// Exception thrown within PhotoProperties.Initialize.  It contains the 
	/// cumulative error event data that may occur during the loading 
	/// of the TagProperty Data.
	/// </summary>
	public sealed class PhotoPropertiesLoadDataException : ApplicationException {
		/// <summary>Collection of errors.</summary>
		/// <remarks>This list is generated when loading the tag property data.</remarks>
		private Array _arError;

		/// <summary>Initializes a new instance of the PhotoPropertiesLoadDataException class 
		/// with a specified error message.</summary>
		public PhotoPropertiesLoadDataException(String message) 
			: base(message) {
	}
		/// <summary>Initializes a new instance of the PhotoPropertiesLoadDataException class 
		/// with a collection of error data.</summary>
		public PhotoPropertiesLoadDataException(String message, ICollection errors)
			: this(message) {
			if (errors != null) {
				_arError = Array.CreateInstance(typeof(string), errors.Count);
				errors.CopyTo(this._arError, 0);
			}
		}

		/// <summary>Gets a message that describes the current 
		/// PhotoPropertiesLoadDataException exception.</summary>
		/// <remarks>The message is comprised of all of the errors 
		/// that occurred while loading the data.</remarks>
		public override String Message {
			get {
				string msg = base.Message;
				if (_arError != null) {
					msg += Environment.NewLine;
					foreach (object obj in _arError) {
						string s = (string)obj;
						msg += Environment.NewLine + s;
					}
				}
				return msg;
			}
		}
	}
}