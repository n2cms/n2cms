using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Behaviors
{
    public interface ISavingBehavior : IBehavior
    {
        void OnSaving(BehaviorContext context);
        void OnSavingChild(BehaviorContext context);
    }
}
