using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Engine;
using N2.Plugin;
using N2.Definitions;

namespace N2.Web
{
    [Service]
    public class DirectUrlInjector : IAutoStart
    {
        private IHost host;
        private IUrlParser parser;
        private IContentItemRepository repository;
        private IDefinitionManager definitions;

        public DirectUrlInjector(IHost host, IUrlParser parser, IContentItemRepository repository, IDefinitionManager definitions)
        {
            this.host = host;
            this.parser = parser;
            this.repository = repository;
            this.definitions = definitions;
        }

        void parser_BuildingUrl(object sender, UrlEventArgs e)
        {
            var source = e.AffectedItem as IUrlSource;
            if (source == null || e.AffectedItem.ID == 0)
                return;

            if (string.IsNullOrEmpty(source.DirectUrl))
                return;

            e.Url = source.DirectUrl;
        }

        void parser_PageNotFound(object sender, PageNotFoundEventArgs e)
        {
            Url url = e.Url;
            var segments = url.Path.Trim('~', '/').Split('/');
            var applicationSegments = Url.ApplicationPath.Count(c => c == '/');
            for (int i = segments.Length; i >= applicationSegments; i--)
            {
                var partialUrl = "/" + string.Join("/", segments, 0, i);
                foreach (var item in repository.Find((Parameter.Like("DirectUrl", partialUrl).Detail() & Parameter.Equal("State", ContentState.Published)).Take(host.Sites.Count + 1)))
                {
                    if (i >= segments.Length)
                    {
                        // direct hit
                        if (TryApplyFoundItem(url, e, item))
                            return;
                    }
                    else
                    {
                        // try to find subpath
                        var remainder = string.Join("/", segments, i, segments.Length - applicationSegments);
                        var child = item.GetChild(remainder);
                        if (child != null)
                        {
                            if (TryApplyFoundItem(url, e, child))
                                return;
                        }
                    }
                }
            }
        }

        private bool TryApplyFoundItem(Url url, PageNotFoundEventArgs e, ContentItem item)
        {
            if (!string.IsNullOrEmpty(url.Authority))
            {
                var site = host.GetSite(item);
                if (!string.IsNullOrEmpty(site.Authority) && !site.Is(url.Authority))
                    return false;
            }

            e.AffectedItem = item;
            if (e.AffectedPath != null)
                e.AffectedPath.CurrentItem = item;
            return true;
        }

        private string ToKey(string url)
        {
            return url.Trim('~', '/');
        }

        public void Start()
        {
            if (DefinitionsContainsUrlSources())
            {
                parser.PageNotFound += parser_PageNotFound;
                parser.BuildingUrl += parser_BuildingUrl;
            }
        }

        public void Stop()
        {
            if (DefinitionsContainsUrlSources())
            {
                parser.PageNotFound -= parser_PageNotFound;
                parser.BuildingUrl -= parser_BuildingUrl;
            }
        }

        private bool DefinitionsContainsUrlSources()
        {
            return definitions.GetDefinitions().Any(d => typeof(IUrlSource).IsAssignableFrom(d.ItemType));
        }
    }
}
