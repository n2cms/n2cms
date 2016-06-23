using N2.Engine;
using N2.Plugin;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace N2.Management.Statistics
{
	[Service]
	public class StatisticsLogger : IAutoStart
	{
		private EventBroker broker;
		private IWebContext context;
		private Collector filler;
		private StatisticsRepository repository;
		static Logger<StatisticsLogger> log;

		public StatisticsLogger(EventBroker broker, IWebContext context, Collector collector, StatisticsRepository repository)
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

		static Regex crawlerExpression = new Regex(@"bot|crawler|baiduspider|80legs|ia_archiver|voyager|curl|wget|yahoo! slurp|mediapartners-google", RegexOptions.IgnoreCase);

		private void OnEndRequest(object sender, EventArgs e)
		{
			if (context.HttpContext.GetViewPreference(Edit.ViewPreference.None) == Edit.ViewPreference.None)
				if (context.HttpContext.Request.UserAgent != null && !crawlerExpression.IsMatch(context.HttpContext.Request.UserAgent))
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