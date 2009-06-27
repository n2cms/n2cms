using N2.Web.UI;

namespace N2.Templates.Mvc.Web
{
	public interface IPageModifier
	{
		void Modify<T>(ContentPage<T> page) where T : ContentItem;
	}
}