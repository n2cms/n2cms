using NUnit.Framework;

namespace N2.Tests.Persistence
{
    public abstract class DatabasePreparingBase : PersistenceAwareBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            base.CreateDatabaseSchema();
        }

        protected Definitions.PersistableItem CreateAndSaveItem(string name, string title, ContentItem parent)
        {
            Definitions.PersistableItem item = CreateOneItem<Definitions.PersistableItem>(0, "nada", parent);
            item.Title = title;
            item.Name = name;
            engine.Persister.Save(item);

            return item;
        }

        protected Definitions.PersistableItem CreateRoot(string name, string title)
        {
            Definitions.PersistableItem item = CreateAndSaveItem(name, title, null);
            return item;
        }
    }
}
