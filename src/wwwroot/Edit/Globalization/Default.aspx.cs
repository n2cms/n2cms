using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Globalization;
using System.Collections.Generic;
using N2.Collections;

namespace N2.Edit.Globalization
{
	[ToolbarPlugin("", "globalization", "~/Edit/Globalization/Default.aspx?selected={selected}", ToolbarArea.Preview, "preview", "~/Edit/Img/Ico/world.gif", 150, ToolTip = "view translations", GlobalResourceClassName = "Toolbar")]
	public partial class _Default : EditPage
	{
		protected ILanguageGateway gateway;
		protected IEnumerable<ILanguage> languages;

		protected override void OnInit(EventArgs e)
		{
            hlCancel.NavigateUrl = SelectedNode.PreviewUrl;

            gateway = Engine.Resolve<ILanguageGateway>();

            cvGlobalizationDisabled.IsValid = gateway.Enabled;
            bool isGlobalized = gateway.GetLanguage(SelectedItem) != null;
            cvOutsideGlobalization.IsValid = isGlobalized;

            if (gateway.Enabled && isGlobalized)
            {
                languages = gateway.GetAvailableLanguages();
                DataBind();
            }
			base.OnInit(e);
		}

		protected IEnumerable<ContentItem> GetChildren(bool getPages)
		{
            ItemList items = new ItemList();
            foreach (ContentItem parent in gateway.FindTranslations(SelectedItem))
            {
                foreach (ContentItem child in parent.GetChildren(Engine.EditManager.GetEditorFilter(User)))
                {
                    if(!items.ContainsAny(gateway.FindTranslations(child)))
                    {
                        items.Add(child);
                    }
                }
            }
            items.Sort();

            foreach (ContentItem item in items)
            {
                if (item is ILanguage)
                    continue;
                else if (item.IsPage == getPages)
                    yield return item;
            }
		}

        protected string ReturnUrl
        {
            get { return Server.UrlEncode(Request.RawUrl); }
        }

		protected IEnumerable<TranslateSpecification> GetTranslations(ContentItem item)
		{
			foreach (TranslateSpecification translate in gateway.GetEditTranslations(item, true))
			{
				translate.EditUrl += "&returnUrl=" + ReturnUrl;
				yield return translate;
			}
		}

        protected void btnAssociate_Click(object sender, EventArgs args)
        {
            List<ContentItem> items = GetSelectedItems();
            if (items.Count < 2)
            {
                cvAssociate.IsValid = false;
                return;
            }
            gateway.Associate(items);

            DataBind();
        }

        private List<ContentItem> GetSelectedItems()
        {
            List<ContentItem> items = new List<ContentItem>();
            foreach (ILanguage language in gateway.GetAvailableLanguages())
            {
                string selectedId = Request[language.LanguageCode];
                int id;
                if (int.TryParse(selectedId, out id))
                    items.Add(Engine.Persister.Get(id));
            }
            return items;
        }
	}
}
