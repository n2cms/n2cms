using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Tests.Fakes
{
	public class FakeIntegrityManager : N2.Integrity.IIntegrityManager
	{
		#region IIntegrityManager Members

		public bool CanCopy(ContentItem source, ContentItem destination)
		{
			return true;
		}

		public bool CanDelete(ContentItem item)
		{
			return true;
		}

		public bool CanMove(ContentItem source, ContentItem destination)
		{
			return true;
		}

		public bool CanSave(ContentItem item)
		{
			return true;
		}

		public bool IsLocallyUnique(string name, ContentItem item)
		{
			return true;
		}

		#endregion



		#region IIntegrityManager Members


		public N2Exception GetMoveException(ContentItem source, ContentItem destination)
		{
			return null;
		}

		public N2Exception GetCopyException(ContentItem source, ContentItem destination)
		{
			return null;
		}

		public N2Exception GetDeleteException(ContentItem item)
		{
			return null;
		}

		public N2Exception GetSaveException(ContentItem item)
		{
			return null;
		}

		#endregion
	}
}
