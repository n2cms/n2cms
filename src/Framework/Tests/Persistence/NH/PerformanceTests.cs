using System;
using N2.Definitions.Static;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH.Finder;
using N2.Tests.Persistence.Definitions;
using N2.Web;
using NUnit.Framework;
using log4net.Appender;
using System.Collections.Generic;
using log4net.Repository.Hierarchy;
using log4net.Config;
using log4net;
using log4net.Core;
using System.Diagnostics;
using System.Linq;
using Shouldly;
using System.Web;
using N2.Edit.Versioning;
using N2.Tests.Fakes;

namespace N2.Tests.Persistence.NH
{
    [TestFixture, Category("Integration")]
    public class PerformanceTests : DatabasePreparingBase
    {
        #region SetUp

        IItemFinder finder;
        ContentItem rootItem;
        ContentItem startPage;
        ContentItem item1;
        ContentItem item2;
        ContentItem item3;
        protected FakeSessionProvider sessionProvider;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            sessionProvider = helper.SessionProvider;

            CreateRootItem();
            SaveVersionAndUpdateRootItem();
            CreateStartPageBelow(rootItem);
            item1 = CreatePageBelow(startPage, 1);
            item2 = CreatePageBelow(startPage, 2);
            item3 = CreatePageBelow(startPage, 3);

            engine.Resolve<IHost>().DefaultSite.RootItemID = rootItem.ID;
            engine.Resolve<IHost>().DefaultSite.StartPageID = startPage.ID;

            finder = new ItemFinder(sessionProvider, new DefinitionMap());

            sessionProvider.OpenSession.Session.Clear();
        }

        #endregion

        [Test]
        public void TypeFiltering_DoesNotSelectNPlus1()
        {
            var initialCount = sessionProvider.SessionFactory.Statistics.QueryExecutionCount;

            var items = finder.Where.Type.Eq(typeof(PersistableItem2))
                .Select<PersistableItem2>();
            Assert.AreEqual(3, items.Count);
            EnumerableAssert.Contains(items, item1);
            EnumerableAssert.Contains(items, item2);
            EnumerableAssert.Contains(items, item3);

            sessionProvider.SessionFactory.Statistics.QueryExecutionCount.ShouldBe(initialCount + 1);
            sessionProvider.SessionFactory.Statistics.GetEntityStatistics("PersistableItem2").FetchCount.ShouldBe(0);
        }

        #region Helpers

        private ContentItem CreatePageBelow(ContentItem parentPage, int index)
        {
            ContentItem item = CreateOneItem<PersistableItem2>(0, "item" + index, parentPage);

            N2.Details.DetailCollection details = item.GetDetailCollection("DetailCollection", true);
            details.Add(true);
            details.Add(index * 1000 + 555);
            details.Add(index * 1000.0 + 555.55);
            details.Add("string in a collection " + index);
            details.Add(parentPage);
            details.Add(new DateTime(2009 + index, 1, 1));

            engine.Persister.Save(item);
            return item;
        }

        private void CreateStartPageBelow(ContentItem root)
        {
            startPage = CreateOneItem<PersistableItem>(0, "start page", root);
            startPage.ZoneName = "AZone";
            startPage.SortOrder = 34;
            startPage.Visible = true;
            startPage["IntDetail"] = 45;
            startPage["DoubleDetail"] = 56.66;
            startPage["BoolDetail"] = true;
            startPage["DateDetail"] = new DateTime(2000, 01, 01);
            startPage["StringDetail"] = "actually another string";
            startPage["StringDetail2"] = "just a string";
            startPage["ObjectDetail"] = new string[] { "two", "three", "four" };
            startPage["ItemDetail"] = root;

            engine.Persister.Save(startPage);
        }

        private void SaveVersionAndUpdateRootItem()
        {
            engine.Resolve<IVersionManager>().AddVersion(rootItem);

            rootItem.Created = DateTime.Today;
            rootItem.Published = new DateTime(2007, 06, 03);
            rootItem.Expires = new DateTime(2017, 06, 03);
            rootItem.ZoneName = "TheZone";
            rootItem.SortOrder = 23;
            rootItem.Visible = false;
            rootItem["IntDetail"] = 43;
            rootItem["DoubleDetail"] = 43.33;
            rootItem["BoolDetail"] = false;
            rootItem["DateDetail"] = new DateTime(1999, 12, 31);
            rootItem["StringDetail"] = "just a string";
            rootItem["StringDetail2"] = "just another string";
            rootItem["ObjectDetail"] = new string[] { "one", "two", "three" };

            engine.Persister.Save(rootItem);
        }

        private void CreateRootItem()
        {
            rootItem = CreateOneItem<PersistableItem>(0, "root", null);
            rootItem.Created = new DateTime(2007, 06, 01);
            rootItem.Published = new DateTime(2007, 06, 02);
            rootItem.Expires = new DateTime(2017, 06, 02);
            rootItem.ZoneName = "ZaZone";
            rootItem.SortOrder = 12;
            rootItem.Visible = true;
            rootItem["IntDetail"] = 32;
            rootItem["DoubleDetail"] = 32.22;
            rootItem["BoolDetail"] = true;
            rootItem["DateDetail"] = new DateTime(1998, 12, 31);
            rootItem["StringDetail"] = "a string in a version";
            rootItem["StringDetail2"] = "just a string";
            rootItem["ObjectDetail"] = new string[] { "zero", "one", "two" };

            engine.Persister.Save(rootItem);
        }

        #endregion
    }

    public class ToSelfAppender : IAppender, IDisposable
    {
        public bool AddStack { get; set; }
        public string Name { get; set; }
        public string LoggerName { get; set; }

        public ToSelfAppender()
        {
        }

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
                GetList().Add(
                    new Row { Caller = AddStack ? GetCaller(new StackTrace()) : "", Sql = loggingEvent.MessageObject.ToString(), Time = N2.Utility.CurrentTime() });
            }
        }

        public List<Row> GetList()
        {
            if (HttpContext.Current != null)
            {
                if (!HttpContext.Current.Items.Contains("CountToContextItemsAppender"))
                    HttpContext.Current.Items["CountToContextItemsAppender"] = new List<Row>();

                return HttpContext.Current.Items["CountToContextItemsAppender"] as List<Row>;
            }

            return nonWebList;
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
                .Where(f => f.GetMethod().DeclaringType != typeof(ToSelfAppender))
                .Select(f => f.GetMethod().DeclaringType.FullName + "." + f.GetMethod().Name + "<br/>")
                .ToArray());
        }

        #endregion

        List<Row> nonWebList = new List<Row>();

        public void Dispose()
        {
            StopAppending(this);
        }

        public static ToSelfAppender StartAppending(string loggerName = "NHibernate.SQL", bool addStack = true)
        {
            var appender = new ToSelfAppender() { AddStack = addStack, LoggerName = loggerName };

            Logger logger = (Logger)LogManager.GetLogger(loggerName).Logger;
            logger.Level = Level.All;
            logger.AddAppender(appender);

            logger.Level = logger.Hierarchy.LevelMap["DEBUG"];
            BasicConfigurator.Configure(logger.Repository);

            return appender;
        }

        private void StopAppending(ToSelfAppender appender)
        {
            Logger logger = (Logger)LogManager.GetLogger(appender.LoggerName).Logger;
            logger.RemoveAppender(appender);
        }

        public class Row
        {
            public string Sql { get; set; }
            public string Caller { get; set; }
            public DateTime Time { get; set; }
        }
    }

}
