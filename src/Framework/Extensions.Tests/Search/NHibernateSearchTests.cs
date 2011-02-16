using System;
using System.Linq;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using System.Diagnostics;
using N2.Definitions;
using N2.Persistence;
using NHibernate.Tool.hbm2ddl;
using N2.Persistence.NH.Finder;
using N2.Tests.Fakes;
using N2.Persistence.NH;
using N2.Collections;
using N2.Details;
using System.Collections;
using NHibernate.Engine;
using N2.Edit.Workflow;
using System.Configuration;
using N2.Configuration;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class NHibernateSearchTests : PersisterTestsBase
	{
		[TestFixtureSetUp]
		public override void TestFixtureSetup()
		{
			FakeWebContextWrapper context = new Fakes.FakeWebContextWrapper();
			DatabaseSection config = (DatabaseSection)ConfigurationManager.GetSection("n2/database");
			TestSupport.Setup(out definitions, out activator, out notifier, out sessionProvider, out finder, out schemaCreator, out proxyFactory, 
				context, config, 
				new [] { new SearchConfigurationBuilderParticipator(context, config) }, persistedTypes);
		}

		[Test]
		public void NHibernateSearch_OnTitle()
		{
			var s = NHibernate.Search.Search.CreateFullTextSession(sessionProvider.OpenSession.Session);

			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			item.Title = "hello world";
			persister.Save(item);

			var results = s.CreateFullTextQuery<ContentItem>("Title:hello").List();
			Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));
			Assert.That(results.Contains(item));
		}

		[Test]
		public void NHibernateSearch_OnDetail()
		{
			var s = NHibernate.Search.Search.CreateFullTextSession(sessionProvider.OpenSession.Session);

			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			item.Title = "hello world";
			item["Hej"] = "Världen";
			persister.Save(item);

			var results = s.CreateFullTextQuery<ContentItem>("Details.StringValue:Världen").List();
			Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));
			Assert.That(results.Contains(item));
		}

		//[Test]
		//public void NHibernateSearch_X()
		//{
		//    //var s = NHibernate.Search.Search.CreateFullTextSession(sessionProvider.OpenSession.Session);

		//    //ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "saveableRoot", null);
		//    //item.Title = "hello world";
		//    //item.Name = "hello-world";
		//    //item.Published = DateTime.Now;
		//    //item.Expires = DateTime.Now.AddDays(1);
		//    //item.SavedBy = "admin";
		//    //item.State = ContentState.New;
		//    //item["Hej"] = "Världen";
		//    //persister.Save(item);

		//    //var results = s.CreateFullTextQuery<ContentItem>("Title:hello").List();
		//    //Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));
		//    //Assert.That(results.Contains(item));
		//}

		//[Test]
		//public void EagerDetails()
		//{
		//    ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "item", null);
		//    item["Hello"] = "world";
		//    item.GetDetailCollection("World", true).Add("Hello");
		//    using(persister)
		//    {
		//        persister.Save(item);
		//    }
		//    using(persister)
		//    {
		//        var mq = sessionProvider.OpenSession.Session.CreateMultiQuery()
		//            .Add("item", sessionProvider.OpenSession.Session.CreateQuery("from ContentItem where ID=:id").SetParameter("id", item.ID))
		//            .Add("details", sessionProvider.OpenSession.Session.CreateQuery("select ci.Details from ContentItem ci where ci.ID=:id").SetParameter("id", item.ID))
		//            .Add("collections", sessionProvider.OpenSession.Session.CreateQuery("select dc from DetailCollection dc where dc.EnclosingItem.ID=:id join fetch dc.Details").SetParameter("id", item.ID))
		//            .SetCacheable(true);
		//        item = (ContentItem)((IList)mq.GetResult("item"))[0];
		//        item.Details = new PersistentContentList<ContentDetail>((ISessionImplementor)sessionProvider.OpenSession.Session, ((IList)mq.GetResult("details")).Cast<ContentDetail>().ToList()) { Owner = item };
		//        item.DetailCollections = new PersistentContentList<DetailCollection>((ISessionImplementor)sessionProvider.OpenSession.Session, ((IList)mq.GetResult("collections")).Cast<DetailCollection>().ToList()) { Owner = item };

		//        //item["Added"] = "B there";
		//        //item.DetailCollections["World"].Add("B there 2");
		//        //item.GetDetailCollection("AddedCollection", true).Add("B there 3");
		//        persister.Save(item);
		//    }
		//}
	}
}
