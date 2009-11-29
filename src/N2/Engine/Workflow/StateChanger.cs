using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Engine.Workflow
{
    /// <summary>
    /// Helps to change state of an item and notify observers about the change.
    /// </summary>
    public class StateChanger
    {
        /// <summary>Is invoked when the state has changed.</summary>
        public event EventHandler<StateChangedEventArgs> StateChanged;

        /// <summary>Changes a state of an item to the new state.</summary>
        /// <param name="item">The item whose state is to be changed.</param>
        /// <param name="toState">The next state of the item.</param>
        public void ChangeTo(ContentItem item, ContentState toState)
        {
            var args = new StateChangedEventArgs(item, item.State);
            item.State = toState;
            if (StateChanged != null)
            {
                StateChanged(this, args);
            }
        }
    }
}
