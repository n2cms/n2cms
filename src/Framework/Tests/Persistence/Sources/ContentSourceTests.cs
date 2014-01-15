using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence.Sources;
using N2.Persistence;

namespace N2.Tests.Persistence.Sources
{
    [TestFixture]
    public class ContentSourceTests
    {
        [Test]
        public void DatabaseSource_IsOrderedLast()
        {
            ContentSource cs = new ContentSource(new Fakes.FakeSecurityManager(), new SourceBase[] { new DatabaseSource(null, null), new ActiveContentSource() });

            Assert.That(cs.Sources.Last(), Is.TypeOf<DatabaseSource>());
        }

        [Test]
        public void DatabaseSource_IsOrdered_AfterInterfaceSource()
        {
            ContentSource cs = new ContentSource(new Fakes.FakeSecurityManager(), new SourceBase[] { 
                new DatabaseSource(null, null), 
                new TestSource()
            });

            Assert.That(cs.Sources.Last(), Is.TypeOf<DatabaseSource>());
        }

        [Test]
        public void Get_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.Get(1));
        }

        [Test]
        public void AppendChildren_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.AppendChildren(Enumerable.Empty<ContentItem>(), new Query()));
        }

        [Test]
        public void Copy_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.Copy(new TestSourceItem(), new TestSourceItem()));
        }

        [Test]
        public void Delete_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.Delete(new TestSourceItem()));
        }

        [Test]
        public void GetChildren_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.GetChildren(new Query()));
        }

        [Test]
        public void GetSource_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.GetSource(new TestSourceItem()));
        }

        [Test]
        public void HasChildren_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.HasChildren(new Query()));
        }

        [Test]
        public void IsProvidedBy_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.IsProvidedBy(new TestSourceItem()));
        }

        [Test]
        public void Move_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.Move(new TestSourceItem(), new TestSourceItem()));
        }

        [Test]
        public void ResolvePath_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.ResolvePath("/hello/world"));
        }

        [Test]
        public void ResolvePath_FromStartingPoint_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.ResolvePath(new TestSourceItem(), "/hello/world"));
        }

        [Test]
        public void Save_FromStartingPoint_IsDelegated_ToSources()
        {
            ContentSource cs = CreateSource();

            Assert.Throws<MarkerException>(() => cs.Save(new TestSourceItem()));
        }

        private static ContentSource CreateSource()
        {
            ContentSource cs = new ContentSource(new Fakes.FakeSecurityManager(), new SourceBase[] { 
                new DatabaseSource(null, null), 
                new TestSource()
            });
            return cs;
        }

        class TestSourceItem : ContentItem, IActiveContent
        {
            public void Save()
            {
                throw new NotImplementedException();
            }

            public void Delete()
            {
                throw new NotImplementedException();
            }

            public void MoveTo(ContentItem destination)
            {
                throw new NotImplementedException();
            }

            public ContentItem CopyTo(ContentItem destination)
            {
                throw new NotImplementedException();
            }
        }

        class MarkerException : Exception
        {
        }

        class TestSource : SourceBase
        {
            public TestSource()
            {
                BaseContentType = typeof(IActiveContent);
            }

            public override bool ProvidesChildrenFor(ContentItem parent)
            {
                throw new MarkerException();
            }

            public override N2.Web.PathData ResolvePath(ContentItem startingPoint, string path)
            {
                throw new MarkerException();
            }

            public override N2.Web.PathData ResolvePath(string path)
            {
                throw new MarkerException();
            }

            public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
            {
                throw new MarkerException();
            }

            public override bool IsProvidedBy(ContentItem item)
            {
                throw new MarkerException();
            }

            public override ContentItem Get(object id)
            {
                throw new MarkerException();
            }

            public override void Save(ContentItem item)
            {
                throw new MarkerException();
            }

            public override void Delete(ContentItem item)
            {
                throw new MarkerException();
            }

            public override ContentItem Move(ContentItem source, ContentItem destination)
            {
                throw new MarkerException();
            }

            public override ContentItem Copy(ContentItem source, ContentItem destination)
            {
                throw new MarkerException();
            }
        }
    }
}
