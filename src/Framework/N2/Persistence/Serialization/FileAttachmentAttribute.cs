using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using N2.Edit.FileSystem;

namespace N2.Persistence.Serialization
{
	/// <summary>
	/// Attaches files to exports.
	/// </summary>
	public class FileAttachmentAttribute : Attribute, IAttachmentHandler
	{
		public string Name { get; set; }

		public void Write(IFileSystem fs, ContentItem item, XmlTextWriter writer)
		{
			string url = item[Name] as string;
			if(!string.IsNullOrEmpty(url))
			{
				string path = url;
				if(fs.FileExists(path))
				{
					using(ElementWriter ew = new ElementWriter("file", writer))
					{
						ew.WriteAttribute("url", url);

						using (var s = fs.OpenFile(path, readOnly: true))
						{
							byte[] fileContents = ReadFully(s);
							string base64representation = Convert.ToBase64String(fileContents);
							ew.Write(base64representation);
						}
					}
				}
			}
		}

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
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

		public void Import(IFileSystem fs, Attachment a)
		{
			if (a.HasContents)
			{
				string path = a.Url;

                if(!fs.DirectoryExists(Path.GetDirectoryName(path)))
                {
                    fs.CreateDirectory(Path.GetDirectoryName(path));
                }

			    var memoryStream = new MemoryStream(a.FileContents);
			    fs.WriteFile(path, memoryStream);
			}
		}

		public int CompareTo(IAttachmentHandler other)
		{
			return Name.CompareTo(other.Name);
		}
	}
}
