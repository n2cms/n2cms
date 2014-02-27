using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using N2.Engine;

namespace N2.Web.Rendering
{
    [Service]
    public class DisplayableRendererSelector
    {
        IDisplayableRenderer[] renderers;
        Dictionary<Type, IDisplayableRenderer> rendererForType = new Dictionary<Type, IDisplayableRenderer>();

        public DisplayableRendererSelector(IDisplayableRenderer[] renderers)
        {
            this.renderers = renderers
                .OrderByDescending(r => Utility.InheritanceDepth(r.HandledDisplayableType))
                .ThenByDescending(r => Utility.InheritanceDepth(r.GetType()))
                .ToArray();
        }

        public virtual IDisplayableRenderer ResolveRenderer(Type displayableType)
        {
            IDisplayableRenderer renderer;
            if (rendererForType.TryGetValue(displayableType, out renderer))
                return renderer;

            var temp = new Dictionary<Type, IDisplayableRenderer>(rendererForType);
            temp[displayableType] = renderer = renderers.FirstOrDefault(r => r.HandledDisplayableType.IsAssignableFrom(displayableType));
            rendererForType = temp;

            return renderer;
        }

        public void Render(RenderingContext context, TextWriter writer)
        {
            var tw = WebExtensions.GetEditableWrapper(context.Content, context.IsEditable, context.PropertyName, context.Displayable, writer);

            using (tw)
            {
                var renderer = ResolveRenderer(context.Displayable.GetType());
                renderer.Render(context, writer);
            }
        }
    }
}
