using System.Web;
using N2.Engine;
using N2.Web;

namespace N2.Edit
{
    [Service(typeof(IAjaxService))]
    public class SlugGenerator : IAjaxService
    {
        private Slug slug;

        public SlugGenerator(Slug slug)
        {
            this.slug = slug;
        }

        public string Name
        {
            get { return "sluggenerator"; }
        }

        public bool RequiresEditAccess
        {
            get { return false; }
        }

        public bool IsValidHttpMethod(string httpMethod)
        {
            return true;
        }

        public void Handle(HttpContextBase context)
        {
            string text = context.Request["title"];

            context.Response.ContentType = "text/plain";
            context.Response.Write(this.slug.Create(text));
        }
    }
}
