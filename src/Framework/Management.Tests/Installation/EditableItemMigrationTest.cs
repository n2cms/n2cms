using N2.Management.Installation;
using N2.Persistence;
using N2.Tests;
using N2.Tests.Edit.Items;
using N2.Tests.Persistence;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Tests.Upgrade
{
    [TestFixture]
    public class EditableItemMigrationTest : DatabasePreparingBase
    {
        IPersister persister;
        private NormalPage page;
        private EditableItemMigration worker;
        private Definitions.IDefinitionManager definitions;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            //definitions = TestSupport.SetupDefinitions(typeof(NormalPage), typeof(NormalItem));
            //persister = TestSupport.SetupFakePersister();
            definitions = engine.Definitions;
            persister = engine.Persister;
            
            page = CreateOneItem<NormalPage>(0, "root", null);
            persister.Save(page);

            worker = new EditableItemMigration(definitions, persister.Repository);
        }

        [Test]
        public void ReferencedItem_IsRenamed_AndDetailIsRemoved()
        {
            var part = CreateOneItem<NormalItem>(0, "part", page);
            persister.Save(part);

            Details.ContentDetail.New("EditableItem", part).AddTo(page);
            persister.Save(page);

            persister.Dispose();

            var result = worker.Migrate(new Edit.Installation.DatabaseStatus());
            result.UpdatedItems.ShouldBe(1);

            persister.Dispose();

            part = persister.Get<NormalItem>(part.ID);
            part.Name.ShouldBe("EditableItem");
            page = persister.Get<NormalPage>(page.ID);
            page.Details["EditableItem"].ShouldBe(null);
            page.EditableItem.ShouldBe(part);
        }
    }
}
