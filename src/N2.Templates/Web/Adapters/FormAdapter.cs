using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.HtmlControls;

namespace N2.Templates.Web.Adapters
{
	/// <summary>
	/// Maintains friendly url between postbacks.
	/// </summary>
	public class FormAdapter : ControlAdapter
	{
		protected override void Render(HtmlTextWriter writer)
		{
			HtmlForm form = Control as HtmlForm;

			writer.WriteBeginTag("form");
			WriteAttributes(writer, form);
			writer.Write(HtmlTextWriter.TagRightChar);

			RenderChildren(writer);

			writer.WriteEndTag("form");
		}

		private void WriteAttributes(HtmlTextWriter writer, HtmlForm form)
		{
			if (form.ID != null)
				writer.WriteAttribute("id", form.ClientID);
			writer.WriteAttribute("name", form.Name);
			writer.WriteAttribute("method", form.Method);

			foreach (string key in form.Attributes.Keys)
				writer.WriteAttribute(key, form.Attributes[key]);

			writer.WriteAttribute("action", Page.Request.RawUrl);
		}
	}
}
