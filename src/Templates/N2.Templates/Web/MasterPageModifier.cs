namespace N2.Templates.Web
{
	public class MasterPageModifier : IPageModifier
	{
		public void Modify<T>(UI.TemplatePage<T> page) 
			where T : Items.AbstractPage
		{
			page.MasterPageFile = Find.StartPage.Layout;
		}
	}
}