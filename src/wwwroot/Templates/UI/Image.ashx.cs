using System.Web;
using System.Web.Hosting;
using N2.Edit.FileSystem;
using N2.Templates.Services;

namespace N2.Templates.UI
{
    public class Image : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
        	//UrlDecode is neccessary to compensate encoding in ImageAdapter
        	string imageUrl = HttpUtility.UrlDecode(context.Request["img"]);
            string w = context.Request["w"];
            string h = context.Request["h"];

            double width = 0;
            double.TryParse(w, out width);

            double height = 0;
            double.TryParse(h, out height);

			IFileSystem fs = N2.Context.Current.Resolve<IFileSystem>();
        	if(fs.FileExists(imageUrl))
        	{
				context.Response.ContentType = "image/jpeg";

				string extension = VirtualPathUtility.GetExtension(imageUrl);
				ImageResizer ir = new ImageResizer();
				using (var s = fs.OpenFile(imageUrl))
				{
					ir.Resize(s, extension, width, height, context.Response.OutputStream);
				}
        	}
        	else
        	{
        		throw new HttpException(404, "Not found");
        	}
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
