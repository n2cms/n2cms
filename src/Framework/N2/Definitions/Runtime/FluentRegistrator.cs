using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Definitions.Static;

namespace N2.Definitions.Runtime
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class FluentRegistrationAttribute : ServiceAttribute
	{
		public FluentRegistrationAttribute()
			: base(typeof(IFluentRegistrator))
		{
		}
	}

	public interface IFluentRegistrator
	{
		IEnumerable<ItemDefinition> Register(DefinitionMap map);
	}

	public abstract class FluentRegistrator<T> : IFluentRegistrator where T : ContentItem
	{
		public abstract void RegisterDefinition(ContentRegistration<T> re);

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

		public FluentDefinitionProvider(DefinitionMap map, IFluentRegistrator[] registrators)
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
