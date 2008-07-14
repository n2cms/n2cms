using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using N2.Edit.Web;
using N2.Globalization;
using N2.Collections;
using N2.Configuration;

namespace N2.Edit.Globalization
{
	[ToolbarPlugin("", "globalization", "~/Edit/Globalization/Default.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "~/Edit/Img/Ico/world.gif", 150, ToolTip = "view translations", GlobalResourceClassName = "Toolbar")]
	public partial class _Default : EditPage
	{
		protected ILanguageGateway gateway;
		protected IEnumerable<ILanguage> languages;

		protected override void OnInit(EventArgs e)
		{
            hlCancel.NavigateUrl = CancelUrl();

            Initialize();
			base.OnInit(e);
		}

        private void Initialize()
        {
            gateway = Engine.Resolve<ILanguageGateway>();

            cvGlobalizationDisabled.IsValid = gateway.Enabled;
            bool isGlobalized = gateway.GetLanguage(SelectedItem) != null;
            cvOutsideGlobalization.IsValid = isGlobalized;
            btnEnable.Visible = !gateway.Enabled;

            if (gateway.Enabled && isGlobalized)
            {
                languages = gateway.GetAvailableLanguages();
                DataBind();
            }
            else
            {
                pnlLanguages.Visible = false;
            }
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

        protected void btnEnable_Click(object sender, EventArgs args)
        {
            try
            {
                System.Configuration.Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

                N2.Configuration.GlobalizationSection globalization = (GlobalizationSection)cfg.GetSection("n2/globalization");
                globalization.Enabled = true;

                cfg.Save();

                cvLanguageRoots.IsValid = Engine.Resolve<ILanguageGateway>().GetAvailableLanguages().GetEnumerator().MoveNext();
                Initialize();
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
                cvEnable.IsValid = false;
            }
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
