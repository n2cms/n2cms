using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace N2.Web.Parts
{
    /// <summary>
    /// Ajax service that adds itself to the ajax request dispatecher upon start.
    /// </summary>
    public abstract class PartsAjaxService : IAjaxService
    {
        public abstract string Name { get;}

        public bool RequiresEditAccess
        {
            get { return true; }
        }

        public bool IsValidHttpMethod(string httpMethod)
        {
            return httpMethod == "POST";
        }

        public void Handle(HttpContextBase context)
        {
            NameValueCollection response = HandleRequest(context.Request.Form);
            context.Response.ContentType = "application/json";
            context.Response.Write(ToJson(response));
        }

        protected string ToJson(NameValueCollection response)
        {
            StringBuilder sb = new StringBuilder();
            using (new N2.Persistence.NH.Finder.StringWrapper(sb, "{", "}"))
            {
                sb.AppendFormat(@"""{0}"": ""{1}""", "error", "false");
                foreach (string key in response.Keys)
                {
                    sb.AppendFormat(@", ""{0}"": ""{1}""", key, response[key]);
                }
            }
            return sb.ToString();
        }

        public abstract NameValueCollection HandleRequest(NameValueCollection request);
    }
}
