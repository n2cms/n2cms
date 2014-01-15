using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Behaviors
{
    public interface IAddingBehavior : IBehavior
    {
        void OnAdding(BehaviorContext context);
        void OnAddingChild(BehaviorContext context);
    }
}
