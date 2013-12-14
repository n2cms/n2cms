using System;
using N2.Configuration;
using N2.Edit;
using N2.Tests.Edit.Items;
using NUnit.Framework;
using N2.Edit.Versioning;

namespace N2.Tests.Edit
{
    public abstract class EditUrlManagerTests : TypeFindingBase
    {
        protected EditUrlManager editUrlManager;
        protected ComplexContainersItem item;
        protected ComplexContainersItem version;
        protected ComplexContainersItem root;

        protected override Type[] GetTypes()
        {
            return new[]
                    {
                        typeof (ComplexContainersItem),
                        typeof (ItemWithRequiredProperty),
                        typeof (ItemWithModification),
                        typeof (NotVersionableItem),
                        typeof (LegacyNotVersionableItem),
                        typeof (ItemWithSecuredContainer)
                    };
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            editUrlManager = new EditUrlManager(null, new EditSection());
            root = CreateOneItem<ComplexContainersItem>(0, "root", null);
            item = CreateOneItem<ComplexContainersItem>(0, "item", root);
            version = new ComplexContainersItem { Name = "version", Title = "version", VersionOf = item, VersionIndex = 2 };
            version.SetVersionKey("VERSKEY");
        }
    }
}
