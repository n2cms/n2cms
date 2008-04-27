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
using System.Diagnostics;
using System.Globalization;

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
			engine.AddComponent("LanguageGateway", typeof(ILanguageGateway), typeof(LanguageGateway));
			engine.AddComponent("LanguageInterceptor", typeof(LanguageInterceptor));
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			root = engine.Definitions.CreateInstance<Items.TranslatedPage>(null);
			engine.Persister.Save(root);
			
			english = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			english.Name = english.Title = "english";
			engine.Persister.Save(english);

			swedish = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			swedish.Name = swedish.Title = "swedish"; 
			engine.Persister.Save(swedish);
			
			italian = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			italian.Name = italian.Title = "italian"; 
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
			ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
			IList<ILanguage> languageRoot = new List<ILanguage>(gateway.GetLanguages());

			Assert.That(languageRoot.Count, Is.EqualTo(3));
		}

		[Test]
		public void LanguageKey_IsAddedTo_SavedItem()
		{
			ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);

			engine.Persister.Save(englishSub);

			Assert.That(englishSub[LanguageGateway.LanguageKey], Is.EqualTo(englishSub.ID));
		}

		[Test]
		public void LanguageKey_IsNotAddedTo_ItemsOutsideA_LanguageRoot()
		{
			ContentItem page = CreateOneItem<Items.TranslatedPage>(0, "page", root);

			engine.Persister.Save(page);

			Assert.That(page[LanguageGateway.LanguageKey], Is.Null);
		}

		[Test]
		public void LanguageKey_IsReused_ForTranslations_OfAPage()
		{
			ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Persister.Save(englishSub);

			ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);

			engine.Resolve<IWebContext>().QueryString[LanguageGateway.LanguageKey] = englishSub.ID.ToString();
			engine.Persister.Save(swedishSub);
			engine.Resolve<IWebContext>().QueryString.Remove(LanguageGateway.LanguageKey);

			Assert.That(swedishSub[LanguageGateway.LanguageKey], Is.EqualTo(englishSub[LanguageGateway.LanguageKey]));
		}

		[Test]
		public void LanguageKey_IsReused_ForTranslations_OfAPage_AndLivesUntilNextScope()
		{
			using (engine.Persister)
			{
				ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
				engine.Persister.Save(englishSub);

				ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);

				engine.Resolve<IWebContext>().QueryString[LanguageGateway.LanguageKey] = englishSub.ID.ToString();
				engine.Persister.Save(swedishSub);
				engine.Resolve<IWebContext>().QueryString.Remove(LanguageGateway.LanguageKey);
			}

			using (engine.Persister)
			{
				ContentItem englishSub = engine.Persister.Get(english.ID).Children[0];
				ContentItem swedishSub = engine.Persister.Get(swedish.ID).Children[0];
				Assert.That(swedishSub[LanguageGateway.LanguageKey], Is.EqualTo(englishSub[LanguageGateway.LanguageKey]));
			}
		}

		[Test]
		public void CanRetrieve_Translations_OfTheSamePage()
		{
			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();

			ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Persister.Save(englishSub);

			ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
			swedishSub[LanguageGateway.LanguageKey] = englishSub.ID;
			engine.Persister.Save(swedishSub);

			IEnumerable<ContentItem> translations = lg.FindTranslations(englishSub);
			EnumerableAssert.Count(2, translations);
		}

		[Test]
		public void FindTranslations_OnLanguageRoot_FindsOtherLanguageRoots()
		{
			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();

			IEnumerable<ContentItem> translations = lg.FindTranslations(english);

			EnumerableAssert.Count(3, translations);
			EnumerableAssert.Contains(translations, english);
			EnumerableAssert.Contains(translations, swedish);
			EnumerableAssert.Contains(translations, italian);
		}

		[Test]
		public void FindTranslations_OnLanguageRoot_FindsOtherLanguageRoots_ButNotChildPages()
		{
			ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Persister.Save(englishSub);

			ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
			swedishSub[LanguageGateway.LanguageKey] = englishSub.ID;
			engine.Persister.Save(swedishSub);

			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();

			IEnumerable<ContentItem> translations = lg.FindTranslations(english);

			EnumerableAssert.Count(3, translations);
			EnumerableAssert.DoesntContain(translations, englishSub);
			EnumerableAssert.DoesntContain(translations, swedishSub);
		}

		[Test]
		public void FindTransaltion_OnNonTranslatedPages_YieldsEmptyCollection()
		{
			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
			IEnumerable<ContentItem> translations = lg.FindTranslations(root);
			EnumerableAssert.Count(0, translations);
		}

		[Test]
		public void GetsTranslationOptions_ForSingleItem()
		{
			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();

			ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Persister.Save(englishSub);

			ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
			swedishSub[LanguageGateway.LanguageKey] = englishSub.ID;
			engine.Persister.Save(swedishSub);

			IList<TranslationOption> options = new List<TranslationOption>(lg.GetTranslationOptions(englishSub, false));

			Assert.That(options.Count, Is.EqualTo(2));
			Assert.That(options[0].IsNew, Is.False);
			Assert.That(options[1].IsNew, Is.True);
		}

		[Test]
		public void GetsTranslationOptions_CanIncludeCurrent()
		{
			ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Persister.Save(englishSub);

			ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
			swedishSub[LanguageGateway.LanguageKey] = englishSub.ID;
			engine.Persister.Save(swedishSub);

			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
			IList<TranslationOption> options = new List<TranslationOption>(lg.GetTranslationOptions(englishSub, true));

			Assert.That(options.Count, Is.EqualTo(3));
		}

		[Test]
		public void GetTranslationOptions_OnNonTranslatedPage_DoesntThrowExceptions()
		{
			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
			IEnumerable<TranslationOption> options = lg.GetTranslationOptions(root, false);
			EnumerableAssert.Count(0, options);
		}

		[Test]
		public void TranslationsOfPage_AreMoved_AlongWithOriginalPage()
		{
			ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Persister.Save(english1);
			ContentItem english2 = CreateOneItem<Items.TranslatedPage>(0, "english2", english);
			engine.Persister.Save(english2);

			ContentItem swedish1 = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
			swedish1[LanguageGateway.LanguageKey] = english1.ID;
			engine.Persister.Save(swedish1);
			ContentItem swedish2 = CreateOneItem<Items.TranslatedPage>(0, "swedish2", swedish);
			swedish2[LanguageGateway.LanguageKey] = english2.ID;
			engine.Persister.Save(swedish2);

			// Swedish translation to the corresponding location
			engine.Persister.Move(english2, english1);

			Assert.That(swedish1.Children.Count, Is.EqualTo(1));
			Assert.That(swedish.Children.Count, Is.EqualTo(1));
			Assert.That(swedish1.Children[0], Is.EqualTo(swedish2));
		}

		[Test]
		public void TranslationsOfPage_AreIgnored_IfTheDestination_IsNotTranslated()
		{
			ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Persister.Save(english1);
			ContentItem english2 = CreateOneItem<Items.TranslatedPage>(0, "english2", english);
			engine.Persister.Save(english2);

			ContentItem swedish1 = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
			engine.Persister.Save(swedish1);
			ContentItem swedish2 = CreateOneItem<Items.TranslatedPage>(0, "swedish2", swedish);
			swedish2[LanguageGateway.LanguageKey] = english2.ID;
			engine.Persister.Save(swedish2);

			// swedish2 is not moved since swedish1 isn't translated
			engine.Persister.Move(english2, english1);

			Assert.That(swedish1.Children.Count, Is.EqualTo(0));
			Assert.That(swedish.Children.Count, Is.EqualTo(2));
		}

		[Test]
		public void TranslationsOfPage_AreDeleted_AlongWithOriginalTranslation()
		{
			ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Persister.Save(english1);
			
			ContentItem swedish1 = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
			swedish1[LanguageGateway.LanguageKey] = english1.ID;
			engine.Persister.Save(swedish1);

			engine.Persister.Delete(english1);

			Assert.That(swedish.Children.Count, Is.EqualTo(0));
			Assert.That(engine.Persister.Get(swedish1.ID), Is.Null);
		}

		[Test]
		public void NewTranslations_InheritsSortOrder_FromPreviousTranslation()
		{
			ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			english1.SortOrder = 123;
			engine.Persister.Save(english1);

			engine.Resolve<IWebContext>().QueryString[LanguageGateway.LanguageKey] = english1.ID.ToString();
			ContentItem swedish1 = engine.Definitions.CreateInstance<Items.TranslatedPage>(swedish);
			engine.Persister.Save(swedish1);
			engine.Resolve<IWebContext>().QueryString.Remove(LanguageGateway.LanguageKey);

			Assert.That(swedish1.SortOrder, Is.EqualTo(english1.SortOrder));
		}
	}
}
