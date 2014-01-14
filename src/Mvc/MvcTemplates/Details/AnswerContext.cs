using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text;

namespace N2.Templates.Mvc.Details
{
    public class AnswerContext
    {
        public AnswerContext()
        {
            Body = new StringBuilder();
            Attachments = new List<Attachment>();
            ValidationErrors = new Dictionary<string, string>();
        }

        public string Subject { get; set; }
        public StringBuilder Body { get; private set; }
        public List<Attachment> Attachments { get; private set; }
        public Dictionary<string, string> ValidationErrors { get; private set; }
        public HttpContextBase HttpContext { get; set; }

        public void AppendAnswer(string question, string answer)
        {
            Body.AppendFormat("{0}: {1}{2}", question, answer, Environment.NewLine);
        }
    }
}
