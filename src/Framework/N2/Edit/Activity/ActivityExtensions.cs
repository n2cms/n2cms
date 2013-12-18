using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Security;
using N2.Engine;

namespace N2.Edit.Activity
{
    public static class ActivityExtensions
    {
        public static TActivity AddActivity<TActivity>(this IEngine engine, TActivity activity) where TActivity : ActivityBase
        {
            var repository = engine.Resolve<ActivityRepository<TActivity>>();
            repository.Add(activity);
            return activity;
        }
    }
}
