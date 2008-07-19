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
using N2.Persistence;
using N2.Edit.Trash;
using N2.Details;

namespace N2.Edit.FileSystem.Items
{
    [Definition]
    [RestrictParents(typeof(AbstractDirectory))]
    [Editables.EditableUpload]
    public class File : AbstractNode, INode, IActiveContent
    {
        public long Size { get; set; }

        public override string IconUrl
        {
            get { return "~/Edit/img/ico/page_white.gif"; }
        }

        public override string TemplateUrl
        {
            get { return "~/Edit/FileSystem/File.aspx"; }
        }

        public override void AddTo(ContentItem newParent)
        {
            if (newParent != null)
                MoveTo(newParent);
        }

        #region IActiveRecord Members

        public void Save()
        {
            string expectedPath = System.IO.Path.Combine(Directory.PhysicalPath, Name);
            if (expectedPath != PhysicalPath)
            {
                if (PhysicalPath != null)
                {
                    System.IO.Directory.Move(PhysicalPath, expectedPath);
                }
                else
                {
                    System.IO.Directory.CreateDirectory(expectedPath);
                }
                PhysicalPath = expectedPath;
            }
        }

        public void Delete()
        {
            System.IO.File.Delete(PhysicalPath);
        }

        public void MoveTo(ContentItem destination)
        {
            AbstractDirectory.EnsureDirectory(destination);

            AbstractDirectory d = destination as AbstractDirectory;
            string from = PhysicalPath;
            string to = System.IO.Path.Combine(d.PhysicalPath, Name);
            if (System.IO.File.Exists(to))
                throw new NameOccupiedException(this, destination);

            System.IO.File.Move(from, to);
            PhysicalPath = to;
            Parent = destination;
        }

        public ContentItem CopyTo(ContentItem destination)
        {
            AbstractDirectory.EnsureDirectory(destination);

            Directory d = destination as Directory;
            string from = PhysicalPath;
            string to = System.IO.Path.Combine(d.PhysicalPath, Name);
            if (System.IO.File.Exists(to))
                throw new NameOccupiedException(this, destination);

            System.IO.File.Copy(from, to);
            return (File)destination.GetChild(Name);
        }

        #endregion
    }
}
