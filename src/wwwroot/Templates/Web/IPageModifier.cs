using N2.Web.UI;

namespace N2.Templates.Web
{
	public interface IPageModifier
	{
		void Modify<T>(ContentPage<T> page) where T : ContentItem;
	}
}