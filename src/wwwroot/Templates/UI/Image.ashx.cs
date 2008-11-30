using System.Web;
using System.Web.Hosting;
using N2.Templates.Services;

namespace N2.Templates.UI
{
    public class Image : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string imageUrl = context.Request["img"];
            string w = context.Request["w"];
            string h = context.Request["h"];

            double width = 0;
            double.TryParse(w, out width);

            double height = 0;
            double.TryParse(h, out height);

            context.Response.ContentType = "image/jpeg";

			string path = HostingEnvironment.MapPath(imageUrl);
            //context.Response.Cache.SetExpires(DateTime.Now.AddHours(1));
            //context.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate | HttpCacheability.Public);
            //context.Response.Cache.SetValidUntilExpires(true);
            ImageResizer ir = new ImageResizer();
            ir.Resize(path, width, height, context.Response.OutputStream);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}