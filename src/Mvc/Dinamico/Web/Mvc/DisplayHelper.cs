using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Definitions;
using System.IO;
using System.Web.UI;
using System.Web;
using System;
using N2.Details;
using N2.Web.Rendering;

namespace N2.Web.Mvc
{
	public class DisplayHelper<TModel>
	{
		public HtmlHelper<TModel> Html { get; set; }
		ContentItem CurrentItem { get; set; }

		class Displayer : IHtmlString
		{
			RenderingContext context;

			public Displayer(RenderingContext context)
			{
				this.context = context;
			}

			public Displayer(HtmlHelper html, string propertyName)
			{
				context = new RenderingContext();
				context.Content = html.CurrentItem();
				var providers = html.ResolveServices<ITemplateProvider>();
				var template = providers.GetTemplate(context.Content);
				context.Displayable = template.Definition.Displayables.FirstOrDefault(d => d.Name == propertyName);
				context.Html = html;
				context.PropertyName = propertyName;
				context.Writer = html.ViewContext.Writer;
			}

			#region IHtmlString Members

			public string ToHtmlString()
			{
				return ToString();
			}

			#endregion

			public void Write(TextWriter writer)
			{
				if (context.Displayable == null)
					return;

				context.Html.ResolveService<DisplayableRendererSelector>()
					.Render(context);
			}

			public override string ToString()
			{
				using (var sw = new StringWriter())
				{
					Write(sw);
					return sw.ToString();
				}
			}
		}

		public DisplayHelper(HtmlHelper<TModel> html, ContentItem current)
		{
			this.Html = html;
			this.CurrentItem = current;
		}

		public IHtmlString Displayable<T>(string name, Action<T> config = null) where T : IContainable, new()
		{
			var re = RegisterExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				re.AddContainable<T>(new T() { Name = name }, config);
			}

			return new Displayer(Html, name);
		}

		public IHtmlString Editable<T>(string name, string title = null, string container = null, Action<T> config = null) where T : IEditable, new()
		{
			var re = RegisterExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				re.Container(container, r => r.AddEditable<T>(new T(), name, title, config));
			}

			return new Displayer(Html, name);
		}
	}

	//public class Executor<TModel> : DynamicObject where TModel : class
	//{
	//    HtmlHelper<TModel> html;
	//    ContentItem current;

	//    public Executor(HtmlHelper<TModel> html, ContentItem current)
	//    {
	//        this.html = html;
	//        this.current = current;
	//    }

	//    public override IEnumerable<string> GetDynamicMemberNames()
	//    {
	//        return current.GetContentType().GetProperties().Select(p => p.Name);
	//    }

	//    public override bool TryGetMember(GetMemberBinder binder, out object result)
	//    {
	//        if (current == null)
	//        {
	//            result = null;
	//            return true;
	//        }

	//        string name = binder.Name;

	//        try
	//        {
	//            object data = html.DisplayContent(current, name).ToString();
	//            result = data.ToHtmlString();
	//        }
	//        catch (N2Exception)
	//        {
	//            if (html.ViewData.ContainsKey("RegistrationExpression"))
	//            {
	//                result = null;
	//                return true;
	//            }

	//            var template = html.ResolveServices<ITemplateProvider>().GetTemplate(current);
	//            var displayable = template.Definition.Displayables.FirstOrDefault(d => d.Name == name);

	//            object data;
	//            if (displayable != null)
	//            {
	//                var vp = new ViewPage();
	//                displayable.AddTo(current, name, vp);

	//                using (var sw = new StringWriter())
	//                using (var htw = new HtmlTextWriter(sw))
	//                {
	//                    vp.RenderControl(htw);
	//                    data = sw.ToString();
	//                }
	//            }
	//            else
	//                data = current[name];

	//            result = data.ToHtmlString();
	//        }

	//        return true;
	//    }
	//}
}