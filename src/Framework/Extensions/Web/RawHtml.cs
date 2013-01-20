/*************************************************************************************************

Raw HTML: Generic Model and MVC Adapter
Licensed to users of N2CMS under the terms of The MIT License (MIT)

Copyright (c) 2013 Benjamin Herila <mailto:ben@herila.net>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, 
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*************************************************************************************************/

using System;
using N2.Web.Mvc;
using N2.Engine;
using N2.Details;

namespace N2.Web
{
    [PartDefinition(Title = "Raw HTML", IconUrl = "~/N2/Resources/Icons/tag.png") ]
    public class RawHtml : ContentItem
    {
        [EditableText(Rows = 10, Columns = 50, TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine)]
        public string HtmlContent
        {
            get { return GetDetail("Body", ""); }
            set { SetDetail("Body", value, ""); }
        }
    }

    [Adapts(typeof(RawHtml))]
    public class RawHtmlAdapter : MvcAdapter
    {
        public override void RenderTemplate(System.Web.Mvc.HtmlHelper html, ContentItem model)
        {
            if (!(model is RawHtml))
                throw new ArgumentException("This adapter can only be used to adapt RawHTML parts.");
            html.ViewContext.Writer.Write((model as RawHtml).HtmlContent);
        }
    }

}