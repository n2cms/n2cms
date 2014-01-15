using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Templates.Mvc.Models.Pages;
using N2.Web.Mvc.Html;

namespace N2.Templates.Mvc.Models
{
    public class NewsContainerModel
    {
        public NewsContainer Container { get; set; }
        public IList<News> News { get; set; }

        public bool IsLast { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }

        public string Tag { get; set; }
    }
}
