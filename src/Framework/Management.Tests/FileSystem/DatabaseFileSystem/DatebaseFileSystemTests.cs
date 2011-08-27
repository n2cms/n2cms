using System;
using System.Collections.Generic;
using System.IO;
using N2.Edit.FileSystem;
using N2.Edit.FileSystem.Items;
using N2.Persistence;
using N2.Tests.Persistence;
using N2.Web;
using NUnit.Framework;
using Directory = N2.Edit.FileSystem.Items.Directory;
using File = N2.Edit.FileSystem.Items.File;

namespace N2.Edit.Tests.FileSystem.DatabaseFileSystem
{
    [TestFixture]
    public class IFileSystemTests : DatabasePreparingBase
	{
		#region Set up & tear down
		string basePath = AppDomain.CurrentDomain.BaseDirectory + @"\FileSystem\";
		IFileSystem fs;
        RootNode root;
        RootDirectory upload;
        
		List<string> operations;
		List<FileEventArgs> arguments;

        [TestFixtureSetUp]
        public override void TestFixtureSetUp()
        {
			base.TestFixtureSetUp();
			N2.Context.Replace(engine);

            Url.DefaultExtension = "/";
        	Url.ApplicationPath = "/";
            
			fs = engine.Resolve<IFileSystem>();

			fs.DirectoryCreated += (s, e) => Triggered("DirectoryCreated", e);
			fs.DirectoryDeleted += (s, e) => Triggered("DirectoryDeleted", e);
			fs.DirectoryMoved += (s, e) => Triggered("DirectoryMoved", e);
			fs.FileCopied += (s, e) => Triggered("FileCopied", e);
			fs.FileDeleted += (s, e) => Triggered("FileDeleted", e);
			fs.FileMoved += (s, e) => Triggered("FileMoved", e);
			fs.FileWritten += (s, e) => Triggered("FileWritten", e);
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

            operations = new List<string>();
            arguments = new List<FileEventArgs>();

			CopyFilesRecursively(new DirectoryInfo(basePath), "/", fs);

			operations = new List<string>();
			arguments = new List<FileEventArgs>();

			root = engine.Resolve<ContentActivator>().CreateInstance<RootNode>(null);
			engine.Persister.Save(root);
			engine.Resolve<IHost>().DefaultSite.RootItemID = root.ID;
			engine.Resolve<IHost>().DefaultSite.StartPageID = root.ID;

			upload = engine.Resolve<ContentActivator>().CreateInstance<RootDirectory>(root);
            upload.Title = "Upload";
            upload.Name = "Upload";
			engine.Persister.Save(upload);			
		}

		[TearDown]
		public override void TearDown()
		{
            // It is of course wrong to use the IFileSystem method to clean up
			fs.DeleteDirectory("/");
			base.TearDown();
		}

		public static void CopyFilesRecursively(DirectoryInfo source, string targetVirtualPath, IFileSystem fs)
		{
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                string newDirVirtualPath = Path.Combine(targetVirtualPath, dir.Name);
                fs.CreateDirectory(newDirVirtualPath);
                CopyFilesRecursively(dir, newDirVirtualPath, fs);
            }
            foreach (FileInfo file in source.GetFiles())
            {
                fs.WriteFile(Path.Combine(targetVirtualPath, file.Name), file.OpenRead());
            }
		}

