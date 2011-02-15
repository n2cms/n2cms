using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence.NH;
using N2.Configuration;
using N2.Web;
using Lucene.Net.Analysis;

namespace N2.Persistence.NH
{
	[Service(typeof(ConfigurationBuilderParticipator))]
	public class SearchConfigurationBuilderParticipator : ConfigurationBuilderParticipator
	{
		IWebContext webContext;
		bool searchEnabled = true;
		bool asyncIndexing = true;
		string indexPath = "~/App_Data/SearchIndex/";

		public SearchConfigurationBuilderParticipator(IWebContext webContext, DatabaseSection config)
		{
			this.webContext = webContext;
			searchEnabled = config.Search.Enabled;
			asyncIndexing = config.Search.AsyncIndexing;
			indexPath = config.Search.IndexPath;
		}

		public override void AlterConfiguration(NHibernate.Cfg.Configuration cfg)
		{
			if (!searchEnabled)
				return;

			cfg.Properties["hibernate.search.default.directory_provider"] = typeof(NHibernate.Search.Store.FSDirectoryProvider).AssemblyQualifiedName;
			cfg.Properties["hibernate.search.default.indexBase"] = webContext.MapPath(indexPath);
			cfg.Properties["hibernate.search.default.indexBase.create"] = "true";
			cfg.Properties[NHibernate.Search.Environment.AnalyzerClass] = typeof(SimpleAnalyzer).AssemblyQualifiedName;
			cfg.Properties[NHibernate.Search.Environment.WorkerExecution] = asyncIndexing ? "async" : "sync";

			var indexListener = new NHibernate.Search.Event.FullTextIndexEventListener();
			cfg.SetListener(NHibernate.Event.ListenerType.PostInsert, indexListener);
			cfg.SetListener(NHibernate.Event.ListenerType.PostUpdate, indexListener);
			cfg.SetListener(NHibernate.Event.ListenerType.PostDelete, indexListener);
		}
	}
}
