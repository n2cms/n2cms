using System;
using System.Collections.Generic;
using NUnit.Framework;
using N2.Edit.FileSystem.Items;
using N2.Tests;
using N2.Edit.FileSystem;
using N2.Web;
using N2.Engine;
using Directory=N2.Edit.FileSystem.Items.Directory;
using File=N2.Edit.FileSystem.Items.File;
using System.IO;
using System.Diagnostics;
using N2.Tests.Persistence;

namespace N2.Edit.Tests.FileSystem
{
    [TestFixture]
    public class FileSystemTests : DatabasePreparingBase
    {
        RootNode root;
        RootDirectory upload;
        
        [TestFixtureSetUp]
        public override void TestFixtureSetUp()
        {
			base.TestFixtureSetUp();
        	N2.Context.Initialize(engine);

            Url.DefaultExtension = "/";
        	Url.ApplicationPath = "/";
			FakeFileSystem fs = (FakeFileSystem)engine.Resolve<IFileSystem>();
			fs.BasePath = AppDomain.CurrentDomain.BaseDirectory + @"\FileSystem\";
			fs.PathProvider = new FakePathProvider(fs.BasePath);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Url.DefaultExtension = ".aspx";
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

			root = engine.Definitions.CreateInstance<RootNode>(null);
			engine.Persister.Save(root);
			engine.Resolve<IHost>().DefaultSite.RootItemID = root.ID;
			engine.Resolve<IHost>().DefaultSite.StartPageID = root.ID;

			upload = engine.Definitions.CreateInstance<RootDirectory>(root);
            upload.Title = "Upload";
            upload.Name = "Upload";
			engine.Persister.Save(upload);
        }

        [Test]
        public void CanListFiles_InRootDirectory()
        {
            IList<File> files = upload.GetFiles();
            Assert.That(files.Count, Is.EqualTo(2));
        }

        [Test]
        public void CanListDirectories_InRootDirectory()
        {
            IList<Directory> directories = upload.GetDirectories();
            Assert.That(directories.Count, Is.EqualTo(2));
        }

        [Test]
        public void CanListDirectories_InSubDirectory()
        {
            IList<Directory> directories = upload.GetDirectories();
            IList<Directory> subDirectories = directories[0].GetDirectories();
            Assert.That(subDirectories.Count, Is.EqualTo(1));
            Assert.That(subDirectories[0].Name, Is.EqualTo("Folder 3"));
        }

        [Test]
        public void CanListFiles_InSubDirectory()
        {
            IList<Directory> directories = upload.GetDirectories();
            IList<File> files = directories[0].GetFiles();
            Assert.That(files.Count, Is.EqualTo(1));
            Assert.That(files[0].Name, Is.EqualTo("File 2.txt"));
        }

        [Test]
        public void CanGetDirectory_ByName()
        {
			Directory d = (Directory)upload.GetChild("Folder1");
            Assert.That(d, Is.Not.Null);
            Assert.That(d.Name, Is.EqualTo("Folder1"));
        }

        [Test]
        public void CanGetFile_ByName()
        {
			File f = (File)upload.GetChild("File.txt");
            Assert.That(f, Is.Not.Null);
            Assert.That(f.Name, Is.EqualTo("File.txt"));
        }

        [Test]
        public void CanGet_FileLength()
        {
			File f = (File)upload.GetChild("File.txt");
            Assert.That(f.Size, Is.EqualTo(13));
        }

        [Test]
        public void CanGetDirectory_ByPath()
        {
			Directory d = (Directory)upload.GetChild("Folder 2/Folder 3");
            Assert.That(d, Is.Not.Null);
            Assert.That(d.Name, Is.EqualTo("Folder 3"));
        }

        [Test]
        public void CanGetFile_ByPath()
        {
			File f = (File)upload.GetChild("Folder 2/Folder 3/File 3.txt");
            Assert.That(f, Is.Not.Null);
            Assert.That(f.Name, Is.EqualTo("File 3.txt"));
        }

        [Test]
        public void CanMoveFile_ToOtherDirectory()
        {
			File f = (File)upload.GetChild("Folder 2/Folder 3/File 3.txt");
			string sourcePath = MapPath(@"/Upload/Folder 2/Folder 3/File 3.txt");
			string destinationPath = MapPath(@"/Upload/Folder1/File 3.txt");
			Directory d = (Directory)upload.GetChild("Folder1");
            try
            {
                f.AddTo(d);
                Assert.That(System.IO.File.Exists(destinationPath));
                Assert.That(!System.IO.File.Exists(sourcePath));
            }
            finally
            {
				if(System.IO.File.Exists(destinationPath))
					System.IO.File.Move(destinationPath, sourcePath);
            }
        }

