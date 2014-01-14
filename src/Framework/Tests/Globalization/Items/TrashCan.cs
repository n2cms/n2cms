using System;

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

        public bool AsyncTrashPurging
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
