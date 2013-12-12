using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace N2.Edit.Activity
{
    public abstract class ActivityBase
    {
        public string PerformedBy { get; set; }
        public DateTime AddedDate { get; set; }
        
        public virtual string ActivityType
        {
            get { return GetType().Name; }
        }
    }
}
