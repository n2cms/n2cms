using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Definitions.Static;

namespace N2.Definitions.Runtime
{
	public abstract class FluentRegisterer<T> : IFluentRegisterer where T : ContentItem
	{
		public abstract void RegisterDefinition(IContentRegistration<T> register);

		public Type ContentType
		{
			get { return typeof(T); }
		}

		public virtual IEnumerable<ItemDefinition> Register(DefinitionMap map)
		{
			var registration = new ContentRegistration<T>(map.GetOrCreateDefinition(ContentType));
			registration.IsDefined = true;
			RegisterDefinition(registration);
			return new [] { registration.Finalize() };
		}
	}

	[Service(typeof(IDefinitionProvider))]
	public class FluentDefinitionProvider : IDefinitionProvider
	{
		public ItemDefinition[] definitionsCache;

		public FluentDefinitionProvider(DefinitionMap map, IFluentRegisterer[] registrators)
		{
			var definitions = registrators.SelectMany(r => r.Register(map));
			definitionsCache = definitions.ToArray();
		}

		public IEnumerable<ItemDefinition> GetDefinitions()
		{
			return definitionsCache;
		}

		/// <summary>The order this definition provider should be invoked, default 0.</summary>
		public int SortOrder { get { return -10; } }
	}
}
