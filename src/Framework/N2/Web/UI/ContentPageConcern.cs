using System.Web.UI;

namespace N2.Web.UI
{
	/// <summary>Base class for UI concerns. Implementors are given a template instance bore it's executed by ASP.NET.</summary>
	public abstract class ContentPageConcern : IContentPageConcern
	{
		/// <summary>Applies the concern to the given template.</summary>
		/// <param name="page">The template to apply the concern to.</param>
		/// <param name="item">The current item.</param>
		public abstract void OnPreInit(Page page, ContentItem item);
	}
}
