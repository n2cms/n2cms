using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Templates.Mvc.Models
{
    public class ContentModelBase<T> 
        where T:ContentItem
    {
        public virtual T CurrentItem { get; set; }
    }
}
