using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Engine;
using N2.Web.Mvc;

namespace N2.Definitions.Runtime
{
	[Service]
	public class ViewTemplateRegistrator
	{
		public ViewTemplateRegistrator()
		{
			QueuedRegistrations = new Queue<ViewTemplateSource>();
		}

		public string TemplateSelectorContainerName { get; set; }

		public event EventHandler RegistrationAdded;

		public Queue<ViewTemplateSource> QueuedRegistrations { get; set; }

		public ViewTemplateRegistrator Add<T>() where T : Controller
		{
			return Add<T>(".cshtml");
		}

		public ViewTemplateRegistrator Add<T>(string viewFileExtension) where T : Controller
		{
			var controllerType = typeof(T);
			string controllerName = controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length);
			Type modelType = typeof(ContentItem);
			Type contentControllerType = Utility.GetBaseTypes(controllerType).FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ContentController<>));
			if (contentControllerType != null)
				modelType = contentControllerType.GetGenericArguments().First();
			var source = new ViewTemplateSource { ControllerName = controllerName, ModelType = modelType, TemplateSelectorContainerName = TemplateSelectorContainerName, ViewFileExtension = viewFileExtension };

			QueuedRegistrations.Enqueue(source);

			if (RegistrationAdded != null)
				RegistrationAdded.Invoke(this, new EventArgs());

			return this;
		}

		public ViewTemplateRegistrator ShowSelectorIn(string templateSelectorContainerName)
		{
			TemplateSelectorContainerName = templateSelectorContainerName;
			foreach(var qr in QueuedRegistrations)
				qr.TemplateSelectorContainerName = templateSelectorContainerName;

			return this;
		}
	}
}