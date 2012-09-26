using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Tests.Persistence;
using NUnit.Framework;
using N2.Edit.Versioning;
using Shouldly;
using N2.Persistence;

namespace N2.Tests.Edit.Versioning
{
    [TestFixture]
    public class ContentVersionRepositoryTests : DatabasePreparingBase
    {
        ContentVersionRepository repository;
		IPersister persister;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
			repository = TestSupport.CreateVersionRepository(ref persister, typeof(Items.NormalPage));
        }

        [Test]
        public void AddPublishedVersion_ASDraft()
        {
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
            persister.Save(page);

            var draft = repository.Save(page, "Hello");
            repository.Repository.Dispose();

            var savedDraft = repository.GetVersion(page);
            savedDraft.Published.ShouldBe(page.Published, TimeSpan.FromSeconds(1));
            savedDraft.PublishedBy.ShouldBe(page.SavedBy);
            savedDraft.Saved.ShouldBe(DateTime.Now, TimeSpan.FromSeconds(1));
            savedDraft.SavedBy.ShouldBe(draft.SavedBy);
            savedDraft.State.ShouldBe(page.State);
            savedDraft.VersionIndex.ShouldBe(page.VersionIndex);
        }

        [Test]
        public void AddDraftVersion_AsDraft()
        {
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
            persister.Save(page);

			var version = page.Clone(true);
			version.VersionOf = page;

            var draft = repository.Save(version, "Hello");
            repository.Repository.Dispose();

            var savedDraft = repository.GetVersion(page);
            savedDraft.Master.Value.ShouldBe(page);
        }

		[Test]
		public void DraftState_IsConsideredDraft()
		{
			var page = CreateOneItem<Items.NormalPage>(0, "page", null);
			persister.Save(page);

			var version = page.Clone(true);
			version.State = ContentState.Draft;
			version.VersionOf = page;

			var draft = repository.Save(version, "Hello");

			repository.HasDraft(page).ShouldBe(true);
			repository.GetDraft(page).ID.ShouldBe(draft.ID);
		}

		[TestCase(ContentState.Deleted)]
		[TestCase(ContentState.New)]
		[TestCase(ContentState.None)]
		[TestCase(ContentState.Published)]
		[TestCase(ContentState.Unpublished)]
		[TestCase(ContentState.Waiting)]
		public void OtherStates_IsNotDraft(ContentState state)
		{
			var page = CreateOneItem<Items.NormalPage>(0, "page", null);
			persister.Save(page);

			var version = page.Clone(true);
			version.State = state;
			version.VersionOf = page;

			var draft = repository.Save(version, "Hello");

			repository.HasDraft(page).ShouldBe(false);
			repository.GetDraft(page).ShouldBe(null);
		}

		[Test]
		public void MultipleDrafts_GreatestVersionIndexIsUsed()
		{
			var page = CreateOneItem<Items.NormalPage>(0, "page", null);
			persister.Save(page);

			var version = page.Clone(true);
			version.VersionIndex = page.VersionIndex + 1;
			version.State = ContentState.Draft;
			version.VersionOf = page;
			var draft1 = repository.Save(version, "Hello");

			var version2 = page.Clone(true);
			version2.VersionIndex = page.VersionIndex + 2;
			version2.State = ContentState.Draft;
			version2.VersionOf = page;
			var draft2 = repository.Save(version2, "Hello");

			repository.GetDraft(page).ID.ShouldBe(draft2.ID);
		}

		[Test]
		public void VersionIndex_IsKeptWhenSavingVersion()
		{
			var page = CreateOneItem<Items.NormalPage>(0, "page", null);
			persister.Save(page);

			var versionItem = page.Clone(true);
			versionItem.VersionIndex = page.VersionIndex + 1;
			versionItem.State = ContentState.Draft;
			versionItem.VersionOf = page;
			
			var version = repository.Save(versionItem, "Hello");

			repository.Repository.Dispose();
			var savedVersion = repository.GetVersion(page, versionItem.VersionIndex);

			savedVersion.VersionIndex.ShouldBe(versionItem.VersionIndex);
			savedVersion.Version.VersionIndex.ShouldBe(versionItem.VersionIndex);
		}
    }
}
