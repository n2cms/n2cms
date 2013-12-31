using Castle.Core;
using N2.Definitions;
using N2.Details;
using N2.Plugin;
using N2.Engine;

namespace N2.Templates.Services
{
    /// <summary>
    /// Examines existing item definitions and add an editable checkbox detail 
    /// to the items implementing the <see cref="ISyndicatable" />
    /// interface.
    /// </summary>
    [Service]
    public class SyndicatableDefinitionAppender : IAutoStart
    {
        private readonly IDefinitionManager definitions;
        private string checkBoxText = "Make available for syndication.";
        private string containerName = Tabs.Content;
        private int sortOrder = 30;
        public static readonly string SyndicatableDetailName = "Syndicate";

        public SyndicatableDefinitionAppender(IDefinitionManager definitions)
        {
            this.definitions = definitions;
        }

        public int SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }

        public string ContainerName
        {
            get { return containerName; }
            set { containerName = value; }
        }

        public string CheckBoxText
        {
            get { return checkBoxText; }
            set { checkBoxText = value; }
        }

        public void Start()
        {
            foreach (ItemDefinition definition in definitions.GetDefinitions())
            {
                if (typeof (ISyndicatable).IsAssignableFrom(definition.ItemType))
                {
                    EditableCheckBoxAttribute ecb = new EditableCheckBoxAttribute(CheckBoxText, 10);
                    ecb.Name = SyndicatableDetailName;
                    ecb.ContainerName = ContainerName;
                    ecb.SortOrder = SortOrder;

                    definition.Add(ecb);
                }
            }
        }

        public void Stop()
        {
        }
    }
}
