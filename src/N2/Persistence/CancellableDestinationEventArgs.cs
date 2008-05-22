using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Persistence
{
    /// <summary>Event argument containing item and destination item.</summary>
    public class CancellableDestinationEventArgs : DestinationEventArgs
    {
        public CancellableDestinationEventArgs(ContentItem item, ContentItem destination)
            : base(item, destination)
        {
        }

		private bool cancel;
		public bool Cancel
		{
			get { return cancel; }
			set { cancel = value; }
		}
	
    }
}
