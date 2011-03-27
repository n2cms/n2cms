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

		#region IContentRegistration Members

		public EditableBuilder<T> RegisterEditable<T>(string name, string title) where T : IEditable, new()
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				re.Add(new T(), name, title);
			}

			return new RegisteringDisplayRenderer<T>(Html, name, re);
		}

		public EditableBuilder<T> RegisterEditable<T>(T editable) where T : IEditable
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				re.Add(editable, editable.Name, editable.Title);
			}

			return new RegisteringDisplayRenderer<T>(Html, editable.Name, re);
		}

		public void RegisterModifier(Details.IContentModifier modifier)
		{
			var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
				re.ContentModifiers.Add(modifier);
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
	}
}