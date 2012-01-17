﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Persistence.Search
{
	[Service(typeof(IIndexer))]
	public class EmptyIndexer : IIndexer
	{
		public virtual IndexStatistics GetStatistics()
		{
			return new IndexStatistics();
		}

		public virtual void Clear()
		{
		}

		public virtual void Optimize()
		{
		}

		public virtual void Delete(int itemID)
		{
		}

		public virtual void Update(ContentItem item)
		{
		}

		public virtual void Unlock()
		{
		}
	}
}
