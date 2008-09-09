using Castle.Core;
using N2.Templates.Items;
using N2.Templates.Web;
using N2.Templates.Web.UI;
using N2.Plugin;

namespace N2.Templates.Services
{
    public class SEOPageModifier : IPageModifier, IStartable, IAutoStart
    {
        private readonly IPageModifierContainer modifierContainer;

        public SEOPageModifier(IPageModifierContainer modifierContainer)
        {
            this.modifierContainer = modifierContainer;
        }

        public void Modify<T>(TemplatePage<T> page) where T : AbstractPage
        {
            new TitleAndMetaTagApplyer(page, page.CurrentPage);
        }

        public void Start()
        {
            modifierContainer.Add(this);
        }

        public void Stop()
        {
            modifierContainer.Remove(this);
        }
    }
}