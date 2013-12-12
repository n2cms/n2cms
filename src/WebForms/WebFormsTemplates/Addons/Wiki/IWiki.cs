using System.Collections.Generic;

namespace N2.Addons.Wiki
{
    public interface IWiki : IArticle
    {
        ContentItem GetChild(string name);
        IEnumerable<string> ModifyRoles { get; }
        string UploadFolder { get; }
        int ImageWidth { get; }
        bool EnableFreeText { get; set; }
    }
}
