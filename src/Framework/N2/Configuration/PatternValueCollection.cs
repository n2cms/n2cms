using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// A collection of pattern replacements for the name editor.
	/// </summary>
	[ConfigurationCollection(typeof(PatternValueElement))]
	public class PatternValueCollection : ConfigurationElementCollection
	{
		public PatternValueCollection()
		{
			BaseAdd(new PatternValueElement("smallA", "[åáàâã@]", "a", true));
			BaseAdd(new PatternValueElement("capitalA", "[ÅÁÀÂÃ]", "a", true));
			BaseAdd(new PatternValueElement("smallAE", "[æä]", "ae", true));
			BaseAdd(new PatternValueElement("capitalAE", "[ÆÄ]", "Ae", true));
			BaseAdd(new PatternValueElement("smallE", "[éè]", "e", true));
			BaseAdd(new PatternValueElement("capitalE", "[ÉÈ]", "E", true));
			BaseAdd(new PatternValueElement("smallI", "[íì]", "i", true));
			BaseAdd(new PatternValueElement("capitalI", "[ÍÌ]", "I", true));
			BaseAdd(new PatternValueElement("smallO", "[øóòôõ]", "o", true));
			BaseAdd(new PatternValueElement("capitalO", "[ØÓÒÔÕ]", "O", true));
			BaseAdd(new PatternValueElement("smallOE", "[ö]", "oe", true));
			BaseAdd(new PatternValueElement("capitalOE", "[Ö]", "Oe", true));
			BaseAdd(new PatternValueElement("smallU", "[úù]", "u", true));
			BaseAdd(new PatternValueElement("capitalU", "[ÚÙ]", "U", true));
			BaseAdd(new PatternValueElement("smallUE", "[ü]", "ue", true));
			BaseAdd(new PatternValueElement("capitalUE", "[Ü]", "Ue", true));
			BaseAdd(new PatternValueElement("germanSZ", "[ß]", "ss", true));
			BaseAdd(new PatternValueElement("theRest", "[^. a-zA-Z0-9_-]", "", true));
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new PatternValueElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((PatternValueElement) element).Name;
		}
	}
}