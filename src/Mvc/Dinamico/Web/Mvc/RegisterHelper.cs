using System;
using System.Web.Mvc;
using N2.Definitions;
using N2.Definitions.Runtime;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	public class RegisterHelper : IContentRegistration
	{
		public HtmlHelper Html { get; set; }
		
		string ContainerName { get; set; }

		public RegisterHelper(HtmlHelper html)
		{
			this.Html = html;
		}

		public EditableBuilder<T> RegisterEditable<T>(string name, string title) where T : IEditable, new()
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				re.Add(new T(), name, title);
			}

			return new DisplayRenderer<T>(Html, name, re);
		}

		public IDisposable BeginContainer(string containerName)
		{
			this.ContainerName = containerName;
			return new ResetOnDispose { PreviousContainerName = ContainerName, Helper = this };
		}

		public void EndContainer()
		{
			this.ContainerName = null;
		}

		#region class ResetOnDispose<T>
		class ResetOnDispose : IDisposable
		{
			public string PreviousContainerName { get; set; }
			public RegisterHelper Helper { get; set; }

			#region IDisposable Members

			public void Dispose()
			{
				Helper.ContainerName = PreviousContainerName;
			}

			#endregion
		}
		#endregion

		public DisplayRenderer<IEditable> this[string detailname]
		{
			get { return new DisplayRenderer<IEditable>(Html, detailname, RegistrationExtensions.GetRegistrationExpression(Html)); }
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