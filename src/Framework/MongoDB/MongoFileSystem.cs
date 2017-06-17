using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using N2.Configuration;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Engine;

namespace N2.Persistence.MongoDB
{
    [Service(typeof(IFileSystem), Configuration = "mongofs", Replaces = typeof(MappedFileSystem))]
    public class MongoFileSystem : IFileSystem
    {
        private readonly ConfigurationManagerWrapper config;

        public MongoFileSystem(ConfigurationManagerWrapper config)
        {
            this.config = config;
            Init();
        }

        #region IFileSystem implementation

        public IEnumerable<FileData> GetFiles(string parentVirtualPath)
        {
            var query = Query.And(Query.EQ("metadata.directory_name", FixPath(parentVirtualPath)), Query.EQ("metadata.is_directory", new BsonBoolean(false)));
            var fileInfos = GetGridFS().Find(query);

            if (!fileInfos.Any())
            {
                return new FileData[0];
            }

            IEnumerable<FileData> fileDatas = fileInfos.Select(GetFileData).OrderBy(f => f.Name);
            return fileDatas;
        }

        public FileData GetFile(string virtualPath)
        {
            var fi = GetGridFS().FindOne(FixPath(virtualPath));
            return GetFileData(fi);
        }

        public IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath)
        {
            var query = Query.And(Query.EQ("metadata.directory_name", FixPath(parentVirtualPath)), Query.EQ("metadata.is_directory", new BsonBoolean(true)));
            var fileInfos = GetGridFS().Find(query);

            if (!fileInfos.Any())
            {
                return new DirectoryData[0];
            }

            IEnumerable<DirectoryData> directoryDatas = fileInfos.Select(GetDirectoryData).OrderBy(d => d.Name);
            return directoryDatas;
        }

        public DirectoryData GetDirectory(string virtualPath)
        {
            var fi = GetGridFS().FindOne(FixPath(virtualPath));
            return GetDirectoryData(fi);
        }

        /// <summary>Searches for files in all Upload Directories.</summary>
        /// <param name="query">The search term</param>
        /// <param name="uploadDirectories">All Upload Directories</param>
        /// <returns>An enumeration of files matching the query.</returns>
        public IEnumerable<FileData> SearchFiles(string query, List<Collections.HierarchyNode<ContentItem>> uploadDirectories)
        {
            //Not implemented
            return null;
        }


        public bool FileExists(string virtualPath)
        {
            return GetGridFS().Exists(FixPath(virtualPath));
        }

        public void MoveFile(string fromVirtualPath, string destinationVirtualPath)
        {
            InternalMoveFile(fromVirtualPath, destinationVirtualPath);

            if (FileMoved != null)
            {
                FileMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
            }
        }

