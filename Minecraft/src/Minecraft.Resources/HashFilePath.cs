using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Minecraft.Resources
{
    public class HashFilePath : IFilePath
    {
        private readonly IFilePath _basePath;
        private readonly Dictionary<string, string> _objects = new Dictionary<string, string>();
        private readonly ICollection<string> _paths;
        private readonly HashFilePath _root;

        private readonly string _currentPath;

        /// <summary>
        /// 创建一个<see cref="HashFilePath"/>
        /// </summary>
        /// <param name="basePath">Path to objects</param>
        /// <param name="indexPath">Path to asset index file</param>
        public HashFilePath(IFilePath basePath, IFilePath indexPath)
        {
            _basePath = basePath;
            _currentPath = "";
            using var stream = indexPath.OpenRead();
            using var json = JsonDocument.Parse(stream);

            foreach (var property in json.RootElement.GetProperty("objects").EnumerateObject())
                _objects.Add(property.Name, property.Value.GetProperty("hash").GetString());

            _paths = _objects.Keys.ToList();

            _root = this;
        }

        public HashFilePath(IFilePath basePath, IFilePath indexPath, string pathName) : this(
            new HashFilePath(basePath, indexPath), pathName)
        {
        }

        public HashFilePath(HashFilePath root, string pathName)
        {
            _basePath = root._basePath;
            _objects = root._objects;
            _paths = root._paths;
            _root = root;

            _currentPath = pathName ?? throw new ArgumentNullException(nameof(pathName));
        }


        void IDisposable.Dispose()
        {
        }

        IFilePath IFilePath.this[string path]
        {
            get
            {
                if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
                return new HashFilePath(_root,
                    _currentPath == ""
                        ? path
                        : $"{_currentPath}/{path}");
            }
        }

        string IFilePath.PathName => _currentPath;
        bool IFilePath.IsFile => _paths.Contains(_currentPath);
        bool IFilePath.IsDirectory => !((IFilePath) this).IsFile && _paths.Any(p => p.StartsWith(_currentPath));
        IFilePath IFilePath.Root => _root;

        IFilePath IFilePath.Up =>
            _currentPath == "" || _currentPath.IndexOf('/') == -1
                ? _root
                : new HashFilePath(_root, _currentPath[.._currentPath.LastIndexOf('/')]);

        IEnumerable<IFilePath> IFilePath.GetChildren()
        {
            if (!((IFilePath) this).IsDirectory)
                return new IFilePath[0];

            var tmp = new HashSet<string>();
            if (_currentPath == "")
            {
                foreach (var path in _paths)
                {
                    tmp.Add(path.IndexOf('/') == -1 ? path : path[..path.IndexOf('/')]);
                }

                return tmp.Select(p => new HashFilePath(_root, p));
            }

            var len = _currentPath.Length + 1;
            foreach (var path in _paths.Where(p => p.StartsWith(_currentPath)))
            {
                var p = path[len..];
                var index = p.IndexOf('/');
                tmp.Add(index == -1 ? path : path[..(len + index)]);
            }

            return tmp.Select(p => new HashFilePath(_root, p));
        }

        private IFilePath GetMappedFilePath()
        {
            var hash = _objects[_currentPath];
            var filePath = _basePath[hash[..2]][hash];
            return filePath;
        }

        Stream IFilePath.OpenRead()
        {
            return GetMappedFilePath().OpenRead();
        }

        Stream IFilePath.OpenWrite()
        {
            return GetMappedFilePath().OpenWrite();
        }

        TextReader IFilePath.OpenText()
        {
            return GetMappedFilePath().OpenText();
        }

        public override string ToString()
        {
            return _currentPath;
        }
    }
}