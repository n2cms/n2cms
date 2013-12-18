using System.Collections.Generic;
using N2.Definitions;
using N2.Details;

namespace N2.Persistence.Serialization
{
    public class DefinedDetailXmlWriter : DetailXmlWriter
    {
        IDefinitionManager definitions;

        public DefinedDetailXmlWriter(IDefinitionManager definitions)
        {
            this.definitions = definitions;
        }

        protected override IEnumerable<ContentDetail> GetDetails(ContentItem item)
        {
            ItemDefinition definition = definitions.GetDefinition(item);
            foreach (ContentDetail detail in item.Details.Values)
            {
                foreach (IEditable editable in definition.Editables)
                {
                    if (detail.Name == editable.Name)
                    {
                        yield return detail;
                        break;
                    }
                }
            }
        }
    }
}
