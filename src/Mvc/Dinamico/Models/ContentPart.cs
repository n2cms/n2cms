using N2;

namespace Dinamico.Models
{
	[PartDefinition]
	public class ContentPart : ContentItem
	{
		[N2.Persistence.Persistable(PersistAs = N2.Persistence.PropertyPersistenceLocation.Detail)]
		public virtual string TemplateName { get; set; }
	}
}