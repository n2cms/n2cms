using System;
using System.Collections.Generic;
using System.Linq;
using N2.Templates.Details;
using N2.Web;

namespace N2.Templates.Mvc.Models.Parts
{
	public abstract class OptionSelectQuestion : Question
	{
		public override TagBuilder CreateHtmlElement()
		{
			var inputs = new List<TagBuilder>();

			foreach (var option in Options)
			{
				var id = ElementID + "_" + option.ID;
				inputs.Add(new TagBuilder("div", new TagBuilder("label", option.Title)
													.Attr("For", id)
												 + new TagBuilder("input")
													.Attr("type", InputType)
													.Attr("value", option.ID.ToString())
													.Attr("name", ElementID)
													.Id(id)));
			}

			return new TagBuilder("div", String.Join(Environment.NewLine, inputs.Select(i => i.ToString()).ToArray()))
				.Attr("class", "alternatives");
		}

		protected abstract string InputType { get; }

		[EditableOptions(Title = "Options", SortOrder = 20)]
		public virtual IList<Option> Options
		{
			get
			{
				var options = new List<Option>();
				foreach (Option o in GetChildren())
					options.Add(o);
				return options;
			}
		}
	}
}