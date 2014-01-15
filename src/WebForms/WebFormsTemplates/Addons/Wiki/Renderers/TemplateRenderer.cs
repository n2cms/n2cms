using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace N2.Addons.Wiki.Renderers
{
    public class TemplateRenderer : IRenderer
    {
        IDictionary<string, ITemplateRenderer> renderers = new Dictionary<string, ITemplateRenderer>();
        public TemplateRenderer(IEnumerable<ITemplateRenderer> renderers)
        {
            foreach (ITemplateRenderer renderer in renderers)
                this.renderers[renderer.Name.ToLower()] = renderer;
        }
        #region IRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            string fragment = context.Fragment.ToString();
            string key = fragment.Substring(2, fragment.Length -4).ToLower();
            if (renderers.ContainsKey(key))
                return renderers[key].AddTo(container, context);
            else
                return null;
        }

        #endregion
    }
}
