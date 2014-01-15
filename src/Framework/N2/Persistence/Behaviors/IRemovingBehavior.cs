using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Behaviors
{
    public interface IRemovingBehavior : IBehavior
    {
        void OnRemoving(BehaviorContext context);
        void OnRemovingChild(BehaviorContext context);
    }
}
