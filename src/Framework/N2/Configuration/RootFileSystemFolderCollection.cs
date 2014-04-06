using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

namespace N2.Configuration
{
    [ConfigurationCollection(typeof(FolderElement))]
    public class RootFileSystemFolderCollection : FileSystemFolderCollection
    {
        public RootFileSystemFolderCollection()
        {
            FolderElement folder = new FolderElement();
            folder.Path = "~/upload/";
            AddDefault(folder);
        }

        [ConfigurationProperty("uploadsWhitelistExpression")]
        public string UploadsWhitelistExpression
        {
            get { return (string)base["uploadsWhitelistExpression"]; }
            set { base["uploadsWhitelistExpression"] = value; }
        }

        [ConfigurationProperty("uploadsBlacklistExpression", DefaultValue = "[.](armx|asax|asbx|axhx|asmx|asp|aspx|axd|cshtml|master|vsdisco|cfm|pl|cgi|ad|adp|crt|ins|mde|msc|sct|vb|swc|wsf|cpl|shs|bas|bat|cmd|com|hlp|hta|isp|js|jse|lnk|mst|pcd|pif|reg|scr|url|vbe|vbs|ws|wsh|php)$")]
        public string UploadsBlacklistExpression
        {
            get { return (string)base["uploadsBlacklistExpression"]; }
            set { base["uploadsBlacklistExpression"] = value; }
        }

        public bool IsTrusted(string filename)
        {
            if (!string.IsNullOrEmpty(UploadsWhitelistExpression))
                return Regex.IsMatch(filename, UploadsWhitelistExpression, RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(UploadsBlacklistExpression))
                return !Regex.IsMatch(filename, UploadsBlacklistExpression, RegexOptions.IgnoreCase);
            return true;
        }
    }
}
