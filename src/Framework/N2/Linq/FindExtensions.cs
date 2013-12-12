using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence.Finder;

namespace N2.Linq
{
    public static class FindExtensions
    {
        public static IQueryAction IsPage(this IQueryBuilder builder)
        {
            return builder.ZoneName.IsNull();
        }

        public static IQueryAction IsPart(this IQueryBuilder builder)
        {
            return builder.ZoneName.IsNull(false);
        }

        public static IQueryAction IsPublished(this IQueryBuilder builder)
        {
            return builder.State.Eq(ContentState.Published);
        }

        public static IQueryAction IsDescendant(this IQueryBuilder builder, ContentItem ancestor)
        {
            return builder.AncestralTrail.Like(ancestor.GetTrail() + "%");
        }

        public static IQueryAction IsDescendantOrSelf(this IQueryBuilder builder, ContentItem ancestor)
        {
            return builder.OpenBracket().AncestralTrail.Like(ancestor.GetTrail() + "%").Or.ID.Eq(ancestor.ID).CloseBracket();
        }

        public static IQueryAction IsAncestor(this IQueryBuilder builder, ContentItem descendant)
        {
            return builder.ID.In(descendant.GetTrail().Split('/').Where(id => !string.IsNullOrEmpty(id)).Select(id => int.Parse(id)).ToArray());
        }

        public static IQueryAction IsAncestorOrSelf(this IQueryBuilder builder, ContentItem descendant)
        {
            return builder.ID.In(descendant.GetTrail().Split('/').Where(id => !string.IsNullOrEmpty(id)).Select(id => int.Parse(id)).Union(new [] { descendant.ID }).ToArray());
        }
    }
}
