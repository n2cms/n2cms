using System.Web.Mvc;
using System.Linq;
using N2.Web;
using N2.Web.Mvc;
//using Reimers.Google;
using System.Net;
//using Reimers.Google.Analytics;
using N2.Management.Myself.Analytics.Models;
using System;
using System.Collections.Generic;

namespace N2.Management.Myself.Analytics.Controllers
{
    [Controls(typeof(Models.ManageAnalyticsPart))]
    public class ManageAnalyticsController : ContentController<Models.ManageAnalyticsPart>
    {
        public override ActionResult Index()
        {
            return Content("<div class='uc'>Analytics Obsolete</div>");
        }
        //
        // GET: /Management/Analytics/

        //public override ActionResult Index()
        //{
        //    if (CurrentItem.Token == null)
        //        return InputToken();
        //    if (CurrentItem.AccountID == 0 || Request["changeaccount"] == CurrentItem.ID.ToString())
        //        return SelectAccount();
            
        //    var dimensions = CurrentItem.Dimensions;
        //    var metrics = CurrentItem.Metrics;

        //    if (dimensions.Count() == 0 || metrics.Count() == 0 || Request["reconfigure"] == CurrentItem.ID.ToString())
        //        return ConfigureStatistics();

        //    return PartialView(CurrentItem);
        //}


        //public ActionResult Statistics()
        //{
        //    var entries = GetStatisticsEntries(DateTime.Today.AddDays(-CurrentItem.ChartPeriod), N2.Utility.CurrentTime());

        //    return PartialView(new AnalyticsViewModel { Entries = entries });
        //}

        //[HttpPost]
        //public ActionResult StatisticsData(DateTime? from, DateTime? to)
        //{
        //    var start = from ?? DateTime.Today.AddDays(-CurrentItem.ChartPeriod);
        //    var stop = to ?? N2.Utility.CurrentTime();
        //    var entries = GetStatisticsEntries(start, stop).ToList();

        //    var labels = new string[entries.Count];
        //    Dictionary<Metric, int[]> metrics = new Dictionary<Metric, int[]>();
        //    foreach(var metric in CurrentItem.Metrics)
        //        metrics[metric] = new int[entries.Count];
            
        //    for (int i = 0; i < entries.Count; i++)
        //    {
        //        labels[i] = string.Join(", ", entries[i].Dimensions.Select(d => DimensionToString(ref start, ref d)).ToArray());
        //        foreach (var metric in entries[i].Metrics)
        //        {
        //            try
        //            {
        //                metrics[metric.Key][i] = int.Parse(metric.Value);
        //            }
        //            catch
        //            {
        //                metrics[metric.Key][i] = 0;
        //            }
        //        }
        //    }

        //    var tickSize = labels.Length < 7 ? 1 : (int)(labels.Length / 7);

        //    var data = new
        //    {
        //        values = metrics.Select(m =>
        //            new
        //            {
        //                label = m.Key.ToString().SplitWords(),
        //                data = m.Value.Select((v, i) => new int[] { i, v })
        //            }),
        //        options = new
        //        {
        //            xaxis = new
        //            {
        //                ticks = labels.Select((l, i) => new { i, l })
        //                    .Where(v => v.i % tickSize == 0)
        //                    .Select((v) => new object[] { v.i, v.l })
        //            }
        //        }
        //    };

        //    return Json(data);
        //}

        //private string DimensionToString(ref DateTime start, ref KeyValuePair<Dimension, string> d)
        //{
        //    switch (d.Key)
        //    {
        //        case Dimension.day:
        //            if (CurrentItem.ChartPeriod <= 1)
        //                return start.AddDays(double.Parse(d.Value)).ToShortDateString();
        //            else
        //                return start.AddDays(double.Parse(d.Value)).DayOfWeek.ToString();
        //        case Dimension.hour:
        //            if (CurrentItem.ChartPeriod <= 1)
        //                return start.AddHours(double.Parse(d.Value)).ToShortTimeString();
        //            else
        //                return start.AddHours(double.Parse(d.Value)).ToString();
        //        case Dimension.month:
        //            if (CurrentItem.ChartPeriod < 365)
        //                return start.AddMonths(int.Parse(d.Value)).ToString("MMM");
        //            else
        //                return start.AddMonths(int.Parse(d.Value)).ToShortDateString();
        //        case Dimension.week:
        //            return start.AddDays(7 * double.Parse(d.Value)).ToShortDateString();
        //        case Dimension.year:
        //            return start.AddYears(int.Parse(d.Value)).Year.ToString();
        //        default:
        //            return d.Value.ToString().SplitWords();
        //    }
        //}

        //[HttpPost]
        //public ActionResult ClearUser()
        //{
        //    CurrentItem.Token = null;
        //    Engine.Persister.Save(CurrentItem);

        //    return Redirect(CurrentPage.Url);
        //}

        //[HttpPost]
        //public ActionResult ChangeAccount()
        //{
        //    N2.Web.Url url = CurrentPage.Url;
        //    return Redirect(url.AppendQuery("changeaccount", CurrentItem.ID));
        //}

        //[HttpPost]
        //public ActionResult Reconfigure()
        //{
        //    N2.Web.Url url = CurrentPage.Url;
        //    return Redirect(url.AppendQuery("reconfigure", CurrentItem.ID));
        //}

        
        //public ActionResult InputToken()
        //{
        //    if (Request["failure"] == CurrentItem.ID.ToString())
        //        ModelState.AddModelError("password", Request["errorText"]);

