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
		readonly IEngine engine;
		readonly IWebContext webContext;
		readonly IUrlParser parser;
		readonly ITypeFinder finder;
		readonly IErrorHandler errorHandler;
		readonly bool rewriteEmptyExtension = true;
		readonly string[] observedExtensions = new[] { ".aspx" };

		IControllerDescriptor[] controllerDescriptors = new IControllerDescriptor[0];

		public RequestDispatcher(IEngine engine, IWebContext webContext, IUrlParser parser, ITypeFinder finder, IErrorHandler errorHandler, HostSection config)
		{
			this.engine = engine;
			this.webContext = webContext;
			this.parser = parser;
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
		public virtual T ResolveAspectController<T>() where T : class, IAspectController
		{
			T controller = RequestItem<T>.Instance;
			if (controller != null) return controller;

			PathData path = ResolveUrl(webContext.Url);

			if (path.IsEmpty()) return null;

			controller = CreateControllerInstance<T>(path);
			if (controller == null) return null;
			
			controller.Path = path;
			controller.Engine = engine;
			
			RequestItem<T>.Instance = controller;

			return controller;
		}

		/// <summary>Adds controller descriptors to the list of descriptors. This is typically auto-wired using the [Controls] attribute.</summary>
		/// <param name="descriptorToAdd">The controller descriptors to add.</param>
		public void RegisterAspectController(params IControllerDescriptor[] descriptorToAdd)
		{
			lock(this)
			{
				List<IControllerDescriptor> references = new List<IControllerDescriptor>(controllerDescriptors);
				references.AddRange(descriptorToAdd);
				references.Sort();
				controllerDescriptors = references.ToArray();
			}
		}

		public PathData ResolveUrl(string url)
		{
			try
			{
				if (IsObservable(url)) return parser.ResolvePath(url);
			}
			catch (Exception ex)
			{
				errorHandler.Notify(ex);
			}
			return PathData.Empty;
		}

		private bool IsObservable(Url url)
		{
			if(url.LocalUrl == Url.ApplicationPath)
				return true;

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

		protected virtual T CreateControllerInstance<T>(PathData path) where T: class, IAspectController
		{
			Type requestedType = typeof (T);

			foreach (IControllerDescriptor reference in controllerDescriptors)
			{
				if (requestedType.IsAssignableFrom(reference.ControllerType) && reference.IsControllerFor(path, requestedType))
				{
					return Activator.CreateInstance(reference.ControllerType) as T;
				}
			}

			throw new N2Exception("Couldn't find an aspect controller '{0}' for the item '{1}' on the path '{2}'.", typeof(T).FullName, path.CurrentItem, path.Path);
		}

		#region IStartable Members

		public void Start()
		{
			List<IControllerDescriptor> references = new List<IControllerDescriptor>();
			foreach (Type controllerType in finder.Find(typeof(IAspectController)))
			{
				foreach (IControllerDescriptor reference in controllerType.GetCustomAttributes(typeof(IControllerDescriptor), false))
				{
					reference.ControllerType = controllerType;
					references.Add(reference);
				}
			}
			RegisterAspectController(references.ToArray());
		}

		public void Stop()
		{
		}

		#endregion
	}
}
