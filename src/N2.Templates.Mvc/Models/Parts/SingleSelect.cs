using System;
using System.Linq;
using MvcContrib;
using MvcContrib.UI;
using MvcContrib.UI.Tags;

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
			var list = new RadioList
			           	{
			           		Name = ElementID
			           	};

			foreach (var field in base.Options)
			{
				var radioField = new RadioField {Id = "ss_el_" + field.ID, Value = field.ID, Label = field.Title};

				list.Add(radioField);
			}

			return new Element("span", new Hash(@class => "alternatives")) {InnerText = list.ToString()};
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