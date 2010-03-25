using System.Web.Mvc;
using System.Linq;
using N2.Web;
using N2.Web.Mvc;
using Reimers.Google;
using System.Net;
using Reimers.Google.Analytics;
using N2.Templates.Mvc.Areas.Management.Models;
using System;
using System.Collections.Generic;

namespace N2.Templates.Mvc.Areas.Management.Controllers
{
	[Controls(typeof(Models.ManageAnalyticsPart))]
	public class ManageAnalyticsController : ContentController<Models.ManageAnalyticsPart>
    {
        //
        // GET: /Management/Analytics/

        public override ActionResult Index()
        {
			if (CurrentItem.Token == null)
				return InputToken();
			if (CurrentItem.AccountID == 0)
				return SelectAccount();
			
			var dimensions = CurrentItem.Dimensions;
			var metrics = CurrentItem.Metrics;

			if (dimensions.Count() == 0 || metrics.Count() == 0)
				return ConfigureStatistics();

            return PartialView(CurrentItem);
		}


		public ActionResult Statistics()
		{
			var rr = new ReportRequestor(CurrentItem.Token);
			var accounts = rr.GetAccounts();

			var entries = rr.RequestReport(accounts.First(a => a.AccountID == CurrentItem.AccountID),
				CurrentItem.Dimensions,
				CurrentItem.Metrics,
				DateTime.Today.AddMonths(-1),
				DateTime.Now);

			return PartialView(new AnalyticsViewModel { Entries = entries });
		}


		[HttpPost]
		public ActionResult ClearUser()
		{
			CurrentItem.Token = null;

			return ClearAccount();
		}

		[HttpPost]
		public ActionResult ClearAccount()
		{
			CurrentItem.AccountID = 0;
			
			return ClearConfiguration();
		}

		[HttpPost]
		public ActionResult ClearConfiguration()
		{
			CurrentItem.Metrics = new Metric[0];
			CurrentItem.Dimensions = new Dimension[0];

			Engine.Persister.Save(CurrentItem);

			return Redirect(CurrentPage.Url);
		}

		
		public ActionResult InputToken()
		{
			if (Request["failure"] == "true")
				ModelState.AddModelError("password", Request["passwordError"]);

			return PartialView("InputToken", CurrentItem);
		}

		[HttpPost]
		public ActionResult InputToken(string username, string password)
		{
			string token = "";
			try
			{
				var tr = new TokenRequestor(username, password);
				token = tr.GetAuthToken();
			}
			catch (WebException ex)
			{
				ModelState.AddModelError("password", ex);

				return RedirectToIndex(ex.Message);
			}

			CurrentItem.Token = token;
			Engine.Persister.Save(CurrentItem);

			return Redirect(CurrentPage.Url);
		}


		public ActionResult SelectAccount()
		{
			var rr = new ReportRequestor(CurrentItem.Token);
			var accounts = rr.GetAccounts();

			return PartialView("SelectAccount", accounts);
		}
		[HttpPost]
		public ActionResult SaveSelectedAccount()
		{
			if (!TryUpdateModel<ManageAnalyticsPart>(CurrentItem, new[] { "AccountID" }))
			{
				return Redirect(CurrentPage.Url);
			}

			Engine.Persister.Save(CurrentItem);
			
			return Redirect(CurrentPage.Url);
		}


		public ActionResult ConfigureStatistics()
		{
			if (Request["failure"] == "true")
				ModelState.AddModelError("password", Request["errorText"]);

			var vd = new ConfigureAnalyticsViewModel
			{
				AllDimensions = Enum.GetValues(typeof(Dimension)).OfType<Dimension>().ToList(),
				AllMetrics = Enum.GetValues(typeof(Metric)).OfType<Metric>().ToList(),
				SelectedDimensions = CurrentItem.Dimensions.ToList(),
				SelectedMetrics = CurrentItem.Metrics.ToList()
			};

			if (Request["failure"] == "true")
				ModelState.AddModelError("password", Request["errorText"]);

			return PartialView("ConfigureStatistics", vd);
		}

		[HttpPost]
		public ActionResult SaveStatisticsConfiguration()
		{
			var dimensions = Read<Dimension>("Dimension").ToList();
			var metrics = Read<Metric>("Metric").ToList();

			if (dimensions.Count == 0 || metrics.Count == 0)
				return RedirectToIndex("Must select at least one from dimensions and one from metrics");

			CurrentItem.Dimensions = dimensions;
			CurrentItem.Metrics = metrics;
			Engine.Persister.Save(CurrentItem);

			return Redirect(CurrentPage.Url);
		}

		private IEnumerable<T> Read<T>(string prefix) where T: struct
		{
			string values = Request.Form[prefix];
			
			if(string.IsNullOrEmpty(values))
				yield break;

			foreach (var value in values.Split(','))
			{
				T e = (T)Enum.Parse(typeof(T), value);
				yield return e;
			}
		}

		private ActionResult RedirectToIndex(string errorText)
		{
			N2.Web.Url url = CurrentPage.Url;
			return Redirect(url.AppendQuery("failure", "true").AppendQuery("errorText", errorText));
		}
    }
}
