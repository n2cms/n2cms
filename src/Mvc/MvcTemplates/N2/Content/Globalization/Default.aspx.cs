using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Configuration;
using N2.Edit.Web;
using N2.Engine.Globalization;
using N2.Web;

namespace N2.Edit.Globalization
{
    [ToolbarPlugin("LANGS", "globalization", "{ManagementUrl}/Content/Globalization/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Options, Targets.Preview, "{ManagementUrl}/Resources/icons/world.png", 150, 
        ToolTip = "view translations",
        GlobalResourceClassName = "Toolbar",
        OptionProvider = typeof(GlobalizationOptionProvider),
        Legacy = true)]
    public partial class _Default : EditPage
    {
        protected ILanguageGateway gateway;
        protected IEnumerable<ILanguage> languages;
        private readonly Engine.Logger<_Default> logger;

        protected override void OnInit(EventArgs e)
        {
            Initialize();
            base.OnInit(e);
        }

        private void Initialize()
        {
            gateway = Engine.Resolve<LanguageGatewaySelector>().GetLanguageGateway(Selection.SelectedItem);

            cvGlobalizationDisabled.IsValid = gateway.Enabled;
            bool isGlobalized = gateway.GetLanguage(Selection.SelectedItem) != null;
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
            var items = new ItemList();
            foreach (var parent in gateway.FindTranslations(Selection.SelectedItem))
            {
	            if (getPages)
	            {
		            foreach (ContentItem child in parent.GetChildPagesUnfiltered().Where(Engine.EditManager.GetEditorFilter(User)))
			            if (!items.ContainsAny(gateway.FindTranslations(child)))
				            items.Add(child);
	            }
	            else
	            {
					foreach (ContentItem child in parent.GetChildPartsUnfiltered().Where(Engine.EditManager.GetEditorFilter(User)))
						if (!items.ContainsAny(gateway.FindTranslations(child)))
							items.Add(child);
	            }
            }
            items.Sort();

            foreach (ContentItem item in items)
            {
	            if (item is ILanguage)
                    continue;
	            if (item.IsPage == getPages)
		            yield return item;
            }
        }

        protected string ReturnUrl
        {
            get { return Server.UrlEncode(Request.RawUrl); }
        }

        protected IEnumerable<TranslateSpecification> GetTranslations(ContentItem item)
        {
            foreach (TranslateSpecification translate in gateway.GetEditTranslations(item, true, true))
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
            try
            {
                gateway.Associate(items);
            }
            catch (N2Exception ex)
            {
                Engine.Resolve<IErrorNotifier>().Notify(ex);
                cvAssociateLanguageRoots.IsValid = false;
            }
            DataBind();
        }

        protected void btnUnassociate_Click(object sender, EventArgs e)
        {
            List<ContentItem> items = GetSelectedItems();
            foreach(ContentItem item in items)
            {
                gateway.Unassociate(item);
            }
            DataBind();
        }

        protected void btnEnable_Click(object sender, EventArgs args)
        {
            try
            {
                System.Configuration.Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

                EngineSection engineConfiguration = (EngineSection)cfg.GetSection("n2/engine");
                engineConfiguration.Globalization.Enabled = true;

                cfg.Save();

                cvLanguageRoots.IsValid = Engine.Resolve<ILanguageGateway>().GetAvailableLanguages().GetEnumerator().MoveNext();
                Initialize();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
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
