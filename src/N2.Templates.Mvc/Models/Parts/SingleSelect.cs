using System;
using System.Linq;
using MvcContrib.FluentHtml.Elements;

namespace N2.Templates.Mvc.Models.Parts
{
	[PartDefinition("Single Select (radio buttons)")]
	public class SingleSelect : OptionSelectQuestion
	{
		public override string ElementID
		{
			get { return "ss_" + ID; }
		}

		public override IElement CreateHtmlElement()
		{
			RadioSet radioSet = new RadioSet(ElementID).Options(base.Options, x => x.ID, x => x.Title);

			return new Literal("").Html(radioSet.ToString()).Class("alternatives");
		}

		public override string GetAnswerText(string value)
		{
			var selectedOption = Options.FirstOrDefault(opt => opt.ID.ToString() == value);

			if (selectedOption == null)
				return String.Empty;

			return selectedOption.Title;
		}
	}
}