        public void DeleteFile(string virtualPath)
        {
            GetGridFS().Delete(FixPath(virtualPath));

            if (FileDeleted != null)
            {
                FileDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        public void CopyFile(string fromVirtualPath, string destinationVirtualPath)
        {
            MongoGridFS mongoGridFs = GetGridFS();

            string fixedDestinationPath = FixPath(destinationVirtualPath);
            mongoGridFs.CopyTo(FixPath(fromVirtualPath), fixedDestinationPath);

            MongoGridFSFileInfo fileInfo = mongoGridFs.FindOne(fixedDestinationPath);
            mongoGridFs.SetMetadata(fileInfo, CreateMetadata(fixedDestinationPath, false));

            if (FileCopied != null)
            {
                FileCopied.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
            }
        }

        public Stream OpenFile(string virtualPath, bool readOnly = false)
        {
            string fixedPath = FixPath(virtualPath);

            if (!FileExists(fixedPath) && !readOnly)
            {
                InternalWriteFile(fixedPath, new MemoryStream(new byte[0]));
            }

            MongoGridFS gridFs = GetGridFS();
            if (readOnly)
            {
                return gridFs.OpenRead(fixedPath);
            }

            MongoGridFSFileInfo fileInfo = gridFs.FindOne(fixedPath);
            gridFs.SetMetadata(fileInfo, CreateMetadata(fixedPath, false));

            return gridFs.OpenWrite(fixedPath);
        }

        public void WriteFile(string virtualPath, Stream inputStream)
        {
            string fixedPath = FixPath(virtualPath);
            InternalWriteFile(fixedPath, inputStream);

            if (FileWritten != null)
            {
                FileWritten.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        public void ReadFileContents(string virtualPath, Stream outputStream)
        {
            using (Stream readStream = GetGridFS().OpenRead(FixPath(virtualPath)))
            {
                readStream.CopyTo(outputStream);
            }
        }

        public bool DirectoryExists(string virtualPath)
        {
            var fileInfo = GetGridFS().FindOne(FixPath(virtualPath));

            if (fileInfo != null && fileInfo.Metadata["is_directory"] == new BsonBoolean(true))
            {
                return true;
            }

            return false;
        }

        public void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
        {
            string fixedPath = FixPath(fromVirtualPath);
            MongoGridFS mongoGridFs = GetGridFS();
            var fileInfos = mongoGridFs.Find(Query.Matches("metadata.directory_name", new BsonRegularExpression(new Regex(@"^" + fixedPath))));

            foreach (var fi in fileInfos)
            {
                string newFileName = FixPath(destinationVirtualPath + "/" + fi.Name.Substring(fromVirtualPath.Length));
                InternalMoveFile(fi.Name, newFileName);
            }

            InternalMoveFile(fromVirtualPath, destinationVirtualPath);

            if (DirectoryMoved != null)
            {
                DirectoryMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
            }
        }

        public void DeleteDirectory(string virtualPath)
        {
            string fixedPath = FixPath(virtualPath);
            MongoGridFS mongoGridFs = GetGridFS();
            var fileInfos = mongoGridFs.Find(Query.Matches("metadata.directory_name", new BsonRegularExpression(new Regex(@"^" + fixedPath))));

            foreach (var fi in fileInfos)
            {
                mongoGridFs.DeleteById(fi.Id);
            }

            mongoGridFs.Delete(fixedPath);

            if (DirectoryDeleted != null)
            {
                DirectoryDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        public void CreateDirectory(string virtualPath)
        {
            string fixedPath = FixPath(virtualPath);
            GetGridFS().Upload(new MemoryStream(new byte[0]), fixedPath, new MongoGridFSCreateOptions
            {
                Metadata = CreateMetadata(fixedPath, true)
            });

            if (DirectoryCreated != null)
            {
                DirectoryCreated.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        public event EventHandler<FileEventArgs> FileWritten;
        public event EventHandler<FileEventArgs> FileCopied;
        public event EventHandler<FileEventArgs> FileMoved;
        public event EventHandler<FileEventArgs> FileDeleted;
        public event EventHandler<FileEventArgs> DirectoryCreated;
        public event EventHandler<FileEventArgs> DirectoryMoved;
        public event EventHandler<FileEventArgs> DirectoryDeleted;

        #endregion

        #region Internal methods

        private void Init()
        {
            IndexKeysBuilder keys = IndexKeys.Ascending("metadata.directory_name");
            IndexOptionsBuilder options = IndexOptions.SetName("directory_name").SetBackground(false);
            MongoGridFS mongoGridFs = GetGridFS();
            mongoGridFs.EnsureIndexes();
            mongoGridFs.Files.EnsureIndex(keys, options);
        }

        private static BsonDocument CreateMetadata(string fixedPath, bool isDirectory)
        {
            return new BsonDocument
            {
                { "directory_name", GetDirectoryName(fixedPath) },
                { "is_directory", isDirectory },
                { "modified_date", DateTime.UtcNow }
            };
        }

        private void InternalWriteFile(string fixedPath, Stream inputStream)
        {
            GetGridFS().Upload(inputStream, fixedPath, new MongoGridFSCreateOptions
            {
                Metadata = CreateMetadata(fixedPath, false)
            });
        }

        private void InternalMoveFile(string fromVirtualPath, string destinationVirtualPath)
        {
            MongoGridFS mongoGridFs = GetGridFS();
            string fixedDestinationPath = FixPath(destinationVirtualPath);
            mongoGridFs.MoveTo(FixPath(fromVirtualPath), fixedDestinationPath);
            MongoGridFSFileInfo fileInfo = mongoGridFs.FindOne(fixedDestinationPath);
            mongoGridFs.SetMetadata(fileInfo, CreateMetadata(fixedDestinationPath, fileInfo.Metadata["is_directory"] == new BsonBoolean(true)));
        }

        private MongoDatabase GetMongoDatabase()
        {
            var connectionString = config.GetConnectionString();
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.ConnectTimeout = TimeSpan.FromSeconds(10);
            var client = new MongoClient(settings);

            var server = client.GetServer();
            var databaseSettings = new MongoDatabaseSettings()
            {
                ReadPreference = ReadPreference.Nearest,
                WriteConcern = WriteConcern.Acknowledged,
            };
            var database = server.GetDatabase(config.Sections.Database.TablePrefix, databaseSettings);
            return database;
        }

        private MongoGridFS GetGridFS()
        {
            return GetMongoDatabase().GridFS;
        }

        public string FixPath(string virtualPath)
        {
            if (virtualPath.StartsWith("~"))
            {
                virtualPath = virtualPath.Substring(1);
            }

            if (virtualPath.EndsWith("/"))
            {
                virtualPath = virtualPath.Substring(0, virtualPath.Length - 1);
            }

            if (!virtualPath.StartsWith("/"))
            {
                virtualPath = "/" + virtualPath;
            }

            return virtualPath.Replace("//", "/").ToLower().Trim();
        }

        public static string Combine(string path1, string path2)
        {
            path1 = path1.TrimEnd('/');
            path2 = path2.TrimStart('/');
            return string.Format("{0}/{1}", path1, path2);
        }

        private static string GetDirectoryName(string path)
        {
            return path.Substring(0, path.LastIndexOf('/'));
        }

        private static string GetName(string path)
        {
            return path.Substring(path.LastIndexOf('/') + 1);
        }

        private DirectoryData GetDirectoryData(MongoGridFSFileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                return null;
            }

            DateTime modifiedDateTime = fileInfo.UploadDate;
            if (fileInfo.Metadata.Contains("modified_date"))
            {
                modifiedDateTime = (fileInfo.Metadata["modified_date"] as BsonDateTime).ToUniversalTime();
            }

            return new DirectoryData
            {
                Created = fileInfo.UploadDate,
                Updated = modifiedDateTime,
                Name = GetName(fileInfo.Name),
                VirtualPath = fileInfo.Name,
            };
        }

        private FileData GetFileData(MongoGridFSFileInfo fileInfo)
        {
            DateTime modifiedDateTime = fileInfo.UploadDate;
            if (fileInfo.Metadata.Contains("modified_date"))
            {
                modifiedDateTime = (fileInfo.Metadata["modified_date"] as BsonDateTime).ToUniversalTime();
            }

            return new FileData
            {
                Created = fileInfo.UploadDate,
                Updated = modifiedDateTime,
                Name = GetName(fileInfo.Name),
                VirtualPath = fileInfo.Name,
                Length = fileInfo.Length
            };
        }

        #endregion
    }
}
