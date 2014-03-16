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

        [ConfigurationProperty("uploadsWhitelistExression")]
        public string UploadsWhitelistExression
        {
            get { return (string)base["uploadsWhitelistExression"]; }
            set { base["uploadsWhitelistExression"] = value; }
        }

        [ConfigurationProperty("uploadsBlacklistExression", DefaultValue = "[.](armx|asax|asbx|axhx|asmx|asp|aspx|axd|cshtml|master|vsdisco|cfm|pl|cgi|ad|adp|crt|ins|mde|msc|sct|vb|swc|wsf|cpl|shs|bas|bat|cmd|com|hlp|hta|isp|js|jse|lnk|mst|pcd|pif|reg|scr|url|vbe|vbs|ws|wsh|exe|com|bat|cmd|ps1)$")]
        public string UploadsBlacklistExression
        {
            get { return (string)base["uploadsBlacklistExression"]; }
            set { base["uploadsBlacklistExression"] = value; }
        }

        public bool IsTrusted(string filename)
        {
            if (!string.IsNullOrEmpty(UploadsWhitelistExression))
                return Regex.IsMatch(filename, UploadsWhitelistExression, RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(UploadsBlacklistExression))
                return !Regex.IsMatch(filename, UploadsBlacklistExression, RegexOptions.IgnoreCase);
            return true;
        }
    }
}
