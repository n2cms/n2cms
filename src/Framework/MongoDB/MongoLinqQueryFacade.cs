using System.Linq;
using MongoDB.Driver.Linq;
using N2.Engine;
using N2.Persistence.NH;

namespace N2.Persistence.MongoDB
{
    [Service(typeof(LinqQueryFacade), Configuration = "mongo", Replaces = typeof(LinqNHQueryFacade))]
    public class MongoLinqQueryFacade : LinqQueryFacade
    {
        private readonly MongoDatabaseProvider provider;

        public MongoLinqQueryFacade(MongoDatabaseProvider provider)
        {
            this.provider = provider;
        }

        public override IQueryable<T> Query<T>()
        {
            return provider.GetCollection<ContentItem>().AsQueryable<T>();
        }
    }
}
