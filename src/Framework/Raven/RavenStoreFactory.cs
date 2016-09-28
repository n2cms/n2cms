using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using Raven.Client.Embedded;
using Raven.Client.Listeners;
using System.Diagnostics;
using Raven.Client;
using Newtonsoft.Json;
using Raven.Client.Document;
using N2.Configuration;
using Raven.Json.Linq;
using N2.Definitions.Static;
using N2.Definitions;

namespace N2.Raven
{
	[Service]
	public class RavenStoreFactory
	{
		private string connectionStringName;
		public bool RunInMemory { get; set; }
		public bool EmbeddedDocumentStore { get; set; }

		public RavenStoreFactory(DatabaseSection config)
		{
			connectionStringName = config.ConnectionStringName;
			RunInMemory = config.Raven.RunInMemory;
			EmbeddedDocumentStore = config.Raven.EmbeddedDocumentStore;
		}

		public virtual IDocumentStore CreateStore(RavenConnectionProvider connections)
		{
			var store = RunInMemory
				? new EmbeddableDocumentStore { RunInMemory = RunInMemory }
				: EmbeddedDocumentStore 
					? new EmbeddableDocumentStore { ConnectionStringName = connectionStringName }
					: new DocumentStore { ConnectionStringName = connectionStringName };

			Initialize(connections, store);

			return store;
		}

		protected virtual void Initialize(RavenConnectionProvider connections, DocumentStore store)
		{
			store.Conventions.MaxNumberOfRequestsPerSession = 100;
			store.Conventions.FindIdentityProperty = pi => pi.Name == "ID";
			store.Conventions.JsonContractResolver = new ContentContractResolver();
			store.Conventions.FindTypeTagName = t => typeof(ContentItem).IsAssignableFrom(t) ? "ContentItem" : null;
			store.Conventions.SaveEnumsAsIntegers = true;
			store.Conventions.CustomizeJsonSerializer = (serializer) =>
			{
				serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
				serializer.ReferenceResolver = new ContentReferenceResolver(connections);
				serializer.Converters.Add(new DetailsJsonConverter());
				serializer.Converters.Add(new DetailCollectionsJsonConverter());
				serializer.TypeNameHandling = TypeNameHandling.All;
			};
			store.RegisterListener((IDocumentConversionListener)new Listener());
			store.RegisterListener((IDocumentDeleteListener)new Listener());
			store.RegisterListener((IDocumentQueryListener)new Listener());
			store.RegisterListener((IDocumentStoreListener)new Listener());
			store.Initialize();
		}

		class Listener : IDocumentConversionListener, IDocumentDeleteListener, IDocumentQueryListener, IDocumentStoreListener
		{
			public void DocumentToEntity(object entity, RavenJObject document, RavenJObject metadata)
			{
				Debug.WriteLine(entity);
				//document.Add("class", new RavenJValue(DefinitionMap.Instance.GetOrCreateDefinition((ITemplateable)entity).Discriminator));
				//metadata.Add("class", new RavenJValue(DefinitionMap.Instance.GetOrCreateDefinition((ITemplateable)entity).Discriminator));
			}

			public void EntityToDocument(object entity, RavenJObject document, RavenJObject metadata)
			{
				Debug.WriteLine(entity);
			}

			public void BeforeDelete(string key, object entityInstance, RavenJObject metadata)
			{
				Debug.WriteLine(entityInstance);
			}

			public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
			{
				Debug.WriteLine(queryCustomization);
			}

			public void AfterStore(string key, object entityInstance, RavenJObject metadata)
			{
				Debug.WriteLine(entityInstance);
			}

			public bool BeforeStore(string key, object entityInstance, RavenJObject metadata)
			{
				Debug.WriteLine(entityInstance);
				return false;
			}
		}
	}
}
