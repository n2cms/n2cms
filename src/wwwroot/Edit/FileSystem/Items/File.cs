using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Integrity;
using N2.Web;

namespace N2.Edit.FileSystem
{
    [RestrictParents(typeof(Directory))]
    public class File : ContentItem, INode
    {
        public long Size { get; set; }
        public virtual string PhysicalPath { get; set; }

        public override string IconUrl
        {
            get { return "~/Edit/img/ico/page_white_text"; }
        }

        public override string TemplateUrl
        {
            get { return "~/Edit/FileSystem/File.aspx"; }
        }

        string INode.PreviewUrl
        {
            get { return N2.Web.Url.Parse(Utility.ToAbsolute(TemplateUrl)).AppendQuery("selected", Path); }
        }

        public override void AddTo(ContentItem newParent)
        {
            if (newParent is Directory)
            {
                Directory dir = newParent as Directory;
                string path = System.IO.Path.Combine(dir.PhysicalPath, Name);
                if (System.IO.File.Exists(path))
                    throw new NameOccupiedException(this, newParent);
                System.IO.File.Move(PhysicalPath, path);
                PhysicalPath = path;
                Parent = newParent;
            }
            else
            {
                new N2Exception(newParent + " is not a Directory. AddTo only works on directories.");
            }
        }
    }
}
