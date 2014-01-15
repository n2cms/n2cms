using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace N2.Templates.Mvc.Services
{
    public interface IViewConcern
    {
        void Apply(ContentItem item, Page page);
    }
}
