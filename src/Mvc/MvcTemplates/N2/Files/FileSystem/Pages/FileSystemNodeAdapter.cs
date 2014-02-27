using N2.Definitions;
using N2.Edit;
using N2.Edit.FileSystem.Items;
using N2.Engine;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Files.FileSystem.Pages
{
    [Adapts(typeof(AbstractNode))]
    public class FileSystemNodeAdapter : NodeAdapter
    {
        public override string GetPreviewUrl(ContentItem item)
        {
            if (item is IFileSystemDirectory)
                return "{ManagementUrl}/Files/FileSystem/Directory.aspx".ToUrl().ResolveTokens().AppendSelection(item);
            if (item is IFileSystemFile)
                return "{ManagementUrl}/Files/FileSystem/File.aspx".ToUrl().ResolveTokens().AppendSelection(item);

            return base.GetPreviewUrl(item);
        }
    }
}
