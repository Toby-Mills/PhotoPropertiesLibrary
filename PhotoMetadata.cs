using System;
using System.Collections;
using System.Xml.Serialization;

namespace JSG.PhotoPropertiesLibrary {

	/// <summary>
	/// The PhotoMetadata class is used to store textual 
	/// and formatting data about image tag properties.</summary>
	/// <remarks>PhotoMetadata is the root attribute for the 
	/// XmlSerializer serialization or deserialization.</remarks>
	[XmlTypeAttribute(Namespace="http://tempuri.org/PhotoMetadata.xsd")]
	[XmlRootAttribute("photoMetadata", Namespace="http://tempuri.org/PhotoMetadata.xsd", IsNullable=false)]
	public class PhotoMetadata {

		/// <summary>A collection of PhotoTagMetadata instances</summary>
		private Hashtable _tagMetadataCollection = new Hashtable();

		/// <summary>Gets the collection of PhotoTagMetadata items.</summary>
		[XmlIgnoreAttribute()]
		public Hashtable TagMetadataCollection {
			get { return _tagMetadataCollection; }
		}

		/// <summary>Gets or sets the array of PhotoTagMetadata items.</summary>
		[XmlElementAttribute("tagMetadata")]
		public PhotoTagMetadata[] TagMetadata {
			get {
				if (_tagMetadataCollection.Count == 0)
					return null;
				PhotoTagMetadata[] tagArray = new PhotoTagMetadata[_tagMetadataCollection.Count];
				_tagMetadataCollection.Values.CopyTo(tagArray, 0);
				return tagArray;
			}
			set {
				if (value == null)
					return;
				PhotoTagMetadata[] tagArray = (PhotoTagMetadata[])value;
				_tagMetadataCollection.Clear();
				foreach(PhotoTagMetadata tag in tagArray) {
					_tagMetadataCollection[tag.Id] = tag;
				}
			}
		}

		/// <summary>The overall category of the PhotoMetadata instance.</summary>
		[XmlAttributeAttribute("category")]
		public string category;
	}

	/// <summary>
	/// The PhotoTagMetadata class stores textual and formatting data 
	/// specific to a image tag property.</summary>
	[XmlTypeAttribute(TypeName="TagMetadata", Namespace="http://tempuri.org/PhotoMetadata.xsd")]
	public class PhotoTagMetadata {

		/// <summary>The name of the PhotoTagMetadata item.</summary>
		[XmlElementAttribute("name")]
		public string Name;

		/// <summary>The description of the PhotoTagMetadata item.</summary>
		[XmlElementAttribute("description")]
		public string Description;

		/// <summary>Pretty-print options specific to the PhotoTagMetadata item.</summary>
		/// <remarks>The ValueOptions array can be one of three types:
		/// <newpara></newpara><para></para>
		/// OptionDescription - An individual Key to Value definition;
		/// OptionRangeDescription - A Key range to Value definition;
		/// OptionOtherwiseDescription - The default definition.
		/// </remarks>
		[XmlArrayAttribute(ElementName="valueOptions")]
		[XmlArrayItemAttribute("option", typeof(OptionDescription), IsNullable=false)]
		[XmlArrayItemAttribute("optionRange", typeof(OptionRangeDescription), IsNullable=false)]
		[XmlArrayItemAttribute("optionOtherwise", typeof(OptionOtherwiseDescription), IsNullable=false)]
		public object[] ValueOptions;

		/// <summary>A specialized formatting instruction for the PhotoTagMetadata item.</summary>
		[XmlElementAttribute("formatInstr")]
		public FormatInstr FormatInstr;

		/// <summary>Is a specialized formatting instruction specified?</summary>
		[XmlIgnoreAttribute()]
		public bool FormatInstrSpecified;

		/// <summary>The id of the PhotoTagMetadata item.</summary>
		[XmlAttributeAttribute("id")]
		public int Id;

		/// <summary>The category of the PhotoTagMetadata item.</summary>
		[XmlAttributeAttribute("category")]
		public string Category;

		/// <summary>Returns a pretty-print format of a PhotoTagMetadata item's value.</summary>
		/// <remarks>This method uses any Option values to determine the pretty-print value.</remarks>
		/// <param name="rawValue">The value</param>
		/// <returns>pretty-print value</returns>
		public string PrettyPrintValue(string rawValue) {
			if (this.ValueOptions == null)
				return rawValue;
			if (this.ValueOptions.GetLength(0) == 0)
				return rawValue;

			// For example (in XML):
			//	<valueOptions>
			//		<option key="0" value="zero"></option>
			//		<option key="1" value="one"></option>
			//		<optionRange from="2" to="4" value="two through four"></optionRange>
			//		<option key="5" value="five"></option>
			//		<optionOtherwise value="others"></optionOtherwise>
			//	</valueOptions>

			string processedVal = null;
			string otherwiseVal = null;
			foreach (object option in this.ValueOptions) {
				if (option is OptionDescription)
					processedVal = ((OptionDescription)option).GetKeyValue(rawValue);
				else if (option is OptionRangeDescription)
					processedVal = ((OptionRangeDescription)option).GetKeyValue(rawValue);
				else if (option is OptionOtherwiseDescription)
					// Save the otherwise value, in case no matches occur.
					otherwiseVal = ((OptionOtherwiseDescription)option).GetKeyValue();

				if (processedVal != null)
					break;
			}

			if (processedVal != null)
				return processedVal;
			else if (otherwiseVal != null)
				return otherwiseVal;
			else
				return rawValue;
		}
	}

