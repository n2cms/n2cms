using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Tests.Persistence;
using NUnit.Framework;
using N2.Edit.Versioning;
using Shouldly;

namespace N2.Tests.Edit.Versioning
{
    [TestFixture]
    public class ContentVersionRepositoryTests : DatabasePreparingBase
    {
        private ContentVersionRepository repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            repository = engine.Resolve<ContentVersionRepository>();
        }

        [Test]
        public void AddPublishedVersion_ASDraft()
        {
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
            engine.Persister.Save(page);

            var draft = repository.CreateDraft(page, CreatePrincipal("Hello"));
            repository.Repository.Dispose();

            var savedDraft = repository.GetDraft(page);
            savedDraft.AssociatedVersion.ID.ShouldBe(draft.AssociatedVersion.ID);
            savedDraft.ChangesJson.ShouldBe(draft.ChangesJson);
            savedDraft.IsDraft.ShouldBe(draft.IsDraft);
            savedDraft.IsPublishedVersion.ShouldBe(draft.IsPublishedVersion);
            savedDraft.MasterVersion.ID.ShouldBe(draft.MasterVersion.ID);
            savedDraft.Published.ShouldBe(draft.Published, TimeSpan.FromSeconds(1));
            savedDraft.PublishedBy.ShouldBe(draft.PublishedBy);
            savedDraft.Saved.ShouldBe(draft.Saved, TimeSpan.FromSeconds(1));
            savedDraft.SavedBy.ShouldBe(draft.SavedBy);
            savedDraft.State.ShouldBe(draft.State);
            savedDraft.VersionIndex.ShouldBe(draft.VersionIndex);
        }

        [Test]
        public void AddDraftVersion_AsDraft()
        {
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
            engine.Persister.Save(page);

            var version = CreateOneItem<Items.NormalPage>(0, "page version", null);
            version.VersionOf = page;
            engine.Persister.Save(version);

            var draft = repository.CreateDraft(version, CreatePrincipal("Hello"));
            repository.Repository.Dispose();

            var savedDraft = repository.GetDraft(page);
            savedDraft.MasterVersion.Value.ShouldBe(page);
            savedDraft.AssociatedVersion.Value.ShouldBe(version);
        }
    }
}
