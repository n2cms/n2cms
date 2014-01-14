using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// A collection of configured item definitions that supports remove and add.
    /// </summary>
    [ConfigurationCollection(typeof(DefinitionElement))]
    public class DefinitionCollection : LazyRemovableCollection<DefinitionElement>
    {
        /// <summary>When set to true this setting makes item types without a [Definition] attribute to be defined.</summary>
        [ConfigurationProperty("defineUnattributedTypes", DefaultValue = false)]
        public bool DefineUnattributedTypes
        {
            get { return (bool)base["defineUnattributedTypes"]; }
            set { base["defineUnattributedTypes"] = value; }
        }

        protected override void OnDeserializeRemoveElement(DefinitionElement element, System.Xml.XmlReader reader)
        {
            element.Description = reader.GetAttribute("description");
            int sortOrder;
            if(int.TryParse(reader.GetAttribute("sortOrder"), out sortOrder))
                element.SortOrder = sortOrder;
            element.Title = reader.GetAttribute("title");
            element.ToolTip = reader.GetAttribute("toolTip");
            element.Type = reader.GetAttribute("type");
        }

        /// <summary>The default container to add editables to if no other container has been specified.</summary>
        [ConfigurationProperty("defaultContainerName", DefaultValue = "Content")]
        public string DefaultContainerName
        {
            get { return (string)base["defaultContainerName"]; }
            set { base["defaultContainerName"] = value; }
        }
    }
}
