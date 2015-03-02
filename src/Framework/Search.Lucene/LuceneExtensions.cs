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

        public static string ToLuceneQuery(this Query query)
        {
            var q = "";
            if (!string.IsNullOrEmpty(query.Text))
                q = query.OnlyPages.HasValue
                     ? string.Format("+(Title:({0})^4 Text:({0}) PartsText:({0}))", query.Text)
                     : string.Format("+(Title:({0})^4 Text:({0}))", query.Text);

            if (query.Ancestor != null)
                q += string.Format(" +Trail:{0}*", query.Ancestor);
            if (query.OnlyPages.HasValue)
                q += string.Format(" +IsPage:{0}", query.OnlyPages.Value.ToString().ToLower());
            if (query.Roles != null)
                q += string.Format(" +Roles:(Everyone {0})", string.Join(" ", query.Roles.ToArray()));
            if (query.Types != null)
                q += string.Format(" +Types:({0})", string.Join(" ", query.Types));
            if (query.LanguageCode != null)
                q += string.Format(" +Language:({0})", query.LanguageCode);
            if (query.Details.Count > 0)
                foreach (var kvp in query.Details)
                {
                    if (TextExtractor.Properties.All.Contains(kvp.Key))
                        q += string.Format(" +{0}:({1})", kvp.Key, kvp.Value);
                    else
                        q += string.Format(" +Detail.{0}:({1})", kvp.Key, kvp.Value);
				}
			if (query.Exclution != null)
				q += string.Format(" -({0})", ToLuceneQuery(query.Exclution));
			if (query.Intersection != null)
				q = string.Format("+({0}) +({1})", q, ToLuceneQuery(query.Intersection));
			if (query.Union != null)
				q = string.Format("({0}) ({1})", q, ToLuceneQuery(query.Union));
            return q;
        }
    }
}
