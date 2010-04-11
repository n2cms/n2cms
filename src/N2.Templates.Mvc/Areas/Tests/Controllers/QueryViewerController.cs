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

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{
	[Controls(typeof(Models.QueryViewerPart))]
    public class QueryViewerController : ContentController<Models.QueryViewerPart>
    {
		static QueryViewerController()
		{
			CountToContextItemsAppender.AddLogger("NHibernate", "All");
			CountToContextItemsAppender.AddLogger("NHibernate.SQL", "Queries");
		}

        //
        // GET: /Tests/QueryViewer/

        public override ActionResult Index()
        {
			return View(new QueryViewData { 
				All = () => CountToContextItemsAppender.GetOrCreateList("All"),
				Queries = () => CountToContextItemsAppender.GetOrCreateList("Queries") 
			});
        }
    }

	public class CountToContextItemsAppender : IAppender
	{
		Func<List<string>> getListToUse;

		public CountToContextItemsAppender(Func<List<string>> getListToUse)
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
				getListToUse().Add(loggingEvent.MessageObject.ToString());
			}
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		#endregion

		public static void AddLogger(string loggerName, string listName)
		{
			Logger logger = (Logger)LogManager.GetLogger(loggerName).Logger;
			logger.Level = Level.All;
			logger.AddAppender(new CountToContextItemsAppender(() => GetOrCreateList(listName)));
			logger.Level = logger.Hierarchy.LevelMap["DEBUG"];
			BasicConfigurator.Configure(logger.Repository);
		}

		[ThreadStatic]
		static List<string> nonWebList = new List<string>();

		public static List<string> GetOrCreateList(string listName)
		{
			if (System.Web.HttpContext.Current != null)
			{
				var list = System.Web.HttpContext.Current.Items[listName] as List<string>;
				if (list == null)
					System.Web.HttpContext.Current.Items[listName] = list = new List<string>();
				return list;
			}

			if (nonWebList.Count > 10000)
				nonWebList.RemoveRange(0, nonWebList.Count / 2);
			return nonWebList;
		}

	}

}
#endif