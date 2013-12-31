using N2.Edit.FileSystem;
using N2.Engine;
using N2.Plugin;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace N2.Management.Resources
{
    /// <summary>
    /// Extracts individual icons out of CSS sprites.
    /// </summary>
    [Service]
    public class IconHandler : IHttpHandler, IAutoStart
    {
        private EventBroker broker;
        private IProvider<VirtualPathProvider> vppProvider;

        public IconHandler(IProvider<VirtualPathProvider> vppProvider, EventBroker broker)
        {
            this.vppProvider = vppProvider;
            this.broker = broker;
        }

        private void HttpApplication_PostResolveRequestCache(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app == null) return;

            string collection = null, key = null;
            try
            {
                var p = app.Request.Path;
                string iconBase = "resources/icons/";
                if (p.IndexOf(iconBase, StringComparison.OrdinalIgnoreCase) >= 0 && p.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                {
                    collection = "silk";
                    p = EatTo(p, iconBase);
                    key = p.Substring(0, p.IndexOf(".png", StringComparison.OrdinalIgnoreCase));
                    app.Context.RemapHandler(this);
                    return;
                }

                iconBase = "resources/img/flags/";
                if (p.IndexOf(iconBase, StringComparison.OrdinalIgnoreCase) >= 0 && p.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                {
                    collection = "flags";
                    p = EatTo(p, iconBase);
                    key = p.Substring(0, p.IndexOf(".png", StringComparison.OrdinalIgnoreCase));
                    app.Context.RemapHandler(this);
                    return;
                }
            }
            finally
            {
                if (key != null)
                    app.Context.Items["key"] = key;
                if (collection != null)
                    app.Context.Items["set"] = collection;
            }
        }

        #region IAutoStart Members

        public void Start()
        {
            broker.PostResolveRequestCache += HttpApplication_PostResolveRequestCache;
        }

        public void Stop()
        {
            broker.PreRequestHandlerExecute -= HttpApplication_PostResolveRequestCache;
        }

        #endregion

        #region string parsing 

        private static int GetNumber(string r)
        {
            var sb = new StringBuilder(r.Length);
            foreach (var ch in r)
                if (ch == '-' || Char.IsDigit(ch))
                {
                    sb.Append(ch);
                }
                else if (Char.IsWhiteSpace(ch))
                {
                    //ignore spaces
                }
                else
                {
                    break;
                }
            return int.Parse(sb.ToString());
        }

        private static string EatTo(string r, string eat)
        {
            return r.Substring(r.IndexOf(eat, StringComparison.OrdinalIgnoreCase) + eat.Length);
        }


        #endregion


        class SpriteInfo
        {
            public Rectangle R;
            public string Key;
        }


        //TODO: Add unit test
        private SpriteInfo ParseCssLine(string input)
        {
            SpriteInfo result = new SpriteInfo();
            input = EatTo(input, ".");
            result.Key = input.Substring(0, input.IndexOf(" {", StringComparison.Ordinal));
            input = EatTo(input, "width:");
            result.R.Width = GetNumber(input);
            input = EatTo(input, "height:");
            result.R.Height = GetNumber(input);
            input = EatTo(input, "position: -");
            result.R.X = GetNumber(input);
            input = EatTo(input, " -");
            result.R.Y = GetNumber(input);
            return result;
        }

        //TODO: Add unit test for icon handler
        //TODO: Make icons work with the ZIP VPP
        public void ProcessRequest(HttpContext context)
        {
            var set = context.Items["set"];
            var key = context.Items["key"];
            var ico = Url.ResolveTokens("{ManagementUrl}/Resources/icons/");

            var cssFile = ico + set + ".css";
            var sprFile = ico + set + ".png";

            var vpp = vppProvider.Get();
            using (var fileStream = vpp.GetFile(cssFile).Open())
            using (var sr = new StreamReader(fileStream))
            {
                var cssLines = new List<string>();
                while (!sr.EndOfStream)
                {
                    var f = sr.ReadLine();
                    if (f.Contains("." + key + " {") || f.Contains("." + key + "{"))
                        cssLines.Add(f);
                }
                if (cssLines.Count == 0)
                    throw new KeyNotFoundException(String.Format("Sprite '{0}' not found in CSS at '{1}'.", key, cssFile));
                if (cssLines.Count > 1)
                    throw new AmbiguousMatchException(String.Format("Sprite '{0}' ambiguous in CSS at '{1}'.", key, cssFile));

                var css = ParseCssLine(cssLines[0]); // and throw the rest away

                using (var bmp = new Bitmap(css.R.Width, css.R.Height))
                {
                    using (var g = Graphics.FromImage(bmp))
                    using (var sprStream = vpp.GetFile(sprFile).Open())
                    using (var spr = Image.FromStream(sprStream))
                    {
                        // draw the sprite into bmp
                        Rectangle destRect = new Rectangle(0, 0, css.R.Width, css.R.Height);
                        g.DrawImage(spr, destRect, css.R, GraphicsUnit.Pixel);
                    }

                    // save bmp
                    using (MemoryStream m = new MemoryStream())
                    {
                        bmp.Save(m, ImageFormat.Png);
                        context.Response.ContentType = "image/png";
                        context.Response.BinaryWrite(m.ToArray());
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
