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
            AddDefault(new PatternValueElement("At", "[@]", "at", true));
            AddDefault(new PatternValueElement("smallAE", "[æ]", "ae", true));
            AddDefault(new PatternValueElement("smallA", "[ä]", "a", true));
            AddDefault(new PatternValueElement("capitalAE", "[Æ]", "Ae", true));
            AddDefault(new PatternValueElement("capitalA", "[Ä]", "A", true));
            AddDefault(new PatternValueElement("smallOE", "[ö]", "o", true));
            AddDefault(new PatternValueElement("capitalOE", "[Ö]", "O", true));
            AddDefault(new PatternValueElement("smallUE", "[ü]", "u", true));
            AddDefault(new PatternValueElement("capitalUE", "[Ü]", "U", true));
        }
    }
}
