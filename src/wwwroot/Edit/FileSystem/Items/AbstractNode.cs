using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Details;
using N2.Edit.Trash;

namespace N2.Edit.FileSystem.Items
{
    [NotThrowable]
    [WithEditableName]
    public abstract class AbstractNode : ContentItem, INode
    {
        public override string Title
        {
            get { return Name; }
            set { Name = value; }
        }
        public virtual string PhysicalPath { get; set; }
        public virtual AbstractDirectory Directory
        {
            get { return Parent as AbstractDirectory; }
        }

        public override string IconUrl
        {
            get { return "~/Edit/img/ico/folder.gif"; }
        }

        public override string TemplateUrl
        {
            get { return "~/Edit/FileSystem/Directory.aspx"; }
        }

        public override string Extension
        {
            get { return string.Empty; }
        }

        string INode.PreviewUrl
        {
            get { return N2.Web.Url.Parse(Utility.ToAbsolute(TemplateUrl)).AppendQuery("selected", Path); }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }
            AbstractNode otherNode = obj as AbstractNode;
            return otherNode != null && PhysicalPath == otherNode.PhysicalPath;
        }

        public override int GetHashCode()
        {
            return ("" + PhysicalPath + ID).GetHashCode();
        }
    }
}
