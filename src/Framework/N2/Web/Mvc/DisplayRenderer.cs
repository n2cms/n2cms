using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Definitions;
using N2.Web.Mvc.Html;
using N2.Web.Rendering;
using N2.Definitions.Runtime;
using N2.Details;
using N2.Web.UI.WebControls;
using System.Diagnostics;

namespace N2.Web.Mvc
{
	public class DisplayRenderer<T> : 
#if NET4
		IHtmlString, 
#endif
		IDisplayRenderer where T : IDisplayable
	{
		private readonly Engine.Logger<DisplayRenderer<T>> logger;
		private bool isEditable = RenderHelper.DefaultEditable;
		private bool isOptional = RenderHelper.DefaultOptional;
		private bool swallowExceptions = RenderHelper.DefaultSwallowExceptions;

		public RenderingContext Context { get; set; }



		public DisplayRenderer(RenderingContext context)
		{
			this.Context = context;
		}

		public DisplayRenderer(HtmlHelper html, string propertyName)
		{
			Context = new RenderingContext();
			Context.Content = html.CurrentItem();
			var template = html.ResolveService<IDefinitionManager>().GetTemplate(Context.Content);
			if (template != null)
				Context.Displayable = template.Definition.Displayables.FirstOrDefault(d => d.Name == propertyName);
			if(!isOptional && Context.Displayable == null)
				throw new N2Exception("No displayable registered for the name '{0}' of the item #{1} of type {2}", propertyName, Context.Content.ID, Context.Content.GetContentType());
			Context.Html = html;
			Context.PropertyName = propertyName;
			Context.IsEditable = RenderHelper.DefaultEditable;
		}



		/// <summary>Control whether this property can be edited via the the UI when navigating using the drag-drop mode.</summary>
		/// <param name="isEditable"></param>
		/// <returns>The same object.</returns>
		public DisplayRenderer<T> Editable(bool isEditable = true)
		{
			this.isEditable = isEditable;

			return this;
		}

		/// <summary>Control whether this property can be edited via the the UI when navigating using the drag-drop mode.</summary>
		/// <param name="isEditable"></param>
		/// <returns>The same object.</returns>
		public DisplayRenderer<T> SwallowExceptions(bool hideExceptions = true)
		{
			this.swallowExceptions = hideExceptions;

			return this;
		}

		/// <summary>Control whether the displayable will throw exceptions when no displayable with a matching name is found.</summary>
		/// <param name="isOptional">Optional is true by default.</param>
		/// <returns>The same object.</returns>
		public DisplayRenderer<T> Optional(bool isOptional = true)
		{
			this.isOptional = isOptional;

			return this;
		}

		#region IHtmlString Members

		public string ToHtmlString()
		{
			return ToString();
		}

		#endregion

		public void Render()
		{
			WriteTo(Context.Html.ViewContext.Writer);
		}

		public override string ToString()
		{
			using (var sw = new StringWriter())
			{
				WriteTo(sw);
				return sw.ToString();
			}
		}

		public void WriteTo(TextWriter writer)
		{
			if (Context.Displayable == null || Context.Content == null)
				return;

			Context.IsEditable = isEditable && ControlPanelExtensions.GetControlPanelState(Context.Html) == ControlPanelState.DragDrop;

			var renderer = Context.Html.ResolveService<DisplayableRendererSelector>();
			if (swallowExceptions)
			{
				try 
				{
					renderer.Render(Context, writer);		
				}
				catch (System.Exception ex)
				{
					logger.Error(ex);
				}
			}
			else
				renderer.Render(Context, writer);
		}

	}
}