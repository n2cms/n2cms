using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using System.Web.Mvc;

namespace Dinamico.Definitions.Dynamic
{
	[Service]
	public class RazorTemplateRegistrator
	{
		public RazorTemplateRegistrator()
		{
			QueuedRegistrations = new Queue<Type>();
		}

		public RazorTemplateRegistrator Add<T>() where T : Controller
		{
			QueuedRegistrations.Enqueue(typeof(T));
			if (RegistrationAdded != null)
				RegistrationAdded.Invoke(this, new EventArgs());

			return this;
		}

		public event EventHandler RegistrationAdded;

		public Queue<Type> QueuedRegistrations { get; set; }
	}
}