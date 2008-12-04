using System.Web;
using System.Text.RegularExpressions;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	/// <summary>
	/// Class used to supply plugins with contextual information.
	/// </summary>
	public class PluginContext
	{
		public PluginContext()
		{
		}

		public PluginContext(ContentItem item, ContentItem memorizedItem, ControlPanelState state)
			: this(item, state)
		{
			Memorized = memorizedItem;
		}

		public PluginContext(ContentItem selected, ControlPanelState state)
		{
			State = state;
			Selected = selected;
		}

		public ControlPanelState State { get; set;}
		public ContentItem Selected { get; set; }
		public ContentItem Memorized { get; set; }
		
		static Regex expressionExpression = new Regex("{(?<expr>[^})]+)}");

		public string Format(string format, bool urlEncode)
		{
			format = format.Replace("{selected}", "{Selected.Path}")
				.Replace("{memory}", "{Memorized.Path}")
				.Replace("{action}", "{Action}");

			return expressionExpression.Replace(format, delegate(Match m) { return Evaluate(m.Groups["expr"].Value, urlEncode); });
		}

		string Evaluate(string expression, bool urlEncode)
		{
			object value = Utility.Evaluate(this, expression);
			
			if (value == null)
				return null;

			if(urlEncode)
				return HttpUtility.UrlEncode(value.ToString());

			return value.ToString();
		}
	}
}
