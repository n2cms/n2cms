using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Reflection;
using N2.Details;
using N2.TemplateWeb.Domain;
using N2.Web;

namespace N2.TemplateWeb
{
    public partial class Default : N2.Web.UI.Page<Domain.MyPageData>
    {
		protected IWebContext WC
		{
			get { return N2.Context.Current.Resolve<IWebContext>(); }
		}

    	protected HyperLink HyperLink1;

        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
			{
				//HyperLink1.Text = CurrentPage.MyLabel;
				//HyperLink1.NavigateUrl = CurrentPage.MyFile;
			}
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
			IList<ContentItem> salutes = N2.Find.Items.Where.Title.Like("Hello W%").Select();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
			IList<MyPageData> notSaluting = N2.Find.Items.Where.Title.NotLike("Hello W%")
				.And.Type.Eq(typeof(MyPageData))
				.Select<MyPageData>();

			foreach (MyPageData page in notSaluting)
			{
				page.Title = "Hello World";
				N2.Context.Persister.Save(page);
			}
		}

        protected void Button3_Click(object sender, EventArgs e)
        {
        	ContentItem child = CurrentPage.GetChild("here here my son");

			if (child == null)
			{
				child = N2.Context.Definitions.CreateInstance<MyPageData>(CurrentPage);
				child.Title = "the lost child";
				child.Name = "here here my son";
				N2.Context.Persister.Save(child);
			}
        }

    	protected IList<Assembly> GetFilteredAssembliyList()
		{
			return N2.Context.Current.Resolve<N2.Engine.ITypeFinder>().GetAssemblies();
		}
    }

	//[N2.Item("Special page")]
	//public class SpecialPage : TextPage
	//{
	//    // inherits definitions from the base class
	//}
		
	//[N2.Item("Default content page")]
	//public class TextPage : N2.ContentItem
	//{
	//    [EditableFreeTextArea("Text", 100)]
	//    public virtual string Text
	//    {
	//        get { return (string)GetDetail("Text"); }
	//        set { SetDetail("Text", value); }
	//    }
	//}


}
