using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using N2.Engine;
using N2.Engine.Castle;
using N2.Tests.Web.Items;
using NUnit.Framework;
using N2.Web;

namespace N2.Tests.Web.WebControls
{
	public abstract class WebControlTestsBase : ItemTestsBase
	{
		protected const string ZoneName = "TheZone";
		
		protected PageItem page;
		protected DataItem data;

		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			var engine = new ContentEngine(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None),
				"n2nodb",
				new WindsorServiceContainer(),
				new EventBroker(),
				new ContainerConfigurer());
			N2.Context.Replace(engine);
			engine.Initialize();
			engine.Host.CurrentSite.RootItemID = 1;
			engine.Host.CurrentSite.StartPageID = 1;
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			page = CreateOneItem<PageItem>(1, "page", null);
			data = CreateOneItem<DataItem>(2, "data", page);
			data.ZoneName = ZoneName;
		}

		protected override T CreateOneItem<T>(int id, string name, ContentItem parent)
		{
			var item = base.CreateOneItem<T>(id, name, parent);
			N2.Context.Current.Persister.Repository.SaveOrUpdate(item);
			return item;
		}

		protected void Initialize(string queryString)
		{
			TestSupport.InitializeHttpContext("/Default.aspx", queryString);
		}
	}
}
