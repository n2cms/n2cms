using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Web.Parsing;
using System.Web.UI.WebControls;

namespace N2.Addons.Wiki.Renderers
{
    public abstract class ListRenderer : IRenderer
    {
        protected abstract string BeginTag { get; }
        protected abstract string EndTag { get; }

        public Control AddTo(Control overallContainer, ViewContext context)
        {
            Component f = context.Fragment;
            int prevLevel = (context.Previous == null || context.Previous.Command != f.Command) ? 0 : context.Previous.Argument.Length;
            int nextLevel = (context.Next == null || context.Next.Command != f.Command) ? 0 : context.Next.Argument.Length;
            int level = f.Argument.Length;

            var container = new PlaceHolder();
            overallContainer.Controls.Add(container);

            if(prevLevel < level)
            {
                for (int i = prevLevel; i < level; i++)
                {
                    Add(BeginTag, container);
                    Add("<li>", container);
                }
            }
            else
                Add("<li>", container);

            context.Renderer.AddTo(context.Fragment.Components, container, context.Article);

            if (nextLevel < level)
            {
                for (int i = nextLevel; i < level; i++)
                {
                    Add("</li>", container);
                    Add(EndTag, container);
                }
                if (nextLevel != 0)
                    Add("</li>", container);
            }
            else if (nextLevel == level)
                Add("</li>", container);

            return container;
        }

        private void Add(string html, Control container)
        {
            container.Controls.Add(new LiteralControl(html));
        }
    }
}
