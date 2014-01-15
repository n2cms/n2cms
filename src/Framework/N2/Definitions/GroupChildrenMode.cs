using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Definitions
{
    public enum GroupChildrenMode
    {
        Ungrouped,
        AlphabeticalIndex,
        PublishedYear,
        PublishedYearMonth,
        PublishedYearMonthDay,
        Type,
        ZoneName,
        Pages,
        PagesAfterTreshold,
        RecentWithArchive
    }
}
