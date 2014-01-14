using N2;
using N2.Definitions;
using N2.Definitions.Runtime;
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSharp.Models
{
    [PageDefinition("Content Page", TemplateUrl = "~/Views/ContentView.aspx")]
    [WithEditableTitle]
    public class ContentPage : ContentItem, IContentPage
    {
        [EditableFreeTextArea]
        public virtual string Text { get; set; }
    }
}
