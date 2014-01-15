using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using N2.Engine;
using MongoDBUpdate = MongoDB.Driver.Builders.Update;
using N2.Details;
using N2.Security;
using N2.Edit.Versioning;
using MongoDB.Bson;
using System.Linq;
using System.Collections;

namespace N2.Persistence.MongoDB
{
    [Service]
    [Service(typeof(IRepository<>),
        Configuration = "mongo",
        Replaces = typeof(NH.NHRepository<>))]
    public class MongoDbRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private MongoDatabaseProvider provider;
        private System.Reflection.PropertyInfo idGetter;

        public MongoDatabaseProvider Provider
        {
            get { return provider; }
        }

        protected MongoCollection<TEntity> Collection
        {
            get { return provider.GetCollection<TEntity>(); }
        }

        public MongoDbRepository(MongoDatabaseProvider provider)
        {
            this.provider = provider;
            idGetter = typeof(TEntity).GetProperty("ID");
        }

        public TEntity Load(object id)
        {
            return Get(id);
        }

        public void Save(TEntity entity)
        {
            Collection.Save(entity);
        }

        public void Update(TEntity entity)
        {
            SaveOrUpdate(entity);
        }

        public TEntity Get(object id)
        {
            var result = provider.IdentityMap.GetOrCreate((int)id, (i) => Collection.FindOne(Query.EQ("_id", i)));
            return result;
        }

        public IEnumerable<TEntity> Find(string propertyName, object value)
        {
            return Find(Parameter.Equal(propertyName, value));
        }

