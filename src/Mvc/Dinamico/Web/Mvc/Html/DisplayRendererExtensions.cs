using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using System.IO;

namespace N2.Web.Mvc.Html
{
	public static class DisplayRendererExtensions
	{
		public static void Render<T>(this EditableBuilder<T> builder) where T : IEditable
		{
			var renderer = builder as IRenderer;
			if (renderer == null)
				return;

			renderer.Render();
		}

		public static void WriteTo<T>(this EditableBuilder<T> builder, TextWriter writer) where T : IEditable
		{
			var renderer = builder as IWriter;
			if (renderer == null)
				return;

			renderer.WriteTo(writer);
		}
	}

}
