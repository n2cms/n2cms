using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using N2.Configuration;
using N2.Edit.FileSystem;
using N2.Edit.FileSystem.NH;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using N2.Tests.Persistence;
using NUnit.Framework;
using System.Threading;
using N2.Web;

namespace N2.Edit.Tests.FileSystem
{

    [TestFixture]
    public class FileSystemTests_DatebaseFileSystem : FileSystemTests
    {
        protected override IFileSystem CreateFileSystem()
        {
            return new DatabaseFileSystem(engine.Resolve<ISessionProvider>(), new DatabaseSection { Files = new FilesElement { ChunkSize = 100 } });
        }
    }
    
    [TestFixture]
    public class FileSystemTests_VirtualFileSystem : FileSystemTests
    {
        protected override IFileSystem CreateFileSystem()
        {
            return new FakeVppFileSystem { PathProvider = new FakePathProvider(AppDomain.CurrentDomain.BaseDirectory) };
        }
    }

    [TestFixture]
    public class FileSystemTests_MappedFileSystem : FileSystemTests
    {
        protected override IFileSystem CreateFileSystem()
        {
            return new FakeMappedFileSystem();
        }
    }

    public abstract class FileSystemTests : DatabasePreparingBase
    {
        #region Set up & tear down
        protected IFileSystem fs;
        
        List<string> operations;
        List<FileEventArgs> arguments;

        [TestFixtureSetUp]
        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            fs = CreateFileSystem();

            fs.DirectoryCreated += (s, e) => Triggered("DirectoryCreated", e);
            fs.DirectoryDeleted += (s, e) => Triggered("DirectoryDeleted", e);
            fs.DirectoryMoved += (s, e) => Triggered("DirectoryMoved", e);
            fs.FileCopied += (s, e) => Triggered("FileCopied", e);
            fs.FileDeleted += (s, e) => Triggered("FileDeleted", e);
            fs.FileMoved += (s, e) => Triggered("FileMoved", e);
            fs.FileWritten += (s, e) => Triggered("FileWritten", e);

            
        }

        protected abstract IFileSystem CreateFileSystem();

        [TestFixtureTearDown]
        public override void TestFixtureTearDown()
        {
            base.TestFixtureTearDown();

            Url.DefaultExtension = ".aspx";
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            operations = new List<string>();
            arguments = new List<FileEventArgs>();
        }

