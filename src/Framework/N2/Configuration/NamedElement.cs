using System.Configuration;

namespace N2.Configuration
{
    public class NamedElement : ConfigurationElement, IIdentifiable
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        #region IIdentifiable Members

        object IIdentifiable.ElementKey
        {
            get { return Name; }
            set { Name = (string)value; }
        }

        #endregion
    }
}
