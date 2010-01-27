using MvcContrib.FluentHtml.Elements;

namespace N2.Templates.Mvc.Details
{
	public interface IQuestion
	{
		IElement CreateHtmlElement();
		string QuestionText { get; }
		string ElementID { get; }
		string GetAnswerText(string value);
	}
}