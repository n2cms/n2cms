using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace N2.Persistence.Serialization
{
	/// <summary>
	/// Attaches files to exports.
	/// </summary>
	public class FileAttachmentAttribute : Attribute, IAttachmentHandler
	{
		public string Name { get; set; }

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
			if(System.Web.HttpContext.Current == null)
				return AppDomain.CurrentDomain.BaseDirectory + url.Replace('/', '\\'); 

			return System.Web.HttpContext.Current.Server.MapPath(url);
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
