using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Resources;

// ////////////////////////////////////////////////////////////////////////
// The PhotoProperties library is used to analyze the tag properties of 
// an image. Encapsulated metadata within the image is decrypted by the 
// System.Drawing.Imaging.PropertyItem methods and formatted with the
// library's stored metadata. The formatted data can be accessed 
// individually, or as a single XML file or stream.
//	
// The library's stored metadata is based upon the information within the 
// "Property Item Descriptions" page, available at
//   http://msdn.microsoft.com/ ... /ImagePropertyTagConstants/PropertyItemDescriptions.asp
// as well as the "Digital Still Camera Image File Format Standard", v2.1
// available at 
//   http://it.jeita.or.jp/jhistory/document/standard/exif_eng/jeida49eng.htm
//
// ////////////////////////////////////////////////////////////////////////
//
// The main classes used within this library are:
//
//	PhotoProperties     (Namespace: JSG.PhotoPropertiesLibrary)
//		Analyzes the tag properties of an image. By matching the image's 
//		PropertyItems with stored categories, descriptions, formatting 
//		instructions, and pretty-print options a readable analysis 
//		can be created. The results can be accessed individually or 
//		as a single XML file or stream.
//
//  PropertyItem        (Namespace: System.Drawing.Imaging)
//		Encapsulates a metadata property to be included 
//		in an image file.
//
//  PhotoMetadata       (Namespace: JSG.PhotoPropertiesLibrary)
//		The root XML serialization class containing a collection
//		of zero or more PhotoTagMetadata instances.
//
//  PhotoTagMetadata    (Namespace: JSG.PhotoPropertiesLibrary)
//		Encapsulates an extended image metadata property 
//		including categories, descriptions, formatting 
//		instructions, and pretty-print options.
//
//  PhotoTagDatum       (Namespace: JSG.PhotoPropertiesLibrary)
//		Encapsulates the value found in an image file 
//		and its associated PhotoTagMetadata.
//
//
// ////////////////////////////////////////////////////////////////////

namespace JSG.PhotoPropertiesLibrary {

	//	PhotoProperties Public Constructors, Properties, and Methods
	//	------------------------------------------------------------
	//	Public Instance Constructors
	//		PhotoProperties Constructor
	//
	//	Public Instance Properties
	//		ImageFileName 
	//			Gets the image file name that was provided in the Analyze method's parameter. 
	//
	//	Public Instance Methods
	//		Initialize (Overloaded)
	//			This method initializes PhotoProperties by loading the tag metadata.
	//			An external "photoMetadata" XML file can be used to initialize the data
	//			(the file must be valid based upon the PhotoMetadata.xsd schema); If 
	//			not supplied, the stored PhotoMetadata.xml file is used. Any errors 
	//			can be accessed by the GetInitializeErrorMessage method.  
	//		Analyze 
	//			Analyzes the tag properties of an image. 
	//		WriteXml (Overloaded)
	//			Formats and writes the analysis as an XML file or stream. The XML format
	//			is based upon the PhotoPropertyOutput.xsd schema.  Options include the
	//			ability to define and link an XSLT file to the XML output.
	//		GetTagDatum 
	//			Returns a specific PhotoTagDatum instance based upon the item's id.
	//		GetInitializeErrorMessage 
	//			Returns a string containing the cumulative error messages that 
	//			may have occurred during initialization. This is especially
	//			pertinent to the case where an external "photoMetadata" XML 
	//			file is used. 
	//
	// ////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The PhotoProperties class is used to analyze the tag properties of an image.
	/// Image metadata decrypted by the System.Drawing.Imaging.PropertyItem is merged with
	/// extended textual and formatting data to provide useable results. These results can be
	/// accessed individually through the GetTagDatum method, or as a single XML file or stream.
	/// </summary>
	public class PhotoProperties {

		private const string RESBASENAME = "JSG.PhotoPropertiesLibrary.PhotoMetadata";
		private const string RESNAME = "PhotoMetadata.xml";

		/// <summary>A string array of errors that may occur during the initialization.</summary>
		private ArrayList _arLoadErrors = new ArrayList();
		/// <summary>A concatenation of _arLoadErrors.</summary>
		private string _initErrors = String.Empty;
		/// <summary>Instance of the PhotoMetadata class.</summary>
		private PhotoMetadata _photoMetadata;
		/// <summary>The image file name.</summary>
		private string _imageFileName;
		/// <summary>A collection of the image's tag data.</summary>
		private Hashtable _photoTagData = new Hashtable();
		/// <summary>Has the initialization successfully occurred?</summary>
		private bool _wasInitialized = false;

		/// <summary>Constructor</summary>
		public PhotoProperties() {
		}

