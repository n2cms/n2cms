using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using Lucene.Net.Index;

namespace N2.Persistence.Search
{
	public static class LuceneExtensions
	{
		public static bool IndexExists(this Directory dir)
		{
			return SegmentInfos.GetCurrentSegmentGeneration(dir) >= 0L;
		}
	}
}
