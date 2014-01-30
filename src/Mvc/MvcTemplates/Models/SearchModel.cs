using System;
using System.Collections.Generic;
using N2.Templates.Mvc.Models.Pages;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
    public class SearchModel
    {
        public SearchModel(ICollection<ContentItem> results)
        {
            Results = results;
        }

        public ICollection<ContentItem> Results { get; private set; }

        public string SearchTerm { get; set; }

        public int TotalResults { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages
        {
            get { return (int) Math.Ceiling((decimal) TotalResults/PageSize); }
        }

        public bool HasSearchTerm
        {
            get { return !String.IsNullOrEmpty(SearchTerm); }
        }
    }
}
