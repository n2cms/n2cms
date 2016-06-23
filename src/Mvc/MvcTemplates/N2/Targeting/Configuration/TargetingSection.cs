using N2.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace N2.Management.Targeting.Configuration
{
	public class TargetingSection : ConfigurationSectionBase
	{
		[ConfigurationProperty("previewSizes")]
		public PreviewElementCollection PreviewSizes
		{
			get { return (PreviewElementCollection)base["previewSizes"]; }
			set { base["previewSizes"] = value; }
		}
	}
	
	public class PreviewElementCollection : LazyRemovableCollection<PreviewElement>
	{
		public PreviewElementCollection()
		{
			AddDefault(new PreviewElement { Name = "iphone", Title = "Mobile (iPhone, etc.)", Width = 320, Height = 480, IconClass = "fa fa-mobile" });
			AddDefault(new PreviewElement { Name = "iphone5", Title = "iPhone 5", Width = 320, Height = 568, IconClass = "fa fa-mobile" });
			AddDefault(new PreviewElement { Name = "tablet", Title = "Tablet (iPad 3, etc.)", Width = 768, Height = 1024, IconClass = "fa fa-tablet" });
			AddDefault(new PreviewElement { Name = "hdtv", Title = "HDTV", Width = 1920, Height = 1080, IconClass = "fa fa-desktop" });
		}
	}

	public class PreviewElement : NamedElement
	{
		[ConfigurationProperty("width")]
		public int Width
		{
			get { return (int)base["width"]; }
			set { base["width"] = value; }
		}

		[ConfigurationProperty("height")]
		public int Height
		{
			get { return (int)base["height"]; }
			set { base["height"] = value; }
		}

		[ConfigurationProperty("title")]
		public string Title
		{
			get { return (string)base["title"]; }
			set { base["title"] = value; }
		}

		[ConfigurationProperty("iconClass", DefaultValue = "fa fa-mobile")]
		public string IconClass
		{
			get { return (string)base["iconClass"]; }
			set { base["iconClass"] = value; }
		}
	}
}