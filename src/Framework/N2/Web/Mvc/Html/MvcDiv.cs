using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public class MvcDiv: IDisposable
	{
		// Fields
		private bool _disposed;
		private readonly TextWriter _writer;

		public MvcDiv(ViewContext viewContext)
		{
			if (viewContext == null)
			{
				throw new ArgumentNullException("viewContext");
			}
			this._writer = viewContext.Writer;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				this._disposed = true;
				this._writer.Write("</div>");
			}
		}

		public void EndDiv()
		{
			this.Dispose(true);
		}
	}

	public static class MvcDivHelperExtension
	{
		// Methods
		public static MvcDiv BeginDiv(this HtmlHelper html)
		{
			return html.BeginDiv(null);
		}

		public static MvcDiv BeginBootstrapXsDiv(this HtmlHelper html, int columns)
		{
			return html.BeginDiv(ResourcesExtensions.BootstrapColumnClass(columns, ResourcesExtensions.BootstrapScreenSize.xs));
		}

		public static MvcDiv BeginDiv(this HtmlHelper html, string cssClass)
		{
			return DivHelper(html, cssClass);
		}

		public static void EndDiv(this HtmlHelper htmlHelper)
		{
			htmlHelper.ViewContext.Writer.Write("</div>");
		}

		internal static MvcDiv DivHelper(HtmlHelper html, string cssClass = "")
		{
			var tagBuilder = new TagBuilder("div");
			if (!string.IsNullOrWhiteSpace(cssClass))
				tagBuilder.Attributes.Add("class", cssClass);
			html.ViewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
			return new MvcDiv(html.ViewContext);
		}
	}
}
