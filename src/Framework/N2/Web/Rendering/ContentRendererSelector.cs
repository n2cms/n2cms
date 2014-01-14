using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using N2.Engine;
using System.Web.Routing;
using N2.Details;

namespace N2.Web.Rendering
{
    [Service]
    public class ContentRendererSelector
    {
        IContentRenderer[] renderers;
        Dictionary<Type, IContentRenderer> rendererForType = new Dictionary<Type, IContentRenderer>();

        public ContentRendererSelector(IContentRenderer[] renderers)
        {
            this.renderers = renderers
                .OrderByDescending(r => Utility.InheritanceDepth(r.HandledContentType))
                .ThenByDescending(r => Utility.InheritanceDepth(r.GetType()))
                .ToArray();
        }

        public virtual IContentRenderer ResolveRenderer(Type displayableType)
        {
            IContentRenderer renderer;
            if (rendererForType.TryGetValue(displayableType, out renderer))
                return renderer;

            renderer = renderers.FirstOrDefault(r => r.HandledContentType.IsAssignableFrom(displayableType));
            if (renderer == null)
                return null;

            var temp = new Dictionary<Type, IContentRenderer>(rendererForType);
            temp[displayableType] = renderer;
            rendererForType = temp;

            return renderer;
        }

        public bool TryRender(RenderingContext context, TextWriter writer)
        {
            var renderer = ResolveRenderer(context.Displayable.GetType());
            if (renderer == null)
                return false;

            renderer.Render(context, writer);
            return true;
        }
    }
}
