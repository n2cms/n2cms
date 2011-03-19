using N2;

namespace Dinamico.Models
{
	[PageDefinition]
	public class ContentPage : TextPage
	{
		[N2.Persistence.Persistable(PersistAs = N2.Persistence.PropertyPersistenceLocation.Detail)]
		public virtual string TemplateName { get; set; }
	}
}