		/// <summary>Returns a string containing the cumulative error messages 
		/// that may have occurred during initialization.</summary>
		/// <returns>The error string. This value is never null.</returns>
		public string GetInitializeErrorMessage() {
			return _initErrors;
		}

		/// <summary>Gets the image file name that was provided 
		/// in the Analyze method's parameter.</summary>
		public string ImageFileName {
			get {
				return _imageFileName;
			}
		}

		/// <summary>
		/// This method initializes PhotoProperties by loading the tag metadata.
		/// Any errors can be accessed by the GetInitializeErrorMessage() method.
		/// </summary>
		/// <param name="xmlFileName">
		/// An external "photoMetadata" XML file used to initialize the data; This file 
		/// must be valid based upon the PhotoMetadata.xsd schema.</param>
		/// <returns>True if the initialization was successful; false otherwise.</returns>
		public bool Initialize(string xmlFileName) {
			_initErrors = String.Empty;

			try {
				LoadTagMetadata(xmlFileName);
				_wasInitialized = true;
			}
			catch (PhotoPropertiesLoadDataException ex) {
				// save a copy of the error message
				_initErrors = String.Copy(ex.Message);
			}

			return _wasInitialized;
		}

		/// <summary>
		/// This method initializes PhotoProperties by loading the tag metadata.
		/// Any errors can be accessed by the GetInitializeErrorMessage() method.
		/// </summary>
		/// <remarks>A stored PhotoMetadata.xml file resource will be loaded.
		/// Initialize(string xmlFileName) allows an external xml file to be used.</remarks>
		/// <returns>True if the initialization was successful; false otherwise.</returns>
		public bool Initialize() {
			return Initialize(null);
		}

		// //////////////////////////////////////////////////////////
		// Load the Tag Properties data from the Xml data file.
		// //////////////////////////////////////////////////////////

		/// <summary>XML event handler for Unknown Attributes;
		/// Used while reading an XML file.</summary>
		private void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e) {
			string errMessage = String.Format(
				"Unknown XML Attribute: {0} (line:{1}, pos:{2})", 
				e.Attr.Name, 
				e.LineNumber, 
				e.LinePosition);
			_arLoadErrors.Add(errMessage);
		}

		/// <summary>XML event handler for Unknown Elements; 
		/// Used while reading an XML file.</summary>
		private void Serializer_UnknownElement(object sender, XmlElementEventArgs e) {
			string errMessage = String.Format(
				"Unknown XML Element: {0} (line:{1}, pos:{2})", 
				e.Element.Name, 
				e.LineNumber, 
				e.LinePosition);
			_arLoadErrors.Add(errMessage);
		}

		/// <summary>XML event handler for Unknown Nodes; 
		/// Used while reading an XML file.</summary>
		private void Serializer_UnknownNode(object sender, XmlNodeEventArgs e) {
			// We've already accounted for attributes
			if (e.NodeType == XmlNodeType.Attribute)
				return;
			// We've already accounted for elements
			if (e.NodeType == XmlNodeType.Element)
				return;
			string errMessage = String.Format(
				"Unknown XML Node: {0} (line:{1}, pos:{2})", 
				e.Name, 
				e.LineNumber, 
				e.LinePosition);
			_arLoadErrors.Add(errMessage);
		}

		/// <summary>Loads the XML tag property data.</summary>
		/// <param name="xmlFileName">An external image file name, or null.</param>
		private void LoadTagMetadata(string xmlFileName) {
			XmlTextReader xreader = null;

			try {
				XmlSerializer serializer = new XmlSerializer(typeof(PhotoMetadata));

				// If the XML document contains unknown attributes, elements, 
				// or others, handle them with their respective Unknown events.
				serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);
				serializer.UnknownElement += new XmlElementEventHandler(Serializer_UnknownElement);
				serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);

				// If not supplied, use the stored resource; else use the external file.
				if (xmlFileName == null || xmlFileName == String.Empty) {
					// From: MotLib.NET(tm) Copyright 2002 Paul DiLascia
					ResourceManager rm = 
						new ResourceManager(RESBASENAME, this.GetType().Assembly);
					Byte[] bytes = (Byte[])rm.GetObject(RESNAME);
					xreader = new XmlTextReader(new MemoryStream(bytes));
				}
				else {
					xreader = new XmlTextReader(xmlFileName);
				}

