using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Activity
{
    public class ActivityUser
    {
        public ActivityUser(string name)
        {
            Name = name;
            Activities = new HashSet<ActivityBase>();
        }

        public string Name { get; set; }

        public HashSet<ActivityBase> Activities { get; set; }
    }
}
