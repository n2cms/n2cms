using System.Configuration;

namespace N2.Configuration
{
	public class VirtualPathCollection : ConfigurationElementCollection
	{
		public VirtualPathCollection()
		{
			BaseAdd(new VirtualPathElement("edit", "~/edit/"));
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new VirtualPathElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((VirtualPathElement) element).Name;
		}

		public string[] GetPaths(N2.Web.IWebContext webContext)
		{
			string[] paths = new string[Count];
			for (int i = 0; i < paths.Length; i++)
			{
				string path = ((VirtualPathElement) BaseGet(i)).VirtualPath;
				paths[i] = webContext.ToAbsolute(path);
			}
			return paths;
		}
	}
}