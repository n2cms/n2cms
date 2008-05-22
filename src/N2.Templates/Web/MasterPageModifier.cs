using System.IO;
namespace N2.Templates.Web
{
	public class MasterPageModifier : IPageModifier
	{
		bool layoutVerified = false;

		public void Modify<T>(UI.TemplatePage<T> page) 
			where T : Items.AbstractPage
		{
			string layout = Find.StartPage.Layout;
			if (!layoutVerified && layout != null)
			{
				if(File.Exists(page.Server.MapPath(layout)))
					layoutVerified = true;
			}

			if (layoutVerified)
			{
				page.MasterPageFile = layout;
			}
		}
	}
}