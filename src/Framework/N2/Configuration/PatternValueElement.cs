using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Defines a replacment pattern for the name editor.
    /// </summary>
    public class PatternValueElement : ConfigurationElement, IIdentifiable
    {
        public PatternValueElement()
        {
        }

        public PatternValueElement(string name, string pattern, string value, bool serverValidate)
        {
            Name = name;
            Pattern = pattern;
            Value = value;
            ServerValidate = serverValidate;
        }

        /// <summary>The name used to reference this element.</summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>A regular expression pattern used match replacements. This pattern should work both server and client side.</summary>
        [ConfigurationProperty("pattern")]
        public string Pattern
        {
            get { return (string)base["pattern"]; }
            set { base["pattern"] = value; }
        }

        /// <summary>The string to replace the pattern with.</summary>
        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }

        /// <summary>Validate on the server side that the pattern is not present in the name.</summary>
        [ConfigurationProperty("serverValidate", DefaultValue = true)]
        public bool ServerValidate
        {
            get { return (bool)base["serverValidate"]; }
            set { base["serverValidate"] = value; }
        }

        #region IIdentifiable Members

        public object ElementKey
        {
            get { return Name; }
            set { Name = (string)value; }
        }

        #endregion
    }
}
