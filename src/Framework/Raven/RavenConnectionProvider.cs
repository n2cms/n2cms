using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Document;
using N2.Engine;
using N2.Web;
using Raven.Client.Embedded;
using System.Diagnostics;
using Newtonsoft.Json;
using Raven.Client.Listeners;

namespace N2.Raven
{
	[Service]
	public class RavenConnectionProvider
	{
		private IDocumentStore store;
		private IWebContext ctx;

		public RavenConnectionProvider(RavenStoreFactory storeFactory, IWebContext ctx)
		{
			this.ctx = ctx;
			store = storeFactory.CreateStore(this);
		}

		public virtual IDocumentSession OpenSession()
		{
			var session = store.OpenSession();
			//session.Advanced.MaxNumberOfRequestsPerSession = 100;
			return session;
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
