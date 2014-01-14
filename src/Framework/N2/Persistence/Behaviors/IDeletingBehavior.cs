using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Behaviors
{
    public interface IDeletingBehavior : IBehavior
    {
        void OnDeleting(BehaviorContext context);
        void OnDeletingChild(BehaviorContext context);
    }
}
