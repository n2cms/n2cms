using System.Collections.Generic;
using System.Linq;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Web;

namespace N2.Edit.Globalization
{
    [Service]
    public class GlobalizationOptionProvider : IProvider<ToolbarOption>
    {
        ILanguageGateway languages;
        IHost host;
        IContentAdapterProvider adapters;

        public GlobalizationOptionProvider(ILanguageGateway languages, IContentAdapterProvider adapters, IHost host)
        {
            this.languages = languages;
            this.host = host;
            this.adapters = adapters;
        }

        #region IProvider<ToolbarOption> Members

        public ToolbarOption Get()
        {
            return GetAll().FirstOrDefault();
        }

        public IEnumerable<ToolbarOption> GetAll()
        {
            return languages.GetAvailableLanguages()
                .Where(l => l is ContentItem)
                .Select((l, i) => new ToolbarOption
                {
                    Title = GetHostPrefix((ContentItem)l) + l.LanguageTitle,
                    Target = Targets.Preview,
                    SortOrder = i,
                    Name = l.LanguageCode,
                    Url = adapters.ResolveAdapter<NodeAdapter>((ContentItem)l).GetPreviewUrl((ContentItem)l)
                });
        }

        private string GetHostPrefix(ContentItem item)
        {
            var site = host.GetSite(item);
            if (site == null || string.IsNullOrEmpty(site.Authority))
                return null;

            return site.Authority + " / ";
        }

        #endregion
    }
}
