using System;

namespace N2.Tests.Fakes
{
    public class FakeIntegrityManager : N2.Integrity.IIntegrityManager
    {
        public bool returnBoolean = true;
        public Exception returnException = null;

        #region IIntegrityManager Members

        public bool CanCopy(ContentItem source, ContentItem destination)
        {
            return returnBoolean;
        }

        public bool CanDelete(ContentItem item)
        {
            return returnBoolean;
        }

        public bool CanMove(ContentItem source, ContentItem destination)
        {
            return returnBoolean;
        }

        public bool CanSave(ContentItem item)
        {
            return returnBoolean;
        }

        public bool IsLocallyUnique(string name, ContentItem item)
        {
            return returnBoolean;
        }

        public bool IsLocallyUniqueAllowDrafts(string name, ContentItem item)
        {
            return returnBoolean;
        }
        public Exception GetMoveException(ContentItem source, ContentItem destination)
        {
            return returnException;
        }

        public Exception GetCopyException(ContentItem source, ContentItem destination)
        {
            return returnException;
        }

        public Exception GetDeleteException(ContentItem item)
        {
            return returnException;
        }

        public Exception GetSaveException(ContentItem item)
        {
            return returnException;
        }

        public Exception GetCreateException(ContentItem item, ContentItem parent)
        {
            return returnException;
        }

        #endregion
    }
}
