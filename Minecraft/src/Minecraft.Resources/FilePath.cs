using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Minecraft.Resources
{
    /// <summary>
    /// 文件路径类
    /// </summary>
    public sealed class FilePath : IFilePath
    {
        private readonly string _pathName;

        /// <summary>
        /// 从当前路径开始
        /// </summary>
        public FilePath() : this(Directory.GetCurrentDirectory())
        {
        }

        /// <summary>
        /// 从指定路径开始
        /// </summary>
        /// <param name="path"></param>
        public FilePath(string path)
        {
            _pathName = Path.GetFullPath(path);
        }

        string IFilePath.PathName => _pathName;

        bool IFilePath.IsFile => File.Exists(_pathName);
        bool IFilePath.IsDirectory => Directory.Exists(_pathName);

        IFilePath IFilePath.this[string path] => path.IndexOfAny(Path.GetInvalidFileNameChars()) == -1
            ? new FilePath(Path.Combine(_pathName, path))
            : new FilePath(path);

        IFilePath IFilePath.Root => new FilePath(Path.GetPathRoot(_pathName));

        IFilePath IFilePath.Up => ((IFilePath) this)[".."];

        IEnumerable<IFilePath> IFilePath.GetChildren()
        {
            return ((IFilePath) this).GetFiles().Concat(((IFilePath) this).GetDirectories());
        }

        IEnumerable<IFilePath> IFilePath.GetFiles()
        {
            return Directory.GetFiles(_pathName).Select(path => ((IFilePath) this)[path]).ToList();
        }

        IEnumerable<IFilePath> IFilePath.GetDirectories()
        {
            return Directory.GetDirectories(_pathName).Select(path => ((IFilePath) this)[path]).ToList();
        }

        Stream IFilePath.OpenRead()
        {
            return File.OpenRead(_pathName);
        }

        Stream IFilePath.OpenWrite()
        {
            return File.OpenWrite(_pathName);
        }

        TextReader IFilePath.OpenText()
        {
            return File.OpenText(_pathName);
        }

        string IFilePath.GetFileName()
        {
            return Path.GetFileName(_pathName);
        }

        string IFilePath.GetDirectoryName()
        {
            return Path.GetDirectoryName(_pathName);
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return _pathName;
        }

        public override bool Equals(object obj)
        {
            return obj is FilePath filePath && filePath._pathName == _pathName;
        }

        public override int GetHashCode()
        {
            return _pathName != null ? _pathName.GetHashCode() : 0;
        }
    }
}