		void Triggered(string operation, FileEventArgs args)
		{
			operations.Add(operation);
			arguments.Add(args);
		}
		#endregion

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
			string sourcePath = "/Upload/Folder 2/Folder 3/File 3.txt";
			string destinationPath = @"/Upload/Folder1/File 3.txt";
			Directory d = (Directory)upload.GetChild("Folder1");
            try
            {
                f.AddTo(d);
                Assert.That(fs.FileExists(destinationPath));
                Assert.That(!fs.FileExists(sourcePath));
            }
            finally
            {
				if(fs.FileExists(destinationPath))
					fs.MoveFile(destinationPath, sourcePath);
            }
		}

		[Test]
		public void MovingFile_TriggersEvent()
		{
			File f = (File)upload.GetChild("Folder 2/Folder 3/File 3.txt");
			string sourcePath = @"/Upload/Folder 2/Folder 3/File 3.txt";
			string destinationPath = @"/Upload/Folder1/File 3.txt";
			Directory d = (Directory)upload.GetChild("Folder1");
			try
			{
				f.AddTo(d);

				Assert.That(arguments[0].SourcePath, Is.EqualTo(@"/Upload/Folder 2/Folder 3/File 3.txt"));
				Assert.That(arguments[0].VirtualPath, Is.EqualTo(@"/Upload/Folder1/File 3.txt"));
				Assert.That(operations[0], Is.EqualTo("FileMoved"));
			}
			finally
			{
				if (System.IO.File.Exists(destinationPath))
					System.IO.File.Move(destinationPath, sourcePath);
			}
		}

        [Test]
        public void CanMoveFile_ToRootDirectory()
        {
			File f = (File)upload.GetChild("Folder 2/File 2.txt");
			string sourcePath = @"/Upload/Folder 2/File 2.txt";
			string destinationPath = @"/Upload/File 2.txt";
            try
            {
                f.AddTo(upload);
                Assert.That(fs.FileExists(destinationPath));
                Assert.That(!fs.FileExists(sourcePath));
            }
            finally
            {
				if (fs.FileExists(destinationPath))
					fs.MoveFile(destinationPath, sourcePath);
            }
        }

    	[Test]
        public void CanMoveDirectory_ToOtherDirectory()
        {
			Directory d = (Directory)upload.GetChild("Folder 2/Folder 3");
			string sourcePath = "/Upload/Folder 2/Folder 3";
    	    string fileInSourcePath = Path.Combine(sourcePath, "File 3.txt");
            string destinationPath = "/Upload/Folder1/Folder 3";
            string fileInDestinationPath = Path.Combine(destinationPath, "File 3.txt");
            try
            {
				d.AddTo(upload.GetChild("Folder1"));
                Assert.That(fs.DirectoryExists(destinationPath));
                Assert.That(fs.FileExists(fileInDestinationPath));
                Assert.That(!fs.DirectoryExists(sourcePath));
                Assert.That(!fs.FileExists(fileInSourcePath));
            }
            finally
            {
				if(fs.DirectoryExists(destinationPath))
					fs.MoveDirectory(destinationPath, sourcePath);
            }
		}

		[Test]
		public void MoveDirectory_TriggersEvent()
		{
			Directory d = (Directory)upload.GetChild("Folder 2/Folder 3");
			string sourcePath = "/Upload/Folder 2/Folder 3";
			string destinationPath = "/Upload/Folder1/Folder 3";
			try
			{
				d.AddTo(upload.GetChild("Folder1"));
				Assert.That(arguments[0].SourcePath, Is.EqualTo("/Upload/Folder 2/Folder 3/"));
				Assert.That(arguments[0].VirtualPath, Is.EqualTo("/Upload/Folder1/Folder 3"));
				Assert.That(operations[0], Is.EqualTo("DirectoryMoved"));
			}
			finally
			{
				if (fs.DirectoryExists(destinationPath))
					fs.MoveDirectory(destinationPath, sourcePath);
			}
		}

        [Test]
        public void CanMoveDirectory_ToRootDirectory()
        {
			Directory d = (Directory)upload.GetChild("Folder 2/Folder 3");
			string sourcePath = "/Upload/Folder 2/Folder 3";
            string destinationPath = "/Upload/Folder 3";
            try
            {
                d.AddTo(upload);
                Assert.That(fs.DirectoryExists(destinationPath));
                Assert.That(!fs.DirectoryExists(sourcePath));
            }
            finally
            {
				if (fs.DirectoryExists(destinationPath))
					fs.MoveDirectory(destinationPath, sourcePath);
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

		[Test]
		public void CopyAndDeleteFile_TriggersCopy()
		{
			CopyAndDelete(delegate(File from, Directory to)
			{
				return from.CopyTo(to);
			});

			Assert.That(operations[0], Is.EqualTo("FileCopied"));
			Assert.That(arguments[0].SourcePath, Is.EqualTo("/Upload/Folder1/File1.txt"));
			Assert.That(arguments[0].VirtualPath, Is.EqualTo("/Upload/Folder 2/File1.txt"));
		}

		[Test]
		public void CopyAndDeleteFile_TriggersDelete()
		{
			CopyAndDelete(delegate(File from, Directory to)
			{
				return from.CopyTo(to);
			});

			Assert.That(operations[1], Is.EqualTo("FileDeleted"));
			Assert.That(arguments[1].SourcePath, Is.EqualTo(null));
			Assert.That(arguments[1].VirtualPath, Is.EqualTo("/Upload/Folder 2/File1.txt"));
		}

		private void AssertMovement(Action<File, Directory> moveAction)
		{
			Directory sourceDirectory = (Directory)upload.GetChild("Folder1");
			Directory destinationDirectory = (Directory)upload.GetChild("Folder 2");
			File f = (File)sourceDirectory.GetChild("File1.txt");

			moveAction(f, destinationDirectory);
			Assert.That(sourceDirectory.GetChild("File1.txt"), Is.Null);
			Assert.That(f.Parent, Is.EqualTo(destinationDirectory));
			Assert.That(destinationDirectory.GetChild("File1.txt"), Is.Not.Null);
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
    }
}
