using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Versioning;

namespace N2.Edit
{
	public class DraftMetaInfo : MetaInfo
	{
		public DraftMetaInfo(ContentItem item)
		{
			Text = "&nbsp;";
			ItemID = item.ID;
			ToolTip = item.SavedBy + ": " + item.Updated;
			Saved = item.Updated;
			SavedBy = item.SavedBy;
			VersionIndex = item.VersionIndex;
		}

		public DraftMetaInfo(DraftInfo draftInfo)
		{
			Text = "&nbsp;";
			ToolTip = draftInfo.SavedBy + ": " + draftInfo.Saved;
			ItemID = draftInfo.ItemID;
			Saved = draftInfo.Saved;
			SavedBy = draftInfo.SavedBy;
			VersionIndex = draftInfo.VersionIndex;
        }

		public int ItemID { get; private set; }
		public DateTime Saved { get; private set; }
		public string SavedBy { get; private set; }
		public int VersionIndex { get; private set; }
	}

	public class MetaInfo
    {
        public string ToolTip { get; set; }
        public string Text { get; set; }
    }
}
