using N2;
using N2.Details;
using N2.Persistence;
using N2.Definitions;

namespace Dinamico.Models
{
	/// <summary>
	/// Content page model used in several places:
	///  * It serves as base class for start page
	///  * It's the base for "template first" definitions located in /dinamico/default/views/contentpages/
	/// </summary>
	[PageDefinition]
	[WithEditableTemplateSelection(ContainerName = Defaults.Containers.Metadata)]
	public class ContentPage : PageModelBase
	{
		/// <summary>
		/// Image used on the page and on listings.
		/// </summary>
		[EditableMediaUpload(PreferredSize = "wide")]
		[Persistable(Length = 256)] // to minimize select+1
		public virtual string Image { get; set; }

		/// <summary>
		/// Title that replaces the regular title when not empty.
		/// </summary>
		[EditableText(Title = "SEO Title", ContainerName = Defaults.Containers.Metadata)]
		public virtual string SeoTitle { get; set; }

		/// <summary>
		/// Summary text displayed in listings.
		/// </summary>
		[EditableText]
		[Persistable(Length = 1024)] // to minimize select+1
		public virtual string Summary { get; set; }

		/// <summary>
		/// Main content of this content item.
		/// </summary>
		[EditableFreeTextArea]
		[DisplayableTokens]
		public virtual string Text { get; set; }
	}
}