using System.Collections.Generic;
using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Represents an editable or an editable container added to an item definition.
    /// </summary>
    public class ContainableElement : NamedElement
    {
        Dictionary<string, string> editableProperties = new Dictionary<string, string>();

        /// <summary>The type of item.</summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

        /// <summary>The title used to describe the type.</summary>
        [ConfigurationProperty("title")]
        public string Title
        {
            get { return (string)base["title"]; }
            set { base["title"] = value; }
        }

        /// <summary>The container onto which to add the container or editable.</summary>
        [ConfigurationProperty("containerName")]
        public string ContainerName
        {
            get { return (string)base["containerName"]; }
            set { base["containerName"] = value; }
        }

        /// <summary>The sort of order of the editable.</summary>
        [ConfigurationProperty("sortOrder")]
        public int? SortOrder
        {
            get { return (int?)base["sortOrder"]; }
            set { base["sortOrder"] = value; }
        }

        public Dictionary<string, string> EditableProperties
        {
            get { return editableProperties; }
            set { editableProperties = value; }
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            EditableProperties[name] = value;
            return true;
        }
    }
}
