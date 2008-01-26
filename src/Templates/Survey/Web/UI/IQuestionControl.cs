using System;
namespace N2.Templates.Survey.Web.UI
{
	public interface IQuestionControl
	{
		string Question { get; }
		string AnswerText { get; }
	}
}
