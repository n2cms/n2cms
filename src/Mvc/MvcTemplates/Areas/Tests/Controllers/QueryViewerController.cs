#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc;
using N2.Web;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Config;
using log4net;
using log4net.Appender;
using N2.Templates.Mvc.Areas.Tests.Models;
using System.Diagnostics;
using System.Reflection;

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{
    [Controls(typeof(Models.QueryViewerPart))]
    public class QueryViewerController : ContentController<Models.QueryViewerPart>
    {
        static QueryViewerController()
        {
            CountToContextItemsAppender.AddLogger("NHibernate", "All", false);
            CountToContextItemsAppender.AddLogger("NHibernate.SQL", "Queries", true);
        }

        //
        // GET: /Tests/QueryViewer/

        public override ActionResult Index()
        {
            if (!CurrentItem.VisibleToEveryone && !Engine.SecurityManager.IsEditor(User))
                return Content("");
            
            return View(new QueryViewData { 
                All = () => CountToContextItemsAppender.GetOrCreateList("All"),
                Queries = () => CountToContextItemsAppender.GetOrCreateList("Queries") 
            });
        }
    }

    public class Row
    {
        public string Sql { get; set; }
        public string Caller { get; set; }
        public DateTime Time { get; set; }
    }

    public class CountToContextItemsAppender : IAppender
    {
        Func<List<Row>> getListToUse;

        public bool AddStack { get; set; }

        public CountToContextItemsAppender(Func<List<Row>> getListToUse)
        {
            this.getListToUse = getListToUse;
        }

        private string name;

        #region IAppender Members

        public void Close()
        {
        }

        public void DoAppend(LoggingEvent loggingEvent)
        {
            if (string.Empty.Equals(loggingEvent.MessageObject))
                return;//can happen for batch queries, this is a noise issue, basically.

            if (loggingEvent.MessageObject != null)
            {
                getListToUse().Add(
                    new Row { Caller = AddStack ? GetCaller(new StackTrace()) : "", Sql = loggingEvent.MessageObject.ToString(), Time = N2.Utility.CurrentTime() });
            }
        }

        private string GetCaller(StackTrace stackTrace)
        {
            return string.Join("", 
                stackTrace.GetFrames()
                .Where(f => f.GetMethod().DeclaringType != null)
                .Where(f => !f.GetMethod().DeclaringType.FullName.StartsWith("Castle"))
                .Where(f => !f.GetMethod().DeclaringType.FullName.StartsWith("NHibernate"))
                .Where(f => !f.GetMethod().DeclaringType.FullName.StartsWith("log4net"))
                .Where(f => !f.GetMethod().DeclaringType.FullName.StartsWith("System"))
                .Where(f => f.GetMethod().DeclaringType != typeof(CountToContextItemsAppender))
                .Select(f => f.GetMethod().DeclaringType.FullName + "." + f.GetMethod().Name + "<br/>")
                .ToArray());
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion

        public static void AddLogger(string loggerName, string listName, bool addStack)
        {
            Logger logger = (Logger)LogManager.GetLogger(loggerName).Logger;
            logger.Level = Level.All;
            logger.AddAppender(new CountToContextItemsAppender(() => GetOrCreateList(listName)) { AddStack = addStack });
            logger.Level = logger.Hierarchy.LevelMap["DEBUG"];
            BasicConfigurator.Configure(logger.Repository);
        }

        [ThreadStatic]
        static List<Row> nonWebList = new List<Row>();

        public static List<Row> GetOrCreateList(string listName)
        {
            if (System.Web.HttpContext.Current != null)
            {
                var list = System.Web.HttpContext.Current.Items[listName] as List<Row>;
                if (list == null)
                    System.Web.HttpContext.Current.Items[listName] = list = new List<Row>();
                return list;
            }
            if (nonWebList == null)
                nonWebList = new List<Row>();
            if (nonWebList.Count > 10000)
                nonWebList.RemoveRange(0, nonWebList.Count / 2);
            return nonWebList;
        }

    }

}
#endif
