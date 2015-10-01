using N2.Edit.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	[ContentHandler]
	public class StatisticsContentHandler : ContentHandlerBase
	{
		private StatisticsRepository repository;

		public StatisticsContentHandler(StatisticsRepository repository)
		{
			this.repository = repository;
		}

		protected override object HandleDataRequest(HttpContextBase context)
		{
			DateTime from, to;
			if (!DateTime.TryParse(context.Request["from"], out from))
				from = Utility.CurrentTime().AddMonths(-1).Date;
			if (!DateTime.TryParse(context.Request["from"], out to))
				to = Utility.CurrentTime().Date.AddDays(1);

			int id;
			if (!int.TryParse(context.Request.QueryString["id"], out id))
				id = 0;

			var statistics = repository.GetStatistics(from, to, id);
			return new { Statistics = statistics };
		}
	}
}