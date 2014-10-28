using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Templates.Details;
using N2.Web.Mvc.Html;
using N2.Details;

namespace N2.Templates.Mvc.Models.Parts.Questions
{
    public enum OptionOrientation
    {
        Vertical,
        Horizontal
    }

    public abstract class OptionSelectQuestion : Question
    {
        public override MvcHtmlString CreateHtmlElement()
        {
            var inputs = new List<TagBuilder>();

            foreach (var option in Options)
            {
                var id = ElementID + "_" + option.ID;
                var label = new TagBuilder("label")
                    .Html(option.Title)
                    .Attr("For", id);
                var input = new TagBuilder("input")
                    .Attr("type", InputType)
                    .Attr("value", option.ID.ToString())
                    .Attr("name", ElementID)
                    .Id(id);
                inputs.Add(new TagBuilder("span").Attr("class", "option").Html(input.ToString(TagRenderMode.SelfClosing) + label.ToString()));
            }

            string innerHtml = string.Join(Environment.NewLine, inputs.Select(i => i.ToString()).ToArray());
            string outerHtml = new TagBuilder("div")
                .Html(innerHtml)
                .Attr("class", "alternatives " + Orientation.ToString().ToLower())
                .ToString();
            return MvcHtmlString.Create(outerHtml);
        }

        protected abstract string InputType { get; }

        [EditableOptions(Title = "Options", SortOrder = 20)]
        public virtual IList<Option> Options
        {
            get
            {
                var options = new List<Option>();
				foreach (Option o in Children.WhereAccessible().OfType<Option>())
                    options.Add(o);
                return options;
            }
        }

        [EditableEnum("Orientation", 30, typeof(OptionOrientation))]
        public virtual OptionOrientation Orientation { get; set; }
    }
}
