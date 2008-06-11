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
using N2.Tests.Globalization.Items;

namespace N2.Tests.Globalization
{
	public abstract class GlobalizationTests : PersistenceAwareBase
	{
		protected ContentItem root;
		protected LanguageRoot english;
		protected LanguageRoot swedish;
		protected LanguageRoot italian;

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

			CreatePageStructure();
		}

		protected abstract void CreatePageStructure();

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
			IList<ILanguage> languageRoot = new List<ILanguage>(gateway.GetAvailableLanguages());

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
		public void LanguageKey_IsReused_ForTranslations_OfAPage_AndLivesAcrossCalls()
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
				english = engine.Persister.Get<LanguageRoot>(english.ID);
				swedish = engine.Persister.Get<LanguageRoot>(swedish.ID);
				ContentItem englishSub = english.Children[english.Children.Count - 1];
				ContentItem swedishSub = swedish.Children[swedish.Children.Count - 1];
				Assert.That(swedishSub[LanguageGateway.LanguageKey], Is.EqualTo(englishSub[LanguageGateway.LanguageKey]));
			}
		}

		[Test]
		public void CanRetrieve_Translations_OfATranslatedPage()
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
		public void FindTranslations_OfLanguageRoot_FindsOtherLanguageRoots()
		{
			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();

			IEnumerable<ContentItem> translations = lg.FindTranslations(english);

			EnumerableAssert.Count(3, translations);
			EnumerableAssert.Contains(translations, english);
			EnumerableAssert.Contains(translations, swedish);
			EnumerableAssert.Contains(translations, italian);
		}

		[Test]
		public void FindTranslations_OfSwedishLanguageRoot_FindsAllLanguageRoots()
		{
			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
			IEnumerable<ContentItem> translations = lg.FindTranslations(swedish);

			EnumerableAssert.Count(3, translations);
		}

		[Test]
		public void FindTranslations_OfItalianLanguageRoot_FindsAllLanguageRoots()
		{
			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
			IEnumerable<ContentItem> translations = lg.FindTranslations(italian);

			EnumerableAssert.Count(3, translations);
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
        public void FindTranslations_DoesntYield_PagesWithoutAccess()
        {
            var englishSub = CreateOneItem<TranslatedPageWithAuthorization>(0, "english1", english);
            engine.Persister.Save(englishSub);

            var swedishSub = CreateOneItem<TranslatedPage>(0, "swedish1", swedish);
            swedishSub[LanguageGateway.LanguageKey] = englishSub.ID;
            engine.Persister.Save(swedishSub);

            englishSub.Authorize = false;
            var lg = engine.Resolve<ILanguageGateway>();
            EnumerableAssert.Count(1, lg.FindTranslations(swedishSub));
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

			IList<TranslateSpecification> options = new List<TranslateSpecification>(lg.GetEditTranslations(englishSub, false));

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
			IList<TranslateSpecification> options = new List<TranslateSpecification>(lg.GetEditTranslations(englishSub, true));

			Assert.That(options.Count, Is.EqualTo(3));
		}

		[Test]
		public void GetTranslationOptions_OnNonTranslatedPage_DoesntThrowExceptions()
		{
			ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
			IEnumerable<TranslateSpecification> options = lg.GetEditTranslations(root, false);
			EnumerableAssert.Count(0, options);
		}

		[Test]
		public void TranslationsOfPage_AreMoved_AlongWithOriginalPage()
		{
			int initialCount = swedish.Children.Count;
			
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
			Assert.That(swedish.Children.Count, Is.EqualTo(1 + initialCount));
			Assert.That(swedish1.Children[0], Is.EqualTo(swedish2));
		}

		[Test]
		public void TranslationsOfPage_AreIgnored_IfTheDestination_IsNotTranslated()
		{
			int initialCount = swedish.Children.Count;

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
			Assert.That(swedish.Children.Count, Is.EqualTo(2 + initialCount));
		}

		[Test]
		public void TranslationsOfPage_AreDeleted_AlongWithOriginalTranslation()
		{
			int initialCount = swedish.Children.Count;

			ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Persister.Save(english1);
			
			ContentItem swedish1 = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
			swedish1[LanguageGateway.LanguageKey] = english1.ID;
			engine.Persister.Save(swedish1);

			engine.Persister.Delete(english1);

			Assert.That(swedish.Children.Count, Is.EqualTo(0 + initialCount));
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

		[Test]
		public void LanguageKey_IsSetOnOriginalItem_WhenSavingOtherItem()
		{
			ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
			engine.Resolve<INHRepository<int, ContentItem>>().Save(englishSub); // bypass events

			ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);

			engine.Resolve<IWebContext>().QueryString[LanguageGateway.LanguageKey] = englishSub.ID.ToString();
			engine.Persister.Save(swedishSub);
			engine.Resolve<IWebContext>().QueryString.Remove(LanguageGateway.LanguageKey);

			Assert.That(englishSub[LanguageGateway.LanguageKey], Is.EqualTo(englishSub.ID));
			Assert.That(swedishSub[LanguageGateway.LanguageKey], Is.EqualTo(englishSub.ID));
		}

		[Test]
		public void MovingOutLanguageRoot_DoesNotAffect_OtherLanguageRoots()
		{
			TranslatedPage trash = CreateOneItem<TranslatedPage>(0, "trash", root);
			engine.Persister.Save(trash);

			engine.Persister.Move(swedish, trash);
			Assert.That(swedish.Parent, Is.EqualTo(trash)); 
			Assert.That(trash.Children.Count, Is.EqualTo(1));
			Assert.That(trash.Children[0], Is.EqualTo(swedish));
		}

		[Test]
		public void MovingLanguageRoot_DoesNotAffect_OtherLanguageRoots()
		{
			int initialCount = english.Children.Count;

			engine.Persister.Move(italian, english);
			Assert.That(italian.Parent, Is.EqualTo(english));
			Assert.That(english.Children.Count, Is.EqualTo(initialCount + 1));
		}

		[Test]
		public void DeletingLanguageRoot_DoesNotAffect_OtherLanguageRoots()
		{
			engine.Persister.Delete(italian);

			Assert.That(engine.Persister.Get(english.ID), Is.EqualTo(english));
			Assert.That(engine.Persister.Get(swedish.ID), Is.EqualTo(swedish));
		}

		[Test]
		public void LanguageRoot_WithEmptyLanguageCode_DoesntCode_LanguageKey()
		{
			english.LanguageCode = "";

			ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);

			engine.Persister.Save(englishSub);

			Assert.That(englishSub[LanguageGateway.LanguageKey], Is.Null);
		}
	}
}
