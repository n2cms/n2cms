using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Globalization.Items
{
    public class TrashCan : ContentItem, N2.Edit.Trash.ITrashCan
    {
        #region ITrashCan Members

        public bool Enabled
        {
            get { throw new NotImplementedException(); }
        }

		public N2.Edit.Trash.TrashPurgeInterval PurgeInterval
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
