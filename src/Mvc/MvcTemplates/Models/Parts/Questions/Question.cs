using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Details;
using N2.Templates.Mvc.Models.Pages;
using System.Web.Mvc;

namespace N2.Templates.Mvc.Models.Parts.Questions
{
    [WithEditableTitle("Question", 10, Focus = false)]
    [RestrictParents(typeof (ISurvey))]
    [AllowedZones("Questions", "")]
    public abstract class Question : PartBase, IQuestion
    {
        #region IQuestion Members

        public abstract MvcHtmlString CreateHtmlElement();

        public virtual string ElementID { get { return "q" + ID; } }

        public virtual string QuestionText
        {
            get { return Title; }
        }

        public virtual void AppendAnswer(AnswerContext context, string postedValue)
        {
            context.AppendAnswer(QuestionText, GetAnswerText(postedValue));
        }

        #endregion

        public virtual string GetAnswerText(string value)
        {
            return value;
        }
    }
}
