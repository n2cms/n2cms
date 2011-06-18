using System;
using System.Xml.Linq;
using N2.Details;
using N2.Persistence.Serialization;
using N2.Persistence;

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

		[Persistable]
		[EditableNumber]
		public virtual int PersistableNumber { get; set; }

		[Persistable]
		[EditableDate]
		public virtual DateTime PersistableDate { get; set; }

		[Persistable]
		[EditableText]
		public virtual string PersistableText { get; set; }

		[Persistable]
		[EditableLink]
		public virtual XmlableItem PersistableLink { get; set; }

		[Persistable]
		[EditableEnum]
		public virtual ContentState PersistableEnum { get; set; }

		[Persistable]
		[EditableEnum]
		public virtual string[] PersistableObject { get; set; }

		public Version Version
		{
			get { return new Version(GetDetail("Version", "1.0.0.0")); }
			set { SetDetail("Version", value.ToString(), "1.0.0.0"); }
		}

		public XDocument Xml
		{
			get { return XDocument.Parse(GetDetail("Xml", "<root/>")); }
			set { SetDetail("Xml", value.ToString()); }
		}
	}

	public class XmlableItem2 : XmlableItem
	{
		[EditableFreeTextArea]
		public virtual string AutoPropertyString { get; set; }
	}
}