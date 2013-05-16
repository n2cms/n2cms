using System.Configuration;

namespace N2.Configuration
{
	public class WebSection : ContentConfigurationSectionBase
	{
		[ConfigurationProperty("docviewer")]
		public DocViewerElement DocViewer
		{
			get { return (DocViewerElement)base["docviewer"]; }
			set { base["docviewer"] = value; }
		}

		[ConfigurationProperty("filelist")]
		public FileListElement FileList
		{
			get { return (FileListElement)base["filelist"]; }
			set { base["filelist"] = value; }
		}

	}
}