using System;
using MvcContrib.FluentHtml.Elements;
using N2.Details;

namespace N2.Templates.Mvc.Models.Parts
{
	[PartDefinition("Text question (textbox)")]
	public class TextQuestion : Question
	{
		[EditableTextBox("Rows", 110)]
		public virtual int Rows
		{
			get { return (int) (GetDetail("Rows") ?? 1); }
			set { SetDetail("Rows", value, 1); }
		}

		[EditableTextBox("Columns", 120)]
		public virtual int? Columns
		{
			get { return (int?) GetDetail("Columns"); }
			set { SetDetail("Columns", value); }
		}

		public override string ElementID
		{
			get { return "txt_" + ID; }
		}

		public override string GetAnswerText(string value)
		{
			return value;
		}

		public override IElement CreateHtmlElement()
		{
			if (Rows == 1)
				return new TextBox(ElementID).Size(Columns ?? 60);
			return new TextArea(ElementID).Rows(Rows).Columns(Columns ?? 60);
		}
	}
}