        [TearDown]
        public override void TearDown()
        {
            if (fs.DirectoryExists("/upload/"))
            {
                try
                {
                    fs.DeleteDirectory("/upload/");
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                    System.Diagnostics.Debug.WriteLine("Trying to recover from " + ex.Message + " with files: " + string.Join(", ", fs.GetDirectories("/upload/").Select(f => f.VirtualPath).Concat(fs.GetFiles("/upload/").Select(f => f.VirtualPath))));
                    fs.DeleteDirectory("/upload/");
                }
            }

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
        public void GetFiles_RetrievesFiles_BelowRoot()
        {
            fs.WriteFile("/upload/hello.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));

            var files = fs.GetFiles("/upload/");

            Assert.That(files.Single().Name, Is.EqualTo("hello.txt"));
        }

        [Test]
        public void GetFiles_DoesntRetrieve_FilesNotThere()
        {
            fs.CreateDirectory("/upload/directory1");

            var files = fs.GetFiles("/upload/");

            Assert.That(files.Any(), Is.False);
        }

        [Test]
        public void GetFiles_RetrievesFiles_BelowSubdirectory()
        {
            fs.CreateDirectory("/upload/world");
            fs.WriteFile("/upload/world/hello.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));

            var files = fs.GetFiles("/upload/world/");

            Assert.That(files.Single().Name, Is.EqualTo("hello.txt"));
        }

        [Test]
        public void GetDirectory_ListsDirectory_BelowRootDirectory()
        {
            fs.CreateDirectory("/upload/world");

            var directories = fs.GetDirectories("/upload");

            Assert.That(directories.Single().Name, Is.EqualTo("world"));
        }

        [Test]
        public void GetDirectory_ListsDirectory_BelowSubDirectory()
        {
            fs.CreateDirectory("/upload/hello/world");

            var directories = fs.GetDirectories("/upload/hello/");

            Assert.That(directories.Single().Name, Is.EqualTo("world"));
        }

        [Test]
        public void GetDirectory_GetsRootDirectory()
        {
            fs.CreateDirectory("/upload");
            var d = fs.GetDirectory("/upload");

            Assert.That(d, Is.Not.Null);
            Assert.That(d.Name, Is.EqualTo("upload"));
        }

        [Test]
        public void GetDirectory_GetsSubDirectory_ByName()
        {
            fs.CreateDirectory("/upload/hello");
            var d = fs.GetDirectory("/upload/hello");

            Assert.That(d, Is.Not.Null);
            Assert.That(d.Name, Is.EqualTo("hello"));
        }

        [Test]
        public void WriteFile_ToRootDirectory_AutoGenerates_RootDirectory()
        {
            fs.WriteFile("/upload/hello.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));
            var d = fs.GetDirectory("/upload");

            Assert.That(d, Is.Not.Null);
            Assert.That(d.Name, Is.EqualTo("upload"));
        }

        [Test]
        public void CreateDirectory_BelowRootDirectory_AutoGenerates_RootDirectory()
        {
            fs.CreateDirectory("/upload/hello");
            var d = fs.GetDirectory("/upload");

            Assert.That(d, Is.Not.Null);
            Assert.That(d.Name, Is.EqualTo("upload"));
        }

        [Test]
        public void GetFile_LoadsFileSize()
        {
            var b = Encoding.UTF8.GetBytes("hello world");
            fs.WriteFile("/upload/hello.txt", new MemoryStream(b));

            var f = fs.GetFile("/upload/hello.txt");

            Assert.That(f.Length, Is.EqualTo(b.Length));
        }

        [Test]
        public void ReadFileContents_LoadsFileContents()
        {
            var writeBuffer = Encoding.UTF8.GetBytes("hello world");
            fs.WriteFile("/upload/hello.txt", new MemoryStream(writeBuffer));

            var readBuffer = new byte[writeBuffer.Length];
            var s = new MemoryStream(readBuffer);
            fs.ReadFileContents("/upload/hello.txt", s);

            Assert.That(readBuffer, Is.EquivalentTo(writeBuffer));
        }

        [Test]
        public void MoveFile_ToOtherDirectory_FileIsInOtherDirectory()
        {
            fs.CreateDirectory("/upload/world");
            fs.WriteFile("/upload/hello.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));

            fs.MoveFile("/upload/hello.txt", "/upload/world/hello.txt");

            var f = fs.GetFile("/upload/world/hello.txt");

            Assert.That(f, Is.Not.Null);
            Assert.That(f.VirtualPath, Is.EqualTo("/upload/world/hello.txt"));
        }

        [Test]
        public void MoveFile_TriggersEvent()
        {
            fs.CreateDirectory("/upload/world");
            fs.WriteFile("/upload/hello.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));

            fs.MoveFile("/upload/hello.txt", "/upload/world/hello.txt");

            Assert.That(this.arguments.Last().SourcePath, Is.EqualTo("/upload/hello.txt"));
            Assert.That(this.arguments.Last().VirtualPath, Is.EqualTo("/upload/world/hello.txt"));
            Assert.That(this.operations.Last(), Is.EqualTo("FileMoved"));
        }

        [Test]
        public void MoveDirectory_ToOtherDirectory_DirectoryIsInOtherDirectory()
        {
            fs.CreateDirectory("/upload/hello");
            fs.CreateDirectory("/upload/world");

            fs.MoveDirectory("/upload/hello", "/upload/world/hello");

            var from = fs.GetDirectory("/upload/hello");
            var to = fs.GetDirectory("/upload/world/hello");

            Assert.That(from, Is.Null);
            Assert.That(to, Is.Not.Null);
        }

        [Test]
        public void MoveDirectory_ToOtherDirectory_SubdirectoryIsInOtherDirectory()
        {
            fs.CreateDirectory("/upload/hello");
            fs.CreateDirectory("/upload/hello/world");
            fs.CreateDirectory("/upload/destination");

            fs.MoveDirectory("/upload/hello", "/upload/destination/hello");

            var to = fs.GetDirectory("/upload/destination/hello/world");

            Assert.That(to, Is.Not.Null);
        }

        [Test]
        public void MoveDirectory_ToOtherDirectory_FileIsInOtherDirectory()
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));
            fs.CreateDirectory("/upload/destination");

            fs.MoveDirectory("/upload/hello", "/upload/destination/hello");

            var from = fs.GetFile("/upload/hello/world.txt");
            var to = fs.GetFile("/upload/destination/hello/world.txt");

            Assert.That(from, Is.Null);
            Assert.That(to, Is.Not.Null);
        }

        [Test]
        public void MoveDirectory_TriggersEvent()
        {
            fs.CreateDirectory("/upload/world");
            fs.CreateDirectory("/upload/hello");

            fs.MoveDirectory("/upload/world", "/upload/hello/world");

            Assert.That(this.arguments.Last().SourcePath, Is.EqualTo("/upload/world"));
            Assert.That(this.arguments.Last().VirtualPath, Is.EqualTo("/upload/hello/world"));
            Assert.That(this.operations.Last(), Is.EqualTo("DirectoryMoved"));
        }

        [Test]
        public void CopyFile_FileIsInBothLocations()
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));
            fs.CreateDirectory("/upload/destination");

            fs.CopyFile("/upload/hello/world.txt", "/upload/destination/world.txt");

            var from = fs.GetFile("/upload/hello/world.txt");
            var to = fs.GetFile("/upload/destination/world.txt");

            Assert.That(from, Is.Not.Null);
            Assert.That(to, Is.Not.Null);
        }

