using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using N2.Tests;
using N2.Web;
using Newtonsoft.Json;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Linq;
using Shouldly;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace N2.Raven.Tests
{
	[TestFixture]
	public class SerializationTests : ItemPersistenceMockingBase
	{
		private ThreadContext context;

		public override void SetUp()
		{
			base.SetUp();
			context = new ThreadContext();
		}

		public override void TearDown()
		{
			context.RequestItems["RavenSession"] = null;

			base.TearDown();
		}

		[Test]
		public void Serialize_ShouldDeserialize_ToSameType()
		{
			var value = CreateOneItem<MyItem>(1, "Hello", null);

			var result = SerializeDeserialize(value);

			result.ShouldBeTypeOf<MyItem>();
		}

		[TestCase(true)]
		[TestCase(42)]
		[TestCase("World")]
		[TestCase(123.456)]
		public void Serialize_ShouldInclude_Details(object value)
		{
			var item = CreateOneItem<MyItem>(1, "Hello", null);
			item["Hello"] = value;

			var result = (MyItem)SerializeDeserialize(item);

			result["Hello"].ShouldBe(value);
		}

		[Test]
		public void Serialize_ShouldInclude_ObjectDetails()
		{
			var item = CreateOneItem<MyItem>(1, "Hello", null);
			item["Hello"] = new string[] { "Hello", "World" };

			var result = (MyItem)SerializeDeserialize(item);

			((string[])result["Hello"]).SequenceEqual(new string[] { "Hello", "World" });
		}

		[Test]
		public void Serialize_ShouldInclude_DetailCollections()
		{
			var item = CreateOneItem<MyItem>(1, "Hello", null);
			item.GetDetailCollection("Hello", true).AddRange(new object[] { true, "hello", 123, 432.234 });

			var result = (MyItem)SerializeDeserialize(item);

			result.GetDetailCollection("Hello", false).SequenceEqual(new object[] { true, "hello", 123, 432.234 });
		}

		[Test]
		public void Serialize_ShouldInclude_ReferencesTo_DirectChildren()
		{
			var root = CreateOneItem<MyItem>(1, "Hello", null);
			CreateOneItem<MyItem>(2, "Child1", root);
			CreateOneItem<MyItem>(3, "Child2", root);

			var result = (ContentItem)SerializeDeserialize(root);

			result.Children.First().ShouldBeTypeOf<MyItem>();
		}

		[Test]
		public void Serialize_ShouldInclude_ReferencesTo_Parent()
		{
			var root = CreateOneItem<MyItem>(1, "Hello", null);
			var child = CreateOneItem<MyItem>(2, "Child1", root);

			var result = (ContentItem)SerializeDeserialize(child);

			result.Parent.Title.ShouldBe(root.Title);
		}

		[Test]
		public void Serialize_ShouldInclude_ReferencesTo_GrandChildren()
		{
			var root = CreateOneItem<MyItem>(1, "Hello", null);
			var child = CreateOneItem<MyItem>(2, "Child1", root);
			CreateOneItem<MyItem>(3, "Child2", child);

			var result = (ContentItem)SerializeDeserialize(root);

			result.Children.First().Children.First().ShouldBeTypeOf<MyItem>();
		}

		[Test]
		public void Serialize_ShouldShouldIncludeChildren_AsReferences()
		{
			var root = CreateOneItem<MyItem>(1, "Hello", null);
			CreateOneItem<MyItem>(2, "Child1", root);
			CreateOneItem<MyItem>(3, "Child2", root);

			using (var sw = new StringWriter())
			using (var jtw = new JsonTextWriter(sw) { Indentation = 1, IndentChar = '\t', Formatting = Formatting.Indented })
			{
				var js = CreateSerializer();

				js.Serialize(jtw, root);

				var item = persister.Get(3);
				item.Title = "Child Three";
				persister.Save(item);

				var result = (ContentItem)js.Deserialize(new JsonTextReader(new StringReader(sw.ToString())));

				result.Children["child2"].Title.ShouldBe("Child Three");
			}
		}

		private object SerializeDeserialize(MyItem value)
		{
			using (var sw = new StringWriter())
			using (var jtw = new JsonTextWriter(sw) { Indentation = 1, IndentChar = '\t', Formatting = Formatting.Indented })
			{
				var js = CreateSerializer();

				js.Serialize(jtw, value);

				Debug.WriteLine(sw.ToString());

				return js.Deserialize(new JsonTextReader(new StringReader(sw.ToString())));
			}
		}

		private JsonSerializer CreateSerializer()
		{
			var rsf = new FakeStoreFactory(persister) { RunInMemory = true };
			var rcp = new RavenConnectionProvider(rsf, context);
			return rsf.CreateStore(rcp).Conventions.CreateSerializer();
		}

		class FakeStoreFactory : RavenStoreFactory
		{
			private Persistence.IPersister persister;

			public FakeStoreFactory(Persistence.IPersister persister)
				: base(new Configuration.DatabaseSection())
			{
				this.persister = persister;
			}

			public override IDocumentStore CreateStore(RavenConnectionProvider connections)
			{
				var session = new FakeDocumentSession(persister);
				var store = new FakeDocumentStore(session) { Url = "http://localhost:8080/" };
				Initialize(connections, store);
				return store;
			}

			class FakeDocumentStore : DocumentStore, IDocumentStore
			{
				private FakeDocumentSession session;

				public FakeDocumentStore(FakeDocumentSession session)
				{
					// TODO: Complete member initialization
					this.session = session;
				}

				public new IDocumentSession OpenSession()
				{
					return session;
				}
			}

			class FakeDocumentSession : IDocumentSession
			{
				private Persistence.IPersister persister;

				public FakeDocumentSession(Persistence.IPersister persister)
				{
					this.persister = persister;
				}
				public ISyncAdvancedSessionOperation Advanced
				{
					get { throw new NotImplementedException(); }
				}

				public void Delete<T>(T entity)
				{
					throw new NotImplementedException();
				}

				public ILoaderWithInclude<T> Include<T>(System.Linq.Expressions.Expression<Func<T, object>> path)
				{
					throw new NotImplementedException();
				}

				public ILoaderWithInclude<object> Include(string path)
				{
					throw new NotImplementedException();
				}

				public T Load<T>(ValueType id)
				{
					return new[] { persister.Get(Convert.ToInt32(id)) }.OfType<T>().FirstOrDefault();
				}

				public T[] Load<T>(IEnumerable<string> ids)
				{
					throw new NotImplementedException();
				}

				public T[] Load<T>(params string[] ids)
				{
					return ids.Select(id => persister.Get(int.Parse(id))).OfType<T>().ToArray();
				}

				public T Load<T>(string id)
				{
					throw new NotImplementedException();
				}

				public IRavenQueryable<T> Query<T, TIndexCreator>() where TIndexCreator : AbstractIndexCreationTask, new()
				{
					throw new NotImplementedException();
				}

				public IRavenQueryable<T> Query<T>()
				{
					return new RQ<T>(persister.Repository.Find().OfType<T>());
				}

				class RQ<T> : IRavenQueryable<T>
				{
					private IEnumerable<T> iEnumerable;

					public RQ(IEnumerable<T> iEnumerable)
					{
						// TODO: Complete member initialization
						this.iEnumerable = iEnumerable;
					}
					public IRavenQueryable<T> Customize(Action<IDocumentQueryCustomization> action)
					{
						throw new NotImplementedException();
					}

					public IRavenQueryable<T> Statistics(out RavenQueryStatistics stats)
					{
						throw new NotImplementedException();
					}

					public IEnumerator<T> GetEnumerator()
					{
						return iEnumerable.GetEnumerator();
					}

					IEnumerator IEnumerable.GetEnumerator()
					{
						return GetEnumerator();
					}

					public Type ElementType
					{
						get { return iEnumerable.AsQueryable().ElementType; }
					}

					public System.Linq.Expressions.Expression Expression
					{
						get { return iEnumerable.AsQueryable().Expression; }
					}

					public IQueryProvider Provider
					{
						get { return iEnumerable.AsQueryable().Provider; }
					}
				}

				public IRavenQueryable<T> Query<T>(string indexName)
				{
					throw new NotImplementedException();
				}

				public void SaveChanges()
				{
					throw new NotImplementedException();
				}

				public void Store(dynamic entity, string id)
				{
					throw new NotImplementedException();
				}

				public void Store(dynamic entity)
				{
					throw new NotImplementedException();
				}

				public void Store(object entity, Guid etag, string id)
				{
					throw new NotImplementedException();
				}

				public void Store(object entity, Guid etag)
				{
					throw new NotImplementedException();
				}

				public void Dispose()
				{
					throw new NotImplementedException();
				}
			}

		}
	}
}
