using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Minecraft.Resources
{
    public sealed class ZipFilePath : IFilePath
    {
        private readonly ZipArchive _archive;
        private readonly string _pathName;

        /// <summary>
        /// 从Zip文件创建<see cref="ZipFilePath" />
        /// </summary>
        /// <param name="zipFile"></param>
        public ZipFilePath(IFilePath zipFile) : this(zipFile.OpenRead())
        {
        }

        /// <summary>
        /// 从Zip文件流内创建<see cref="ZipFilePath" />
        /// </summary>
        /// <param name="stream">Zip文件流</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public ZipFilePath(Stream stream) : this(new ZipArchive(stream))
        {
        }

        /// <summary>
        /// 从<see cref="ZipArchive" />内创建<see cref="ZipFilePath" />
        /// </summary>
        /// <param name="zipArchive">Zip归档</param>
        /// <param name="path">路径名</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public ZipFilePath(ZipArchive zipArchive, string path = "")
        {
            _archive = zipArchive;
            _pathName = path;
        }

        IFilePath IFilePath.this[string path]
        {
            get
            {
                var temp = _pathName == "" ? path : $"{_pathName}{path}";
                string temp2;
                return _archive.GetEntry(temp2 = $"{temp}/") == null
                    ? new ZipFilePath(_archive, temp)
                    : new ZipFilePath(_archive, temp2);
            }
        }

        string IFilePath.PathName => _pathName;

        bool IFilePath.IsFile
        {
            get
            {
                var entry = GetEntry();
                return entry != null && entry.Name != "";
            }
        }

        bool IFilePath.IsDirectory =>
            _pathName == "" || !((IFilePath) this).IsFile && GetPaths().Any(p => p.StartsWith(_pathName));

        bool IFilePath.Exists => _pathName == "" || _archive.GetEntry(_pathName) != null;

        IFilePath IFilePath.Root => new ZipFilePath(_archive);

        IFilePath IFilePath.Up
        {
            get
            {
                int index;
                return (index = _pathName.LastIndexOf('/')) == -1
                    ? ((IFilePath) this).Root
                    : new ZipFilePath(_archive, _pathName[..index]);
            }
        }

        private IEnumerable<string> GetPaths()
        {
            return _archive.Entries.Select(e => e.FullName);
        }

        IEnumerable<IFilePath> IFilePath.GetChildren()
        {
            var directoryName = GetDirectoryName(_pathName);
            return _archive.Entries
                .Where(entry =>
                    entry.FullName.EndsWith('/')
                        ? GetDirectoryName(entry.FullName[..^1]) == directoryName
                        : GetDirectoryName(entry.FullName) == directoryName)
                .Select(entry => new ZipFilePath(_archive, entry.FullName));
        }

        Stream IFilePath.OpenRead()
        {
            return _archive.GetEntry(_pathName)?.Open();
        }

        Stream IFilePath.OpenWrite()
        {
            throw new NotSupportedException();
        }

        string IFilePath.GetFileName()
        {
            return GetFileName(_pathName);
        }

        string IFilePath.GetDirectoryName()
        {
            return GetDirectoryName(_pathName);
        }

        public void Dispose()
        {
            _archive?.Dispose();
        }

        private ZipArchiveEntry GetEntry()
        {
            return _archive.GetEntry(_pathName);
        }

        private static string GetFileName(string path)
        {
            var index = path.LastIndexOf('/');
            string temp;
            return index == -1
                ? path
                : index + 1 == path.Length
                    ? (temp = path[..index])[(temp.LastIndexOf('/') + 1)..]
                    : path[(index + 1)..];
        }

        private static string GetDirectoryName(string path)
        {
            return path[..(path.LastIndexOf('/') + 1)];
        }

        public override bool Equals(object obj)
        {
            return obj is ZipFilePath path && _pathName == path._pathName;
        }

        public override int GetHashCode()
        {
            return _pathName.GetHashCode();
        }

        public override string ToString()
        {
            return _pathName;
        }
    }
}