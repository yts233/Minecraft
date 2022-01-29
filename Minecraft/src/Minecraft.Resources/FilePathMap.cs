using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Minecraft.Resources
{
    public class FilePathMap : IFilePath
    {
        private readonly string _pathName;
        private readonly FilePathMap _root;
        private readonly FilePathMap _up;
        private readonly IEnumerable<IFilePath> _currentPaths;

        public FilePathMap(IEnumerable<IFilePath> basePaths)
        {
            _root = this;
            _pathName = "";
            _up = this;
            _currentPaths = basePaths.Where(p => p.Exists).ToList();
        }

        public FilePathMap(FilePathMap basePath, string fileOrDirectoryName)
        {
            _up = basePath;
            _root = basePath._root;
            _pathName = basePath._pathName == "" ? fileOrDirectoryName : $"{basePath._pathName}/{fileOrDirectoryName}";
            _currentPaths = basePath._currentPaths.Select(p => p[fileOrDirectoryName]).Where(p => p.Exists).ToList();
        }

        void IDisposable.Dispose()
        {
            foreach (var basePath in _currentPaths)
            {
                basePath.Dispose();
            }
        }

        IFilePath IFilePath.this[string path] => new FilePathMap(this, path);

        string IFilePath.PathName => _pathName;

        bool IFilePath.IsFile => _currentPaths.Any(p => p.IsFile);

        bool IFilePath.IsDirectory => _currentPaths.Any(p => p.IsDirectory);

        IFilePath IFilePath.Root => _root;

        IFilePath IFilePath.Up => _up;

        IEnumerable<IFilePath> IFilePath.GetChildren()
        {
            var tmp = new HashSet<string>();

            static string GetName(IFilePath filePath)
            {
                try
                {
                    return filePath.GetFileName();
                }
                catch
                {
                    return filePath.GetDirectoryName();
                }
            }

            foreach (var s in _currentPaths.SelectMany(p => p.GetChildren().Where(q => q.Exists).Select(GetName)))
            {
                tmp.Add(s);
            }

            return tmp.Select(n => new FilePathMap(this, n));
        }

        Stream IFilePath.OpenRead()
        {
            return _currentPaths.First(p => p.IsFile).OpenRead();
        }

        Stream IFilePath.OpenWrite()
        {
            return _currentPaths.First(p => p.IsFile).OpenWrite();
        }

        TextReader IFilePath.OpenText()
        {
            return _currentPaths.First(p => p.IsFile).OpenText();
        }

        public override string ToString()
        {
            return _pathName;
        }
    }
}