using System;
using System.Linq;
using MvcContrib;
using MvcContrib.UI;
using MvcContrib.UI.Tags;

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
			var list = new CheckBoxList
			           	{
			           		Name = ElementID
			           	};

			foreach (var field in base.Options)
			{
				var radioField = new CheckBoxField {Id = "ms_el_" + field.ID, Value = field.ID, Label = field.Title};

				list.Add(radioField);
			}

			return new Element("span", new Hash(@class => "alternatives")) {InnerText = list.ToString()};
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