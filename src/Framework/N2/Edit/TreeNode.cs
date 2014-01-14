using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit
{
    public class TreeNode
    {
        public int ID { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string ToolTip { get; set; }

        public string PreviewUrl { get; set; }
        public string Target { get; set; }
        public string CssClass { get; set; }

        public string IconUrl { get; set; }
        public string IconClass { get; set; }

        public Security.Permission MaximumPermission { get; set; }

        public IDictionary<string, MetaInfo> MetaInformation { get; set; }

        public ContentState State { get; set; }

        public int SortOrder { get; set; }

        public int VersionIndex { get; set; }

        public string ZoneName { get; set; }
    }
}
