using N2.Details;
using N2.Persistence.Serialization;
using System;

namespace N2.Tests.Serialization.Items
{
	public class XmlableItem : ContentItem
	{
		[EditableImage("Image", 100)]
		[FakeAttachment]
		public virtual string ImageUrl	
		{
			get { return (string)(GetDetail("ImageUrl") ?? string.Empty); }
			set { SetDetail("ImageUrl", value); }
		}
		[EditableFreeTextArea("TextFile", 100)]
		[FileAttachment]
		public virtual string TextFile
		{
			get { return (string)(GetDetail("TextFile") ?? string.Empty); }
			set { SetDetail("TextFile", value); }
		}
		
		public Version Version
		{
			get { return new Version(GetDetail("Version", "1.0.0.0")); }
			set { SetDetail("Version", value.ToString(), "1.0.0.0"); }
		}

	}

	public class XmlableItem2 : XmlableItem
	{
		[EditableFreeTextArea]
		public virtual string AutoPropertyString { get; set; }
	}
}