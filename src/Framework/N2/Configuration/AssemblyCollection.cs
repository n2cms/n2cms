using System.Configuration;

namespace N2.Configuration
{


	[ConfigurationCollection(typeof(AssemblyElement))]
	public class AssemblyCollection : LazyRemovableCollection<AssemblyElement>
	{
		public AssemblyCollection()
		{
			AddDefault(new AssemblyElement("N2"));
			AddDefault(new AssemblyElement("N2.Management"));
		}

		protected override void OnDeserializeRemoveElement(AssemblyElement element, System.Xml.XmlReader reader)
		{
			element.Assembly = reader.GetAttribute("assembly");
			base.OnDeserializeRemoveElement(element, reader);
		}

		[ConfigurationProperty("restrictToLoadingPattern")]
		public string RestrictToLoadingPattern
		{
			get { return (string)base["restrictToLoadingPattern"]; }
			set { base["restrictToLoadingPattern"] = value; }
		}

		[ConfigurationProperty("skipLoadingPattern")]
		public string SkipLoadingPattern
		{
			get { return (string)base["skipLoadingPattern"]; }
			set { base["skipLoadingPattern"] = value; }
		}

		[ConfigurationProperty("enableTypeCache", DefaultValue = true)]
		public bool EnableTypeCache
		{
			get { return (bool)base["enableTypeCache"]; }
			set { base["enableTypeCache"] = value; }
		}
	}
}
