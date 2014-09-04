using System;
using System.Collections.Generic;
using System.Linq;
using N2.Edit.Trash;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Persistence;
using N2.Security;
using N2.Tests.Globalization.Items;
using N2.Tests.Persistence;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Globalization
{
    public abstract class GlobalizationTests : DatabasePreparingBase
    {
        protected ContentItem root;
        protected LanguageRoot english;
        protected LanguageRoot swedish;
        protected LanguageRoot italian;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CreatePageStructure();
        }

        protected abstract void CreatePageStructure();

        [Test]
        public void CanList_LanguageRoots()
        {
            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            IList<ILanguage> languageRoot = new List<ILanguage>(gateway.GetAvailableLanguages());

            Assert.That(languageRoot.Count, Is.EqualTo(3));
        }

        [Test]
        public void LanguageKey_IsNotAddedTo_LonelySavedItem()
        {
            ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);

            engine.Persister.Save(englishSub);

            Assert.That(englishSub.TranslationKey, Is.Null);
        }

        [Test]
        public void LanguageKey_IsNotAddedTo_ItemsOutside_OfLanguageRoots()
        {
            ContentItem page = CreateOneItem<Items.TranslatedPage>(0, "page", root);

            engine.Persister.Save(page);

            Assert.That(page.TranslationKey, Is.Null);
        }

        [Test]
        public void LanguageKey_IsReused_ForTranslations_OfAPage()
        {
            ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            engine.Persister.Save(englishSub);

            ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);

            using (new LanguageKeyScope(engine, englishSub.ID))
            {
                engine.Persister.Save(swedishSub);
            }

            Assert.That(swedishSub.TranslationKey, Is.EqualTo(englishSub.TranslationKey));
        }

        [Test]
        public void LanguageKey_IsReused_ForTranslations_OfAPage_AndLivesAcrossCalls()
        {
            using (engine.Persister)
            {
                ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
                engine.Persister.Save(englishSub);

                ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);

                using (new LanguageKeyScope(engine, englishSub.ID))
                {
                    engine.Persister.Save(swedishSub);
                }
            }

            using (engine.Persister)
            {
                english = engine.Persister.Get<LanguageRoot>(english.ID);
                swedish = engine.Persister.Get<LanguageRoot>(swedish.ID);
                ContentItem englishSub = english.Children[english.Children.Count - 1];
                ContentItem swedishSub = swedish.Children[swedish.Children.Count - 1];
                Assert.That(swedishSub.TranslationKey, Is.EqualTo(englishSub.TranslationKey));
            }
        }

        [Test]
        public void CanRetrieve_Translations_OfATranslatedPage()
        {
            ILanguageGateway lg = engine.Resolve<ILanguageGateway>();

            ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            engine.Persister.Save(englishSub);

            ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
            using (new LanguageKeyScope(engine, englishSub.ID))
            {
                engine.Persister.Save(swedishSub);
            }
            var translations = lg.FindTranslations(englishSub);
            Assert.That(translations.Count(), Is.EqualTo(2));
        }

        [Test]
        public void FindTranslations_OfLanguageRoot_FindsOtherLanguageRoots()
        {
            ILanguageGateway lg = engine.Resolve<ILanguageGateway>();

            var translations = lg.FindTranslations(english);

            Assert.That(translations.Count(), Is.EqualTo(3));
            Assert.That(translations.Contains(english));
            Assert.That(translations.Contains(swedish));
            Assert.That(translations.Contains(italian));
        }

        [Test]
        public void FindTranslations_OfSwedishLanguageRoot_FindsAllLanguageRoots()
        {
            ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
            var translations = lg.FindTranslations(swedish);

            Assert.That(translations.Count(), Is.EqualTo(3));
        }

        [Test]
        public void FindTranslations_OfItalianLanguageRoot_FindsAllLanguageRoots()
        {
            ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
            var translations = lg.FindTranslations(italian);

            Assert.That(translations.Count(), Is.EqualTo(3));
        }

        [Test]
        public void FindTranslations_OnLanguageRoot_FindsOtherLanguageRoots_ButNotChildPages()
        {
            ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            engine.Persister.Save(englishSub);

            ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
            swedishSub.TranslationKey = englishSub.ID;
            engine.Persister.Save(swedishSub);

            ILanguageGateway lg = engine.Resolve<ILanguageGateway>();

            var translations = lg.FindTranslations(english);

            Assert.That(translations.Count(), Is.EqualTo(3));
            Assert.That(translations.Contains(englishSub), Is.False);
            Assert.That(translations.Contains(swedishSub), Is.False);
        }

        [Test]
        public void FindTranslations_DoesntYield_PagesWithoutAccess()
        {
            var englishSub = CreateOneItem<TranslatedPageWithAuthorization>(0, "english1", english);
            engine.Persister.Save(englishSub);

            var swedishSub = CreateOneItem<TranslatedPage>(0, "swedish1", swedish);
            swedishSub.TranslationKey = englishSub.ID;
            engine.Persister.Save(swedishSub);

            englishSub.Authorize = false;
            var lg = engine.Resolve<ILanguageGateway>();
            var translations = lg.FindTranslations(swedishSub);
            Assert.That(translations.Count(), Is.EqualTo(1));
        }

        [Test]
        public void FindTransaltion_OnNonTranslatedPages_YieldsEmptyCollection()
        {
            ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
            var translations = lg.FindTranslations(root);
            Assert.That(translations.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetsTranslationOptions_ForSingleItem()
        {
            ILanguageGateway lg = engine.Resolve<ILanguageGateway>();

            ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            engine.Persister.Save(englishSub);

            ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
            using (new LanguageKeyScope(engine, englishSub.ID))
            {
                engine.Persister.Save(swedishSub);
            }
            IList<TranslateSpecification> options = new List<TranslateSpecification>(lg.GetEditTranslations(englishSub, false, false));

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
            swedishSub.TranslationKey = englishSub.ID;
            engine.Persister.Save(swedishSub);

            ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
            IList<TranslateSpecification> options = new List<TranslateSpecification>(lg.GetEditTranslations(englishSub, true, false));

            Assert.That(options.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetTranslationOptions_OnNonTranslatedPage_DoesntThrowExceptions()
        {
            ILanguageGateway lg = engine.Resolve<ILanguageGateway>();
            IEnumerable<TranslateSpecification> options = lg.GetEditTranslations(root, false, false);
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
            using (new LanguageKeyScope(engine, english1.ID))
            {
                engine.Persister.Save(swedish1);
            }
            ContentItem swedish2 = CreateOneItem<Items.TranslatedPage>(0, "swedish2", swedish);
            using (new LanguageKeyScope(engine, english2.ID))
            {
                engine.Persister.Save(swedish2);
            }

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
            using (new LanguageKeyScope(engine, english2.ID))
            {
                engine.Persister.Save(swedish2);
            }

            // swedish2 is not moved since swedish1 isn't translated
            engine.Persister.Move(english2, english1);

            Assert.That(swedish1.Children.Count, Is.EqualTo(0));
            Assert.That(swedish.Children.Count, Is.EqualTo(2 + initialCount));
        }

        [Test]
        public void TranslationsOfPage_IsNotDeleted_AlongWithOriginalTranslation()
        {
            int initialCount = swedish.Children.Count;

            ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            engine.Persister.Save(english1);
            
            ContentItem swedish1 = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
            using (new LanguageKeyScope(engine, english1.ID))
            {
                engine.Persister.Save(swedish1);
            }

            using (engine.SecurityManager.Disable())
            {
                engine.Persister.Delete(english1);
            }
            Assert.That(swedish.Children.Count, Is.EqualTo(1 + initialCount));
            Assert.That(engine.Persister.Get(swedish1.ID), Is.EqualTo(swedish1));
        }

        [Test]
        public void NewTranslations_InheritsSortOrder_FromPreviousTranslation()
        {
            ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            english1.SortOrder = 123;
            engine.Persister.Save(english1);

            using (new LanguageKeyScope(engine, english1.ID))
            {
                ContentItem swedish1 = engine.Resolve<ContentActivator>().CreateInstance<Items.TranslatedPage>(swedish);
                engine.Persister.Save(swedish1);
                Assert.That(swedish1.SortOrder, Is.EqualTo(english1.SortOrder));
            }
        }

        [Test]
        public void LanguageKey_IsSetOnOriginalItem_WhenSavingOtherItem()
        {
            ContentItem englishSub = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            engine.Persister.Repository.SaveOrUpdate(englishSub); // bypass events

            ContentItem swedishSub = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
            using (new LanguageKeyScope(engine, englishSub.ID))
            {
                engine.Persister.Save(swedishSub);
            }

            Assert.That(englishSub.TranslationKey, Is.EqualTo(englishSub.ID));
            Assert.That(swedishSub.TranslationKey, Is.EqualTo(englishSub.ID));
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

            Assert.That(englishSub.TranslationKey, Is.Null);
        }

        [Test]
        public void CanAssociateItems()
        {
            ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            engine.Persister.Save(english1);

            ContentItem swedish1 = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
            engine.Persister.Save(swedish1);

            ContentItem italian1 = CreateOneItem<Items.TranslatedPage>(0, "italian1", italian);
            engine.Persister.Save(italian1);

            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            gateway.Associate(new ContentItem[] { english1, swedish1, italian1 });

            var translations = gateway.FindTranslations(english1);
            EnumerableAssert.Count(3, translations);
            EnumerableAssert.Contains(translations, english1);
            EnumerableAssert.Contains(translations, swedish1);
            EnumerableAssert.Contains(translations, italian1);
        }

        [Test]
        public void CanChangeAssociation()
        {
            ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            engine.Persister.Save(english1);
            
            var scope = new LanguageKeyScope(engine, english1.ID);
            ContentItem swedish1 = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
            engine.Persister.Save(swedish1);

            ContentItem italian1 = CreateOneItem<Items.TranslatedPage>(0, "italian1", italian);
            engine.Persister.Save(italian1);
            scope.Dispose();

            ContentItem italian2 = CreateOneItem<Items.TranslatedPage>(0, "italian2", italian);
            engine.Persister.Save(italian2);

            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            gateway.Associate(new ContentItem[] { english1, italian2 });
           
            var translations = gateway.FindTranslations(english1);
            Assert.That(translations.Count(), Is.EqualTo(3));
            Assert.That(translations.Contains(english1));
            Assert.That(translations.Contains(swedish1));
            Assert.That(translations.Contains(italian2));
        }

        [Test]
        public void DoesntDeassociate_UnrelatedItems()
        {
            ContentItem english1 = CreateOneItem<Items.TranslatedPage>(0, "english1", english);
            engine.Persister.Save(english1);

            var scope = new LanguageKeyScope(engine, english1.ID);
            ContentItem swedish1 = CreateOneItem<Items.TranslatedPage>(0, "swedish1", swedish);
            engine.Persister.Save(swedish1);

            ContentItem italian1 = CreateOneItem<Items.TranslatedPage>(0, "italian1", italian);
            engine.Persister.Save(italian1);
            scope.Dispose();

            ContentItem italian2 = CreateOneItem<Items.TranslatedPage>(0, "italian2", italian);
            engine.Persister.Save(italian2);

            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            gateway.Associate(new ContentItem[] { swedish1, italian2 });

            var translations = gateway.FindTranslations(english1);
            Assert.That(translations.Count(), Is.EqualTo(3));
            Assert.That(translations.Contains(english1));
            Assert.That(translations.Contains(swedish1));
            Assert.That(translations.Contains(italian2));
        }

        [Test]
        public void GetLanguage_DoesntGetLanguageRoots_WithNoLanguageCode()
        {
            english.LanguageCode = "";
            engine.Persister.Save(english);

            ContentItem page = CreateOneItem<Items.TranslatedPage>(0, "page", english);
            engine.Persister.Save(page);

            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            Assert.That(gateway.GetLanguage(page), Is.Null);
        }

        [Test]
        public void AvailableLanguages_AreRefreshed_WhenNewLanguageNode_IsAdded()
        {
            LanguageRoot dutch = new LanguageRoot();
            dutch.LanguageCode = "nl-NL";
            dutch.AddTo(root);
            engine.Persister.Save(dutch);

            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            Assert.That(gateway.GetAvailableLanguages().Count(), Is.EqualTo(4));
        }

        [Test]
        public void Translations_AreNotCarried_WhenCopying()
        {
            ContentItem english1 = CreateOneItem<TranslatedPage>(0, "english1", english);
            engine.Persister.Save(english1);

            ContentItem swedish1 = CreateOneItem<TranslatedPage>(0, "swedish1", swedish);
            engine.Persister.Save(swedish1);

            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            gateway.Associate(new[] { english1, swedish1 });

            ContentItem english1copy = engine.Persister.Copy(english1, english1);

            Assert.That(!gateway.FindTranslations(swedish1).Contains(english1copy));
            Assert.That(english1copy.TranslationKey, Is.Null, "Expected language association to be cleared from copy.");
            Assert.That(gateway.FindTranslations(swedish1).Count(), Is.EqualTo(2));
        }

        [Test]
        public void Translations_AreNotCarried_WhenMoving_ToTrash()
        {
            ContentItem english1 = CreateOneItem<TranslatedPage>(0, "english1", english);
            engine.Persister.Save(english1);

            ContentItem swedish1 = CreateOneItem<TranslatedPage>(0, "swedish1", swedish);
            engine.Persister.Save(swedish1);

            ContentItem trash = CreateOneItem<Trash>(0, "trash", root);
            engine.Persister.Save(trash);

            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            gateway.Associate(new[] { english1, swedish1 });

            using (engine.SecurityManager.Disable())
            {
                engine.Persister.Delete(english1);
            }
            Assert.That(!gateway.FindTranslations(swedish1).Contains(english1));
            Assert.That(english1.TranslationKey, Is.Null, "Expected language association to be cleared from copy.");
            Assert.That(gateway.FindTranslations(swedish1).Count(), Is.EqualTo(1));
        }

        class Trash : ContentItem, ITrashCan
        {
            #region ITrashCan Members

            public bool Enabled
            {
                get { return true; }
            }

            public TrashPurgeInterval PurgeInterval
            {
                get { return TrashPurgeInterval.Monthly; }
            }

            public bool AsyncTrashPurging
            {
                get { throw new NotImplementedException(); }
            }
            #endregion
        }


        private class LanguageKeyScope : IDisposable
        {
            ThreadContext context;
            public LanguageKeyScope(IEngine engine, int key)
            {
                context = (ThreadContext)engine.Resolve<IWebContext>();
                context.Url = context.Url.SetQueryParameter(LanguageGateway.TranslationKey, key.ToString());
            }

            #region IDisposable Members

            public void Dispose()
            {
                context.Url = context.Url.SetQueryParameter(LanguageGateway.TranslationKey, null);
            }

            #endregion
        }
    }
}
