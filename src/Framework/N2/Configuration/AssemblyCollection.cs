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
	}
}