				// Read XML data
				_photoMetadata = (PhotoMetadata)serializer.Deserialize(xreader);
			}
			catch (System.InvalidOperationException e) {
				// A System.IO.FileNotFoundException InnerException occurs
				// when the XML file can not be found.
				if (e.InnerException is System.IO.FileNotFoundException)
					_arLoadErrors.Add(e.InnerException.Message);
				else
					_arLoadErrors.Add(e.ToString());
			}
			catch (System.Exception e) {
				_arLoadErrors.Add(e.ToString());
			}
			finally {
				if (xreader != null)
					xreader.Close();
			}

			if (_arLoadErrors.Count > 0) {
				string errMsg = "XML Errors were found while loading the XML Tag Properties.";
				PhotoPropertiesLoadDataException excep = 
					new PhotoPropertiesLoadDataException(errMsg, _arLoadErrors);
				_arLoadErrors.Clear();
				throw excep;
			}
		}

		// /////////////////////////////////////////////////////////////
		// Analyze a photo's properties and values
		// /////////////////////////////////////////////////////////////

		/// <summary>Analyzes the tag properties of an image.</summary>
		public void Analyze(string fileName) {
			if (!_wasInitialized)
				throw new PhotoPropertiesException("PhotoProperties was not initialized.");

			// Save the file name
			_imageFileName = fileName;

			// Empty the previous analysis data
			_photoTagData.Clear();

			// Create an Image object from the specified file.
			Image img = null;
            System.IO.FileStream fs;

            try {
                fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
                img = Image.FromStream(fs);
                    
                //img = Image.FromFile(fileName, true);
			}

                ///////////////////////////////////////////////////

            /////////////////////////////////////////////////////////

			// An invalid image will throw an OutOfMemoryException 
			// exception
			catch (OutOfMemoryException) {
				throw new InvalidOperationException("'" 
					+ fileName 
					+ "' is not a valid image file.");
			}

			// Can't use the following code due to an internal bug 
			// that causes an exception when an empty tag exists
			//		PropertyItem[] propItems = img.PropertyItems;
			//		foreach (PropertyItem pi in propItems) {

			// Enumerate the image's PropertyItems, saving 
			// pertainent data in the PhotoTagData collection.
			int[] propIds = img.PropertyIdList;
			foreach (int id in propIds) {
				// Since an exception can occur while accessing properties 
				// with null values we must use try-catch.
				try {
					PropertyItem prop = img.GetPropertyItem(id);
					PhotoTagMetadata tag = 
						(PhotoTagMetadata)_photoMetadata.TagMetadataCollection[prop.Id];
					// If the tag does not exist in the PhotoTagMetadata collection, continue.
					if (tag == null) {
						continue;
					}
					_photoTagData[prop.Id] = 
						new PhotoTagDatum(prop.Id, 
								tag, 
								PropertyTagFormat.FormatValue(prop, tag));
				}
				catch (Exception) {
				}
			}
            fs.Close();
		}

		// /////////////////////////////////////////////////////////////
		// Get a specific tag
		// /////////////////////////////////////////////////////////////

		/// <summary>Returns a specified PhotoTagDatum instance.</summary>
		/// <param name="propId">A specific property id</param>
		public PhotoTagDatum GetTagDatum(int propId) {
			return (PhotoTagDatum)_photoTagData[propId];
		}

		// /////////////////////////////////////////////////////////////
		// Create XML Result
		// /////////////////////////////////////////////////////////////

		// /////////////////////////////////////////////////////////////
		// For example:
		//
		//		<?xml version="1.0" encoding="utf-8"?>
		//		<photoTagProperties xmlns="http://tempuri.org/PhotoPropertyOutput.xsd">
		//		  <imageFile>000PC140001.JPG</imageFile>
		//		  <created>2003-01-06T23:35:48.5937500-05:00</created>
		//		  <createdLocal>1/6/2003 11:35:48 PM</createdLocal>
		//		  <tagData>
		//		    ...
		//		    <tagDatum id="33434" category="EXIF">
		//		      <name>ExposureTime</name>
		//		      <description>Exposure time, measured in seconds.</description>
		//		      <value>1/50</value>
		//		      <prettyPrintValue>1/50</prettyPrintValue>
		//		    </tagDatum>
		//		    ...
		//		  </tagData>
		//		</photoTagProperties>
		// /////////////////////////////////////////////////////////////

		/// <summary>Formats and writes the analysis as an XML file.
		/// The XML format is based upon the PhotoPropertyOutput.xsd schema.
		/// Options include the ability to define and link an XSLT file 
		/// to the XML output.</summary>
		/// <param name="fileOutput">The XML file name</param>
		/// <param name="resultOptions">Options affecting the XML result.</param>
		public void WriteXml(string fileOutput, ResultOptions resultOptions) {

			if (!_wasInitialized)
				throw new PhotoPropertiesException("PhotoProperties was not initialized.");

			XmlTextWriter xmlWriter = new XmlTextWriter(fileOutput, System.Text.Encoding.UTF8);
			WriteXml(xmlWriter, resultOptions);

			// Flush the buffer and close the writer
			xmlWriter.Flush();
			xmlWriter.Close();  
		}

		/// <summary>Formats and writes the analysis as an XML stream.
		/// The XML format is based upon the PhotoPropertyOutput.xsd schema.
		/// Options include the ability to define and link an XSLT file 
		/// to the XML output.</summary>
		/// <param name="strmWriter">A stream</param>
		/// <param name="resultOptions">Options affecting the XML result.</param>
		/// <remarks>The stream will be automatically closed.</remarks>
		public void WriteXml(Stream strmWriter, ResultOptions resultOptions) {

			if (!_wasInitialized)
				throw new PhotoPropertiesException("PhotoProperties was not initialized.");

			XmlTextWriter xmlWriter = new XmlTextWriter(strmWriter, System.Text.Encoding.UTF8);
			WriteXml(xmlWriter, resultOptions);

			// Flush the buffer and close the writer
			xmlWriter.Flush();
			xmlWriter.Close();
		}

		/// <summary>Formats and writes the analysis to an XmlTextWriter.
		/// The XML format is based upon the PhotoPropertyOutput.xsd schema.
		/// Options include the ability to define and link an XSLT file 
		/// to the XML output.</summary>
		/// <param name="xmlWriter">An XmlTextWriter instance.</param>
		/// <param name="resultOptions">Options affecting the XML result.</param>
		public void WriteXml(XmlTextWriter xmlWriter, ResultOptions resultOptions) {

			if (!_wasInitialized)
				throw new PhotoPropertiesException("PhotoProperties was not initialized.");

			Array propArray = Array.CreateInstance(typeof(PhotoTagDatum), _photoTagData.Count);
			_photoTagData.Values.CopyTo(propArray, 0);
			Array.Sort(propArray);

			xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.Indentation = 2;

			xmlWriter.WriteStartDocument();

			// If the XSLT transform is to be included and the file name was provided 
			// in the configuration file, write the XLST instruction
			if (resultOptions.IncludeXSLT && resultOptions.XSLTransform != null)
				xmlWriter.WriteProcessingInstruction("xml-stylesheet", 
					"type='text/xsl' href='" + resultOptions.XSLTransform + "'");

			// <photoTagProperties>
			xmlWriter.WriteStartElement("photoTagProperties");

			// If provided in the configuration file,
			//  write the namespace declaration
			if (resultOptions.XMLNamespace != null)
				xmlWriter.WriteAttributeString("xmlns", resultOptions.XMLNamespace);

			// <imagefile />
			xmlWriter.WriteElementString("imageFile", _imageFileName);
			// <created /> - the current DateTime (in XML format)
			DateTime dtNow = DateTime.Now;
			xmlWriter.WriteElementString("created", XmlConvert.ToString(dtNow));
			// <createdLocal /> - the current DateTime (in local format)
			xmlWriter.WriteElementString("createdLocal", dtNow.ToString("G", null));
			
			// <tagData>
			xmlWriter.WriteStartElement("tagData");

			for (int i = 0; i < propArray.Length; i++) {
				PhotoTagDatum photoDatum = (PhotoTagDatum)propArray.GetValue(i);
				if (photoDatum == null)
					continue;

				// An example of the tagDatum element:
				//  <tagDatum id="33434" category="EXIF">
				//    <name>ExposureTime</name>
				//    <description>Exposure time, measured in seconds.</description>
				//   <value>1/50</value>
				//    <prettyPrintValue>1/50</prettyPrintValue>
				//  </tagDatum>

				// <tagDatum>
				xmlWriter.WriteStartElement("tagDatum");

				// <tagDatum id="...">
				xmlWriter.WriteAttributeString("id", XmlConvert.ToString(photoDatum.Id));
				// <tagDatum category="...">
				xmlWriter.WriteAttributeString("category", photoDatum.Category);
				// <name />
				xmlWriter.WriteElementString("name", photoDatum.Name);
				// <description />
				xmlWriter.WriteElementString("description", photoDatum.Description);
				// <value />
				xmlWriter.WriteElementString("value", photoDatum.Value);
				// <PrettyPrintValue />
				//   Only include the pretty-print value if
				//   the Value and PrettyPrintValue are NOT equal.
				if (photoDatum.Value != photoDatum.PrettyPrintValue)
					xmlWriter.WriteElementString("prettyPrintValue", 
							photoDatum.PrettyPrintValue);

				// </tagDatum>
				xmlWriter.WriteEndElement();
			}

			// </tagData>
			xmlWriter.WriteEndElement();

			// </photoTagProperties>
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndDocument();
		}
	}
}
