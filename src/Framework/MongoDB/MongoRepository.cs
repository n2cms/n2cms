#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using N2.Engine;
using _MongoDB = MongoDB;
using N2.Details;

namespace N2.Persistence.MongoDB
{
    [Service(typeof(IRepository<>), Key = "n2.repository.generic")]//, Replaces = typeof(NH.NHRepository<>))]
    public class MongoDbRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public readonly MongoDatabase Database;

        static MongoDbRepository()
        {
            var myConventions = new ConventionProfile();
            myConventions.SetIgnoreIfNullConvention(new AlwaysIgnoreIfNullConvention());
            BsonClassMap.RegisterConventions(myConventions, t => true);

			BsonClassMap.RegisterClassMap<ContentDetail>(cm =>
			{
				cm.AutoMap();
			});
            BsonClassMap.RegisterClassMap<ContentItem>(cm =>
                                                           {
                                                               cm.AutoMap();
                                                               cm.MapIdProperty(ci => ci.ID).SetIdGenerator(new IntIdGenerator());
															   cm.UnmapProperty(ci => ci.Children);
															   cm.UnmapProperty(ci => ci.Details);
															   cm.UnmapProperty(ci => ci.DetailCollections);
                                                           });
        }

        public MongoDbRepository(Configuration.ConfigurationManagerWrapper config)
        {
            //var connectionString = "mongodb://localhost/?safe=true"; //this will eventually come from config
			var connectionString = config.GetConnectionString();
			var client = new MongoClient(connectionString);
			var server = client.GetServer();
            Database = server.GetDatabase("n2cms");
        }

        public TEntity Load(object id)
        {
            throw new NotImplementedException();
        }

        public void Save(TEntity entity)
        {
            var col = Database.GetCollection<TEntity>(typeof(TEntity).Name);
            col.Save(entity);
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity Get(object id)
        {
            var col = GetCollection();
            var result = col.FindOne(Query.EQ("_id", (int)id));
            return result;
        }

        protected MongoCollection<TEntity> GetCollection()
        {
            return Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public T Get<T>(object id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Find(string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Find(IParameter parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            //todo: sort this! it sucks
            var idValue = (int)entity.GetType().GetProperty("ID").GetValue(entity, null);
            GetCollection().Remove(Query.EQ("_id", idValue));
        }

        public void SaveOrUpdate(TEntity entity)
        {
            //todo: sort this! it sucks
            var idValue = (int)entity.GetType().GetProperty("ID").GetValue(entity, null);

            var update = _MongoDB.Driver.Builders.Update.Replace(entity);

            var result = GetCollection().Update(
                    Query.EQ("_id", idValue),
                    update,
                    UpdateFlags.Upsert,
                    SafeMode.True);
        }

        public bool Exists()
        {
            throw new NotImplementedException();
        }

        public long Count()
        {
            throw new NotImplementedException();
        }

        public long Count(IParameter parameters)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            //not required for mongodb
        }

        public ITransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public ITransaction GetTransaction()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
