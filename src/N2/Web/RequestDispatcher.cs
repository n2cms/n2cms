using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using N2.Engine;
using Castle.Core;
using N2.Plugin;
using N2.Configuration;
using System.Web.Hosting;

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
		readonly IErrorHandler errorHandler;
		readonly bool rewriteEmptyExtension;
		readonly string[] observedExtensions = new[] { ".aspx" };
                
		IControllerDescriptor[] controllerDescriptor = new IControllerDescriptor[0];

		public RequestDispatcher(IUrlParser parser, IWebContext webContext, ITypeFinder finder, IErrorHandler errorHandler, HostSection config)
		{
			this.parser = parser;
			this.webContext = webContext;
			this.finder = finder;
			this.errorHandler = errorHandler;
			rewriteEmptyExtension = config.Web.ObserveEmptyExtension;
			StringCollection additionalExtensions = config.Web.ObservedExtensions;
            if (additionalExtensions != null && additionalExtensions.Count > 0)
            {
                observedExtensions = new string[additionalExtensions.Count + 1];
                additionalExtensions.CopyTo(observedExtensions, 1);
            }
			observedExtensions[0] = config.Web.Extension;
		}

		/// <summary>Resolves the controller for the current Url.</summary>
		/// <returns>A suitable controller for the given Url.</returns>
		public virtual BaseController ResolveController()
		{
			Url url = webContext.Url;
			PathData path;
			try
			{
				if (IsObservable(url)) path = ResolvePath(url);
				else path = PathData.Empty;
			}
			catch (Exception ex)
			{
				errorHandler.Notify(ex);
				path = PathData.Empty;
			}
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

		private bool IsObservable(Url url)
		{
			string extension = url.Extension;
			if (rewriteEmptyExtension && string.IsNullOrEmpty(extension))
				return true;
			foreach (string observed in observedExtensions)
				if (string.Equals(observed, extension, StringComparison.InvariantCultureIgnoreCase))
					return true;
			if (url.GetQuery("page") != null)
				return true;

			return false;
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
