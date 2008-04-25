using System;
using System.Collections.Generic;
using System.Text;
using N2.Globalization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using N2.Persistence;
using Rhino.Mocks;
using N2.Web;
using N2.Persistence.NH;
using N2.Tests.Persistence;

namespace N2.Tests.Globalization
{
	[TestFixture]
	public class LanguageRootTests : PersistenceAwareBase
	{
		ContentItem root;
		ContentItem english;
		ContentItem swedish;
		ContentItem italian;

		[TestFixtureSetUp]
		public override void TestFixtureSetUp()
		{
			base.TestFixtureSetUp();

			engine.AddComponent("LanguageGateway", typeof(LanguageGateway));
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			root = engine.Definitions.CreateInstance<Items.TranslatedPage>(null);
			engine.Persister.Save(root);
			english = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			engine.Persister.Save(english);
			swedish = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			engine.Persister.Save(swedish);
			italian = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			engine.Persister.Save(italian);

			engine.Resolve<Site>().RootItemID = root.ID;
			engine.Resolve<Site>().StartPageID = root.ID;
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();

			engine.Persister.Delete(root);
		}

		[Test]
		public void CanList_LanguageRoots()
		{
			mocks.ReplayAll();

			LanguageGateway gateway = engine.Resolve<LanguageGateway>();
			IList<ILanguageRoot> languageRoot = new List<ILanguageRoot>(gateway.GetLanguages());

			Assert.That(languageRoot.Count, Is.EqualTo(3));
		}

		[Test]
		public void LanguageKey_IsAddedTo_SavedItem()
		{
			mocks.ReplayAll();

			ContentItem englishSub = CreateOneItem<Items.LanguageRoot>(0, "english1", english);

			engine.Persister.Save(englishSub);

			Assert.That(englishSub[LanguageGateway.LanguageKey], Is.EqualTo(englishSub.ID));
		}

		[Test]
		public void LanguageKey_IsNotAddedTo_ItemsOutsideA_LanguageRoot()
		{
			mocks.ReplayAll();

			ContentItem page = CreateOneItem<Items.LanguageRoot>(0, "page", root);

			engine.Persister.Save(page);

			Assert.That(page[LanguageGateway.LanguageKey], Is.Null);
		}

		[Test]
		public void LanguageKey_IsReused_ForTranslations_OfAPage()
		{
			mocks.ReplayAll();

			ContentItem englishSub = CreateOneItem<Items.LanguageRoot>(0, "english1", english);
			engine.Persister.Save(englishSub);

			engine.Resolve<IWebContext>().QueryString[LanguageGateway.LanguageKey] = englishSub.ID.ToString();

			ContentItem swedishSub = CreateOneItem<Items.LanguageRoot>(0, "swedish1", swedish);
			engine.Persister.Save(swedishSub);

			Assert.That(swedishSub[LanguageGateway.LanguageKey], Is.EqualTo(englishSub[LanguageGateway.LanguageKey]));
		}
	}
}
