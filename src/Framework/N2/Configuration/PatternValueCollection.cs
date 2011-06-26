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
			AddDefault(new PatternValueElement("smallA", "[åáàâã@]", "a", true));
			AddDefault(new PatternValueElement("capitalA", "[ÅÁÀÂÃ]", "a", true));
			AddDefault(new PatternValueElement("smallAE", "[æä]", "ae", true));
			AddDefault(new PatternValueElement("capitalAE", "[ÆÄ]", "Ae", true));
			AddDefault(new PatternValueElement("smallE", "[éè]", "e", true));
			AddDefault(new PatternValueElement("capitalE", "[ÉÈ]", "E", true));
			AddDefault(new PatternValueElement("smallI", "[íì]", "i", true));
			AddDefault(new PatternValueElement("capitalI", "[ÍÌ]", "I", true));
			AddDefault(new PatternValueElement("smallO", "[øóòôõ]", "o", true));
			AddDefault(new PatternValueElement("capitalO", "[ØÓÒÔÕ]", "O", true));
			AddDefault(new PatternValueElement("smallOE", "[ö]", "oe", true));
			AddDefault(new PatternValueElement("capitalOE", "[Ö]", "Oe", true));
			AddDefault(new PatternValueElement("smallU", "[úù]", "u", true));
			AddDefault(new PatternValueElement("capitalU", "[ÚÙ]", "U", true));
			AddDefault(new PatternValueElement("smallUE", "[ü]", "ue", true));
			AddDefault(new PatternValueElement("capitalUE", "[Ü]", "Ue", true));
			AddDefault(new PatternValueElement("germanSZ", "[ß]", "ss", true));
			AddDefault(new PatternValueElement("theRest", "[^. a-zA-Z0-9_-]", "", true));
		}
	}
}