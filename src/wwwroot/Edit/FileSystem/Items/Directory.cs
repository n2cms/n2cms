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
using System.Collections.Generic;
using System.IO;
using N2.Collections;
using N2.Web;
using System.Diagnostics;
using N2.Edit.Trash;
using N2.Details;
using N2.Persistence;

namespace N2.Edit.FileSystem.Items
{
    [Definition]
    [RestrictParents(typeof(AbstractDirectory))]
    public class Directory : AbstractDirectory, IActiveContent
    {
        public override void AddTo(ContentItem newParent)
        {
            if (newParent is AbstractDirectory)
            {
                AbstractDirectory dir = newParent as AbstractDirectory;
                string from = PhysicalPath;
                string to = System.IO.Path.Combine(dir.PhysicalPath, Name);
                if (System.IO.Directory.Exists(to))
                    throw new NameOccupiedException(this, newParent);

                if (from != null)
                    System.IO.Directory.Move(from, to);
                else
                    System.IO.Directory.CreateDirectory(to);
                PhysicalPath = to;
                Parent = newParent;
            }
            else if(newParent != null)
            {
                new N2Exception(newParent + " is not a Directory. AddTo only works on directories.");
            }
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
            System.IO.Directory.Delete(PhysicalPath);
        }

        public void MoveTo(ContentItem destination)
        {
            AbstractDirectory.EnsureDirectory(destination);

            Directory d = destination as Directory;
            string from = PhysicalPath;
            string to = System.IO.Path.Combine(d.PhysicalPath, Name);
            if (System.IO.File.Exists(to))
                throw new NameOccupiedException(this, destination);

            System.IO.Directory.Move(from, to);
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

            System.IO.Directory.CreateDirectory(to);
            Directory copy = (Directory)destination.GetChild(Name);
            foreach (Directory childDir in GetDirectories())
                childDir.CopyTo(copy);
            foreach (File f in GetFiles())
                f.CopyTo(copy);

            return copy;
        }

        #endregion
    }
}
