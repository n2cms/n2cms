using System.Configuration;

namespace N2.Configuration
{
    public class NameEditorElement : ConfigurationElement
    {
        /// <summary>Gets or sets the character that replaces whitespace when updating the name (default is '-').</summary>
        [ConfigurationProperty("whitespaceReplacement", DefaultValue = '-')]
        public char WhitespaceReplacement
        {
            get { return (char)base["whitespaceReplacement"]; }
            set { base["whitespaceReplacement"] = value; }
        }

        /// <summary>Gets or sets whether names should be made lowercase.</summary>
        [ConfigurationProperty("toLower", DefaultValue = true)]
        public bool ToLower
        {
            get { return (bool)base["toLower"]; }
            set { base["toLower"] = value; }
        }

        /// <summary>Allow editor to choose wether to update name automatically.</summary>
        [ConfigurationProperty("showKeepUpdated", DefaultValue = true)]
        public bool ShowKeepUpdated
        {
            get { return (bool)base["showKeepUpdated"]; }
            set { base["showKeepUpdated"] = value; }
        }

        /// <summary>Collection of regex replacements for the name title to name editor.</summary>
        [ConfigurationProperty("replacements")]
        public PatternValueCollection Replacements
        {
            get { return (PatternValueCollection)base["replacements"]; }
            set { base["replacements"] = value; }
        }
    }
}
