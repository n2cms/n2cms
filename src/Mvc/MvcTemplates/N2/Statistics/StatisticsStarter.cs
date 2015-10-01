using N2.Engine;
using N2.Plugin;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	[Service]
	public class StatisticsStarter : IAutoStart
	{
		private EventBroker broker;
		private IWebContext context;
		private Collector filler;
		private BucketRepository repository;
		static Logger<StatisticsStarter> log;

		public StatisticsStarter(EventBroker broker, IWebContext context, Collector collector, BucketRepository repository)
		{
			this.broker = broker;
			this.context = context;
			this.filler = collector;
			this.repository = repository;
		}

		void OnDomainUnload(object sender, EventArgs e)
		{
			try
			{
				var buckets = filler.CheckoutBuckets();
				repository.Save(buckets);
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			filler.RegisterView(context.CurrentPath);
		}

		public void Start()
		{
			broker.EndRequest += OnEndRequest;
			try
			{
				AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}
		}

		public void Stop()
		{
			broker.EndRequest -= OnEndRequest;
			try
			{
				AppDomain.CurrentDomain.DomainUnload -= OnDomainUnload;
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}
		}
	}
}