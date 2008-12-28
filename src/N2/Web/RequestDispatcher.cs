using System;
using System.Collections.Generic;
using N2.Engine;
using Castle.Core;
using N2.Plugin;

namespace N2.Web
{
	/// <summary>
	/// Resolves the controller to handle a certain request. Supports a default 
	/// controller or additional imprativly using the ConnectControllers method 
	/// or declarativly using the [Controls] attribute registered.
	/// </summary>
	public class RequestDispatcher : IRequestDispatcher, IStartable, IAutoStart
	{
		readonly IUrlParser parser;
		readonly IWebContext webContext;
		readonly ITypeFinder finder;

		IControllerDescriptor[] controllerDescriptor = new IControllerDescriptor[0];

		public RequestDispatcher(IUrlParser parser, IWebContext webContext, ITypeFinder finder)
		{
			this.parser = parser;
			this.webContext = webContext;
			this.finder = finder;
		}

		/// <summary>Resolves the controller for the current Url.</summary>
		/// <returns>A suitable controller for the given Url.</returns>
		public virtual BaseController ResolveController()
		{
			PathData path = ResolvePath(webContext.Url);
			BaseController controller = CreateControllerInstance(path);
			controller.Path = path;
			return controller;
		}

		/// <summary>Adds controller descriptors to the list of descriptors. This is typically auto-wired using the [Controls] attribute.</summary>
		/// <param name="descriptorToAdd">The controller descriptors to add.</param>
		public void RegisterControllerDescriptor(params IControllerDescriptor[] descriptorToAdd)
		{
			lock(this)
			{
				List<IControllerDescriptor> references = new List<IControllerDescriptor>(controllerDescriptor);
				references.AddRange(descriptorToAdd);
				references.Sort();
				controllerDescriptor = references.ToArray();
			}
		}

		protected virtual PathData ResolvePath(string url)
		{
			return parser.ResolvePath(url);
		}

		protected virtual BaseController CreateControllerInstance(PathData path)
		{
			if (!path.IsEmpty())
			{
				foreach (IControllerDescriptor reference in controllerDescriptor)
				{
					if (reference.IsControllerFor(path))
					{
						return Activator.CreateInstance(reference.ControllerType) as BaseController;
					}
				}
			}

			return new BaseController();
		}

		#region IStartable Members

		public void Start()
		{
			List<IControllerDescriptor> references = new List<IControllerDescriptor>();
			foreach (Type controllerType in finder.Find(typeof(BaseController)))
			{
				foreach (IControllerDescriptor reference in controllerType.GetCustomAttributes(typeof(IControllerDescriptor), false))
				{
					reference.ControllerType = controllerType;
					references.Add(reference);
				}
			}
			RegisterControllerDescriptor(references.ToArray());
		}

		public void Stop()
		{
		}

		#endregion
	}
}
