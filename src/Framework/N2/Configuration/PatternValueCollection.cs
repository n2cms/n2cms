using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// A collection of pattern replacements for the name editor.
	/// </summary>
	[ConfigurationCollection(typeof(PatternValueElement))]
	public class PatternValueCollection : LazyRemovableCollection<PatternValueElement>
	{
		public PatternValueCollection()
		{
			AddDefault(new PatternValueElement("At", "[@]", "a", true));
			AddDefault(new PatternValueElement("smallAE", "[æä]", "ae", true));
			AddDefault(new PatternValueElement("capitalAE", "[ÆÄ]", "Ae", true));
			AddDefault(new PatternValueElement("smallOE", "[ö]", "oe", true));
			AddDefault(new PatternValueElement("capitalOE", "[Ö]", "Oe", true));
			AddDefault(new PatternValueElement("smallUE", "[ü]", "ue", true));
			AddDefault(new PatternValueElement("capitalUE", "[Ü]", "Ue", true));
		}
	}
}