        public IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll)
        {
            if (propertyValuesToMatchAll.Length > 1)
                return Find(new ParameterCollection(propertyValuesToMatchAll));
            else if (propertyValuesToMatchAll.Length == 1)
                return Find((IParameter)propertyValuesToMatchAll[0]);
            else
                return Collection.FindAll();
        }

        public IEnumerable<TEntity> Find(IParameter parameters)
        {
            var pc = parameters as ParameterCollection;
            if (pc != null)
            {
                var cursor = pc.Count == 0
                    ? Collection.FindAll()
                    : Collection.Find(parameters.CreateQuery());

                if (pc.Range != null && pc.Range.Skip > 0)
                    cursor = cursor.SetSkip(pc.Range.Skip);
                if (pc.Range != null && pc.Range.Take > 0)
                    cursor = cursor.SetLimit(pc.Range.Take);
                if (pc.Order != null && pc.Order.HasValue)
                    cursor = cursor.SetSortOrder(pc.Order.Descending
                        ? SortBy.Descending(pc.Order.Property.TranslateProperty())
                        : SortBy.Ascending(pc.Order.Property.TranslateProperty()));
                return cursor;
            }
            else
                return Collection.Find(parameters.CreateQuery());
        }

        public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
        {
            //var map = "function() { emit(this._id, { " + string.Join(", ", properties.Select(p => p == "ID" ? "_id" : p).Select(p => "'" + p + "': this['" + p + "']")) +  " }); }";
            //var reduce = "function(key, values) {  db.result.save(values[0]); return null; }";
            //var finalize = "function(key, value) { db.collection_2.insert(value); }";
            //var result = Collection.MapReduce(parameters.CreateQuery(), map, reduce, new MapReduceOptionsBuilder().SetFinalize(finalize));
            return Find(parameters).Select(e => properties.ToDictionary(p => p, p => Utility.GetProperty(e, p)));
        }

        public void Delete(TEntity entity)
        {
            int idValue = GetEntityId(entity);
            Collection.Remove(Query.EQ("_id", idValue));
            Provider.IdentityMap.Remove<TEntity>(idValue);
        }

        protected virtual int GetEntityId(TEntity entity)
        {
            return (int)idGetter.GetValue(entity, null);
        }

        public void SaveOrUpdate(TEntity entity)
        {
            var idValue = GetEntityId(entity);

            if (idValue != 0)
            {
                var update = MongoDBUpdate.Replace(entity);
                var result = Collection.Update(
                    Query.EQ("_id", idValue),
                    update,
                    UpdateFlags.Upsert,
                    WriteConcern.Acknowledged);
            }
            else
            {
                Save(entity);
            }
        }

        public bool Exists()
        {
            return Count() > 0;
        }

        public long Count()
        {
            return Collection.Count();
        }

        public long Count(IParameter parameters)
        {
            return Collection.Count(parameters.CreateQuery());
        }

        public void Flush()
        {
            //not required for mongodb
        }

        class NullTransaction : ITransaction
        {
            public void Commit()
            {
                if (Committed != null)
                    Committed(this, new EventArgs());
            }

            public void Rollback()
            {
                if (Rollbacked != null)
                    Rollbacked(this, new EventArgs());
            }

            public event EventHandler Committed;

            public event EventHandler Rollbacked;

            public event EventHandler Disposed;

            public void Dispose()
            {
                if (Disposed != null)
                    Disposed(this, new EventArgs());
            }
        }

        public ITransaction BeginTransaction()
        {
            return new NullTransaction();
        }

        public ITransaction GetTransaction()
        {
            return new NullTransaction();
        }

        public void Dispose()
        {
            Provider.Dispose();
        }
    }

    internal static class MongoRepositoryExtensions
    {
        public static IMongoQuery CreateQuery(this IParameter parameter)
        {
            if (parameter is ParameterCollection)
            {
                var pc = parameter as ParameterCollection;
                switch (pc.Operator)
                {
                    case Operator.And:
                        return Query.And(pc.Select(p => p.CreateQuery()));
                    case Operator.Or:
                        return Query.Or(pc.Select(p => p.CreateQuery()));
                    case Operator.None:
                    default:
                        throw new NotSupportedException();
                }
            }
            else if (parameter is Parameter)
            {
                var p = (Parameter)parameter;

                if (p.IsDetail)
                {
                    var valueExpression = GetValueExpression(p.Comparison.HasFlag(Comparison.In)
                        ? ContentDetail.GetAssociatedEnumerablePropertyName(p.Value as IEnumerable)
                        : ContentDetail.GetAssociatedPropertyName(p.Value), p.Comparison, p.Value);
                    var detailExpression = (p.Name == null)
                        ? valueExpression
                        : Query.And(
                            Query.EQ("Name", p.Name),
                            valueExpression);

                    return Query.Or(
                        Query.ElemMatch("Details", detailExpression),
                        Query.ElemMatch("DetailCollections.Details", detailExpression));
                }

                p.Name = p.Name.TranslateProperty();
                
                return GetValueExpression(p.Name, p.Comparison, p.Value);
            }
            throw new NotSupportedException();
        }

        internal static string TranslateProperty(this string propertyName)
        {
            if (propertyName == "ID")
                return "_id";
            else if (propertyName == "class")
                return "_t";
            return propertyName;
        }

        private static IMongoQuery GetValueExpression(string name, Comparison comparison, object value)
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return Query.EQ(name, CreateValue(value));
                case Comparison.GreaterOrEqual:
                    return Query.GTE(name, CreateValue(value));
                case Comparison.GreaterThan:
                    return Query.GT(name, CreateValue(value));
                case Comparison.LessOrEqual:
                    return Query.LTE(name, CreateValue(value));
                case Comparison.LessThan:
                    return Query.LT(name, CreateValue(value));
                case Comparison.Like:
                    return Query.Matches(name, value.ToString().Replace("%", ".*").Replace("/", "\\/"));
                case Comparison.NotEqual:
                    return Query.Not(Query.EQ(name, CreateValue(value)));
                case Comparison.NotLike:
                    return Query.Not(Query.Matches(name, value.ToString().Replace("%", ".*")));
                case Comparison.NotNull:
                    return Query.Exists(name);
                case Comparison.Null:
                    return Query.NotExists(name);
                case Comparison.In:
                    return Query.In(name, ((IEnumerable)value).OfType<object>().Select(v => CreateValue(v)));
                case Comparison.NotIn:
                    return Query.Not(Query.In(name, ((IEnumerable)value).OfType<object>().Select(v => CreateValue(v))));
                default:
                    throw new NotSupportedException();
            }
        }

        private static BsonValue CreateValue(object value)
        {
            if (value is ContentItem)
                return BsonValue.Create(((ContentItem)value).ID);
            return BsonValue.Create(value);
        }
    }
}
