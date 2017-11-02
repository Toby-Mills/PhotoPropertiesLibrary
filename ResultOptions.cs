using System;

namespace JSG.PhotoPropertiesLibrary {

	/// <summary>Options used when creating the XML output of an analysis.</summary>
	public class ResultOptions {

		private string _XMLNamespace;
		private string _XSLTransform;
		private bool _includeXSLT = true;

		/// <summary>Constructor</summary>
		public ResultOptions() {
		}

		/// <summary>The XML output's namespace.</summary>
		/// <remarks>For example: "http://tempuri.org/PhotoPropertyOutput.xsd".</remarks>
		public string XMLNamespace {
			get { return _XMLNamespace; }
			set { _XMLNamespace = value; }
		}
		/// <summary>The XML output's xml-stylesheet processing instruction.</summary>
		/// <remarks>For example: "PhotoPropertyOutput.xslt".</remarks>
		public string XSLTransform {
			get { return _XSLTransform; }
			set { _XSLTransform = value; }
		}
		/// <summary>Should the xml-stylesheet processing instruction (XSLT)
		/// be included in the output?</summary>
		public bool IncludeXSLT {
			get { return _includeXSLT; }
			set { _includeXSLT = value; }
		}
	}
}
