using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace N2.Serialization
{
	public class FileAttachmentAttribute : Attribute, IAttachmentHandler
	{
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public void Write(ContentItem item, XmlTextWriter writer)
		{
			string url = item[Name] as string;
			if(!string.IsNullOrEmpty(url))
			{
				string path = MapPath(url);
				if(File.Exists(path))
				{
					using(ElementWriter ew = new ElementWriter("file", writer))
					{
						ew.WriteAttribute("url", url);

						byte[] fileContents = File.ReadAllBytes(path);
						string base64representation = Convert.ToBase64String(fileContents);
						ew.Write(base64representation);
					}
				}
			}
		}

		protected virtual string MapPath(string url)
		{
			return AppDomain.CurrentDomain.BaseDirectory + url.Replace('/', '\\');
		}

		public Attachment Read(XPathNavigator navigator, ContentItem item)
		{
			string url = navigator.GetAttribute("url", string.Empty);

			if(!string.IsNullOrEmpty(url))
			{
				return new Attachment(this, url, item, Convert.FromBase64String(navigator.Value));
			}

			return null;
		}

		public void Import(Attachment a)
		{
			if (a.HasContents)
			{
				string path = MapPath(a.Url);
				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.WriteAllBytes(path, a.FileContents);
			}
		}

		public int CompareTo(IAttachmentHandler other)
		{
			return Name.CompareTo(other.Name);
		}
	}
}
