using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions.Behaviors
{
	public interface ISavingBehavior
	{
		void OnSaving(BehaviorContext context);
		void OnSavingChild(BehaviorContext context);
	}
}