        //    return PartialView("InputToken", CurrentItem);
        //}

        //[HttpPost]
        //public ActionResult InputToken(string username, string password)
        //{
        //    string token = "";
        //    try
        //    {
        //        var tr = new TokenRequestor(username, password);
        //        token = tr.GetAuthToken();
        //    }
        //    catch (WebException ex)
        //    {
        //        ModelState.AddModelError("password", ex);

        //        return RedirectToIndex(ex.Message);
        //    }

        //    CurrentItem.Token = token;
        //    Engine.Persister.Save(CurrentItem);

        //    return Redirect(CurrentPage.Url);
        //}


        //public ActionResult SelectAccount()
        //{
        //    var rr = new ReportRequestor(CurrentItem.Token);
        //    var accounts = rr.GetAccounts();

        //    return PartialView("SelectAccount", accounts);
        //}
        //[HttpPost]
        //public ActionResult SaveSelectedAccount(int profileID)
        //{
        //    var rr = new ReportRequestor(CurrentItem.Token);
        //    var accounts = rr.GetAccounts();

        //    var account = accounts.Where(a => a.ProfileID == profileID).FirstOrDefault();
        //    if (account != null)
        //    {
        //        CurrentItem.ProfileID = profileID;
        //        CurrentItem.AccountID = account.AccountID;
        //        CurrentItem.AccountName = account.AccountName;
        //        CurrentItem.Title = account.Title;
        //        Engine.Persister.Save(CurrentItem);
        //    }
        //    return Redirect(CurrentPage.Url);
        //}


        //public ActionResult ConfigureStatistics()
        //{
        //    if (Request["failure"] == CurrentItem.ID.ToString())
        //        ModelState.AddModelError("password", Request["errorText"]);

        //    var vd = new ConfigureAnalyticsViewModel
        //    {
        //        AllDimensions = Enum.GetValues(typeof(Dimension)).OfType<Dimension>().ToList(),
        //        AllMetrics = Enum.GetValues(typeof(Metric)).OfType<Metric>().ToList(),
        //        SelectedDimensions = CurrentItem.Dimensions.ToList(),
        //        SelectedMetrics = CurrentItem.Metrics.ToList(),
        //        Period = CurrentItem.ChartPeriod,
        //        Periods = new[] { SLI("Day", 1), SLI("Week", 7), SLI("2 weeks", 14), SLI("Month", 31), SLI("Quarter", 92), SLI("Year", 365), SLI("2 years", 2 * 365), SLI("4 years", 4 * 365) }
        //    };
        //    if (vd.SelectedDimensions.Count == 0)
        //        vd.SelectedDimensions.Add(Dimension.date);
        //    if (vd.SelectedMetrics.Count == 0)
        //        vd.SelectedMetrics.AddRange(new[] { Metric.visits, Metric.pageviews });

        //    if (Request["failure"] == CurrentItem.ID.ToString())
        //        ModelState.AddModelError("password", Request["errorText"]);

        //    return PartialView("ConfigureStatistics", vd);
        //}

        //private SelectListItem SLI(string text, int value)
        //{
        //    return new SelectListItem
        //    {
        //        Text = text,
        //        Value = value.ToString()
        //    };
        //} 

        //[HttpPost]
        //public ActionResult SaveStatisticsConfiguration(int period)
        //{
        //    var dimensions = Read<Dimension>("Dimension").ToList();
        //    var metrics = Read<Metric>("Metric").ToList();

        //    if (dimensions.Count == 0 || metrics.Count == 0)
        //        return RedirectToIndex("Must select at least one from dimensions and one from metrics");

        //    CurrentItem.ChartPeriod = period;
        //    CurrentItem.Dimensions = dimensions;
        //    CurrentItem.Metrics = metrics;
        //    Engine.Persister.Save(CurrentItem);

        //    return Redirect(CurrentPage.Url);
        //}

        //private IEnumerable<T> Read<T>(string prefix) where T: struct
        //{
        //    string values = Request.Form[prefix];
            
        //    if(string.IsNullOrEmpty(values))
        //        yield break;

        //    foreach (var value in values.Split(','))
        //    {
        //        T e = (T)Enum.Parse(typeof(T), value);
        //        yield return e;
        //    }
        //}

        //private ActionResult RedirectToIndex(string errorText)
        //{
        //    N2.Web.Url url = CurrentPage.Url;
        //    return Redirect(url.AppendQuery("failure", CurrentItem.ID).AppendQuery("errorText", errorText));
        //}

        //private IEnumerable<GenericEntry> GetStatisticsEntries(DateTime from, DateTime to)
        //{
        //    var rr = new ReportRequestor(CurrentItem.Token);
            
        //    var entries = rr.RequestReport(
        //        new AnalyticsAccountInfo { AccountID = CurrentItem.AccountID, ProfileID = CurrentItem.ProfileID, Title = CurrentItem.Title, AccountName = CurrentItem.AccountName },
        //        CurrentItem.Dimensions,
        //        CurrentItem.Metrics,
        //        from,
        //        to);
        //    return entries;
        //}

        ////protected override PartialViewResult PartialView(string viewName, object model)
        ////{
        ////    return base.PartialView(N2.Web.Url.ResolveTokens("{ManagementUrl}/Myself/Analytics/Views/ManageAnalytics/" + viewName + ".ascx"), model);
        ////}
    }
}