	/// <summary>Defines a Key to Value option description.</summary>
	[XmlTypeAttribute(TypeName="OptionDescription", Namespace="http://tempuri.org/PhotoMetadata.xsd")]
	public class OptionDescription {
    
		/// <summary>The key</summary>
		[XmlAttributeAttribute("key")]
		public string Key;

		/// <summary>The key's 'typeof'</summary>
		[XmlAttributeAttribute("keyType")]
		[System.ComponentModel.DefaultValueAttribute(OptionKeyType.INT)]
		public OptionKeyType KeyType = OptionKeyType.INT;

		/// <summary>The description</summary>
		[XmlAttributeAttribute("value")]
		public string Value;

		public string GetKeyValue(string rawKey) {
			// Is the rawKey equal to the Key?
			bool matched = false;
			try {
				switch (this.KeyType) {
					case OptionKeyType.CHAR: {
						char key = Convert.ToChar(this.Key);
						if (key == Convert.ToChar(rawKey))
							matched = true;
						break;
					}
					case OptionKeyType.INT: {
						int key = Convert.ToInt32(this.Key);
						if (key == Convert.ToInt32(rawKey))
							matched = true;
						break;
					}
					case OptionKeyType.STRING:
						if (this.Key == rawKey)
							matched = true;
						break;
				}
			}
			catch (System.Exception) {
			}

			if (matched)
				return this.Value;
			else
				return null;
		}
	}

	/// <summary>Defines the default option description.</summary>
	[XmlTypeAttribute(TypeName="OptionOtherwiseDescription", Namespace="http://tempuri.org/PhotoMetadata.xsd")]
	public class OptionOtherwiseDescription {
    
		/// <summary>The default description</summary>
		[XmlAttributeAttribute("value")]
		public string Value;

		public string GetKeyValue() {
			return this.Value;
		}
	}

	/// <summary>Defines a Key Range to Value option description.</summary>
	[XmlTypeAttribute(TypeName="OptionRangeDescription", Namespace="http://tempuri.org/PhotoMetadata.xsd")]
	public class OptionRangeDescription {
    
		/// <summary>The start of the range (inclusive)</summary>
		[XmlAttributeAttribute("from")]
		public int From;

		/// <summary>The end of the range (inclusive)</summary>
		[XmlAttributeAttribute("to")]
		public int To;

		/// <summary>The description</summary>
		[XmlAttributeAttribute("value")]
		public string Value;

		public string GetKeyValue(string rawKey) {
			// Is the RawValue between the To and From keys, inclusive?
			bool matched = false;
			try {
				int iRawKey = Convert.ToInt32(rawKey);
				if (iRawKey >= this.From && iRawKey <= this.To)
					matched = true;
			}
			catch (System.Exception) {
			}

			if (matched)
				return this.Value;
			else
				return null;
		}
	}

	/// <summary>The possible key types to be used in the OptionDescription.</summary>
	[XmlTypeAttribute(TypeName="OptionKeyType", Namespace="http://tempuri.org/PhotoMetadata.xsd")]
	public enum OptionKeyType {
		/// <summary>Integer</summary>
		INT,
		/// <summary>Single character</summary>
		CHAR,
		/// <summary>String</summary>
		STRING,
	}

	/// <summary>The possible specialized formatting instruction 
	/// when reading an image's tag property value.</summary>
	[XmlTypeAttribute(TypeName="FormatInstr", Namespace="http://tempuri.org/PhotoMetadata.xsd")]
	public enum FormatInstr {
		/// <summary>No formatting instruction</summary>
		NO_OP,
		/// <summary>Instruction to change the value to a fraction.</summary>
		/// <remarks>This is only applicable to RATIONAL and SRATIONAL tags.</remarks>
		FRACTION,
		/// <summary>Instruction to format the bytes as a non-null terminated string.</summary>
		/// <remarks>This is only applicable to UNDEFINED tags.</remarks>
		ALLCHAR,
		/// <summary>Instruction to format the bytes as a Base-64 string.</summary>
		/// <remarks>This is only applicable to BYTE tags.</remarks>
		BASE64
	}
}