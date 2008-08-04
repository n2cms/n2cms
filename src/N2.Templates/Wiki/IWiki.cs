using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki
{
    public interface IWiki : IArticle
    {
        ContentItem GetChild(string name);
        IEnumerable<string> ModifyRoles { get; }
        string UploadFolder { get; }
    }
}
