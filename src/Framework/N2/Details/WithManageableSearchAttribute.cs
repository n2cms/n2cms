using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Details
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WithManageableSearchAttribute : EditableUserControlAttribute
    {
        public WithManageableSearchAttribute()
            : base("{ManagementUrl}/Search/ManageIndex.ascx", 1000)
        {
            Name = "ManageSearch";
        }
    }
}
