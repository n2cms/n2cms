using System;
using System.IO;
using N2.Integrity;
using N2.Persistence;
using System.Diagnostics;
using N2.Installation;
using System.Web;

namespace N2.Edit.FileSystem.Items
{
    [Definition(Installer = InstallerHint.NeverRootOrStartPage)]
    [RestrictParents(typeof(AbstractDirectory))]
    [Editables.EditableUpload]
    public class File : AbstractNode, IActiveContent
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

		public override string Url
		{
			get { return Directory.Url + "/" + Name; }
		}

		public virtual void TransmitTo(Stream stream)
		{
			FileSystem.ReadFileContents(Url, stream);
		}

		public void WriteToDisk(Stream stream)
		{
			FileSystem.WriteFile(Url, stream);
		}

		public bool Exists
		{
			get { return FileSystem.FileExists(Url); }
		}

        #region IActiveRecord Members

        public void Save()
        {
			//string expectedPath = System.IO.Path.Combine(Directory.PhysicalPath, Name);
			//if (expectedPath != PhysicalPath)
			//{
			//    try
			//    {
			//        if (PhysicalPath != null)
			//        {
			//            System.IO.Directory.Move(PhysicalPath, expectedPath);
			//        }
			//        else
			//        {
			//            System.IO.Directory.CreateDirectory(expectedPath);
			//        }
			//        PhysicalPath = expectedPath;
			//    }
			//    catch (Exception ex)
			//    {
			//        Trace.TraceError(ex.ToString());
			//    }
			//}
        }

        public void Delete()
        {
			FileSystem.DeleteFile(Url);
			//try
			//{
			//    System.IO.File.Delete(PhysicalPath);
			//}
			//catch (Exception ex)
			//{
			//    Trace.TraceError(ex.ToString());
			//}
        }

        public void MoveTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

			string to = VirtualPathUtility.Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.MoveFile(Url, to);

			//string from = PhysicalPath;
			//string to = System.IO.Path.Combine(d.PhysicalPath, Name);
			//if (System.IO.File.Exists(to))
			//    throw new NameOccupiedException(this, destination);

			//try
			//{
			//    System.IO.File.Move(from, to);
			//    PhysicalPath = to;
			//    Parent = destination;
			//}
			//catch (Exception ex)
			//{
			//    Trace.TraceError(ex.ToString());
			//}
        }

        public ContentItem CopyTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

			string to = VirtualPathUtility.Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.CopyFile(Url, to);

			return CreateFile(FileSystem.GetFile(to));

			//string from = PhysicalPath;
			//string to = System.IO.Path.Combine(d.PhysicalPath, Name);
			//if (System.IO.File.Exists(to))
			//    throw new NameOccupiedException(this, destination);


			//try
			//{
			//    System.IO.File.Copy(from, to);
			//    return (File)destination.GetChild(Name);
			//}
			//catch (Exception ex)
			//{
			//    Trace.TraceError(ex.ToString());
			//    return this;
			//}
        }

        #endregion
	}
}
