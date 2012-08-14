using System;
using System.Web.Mvc;
using N2.Definitions;
using N2.Definitions.Runtime;
using N2.Web.Mvc.Html;
using N2.Details;
using N2.Engine;

namespace N2.Web.Mvc
{
	public class RegisterHelper : IContentRegistration
	{
		#region Static
		public static RegisteringDisplayRendererFactory RendererFactory { get; set; }

		static RegisterHelper()
		{
			RendererFactory = new RegisteringDisplayRendererFactory();
		}
		#endregion

		public HtmlHelper Html { get; set; }
		
		string ContainerName { get; set; }

		public RegisterHelper(HtmlHelper html)
		{
			this.Html = html;
		}


		public Builder<T> RegisterDisplayable<T>(string name) where T : IDisplayable, new()
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				re.Add(new T() { Name = name });
			}

			return new Builder<T>(name, re);
		}

		#region IContentRegistration Members

		public EditableBuilder<T> RegisterEditable<T>(string name, string title) where T : IEditable, new()
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				re.Add(new T(), name, title);
			}

			return RendererFactory.Create<T>(Rendering.RenderingContext.Create(Html, name), re);
		}

		public EditableBuilder<T> RegisterEditable<T>(T editable) where T : IEditable
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				re.Add(editable, editable.Name, editable.Title);
			}

			return RendererFactory.Create<T>(Rendering.RenderingContext.Create(Html, editable.Name), re);
		}


		public Builder<T> Register<T>(T editable) where T : IUniquelyNamed
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				re.Add(editable);
			}

			return new Builder<T>(editable.Name, re);
		}

		public void RegisterModifier(Details.IContentTransformer modifier)
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
				re.Definition.ContentTransformers.Add(modifier);
		}

		public Builder<T> RegisterRefiner<T>(T refiner) where T : ISortableRefiner
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
				return re.RegisterRefiner<T>(refiner);
			return new Builder<T>(null);
		}

		#endregion

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

		#region class RegisteringDisplayRendererFactory
		public class RegisteringDisplayRendererFactory
		{
			public virtual RegisteringDisplayRenderer<T> Create<T>(Rendering.RenderingContext context, ContentRegistration re) where T : IEditable
			{
				return new RegisteringDisplayRenderer<T>(context, re);
			}
		}
		#endregion
	}
}