using MvcContrib.FluentHtml.Elements;
using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Details;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Models.Parts
{
	[WithEditableTitle("Question", 10, Focus = false)]
	[RestrictParents(typeof (ISurvey))]
	[AllowedZones("Questions", "")]
	public abstract class Question : AbstractItem, IQuestion
	{
		public abstract IElement CreateHtmlElement();

		public abstract string ElementID { get; }
		public abstract string GetAnswerText(string value);

		public virtual string QuestionText
		{
			get { return Title; }
		}
	}
}