using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using N2.Web;

namespace N2.Management.Resources
{
	/// <summary>
	/// Extracts individual icons out of CSS sprites.
	/// </summary>
	public class IconHandler : IHttpHandler
	{

		#region string parsing 

		private int GetNumber(string r)
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

		private string EatTo(string r, string eat)
		{
			return r.Substring(r.IndexOf(eat, StringComparison.Ordinal) + eat.Length);
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
		public void ProcessRequest(HttpContext context)
		{
			var set = context.Request["set"];
			var key = context.Request["key"];
			var ico = Url.ResolveTokens("{ManagementUrl}/Resources/icons/");

			var cssFile = ico + set + ".css";
			var sprFile = ico + set + ".png";

			cssFile = context.Server.MapPath(cssFile);
			sprFile = context.Server.MapPath(sprFile);

			var cssLines = File.ReadAllLines(cssFile).Where(f =>
				f.Contains("." + key + " {") ||
				f.Contains("." + key + "{")).ToArray();
			if (cssLines.Length == 0)
				throw new KeyNotFoundException(String.Format("Sprite '{0}' not found in CSS at '{1}'.", key, cssFile));
			if (cssLines.Length > 1)
				throw new AmbiguousMatchException(String.Format("Sprite '{0}' ambiguous in CSS at '{1}'.", key, cssFile));

			var css = ParseCssLine(cssLines[0]); // and throw the rest away

			using (var bmp = new Bitmap(css.R.Width, css.R.Height))
			{
				using (var g = Graphics.FromImage(bmp))
				using (var spr = Image.FromFile(sprFile))
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

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}
	}
}