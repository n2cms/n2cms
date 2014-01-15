using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace N2.Addons.Wiki.Renderers
{
    public class TextRenderer : IRenderer
    {
        #region IControlRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            Literal l = new Literal();
            using (var writer = new StringWriter())
            {
                bool newlineToPrev = context.Previous == null || context.Previous.Command == "Heading" || context.Previous.Command == "UnorderedListItem" || context.Previous.Command == "OrderedListItem" || context.Previous.Tokens.Last().Type == N2.Web.Parsing.TokenType.Element;
                bool newlineToNext = context.Next == null || context.Next.Tokens.First().Type == N2.Web.Parsing.TokenType.Element;
                
                for (int i = 0; i < context.Fragment.Tokens.Count; i++)
                {
                    var t = context.Fragment.Tokens[i];

                    if(t.Type == N2.Web.Parsing.TokenType.NewLine)
                    {
                        // ignore newlines next to an element
                        bool isFirst = i == 0;
                        bool isLast = i == context.Fragment.Tokens.Count - 1;
                        if (newlineToPrev && isFirst) 
                            continue;
                        if (newlineToNext && isLast)
                            continue;
                        if (!isFirst && (context.Fragment.Tokens[i - 1].Type == N2.Web.Parsing.TokenType.Element || context.Fragment.Tokens[i - 1].Type == N2.Web.Parsing.TokenType.EndElement))
                            continue;
                        if (!isLast && (context.Fragment.Tokens[i + 1].Type == N2.Web.Parsing.TokenType.Element || context.Fragment.Tokens[i + 1].Type == N2.Web.Parsing.TokenType.EndElement))
                            continue;
                    }

                    if (t.Type == N2.Web.Parsing.TokenType.NewLine)
                        writer.Write(t.Fragment.Replace(Environment.NewLine, "<br/>"));
                    else
                        writer.Write(t.Fragment);
                }
                l.Text = writer.ToString();
            }
            container.Controls.Add(l);
            return l;
        }

        #endregion
    }
}
