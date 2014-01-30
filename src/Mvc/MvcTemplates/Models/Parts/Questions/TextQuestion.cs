using System;
using System.Web.Mvc;
using N2.Details;
using N2.Web.Mvc.Html;

namespace N2.Templates.Mvc.Models.Parts.Questions
{
    [PartDefinition("Text question (textbox)")]
    public class TextQuestion : Question
    {
        [EditableNumber("Rows", 110)]
        public virtual int Rows
        {
            get { return (int) (GetDetail("Rows") ?? 1); }
            set { SetDetail("Rows", value, 1); }
        }

        [EditableNumber("Columns", 120)]
        public virtual int? Columns
        {
            get { return (int?) GetDetail("Columns"); }
            set { SetDetail("Columns", value); }
        }

        public override string ElementID
        {
            get { return "txt_" + ID; }
        }

        public override MvcHtmlString CreateHtmlElement()
        {
            if (Rows == 1)
            {
                var html = new TagBuilder("input")
                    .Attr("type", "text")
                    .Attr("name", ElementID)
                    .Attr("size", (Columns ?? 60).ToString())
                    .ToString(TagRenderMode.SelfClosing);
                return MvcHtmlString.Create(html);
            }
            else
            {
                var html = new TagBuilder("textarea")
                    .Attr("rows", Rows.ToString())
                    .Attr("name", ElementID)
                    .Attr("cols", (Columns ?? 60).ToString())
                    .ToString();
                return MvcHtmlString.Create(html);
            }

        }
    }
}
