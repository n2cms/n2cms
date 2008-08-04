using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Templates.Wiki.Renderers
{
    public class ListRenderer : IRenderer
    {
        #region IRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            int level = DetermineLevel(context.Fragment.Value);
            bool ordered = context.Fragment.Name == "OrderedList";
            //Fragment f = context.Fragment;
            //Fragment prev = FindNext(f, f.Name);
            //Fragment next = FindPrevious(f, f.Name);

            //int openLevels = level;
            //if (prev != null)
            //{
            //    if (prev.Name == f.Name)
            //    {
            //        openLevels = level - DetermineLevel(prev.Value);
            //    }
            //}
            
            //for (int i = 0; i < level; i++)
            //{
            //    container.Controls.Add(new LiteralControl(ordered ? "<ol>" : "<ul>"));
            //}
            
            LiteralControl lc = new LiteralControl(level > 0 ? "<li>" : "</li>");
            container.Controls.Add(lc);

            //if (level < 0)
            //{
            //    int closeLevels = 1;
            //    if (prev != null && prev.Name == f.Name)
            //    {
            //        closeLevels = DetermineLevel(prev.Value);
            //    }
            //    if (next != null && next.Name == f.Name)
            //    {
            //        closeLevels -= DetermineLevel(next.Value);
            //    }
            //    for (int i = 0; i < closeLevels; i++)
            //    {
            //        container.Controls.Add(new LiteralControl(ordered ? "</ol>" : "</ul>"));
            //    }
            //}
            return lc;
        }

        private static Fragment FindPrevious(Fragment f, string name)
        {
            while (f.Previous != null)
            {
                if (f.Previous.Name == name)
                    break;
                f = f.Previous;
            }
            return f.Previous;
        }

        private static Fragment FindNext(Fragment f, string name)
        {
            while (f.Next != null)
            {
                if (f.Next.Name == name)
                    break;
                f = f.Next;
            }
            return f.Next;
        }

        private static int DetermineLevel(string html)
        {
            if (html.StartsWith("***") || html.StartsWith("###"))
                return 3;
            else if (html.StartsWith("**") || html.StartsWith("##"))
                return 2;
            else if (html.StartsWith("*") || html.StartsWith("#"))
                return 1;
            else
                return -1;
        }

        #endregion
    }
}
