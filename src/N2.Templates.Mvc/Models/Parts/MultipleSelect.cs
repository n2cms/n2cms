using System;
using System.Linq;
using MvcContrib.FluentHtml.Elements;

namespace N2.Templates.Mvc.Models.Parts
{
	[PartDefinition("Multiple Select (check boxes)")]
	public class MultipleSelect : OptionSelectQuestion
	{
		public override string ElementID
		{
			get { return "ms_" + ID; }
		}

		public override IElement CreateHtmlElement()
		{
			var cbl = new CheckBoxList(ElementID).Options(base.Options, x => x.ID, x => x.Title);

			return new Literal("").Html(cbl.ToString()).Class("alternatives");
		}

		public override string GetAnswerText(string value)
		{
			var values = (value ?? String.Empty).Split(',');

			var selectedOptions = Options.Where(opt => values.Contains(opt.ID.ToString())).Select(opt => opt.Title).ToArray();

			if (selectedOptions.Length == 0)
				return String.Empty;

			return String.Join(", ", selectedOptions);
		}
	}
}