using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence.NH.Finder;
using N2.Persistence.Finder;
using N2.Definitions;

namespace N2.Tests.Fakes
{
	public class FakeItemFinder : IItemFinder
	{
		IDefinitionManager definitions;

		public FakeItemFinder(IDefinitionManager definitions)
			: this(definitions, () => Enumerable.Empty<ContentItem>())
		{
		}
		public FakeItemFinder(IDefinitionManager definitions, Func<IEnumerable<ContentItem>> selector)
		{
			this.definitions = definitions;
			Selector = selector; 
		}

		public Func<IEnumerable<ContentItem>> Selector { get; set; }

		#region IItemFinder Members

		public IQueryBuilder Where
		{
			get { return new FakeQueryBuilder(definitions, Selector); }
		}

		public IQueryEnding All
		{
			get { return new FakeQueryBuilder(definitions, Selector); }
		}

		#endregion

		class FakeQueryBuilder : QueryBuilder
		{
			public FakeQueryBuilder(IDefinitionManager definitions, Func<IEnumerable<ContentItem>> selector)
				: base(null, definitions)
			{
				this.Selector = selector;
			}

			public Func<IEnumerable<ContentItem>> Selector { get; set; }

			public override System.Collections.Generic.IList<T> Select<T>()
			{
				return Selector().Cast<T>().ToList();
			}
			public override int Count()
			{
				return Selector().Count();
			}
		}
	}
}