        [Test]
        public void CanMoveFile_ToRootDirectory()
        {
			File f = (File)upload.GetChild("Folder 2/File 2.txt");
			string sourcePath = MapPath(@"/Upload/Folder 2/File 2.txt");
			string destinationPath = MapPath(@"/Upload/File 2.txt");
            try
            {
                f.AddTo(upload);
                Assert.That(System.IO.File.Exists(destinationPath));
                Assert.That(!System.IO.File.Exists(sourcePath));
            }
            finally
            {
				if (System.IO.File.Exists(destinationPath))
					System.IO.File.Move(destinationPath, sourcePath);
            }
        }

    	[Test]
        public void CanMoveDirectory_ToOtherDirectory()
        {
			Directory d = (Directory)upload.GetChild("Folder 2/Folder 3");
			string sourcePath = MapPath("/Upload/Folder 2/Folder 3");
            string destinationPath = MapPath("/Upload/Folder1/Folder 3");
            try
            {
				d.AddTo(upload.GetChild("Folder1"));
                Assert.That(System.IO.Directory.Exists(destinationPath));
                Assert.That(!System.IO.Directory.Exists(sourcePath));
            }
            finally
            {
				if(System.IO.Directory.Exists(destinationPath))
					System.IO.Directory.Move(destinationPath, sourcePath);
            }
        }

        [Test]
        public void CanMoveDirectory_ToRootDirectory()
        {
			Directory d = (Directory)upload.GetChild("Folder 2/Folder 3");
			string sourcePath = MapPath("/Upload/Folder 2/Folder 3");
            string destinationPath = MapPath("/Upload/Folder 3");
            try
            {
                d.AddTo(upload);
                Assert.That(System.IO.Directory.Exists(destinationPath));
                Assert.That(!System.IO.Directory.Exists(sourcePath));
            }
            finally
            {
				if (System.IO.Directory.Exists(destinationPath))
					System.IO.Directory.Move(destinationPath, sourcePath);
            }
        }

        [Test]
        public void CanMove_RootDirectory_ToContentItem()
        {
            upload.AddTo(root);

            Assert.That(upload.Parent, Is.EqualTo(root));
            Assert.That(root.Children.Contains(upload));
        }

        [Test]
        public void CanMoveFile()
        {
            AssertMovement((from, to) => from.MoveTo(to));
        }

        [Test]
        public void CanMoveFile_UsingPersister()
        {
            AssertMovement((from, to) => engine.Persister.Move(from, to));
        }

        private void AssertMovement(N2.Engine.Action<File, Directory> moveAction)
        {
			Directory sourceDirectory = (Directory)upload.GetChild("Folder1");
			Directory destinationDirectory = (Directory)upload.GetChild("Folder 2");
			File f = (File)sourceDirectory.GetChild("File1.txt");
            try
            {
                moveAction(f, destinationDirectory);
				Assert.That(sourceDirectory.GetChild("File1.txt"), Is.Null);
                Assert.That(f.Parent, Is.EqualTo(destinationDirectory));
				Assert.That(destinationDirectory.GetChild("File1.txt"), Is.Not.Null);
            }
			catch(Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
            finally
            {
            	try
            	{
					f.MoveTo(sourceDirectory);
				}
            	catch (Exception ex)
            	{
					Trace.WriteLine(ex.ToString());
            	}
            }
        }

        [Test]
        public void CanCopyAndDeleteFile()
        {
            CopyAndDelete(delegate(File from, Directory to)
            {
                return from.CopyTo(to);
            });
        }

        [Test]
        public void CanCopyAndDeleteFile_UsingPersister()
        {
            CopyAndDelete(delegate(File from, Directory to)
            {
                return engine.Persister.Copy(from, to);
            });
        }

        private void CopyAndDelete(Func<File, Directory, ContentItem> copyAction)
        {
			Directory d1 = (Directory)upload.GetChild("Folder1");
			Directory d2 = (Directory)upload.GetChild("Folder 2");
			File f = (File)d1.GetChild("File1.txt");
            File fCopy = null;
            try
            {
                fCopy = (File)copyAction(f, d2);
				Assert.That(d2.GetChild("File1.txt"), Is.Not.Null);
                Assert.That(fCopy.Parent, Is.EqualTo(d2));
				Assert.That(d1.GetChild("File1.txt"), Is.Not.Null);
                Assert.That(f.Parent, Is.EqualTo(d1));
            }
            finally
            {
                if (fCopy != null)
                    fCopy.Delete();
            }
		}

		string MapPath(string path)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystem" + path.Replace('/', '\\'));
		}
    }
}
