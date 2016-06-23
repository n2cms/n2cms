using System;
using System.Collections.Generic;
using System.Linq;
using N2.Engine;
using N2.Definitions.Static;

namespace N2.Definitions.Runtime
{
	public abstract class FluentRegisterer<T> : IFluentRegisterer where T : ContentItem
	{
		public abstract void RegisterDefinition(IContentRegistration<T> register);

		public Type RegisteredType
		{
			get { return typeof(T); }
		}

		public virtual IEnumerable<ItemDefinition> Register(DefinitionMap map)
		{
			var registration = new ContentRegistration<T>(map.GetOrCreateDefinition(RegisteredType));
			registration.IsDefined = true;
			RegisterDefinition(registration);
			return new [] { registration.Finalize() };
		}
	}

	[Service(typeof(IDefinitionProvider))]
	public class FluentDefinitionProvider : IDefinitionProvider
	{
		public ItemDefinition[] definitionsCache;

		public FluentDefinitionProvider(DefinitionMap map, IFluentRegisterer[] registerers)
		{
			var definitions = registerers
				.OrderBy(r => Utility.InheritanceDepth(r.RegisteredType))
				.SelectMany(r => r.Register(map));
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
