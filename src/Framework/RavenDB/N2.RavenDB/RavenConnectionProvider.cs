using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Document;
using N2.Engine;
using N2.Web;
using Raven.Client.Embedded;

namespace N2.RavenDB
{
	[Service]
	public class RavenConnectionProvider
	{
		private EmbeddableDocumentStore store;
		private IWebContext ctx;

		public RavenConnectionProvider(IWebContext ctx)
		{
			this.ctx = ctx;
			store = new EmbeddableDocumentStore() { ConnectionStringName = "N2CMS" };
			store.RunInMemory = true;
			//store.Conventions.JsonContractResolver = new ContentContractResolver();
			store.Conventions.CustomizeJsonSerializer = js => js.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			store.Initialize();
		}

		public virtual IDocumentSession OpenSession()
		{
			return store.OpenSession();
		}

		public IDocumentSession Session
		{
			get
			{
				var session = ctx.RequestItems["RavenSession"] as IDocumentSession;
				if (session == null)
				{
					ctx.RequestItems["RavenSession"] = session = OpenSession();
				}
				return session;
			}
			set
			{
				ctx.RequestItems["RavenSession"] = value;
			}
		}
	}
}