        [Test]
        public void CopyFile_TriggersEvent()
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));
            fs.CreateDirectory("/upload/destination");

            fs.CopyFile("/upload/hello/world.txt", "/upload/destination/world.txt");

            Assert.That(operations.Last(), Is.EqualTo("FileCopied"));
            Assert.That(arguments.Last().SourcePath, Is.EqualTo("/upload/hello/world.txt"));
            Assert.That(arguments.Last().VirtualPath, Is.EqualTo("/upload/destination/world.txt"));
        }

        [Test]
        public void DeleteFile_RemovesFile()
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));

            fs.DeleteFile("/upload/hello/world.txt");

            var f = fs.GetFile("/upload/hello/world.txt");
            Assert.That(f, Is.Null);
        }

        [Test]
        public void DeleteDirectory_RemovesDirectory()
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));

            fs.DeleteDirectory("/upload/hello/");

            var d = fs.GetDirectory("/upload/hello");
            Assert.That(d, Is.Null);
        }

        [Test]
        public void DeleteDirectory_RemovesContainedFile()
        {
            fs.CreateDirectory("/upload/hello");

            fs.DeleteDirectory("/upload/hello/");

            var f = fs.GetFile("/upload/hello/world.txt");
            Assert.That(f, Is.Null);
        }

        [TestCase("")]
        [TestCase("hello world")]
        [TestCase("hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world")]
        [TestCase("hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world")]
        [TestCase("hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world hello world")]
        public void OpenFile_ReadsFileSizes(string text)
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes(text)));

            using (var s = fs.OpenFile("/upload/hello/world.txt"))
            using (var sr = new StreamReader(s))
            {
                var contents = sr.ReadToEnd();

                Assert.That(contents, Is.EqualTo(text));
            }
        }

        [Test]
        public void WriteFile_ModifiesFileContents()
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("world hello")));

            var buffer = new byte["world".Length];
            var ms = new MemoryStream(buffer);

            using (var s = new StreamReader(fs.OpenFile("/upload/hello/world.txt")))
            {
                var contents = s.ReadToEnd();

                Assert.That(contents, Is.EqualTo("world hello"));
            }
        }

        [Test]
        public void WriteFile_ExpandFileContents()
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello")));
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));

            var buffer = new byte["world".Length];
            var ms = new MemoryStream(buffer);

            using (var s = new StreamReader(fs.OpenFile("/upload/hello/world.txt")))
            {
                var contents = s.ReadToEnd();

                Assert.That(contents, Is.EqualTo("hello world"));
            }
        }

        [Test]
        public void WriteFile_ReduceFileContents()
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello world")));
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("world")));

            var buffer = new byte["world".Length];
            var ms = new MemoryStream(buffer);

            using (var s = new StreamReader(fs.OpenFile("/upload/hello/world.txt")))
            {
                var contents = s.ReadToEnd();

                Assert.That(contents, Is.EqualTo("world"));
            }
        }

        [Test]
        public void OpenFile_ToWriteFileContents_ChangesFile()
        {
            fs.CreateDirectory("/upload/hello");
            fs.WriteFile("/upload/hello/world.txt", new MemoryStream(Encoding.UTF8.GetBytes("hello")));

            using (var s = fs.OpenFile("/upload/hello/world.txt"))
            {
                var buffer = Encoding.UTF8.GetBytes("world");
                s.Write(buffer, 0, buffer.Length);
            }

            using (var s = new StreamReader(fs.OpenFile("/upload/hello/world.txt")))
            {
                var contents = s.ReadToEnd();

                Assert.That(contents, Is.EqualTo("world"));
            }
        }
    }
}
