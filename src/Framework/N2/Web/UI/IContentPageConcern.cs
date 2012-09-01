using System;

namespace N2.Web.UI
{
	/// <summary>
	/// When applied to an attribute on a <see cref="ContentPage"/> the interface is called by the page.
	/// </summary>
	public interface IContentPageConcern
	{
		void OnPreInit(System.Web.UI.Page page, N2.ContentItem item);
	}
}
