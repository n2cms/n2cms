using N2;
using N2.Details;
using Dinamico.Definitions.Dynamic;

namespace Dinamico.Models
{
	[PartDefinition]
	public class ContentPart : ContentItem
	{
		[N2.Persistence.Persistable(PersistAs = N2.Persistence.PropertyPersistenceLocation.Detail)]
		public virtual string TemplateName { get; set; }
	}
}