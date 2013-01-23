using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;

namespace N2.Persistence.MongoDB
{
    public class ContentItemRepository : MongoDbRepository<ContentItem>, IContentItemRepository
    {
        public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
        {
            throw new NotImplementedException();
        }

        public int RemoveReferencesToRecursive(ContentItem target)
        {
            throw new NotImplementedException();
        }

        public ICollection<ContentItem> FindAll()
        {
            return GetCollection().AsQueryable().ToList();
        }

        public void DropDatabase()
        {
            Database.Drop();
        }
    }
}
