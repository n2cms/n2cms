using System;
using System.Web;
using N2;
using N2.Definitions;
using N2.Edit.LinkTracker;
using N2.Edit.Wizard;
using N2.MediumTrust.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Trashcan;
using N2.Web;

namespace MediumTrustTest
{
	public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
		}

		public override void Init()
		{
			base.Init();

			MediumTrustFactory factory = ((MediumTrustFactory) N2.Context.Current);
			Site site = factory.Resolve<Site>();
			IPersister persister = factory.Resolve<IPersister>();

			StartTrash(factory, persister, site);
			StartTracker(factory, persister);
			StartWizard(factory, persister, site);

			factory.EditManager.EnableVersioning = true;
		}

		private void StartWizard(MediumTrustFactory factory, IPersister persister, Site site)
		{
			LocationWizard wizard = new LocationWizard(persister, site);
			factory.AddComponentInstance(typeof (LocationWizard).FullName, typeof (LocationWizard), wizard);
		}

		private void StartTrash(MediumTrustFactory factory, IPersister persister, Site site)
		{
			IDefinitionManager definitions = factory.Resolve<IDefinitionManager>();
			TrashHandler handler = new TrashHandler(persister, definitions, site);
			DeleteInterceptor interceptor = new DeleteInterceptor(persister, handler);
			interceptor.Start();
			factory.AddComponentInstance(typeof(TrashHandler).FullName, typeof(TrashHandler), handler);
			factory.AddComponentInstance(typeof(DeleteInterceptor).FullName, typeof(DeleteInterceptor), interceptor);
		}

		private void StartTracker(MediumTrustFactory factory, IPersister persister)
		{
			IItemFinder finder = factory.Resolve<IItemFinder>();
			IUrlParser parser = factory.Resolve<IUrlParser>();
			Tracker tracker = new Tracker(persister, finder, parser);
			tracker.Start();
			factory.AddComponentInstance(typeof(Tracker).FullName, typeof(Tracker), tracker);
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}
	}
}