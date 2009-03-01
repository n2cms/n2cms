using System.Web;
using N2.Integrity;
using N2.Details;
using N2.Persistence;
using N2.Installation;

namespace N2.Edit.FileSystem.Items
{
    [Definition(Installer = InstallerHint.NeverRootOrStartPage)]
    [RestrictParents(typeof(AbstractDirectory))]
    [WithEditableName]
    public class Directory : AbstractDirectory, IActiveContent
    {
        public override void AddTo(ContentItem newParent)
        {
            if (newParent is AbstractDirectory)
            {
				AbstractDirectory dir = EnsureDirectory(newParent);

				string to = VirtualPathUtility.Combine(dir.Url, Name);
				if (FileSystem.FileExists(to))
					throw new NameOccupiedException(this, dir);

				if(FileSystem.DirectoryExists(Url))
					FileSystem.MoveDirectory(Url, to);
				else
					FileSystem.CreateDirectory(to);
            	
				Parent = newParent;

				//string from = PhysicalPath;
				//string to = System.IO.Path.Combine(dir.PhysicalPath, Name);
				//if (System.IO.Directory.Exists(to))
				//    throw new NameOccupiedException(this, newParent);

				//if (from != null)
				//    System.IO.Directory.Move(from, to);
				//else
				//    System.IO.Directory.CreateDirectory(to);
				//PhysicalPath = to;
				//Parent = newParent;
            }
            else if(newParent != null)
            {
                new N2Exception(newParent + " is not a Directory. AddTo only works on directories.");
            }
        }

        #region IActiveRecord Members

        public void Save()
        {
			if(!FileSystem.DirectoryExists(Url))
				FileSystem.CreateDirectory(Url);
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
			FileSystem.DeleteDirectory(Url);
			//try
			//{
			//    System.IO.Directory.Delete(PhysicalPath, true);
			//}
			//catch (Exception ex)
			//{
			//    Trace.TraceError(ex.ToString());
			//}
        }

        public void MoveTo(ContentItem destination)
        {
			AbstractDirectory d = EnsureDirectory(destination);

			string to = VirtualPathUtility.Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.MoveDirectory(Url, to);

			//AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

			//string from = PhysicalPath;
			//string to = System.IO.Path.Combine(d.PhysicalPath, Name);
			//if (System.IO.File.Exists(to))
			//    throw new NameOccupiedException(this, destination);

			//try
			//{
			//    System.IO.Directory.Move(from, to);
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

			FileSystem.CreateDirectory(to);
        	Directory copy = CreateDirectory(FileSystem.GetDirectory(to));

			foreach (File f in GetFiles())
				f.CopyTo(copy);

			foreach (Directory childDir in GetDirectories())
				childDir.CopyTo(copy);

        	return copy;

			//string from = PhysicalPath;
			//string to = System.IO.Path.Combine(d.PhysicalPath, Name);
			//if (System.IO.File.Exists(to))
			//    throw new NameOccupiedException(this, destination);

			//try
			//{
			//    System.IO.Directory.CreateDirectory(to);
			//    Directory copy = (Directory)destination.GetChild(Name);
			//    foreach (Directory childDir in GetDirectories())
			//        childDir.CopyTo(copy);
			//    foreach (File f in GetFiles())
			//        f.CopyTo(copy);

			//    return copy;
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
