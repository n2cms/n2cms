using System.Web.UI;

namespace N2.Templates.Mvc.Web
{
	public interface IPageModifier
	{
		void Modify(Page page);
	}
}