using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using N2.Plugin;
using N2.Web;

namespace N2.Engine
{
	/// <summary>
	/// Keeps track of and provides content adapters in the system.
	/// </summary>
	[Service(typeof(IContentAdapterProvider))]
	public class ContentAdapterProvider : IContentAdapterProvider, IAutoStart
	{
		private readonly Engine.Logger<ContentAdapterProvider> logger;
		readonly IEngine engine;
		readonly ITypeFinder finder;
		AbstractContentAdapter[] adapters = new AbstractContentAdapter[0];

		public ContentAdapterProvider(IEngine engine, ITypeFinder finder)
		{
			this.engine = engine;
			this.finder = finder;
		}

		public IEnumerable<AbstractContentAdapter> Adapters
		{
			get { return adapters; }
		}

		#region IContentAdapterProvider Members

		/// <summary>Resolves the adapter for the current type.</summary>
		/// <returns>A suitable adapter for the given type.</returns>
		public T ResolveAdapter<T>(Type contentType) where T : AbstractContentAdapter
		{
			foreach (var adapter in adapters)
			{
				var a = adapter as T;
				if (a != null && a.AdaptedType.IsAssignableFrom(contentType))
					return a;
			}

			throw new NotSupportedException("No " + typeof(T) + " adapter supports " + contentType);
		}

		/// <summary>Resolves the adapter for the current item.</summary>
		/// <returns>A suitable adapter for the given item.</returns>
		public T ResolveAdapter<T>(ContentItem item) where T : AbstractContentAdapter
		{
			Type contentType = item != null ? item.GetContentType() : typeof(ContentItem);
			return ResolveAdapter<T>(contentType);
		}

		/// <summary>Adds an adapter to the list of adapters. This is typically auto-wired using the [Adapts] attribute.</summary>
		/// <param name="adapterToAdd">The adapter instnace to add.</param>
		public void RegisterAdapter(params AbstractContentAdapter[] adapterToAdd)
		{
			lock (this)
			{
				logger.DebugFormat("Registering {0} adapters", adapterToAdd);

				List<AbstractContentAdapter> references = new List<AbstractContentAdapter>(adapters);
				references.AddRange(adapterToAdd);
				references.Sort();
				adapters = references.ToArray();
			}
		}

		/// <summary>Removes an adapter from the list of adapters.</summary>
		/// <param name="descriptorsToAdd">The adapter to add.</param>
		public void UnregisterAdapter(params AbstractContentAdapter[] adaptersToRemove)
		{
			lock (this)
			{
				logger.DebugFormat("Unregistering {0} adapters", adaptersToRemove);

				List<AbstractContentAdapter> references = new List<AbstractContentAdapter>(adapters);
				foreach (var adapterToRemove in adaptersToRemove)
					references.Remove(adapterToRemove);
				adapters = references.ToArray();
			}
		}

		#endregion

		protected virtual T CreateAdapterInstance<T>(PathData path) where T : AbstractContentAdapter
		{
			Type requestedType = typeof(T);

			foreach (AbstractContentAdapter adapter in adapters)
			{
				var a = adapter as T;
				if (a != null && requestedType.IsAssignableFrom(a.AdaptedType))
				{
					return a;
				}
			}

			throw new N2Exception("Couldn't find a content adapter '{0}' for the item '{1}' on the path '{2}'.", typeof(T).FullName, path.CurrentItem, path.Path);
		}

		#region IStartable Members

		public void Start()
		{
			List<AbstractContentAdapter> references = new List<AbstractContentAdapter>();
			foreach (var at in finder.Find<AdaptsAttribute>(typeof(AbstractContentAdapter), inherit: false))
			{
				if (at.Type.IsAbstract)
					continue;

				logger.DebugFormat("Found adapter {0} adapting {1}", at.Type.Name, at.Attribute.ContentType);

				var adapter = CreateAdapter(at.Type, at.Attribute.ContentType);
				references.Add(adapter);
			}

			RegisterAdapter(references.ToArray());
		}

		private AbstractContentAdapter CreateAdapter(Type adapterType, Type contentType)
		{
			AbstractContentAdapter adapter;
			if (!ContainsServiceOfType(engine.Container.ResolveAll(adapterType), adapterType))
			{
				// add the adapter to the IoC container to resolve it's dependencies
				engine.Container.AddComponent(adapterType.FullName, adapterType, adapterType);
				adapter = engine.Container.Resolve(adapterType) as AbstractContentAdapter;
			}
			else
				adapter = engine.Container.Resolve(adapterType) as AbstractContentAdapter;
			
			if(adapter == null)
				throw new ArgumentException("Cannot create adapter of type " + adapterType + " for content type " + contentType);

			adapter.AdaptedType = contentType;
			adapter.engine = engine;
			return adapter;
		}
		 
		private bool ContainsServiceOfType(IEnumerable<object> services, Type adapterType)
		{
			foreach (var service in services)
			{
				if (service.GetType() == adapterType)
					return true;
			}
			return false;
		}

		public void Stop()
		{
		}

		#endregion
	}
}
