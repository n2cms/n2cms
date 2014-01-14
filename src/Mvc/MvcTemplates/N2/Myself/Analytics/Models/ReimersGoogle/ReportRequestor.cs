// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportRequestor.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2006
// </copyright>
// <summary>
//   Gets reports from Google Analytics API.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Reimers.Google.Analytics.Reports;

namespace Reimers.Google.Analytics
{
    /// <summary>
    /// Gets reports from Google Analytics API.
    /// </summary>
    public class ReportRequestor
    {
        #region Fields
        
        /// <summary>
        /// The cultureinfo to use.
        /// </summary>
        private static readonly CultureInfo Ci = CultureInfo.GetCultureInfo("en-US");

        /// <summary>
        /// The defined request url.
        /// </summary>
        private const string RequestUrlFormat =
            "https://www.google.com/analytics/feeds/data?ids={0}&dimensions={1}&metrics={2}&start-date={3}&end-date={4}&max-results={5}";

        /// <summary>
        /// The token requestor to use.
        /// </summary>
        private readonly TokenRequestor _tokenRequestor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportRequestor"/> class.
        /// </summary>
        public ReportRequestor()
        {
            _tokenRequestor = new TokenRequestor();
        }

        public ReportRequestor(string authToken)
        {
            AuthToken = authToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportRequestor"/> class.
        /// </summary>
        /// <param name="email">
        /// The Google account email to use for log in.
        /// </param>
        /// <param name="password">
        /// The password to use for log in.
        /// </param>
        public ReportRequestor(string email, string password) : this()
        {
            _tokenRequestor.Username = email;
            _tokenRequestor.Password = password;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the email to use for log in.
        /// </summary>
        public string Email
        {
            get { return _tokenRequestor.Username; }
            set { _tokenRequestor.Username = value; }
        }

        /// <summary>
        /// Gets or sets the password to use for log in.
        /// </summary>
        public string Password
        {
            get { return _tokenRequestor.Password; }
            set { _tokenRequestor.Password = value; }
        }

        public string AuthToken { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> with all the <see cref="AnalyticsAccountInfo"/> available to the user.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/>.</returns>
        public IEnumerable<AnalyticsAccountInfo> GetAccounts()
        {
            string token = AuthToken ?? _tokenRequestor.GetAuthToken();
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create("https://www.google.com/analytics/feeds/accounts/default");
            req.Headers.Add("Authorization: GoogleLogin auth=" + token);
            HttpWebResponse response = (HttpWebResponse) req.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream);
            string responseXml = sr.ReadToEnd();
            XDocument doc = XDocument.Parse(responseXml);
            XNamespace dxpSpace = doc.Root.GetNamespaceOfPrefix("dxp");
            XNamespace defaultSpace = doc.Root.GetDefaultNamespace();

            IEnumerable<AnalyticsAccountInfo> entries =
                from en in doc.Root.Descendants(defaultSpace + "entry")
                select new AnalyticsAccountInfo
                        {
                            AccountID =
                                Convert.ToInt32(
                                en.Elements(dxpSpace + "property").Where(
                                    xe => xe.Attribute("name").Value == "ga:accountId").First().
                                    Attribute("value").Value),
                            AccountName =
                                en.Elements(dxpSpace + "property").Where(
                                xe => xe.Attribute("name").Value == "ga:accountName").First().
                                Attribute("value").Value,
                            ID = en.Element(defaultSpace + "id").Value,
                            Title = en.Element(defaultSpace + "title").Value,
                            ProfileID =
                                Convert.ToInt32(
                                en.Elements(dxpSpace + "property").Where(
                                    xe => xe.Attribute("name").Value == "ga:profileId").First().
                                    Attribute("value").Value),
                            WebPropertyID =
                                en.Elements(dxpSpace + "property").Where(
                                xe => xe.Attribute("name").Value == "ga:webPropertyId").First()
                                .Attribute("value").Value
                        };
            return entries;
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> with all the <see cref="AnalyticsAccountInfo"/> available to the user.
        /// </summary>
        /// <param name="callback">The <see cref="CallbackDelegate{T}"/> to invoke when the accounts have been received.</param>
        public void GetAccountsAsync(CallbackDelegate<IEnumerable<AnalyticsAccountInfo>> callback)
        {
            if (callback != null)
            {
                ThreadPool.QueueUserWorkItem(
                    o =>
                        {
                            IEnumerable<AnalyticsAccountInfo> result = GetAccounts();
                            callback(result);
                        });
            }
        }

        /// <summary>
        /// Requests a report from Google Analytics with the given dimensions and metrics.
        /// </summary>
        /// <param name="account">The <see cref="AnalyticsAccountInfo"></see> to get the reports for.</param>
        /// <param name="dimensions">A list of <see cref="Dimension"></see> for the report.</param>
        /// <param name="metrics">A list of <see cref="Metric"></see> for the report.</param>
        /// <param name="from">The start <see cref="DateTime"></see> of the report data.</param>
        /// <param name="to">The end <see cref="DateTime"></see> of the report data.</param>
        /// <returns>An <see cref="IEnumerable{T}"></see> of <see cref="GenericEntry"></see>.</returns>
        public IEnumerable<GenericEntry> RequestReport(AnalyticsAccountInfo account, IEnumerable<Dimension> dimensions, IEnumerable<Metric> metrics, DateTime from, DateTime to)
        {
            return RequestReport(account, dimensions, metrics, from, to, 1000);
        }
        
        /// <summary>
        /// Requests a report from Google Analytics with the given dimensions and metrics.
        /// </summary>
        /// <param name="account">The <see cref="AnalyticsAccountInfo"/> to get the reports for.</param>
        /// <param name="dimensions">A list of <see cref="Dimension"/> for the report.</param>
        /// <param name="metrics">A list of <see cref="Metric"/> for the report.</param>
        /// <param name="from">The start <see cref="DateTime"/> of the report data.</param>
        /// <param name="to">The end <see cref="DateTime"/> of the report data.</param>
        /// <param name="maxResults">The max amount of results to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="GenericEntry"/>.</returns>
        public IEnumerable<GenericEntry> RequestReport(AnalyticsAccountInfo account, IEnumerable<Dimension> dimensions, IEnumerable<Metric> metrics, DateTime from, DateTime to, int maxResults)
        {
            XDocument doc = GetReport(account, dimensions, metrics, from, to, maxResults);
            if (doc.Root != null)
            {
                XNamespace dxpSpace = doc.Root.GetNamespaceOfPrefix("dxp");
                XNamespace defaultSpace = doc.Root.GetDefaultNamespace();
                IEnumerable<GenericEntry> gr =
                    from r in doc.Root.Descendants(defaultSpace + "entry")
                    select new GenericEntry
                            {
                                Dimensions = new List<KeyValuePair<Dimension, string>>(
                                    from rd in r.Elements(dxpSpace + "dimension")
                                    select new KeyValuePair<Dimension, string>(
                                        (Dimension)Enum.Parse(typeof(Dimension), rd.Attribute("name").Value.Replace("ga:", string.Empty), true), rd.Attribute("value").Value)),
                                Metrics = new List<KeyValuePair<Metric, string>>(
                                    from rm in r.Elements(dxpSpace + "metric")
                                    select new KeyValuePair<Metric, string>(
                                        (Metric)Enum.Parse(typeof(Metric), rm.Attribute("name").Value.Replace("ga:", string.Empty), true), rm.Attribute("value").Value))
                            };

                return gr;
            }

            return null;
        }

        /// <summary>
        /// Creates an asynchronous request for a report from Google Analytics with the given dimensions and metrics.
        /// </summary>
        /// <param name="account">The <see cref="AnalyticsAccountInfo"/> to get the reports for.</param>
        /// <param name="dimensions">A list of <see cref="Dimension"/> for the report.</param>
        /// <param name="metrics">A list of <see cref="Metric"/> for the report.</param>
        /// <param name="from">The start <see cref="DateTime"/> of the report data.</param>
        /// <param name="to">The end <see cref="DateTime"/> of the report data.</param>
        /// <param name="callback">The <see cref="CallbackDelegate{T}"/> to invoke when the data has been received.</param>
        public void RequestReportAsync(AnalyticsAccountInfo account, IEnumerable<Dimension> dimensions, IEnumerable<Metric> metrics, DateTime from, DateTime to, CallbackDelegate<IEnumerable<GenericEntry>> callback)
        {
            if (callback != null)
            {
                ThreadPool.QueueUserWorkItem(o =>
                                                {
                                                    IEnumerable<GenericEntry> result = RequestReport(
                                                        account,
                                                        dimensions,
                                                        metrics,
                                                        from,
                                                        to);
                                                    callback(result);
                                                });
            }
        }

        /// <summary>
        /// Requests a report from Google Analytics with predefined dimensions and metrics.
        /// </summary>
        /// <param name="account">The <see cref="AnalyticsAccountInfo"/> to get the reports for.</param>
        /// <param name="from">The start <see cref="DateTime"/> of the report data.</param>
        /// <param name="to">The end <see cref="DateTime"/> of the report data.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="CityReport"/>.</returns>
        public IEnumerable<CityReport> GetUserCountByLocation(AnalyticsAccountInfo account, DateTime from, DateTime to)
        {
            IEnumerable<GenericEntry> report = RequestReport(
                account,
                new[] { Dimension.date, Dimension.city, Dimension.latitude, Dimension.longitude },
                new[] { Metric.visits }, 
                from, 
                to);
            IEnumerable<CityReport> cr = from r in report
                                         select new CityReport
                                                    {
                                                        Date =
                                                            DateTime.ParseExact(r.Dimensions.First(d => d.Key == Dimension.date).Value, "yyyyMMdd", Ci),
                                                        City = r.Dimensions.First(d => d.Key == Dimension.city).Value,
                                                        Latitude =
                                                            Convert.ToDouble(
                                                            r.Dimensions.First(d => d.Key == Dimension.latitude).Value,
                                                            Ci),
                                                        Longitude =
                                                            Convert.ToDouble(
                                                            r.Dimensions.First(d => d.Key == Dimension.longitude).Value,
                                                            Ci),
                                                        Count = Convert.ToInt32(r.Metrics.First(m => m.Key == Metric.visits).Value)
                                                    };
            return cr;
        }

        /// <summary>
        /// Creates an asynchronous request for a report from Google Analytics with the given dimensions and metrics.
        /// </summary>
        /// <param name="account">The <see cref="AnalyticsAccountInfo"/> to get the reports for.</param>
        /// <param name="from">The start <see cref="DateTime"/> of the report data.</param>
        /// <param name="to">The end <see cref="DateTime"/> of the report data.</param>
        /// <param name="callback">The <see cref="CallbackDelegate{T}"/> to invoke when the data has been received.</param>
        public void GetUserCountByLocationAsync(AnalyticsAccountInfo account, DateTime from, DateTime to, CallbackDelegate<IEnumerable<CityReport>> callback)
        {
            if (callback != null)
            {
                ThreadPool.QueueUserWorkItem(o =>
                                                {
                                                    IEnumerable<CityReport> result = GetUserCountByLocation(account, from, to);
                                                    callback(result);
                                                });
            }
        }

        /// <summary>
        /// Gets the report with the defined dimensions and metrics.
        /// </summary>
        /// <param name="account">
        /// The account to fetch the report for.
        /// </param>
        /// <param name="dimensions">
        /// The defined dimensions.
        /// </param>
        /// <param name="metrics">
        /// The defined metrics.
        /// </param>
        /// <param name="from">
        /// The from date.
        /// </param>
        /// <param name="to">
        /// The to date.
        /// </param>
        /// <returns>
        /// An <see cref="XDocument"/>.
        /// </returns>
        private XDocument GetReport(AnalyticsAccountInfo account, IEnumerable<Dimension> dimensions, IEnumerable<Metric> metrics, DateTime from, DateTime to, int maxResults)
        {
            string token = AuthToken ?? _tokenRequestor.GetAuthToken();
            StringBuilder dims = new StringBuilder();
            foreach (Dimension item in dimensions)
            {
                dims.Append("ga:" + item + ",");
            }
            
            StringBuilder mets = new StringBuilder();
            foreach (Metric item in metrics)
            {
                mets.Append("ga:" + item + ",");
            }

            string requestUrl = string.Format(
                RequestUrlFormat, 
                "ga:" + account.ProfileID,
                dims.ToString().Trim(','), 
                mets.ToString().Trim(','),
                from.ToString("yyyy-MM-dd"), 
                to.ToString("yyyy-MM-dd"),
                maxResults);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUrl);
            req.Headers.Add("Authorization: GoogleLogin auth=" + token);
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            Stream responseStream = response.GetResponseStream();
            string responseXml = new StreamReader(responseStream, Encoding.UTF8, true).ReadToEnd();
            XDocument doc = XDocument.Parse(responseXml);
            return doc;
        }

        #endregion
    }
}
