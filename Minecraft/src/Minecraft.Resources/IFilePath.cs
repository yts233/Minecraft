using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Minecraft.Resources
{
    public interface IFilePath : IEquatable<IFilePath>, IEnumerable<IFilePath>, IDisposable
    {
        /// <summary>
        /// 以当前路径进入子路径
        /// </summary>
        /// <param name="path"></param>
        IFilePath this[string path] { get; }

        /// <summary>
        /// 当前路径名
        /// </summary>
        string PathName { get; }

        /// <summary>
        /// 是否为文件
        /// </summary>
        bool IsFile { get; }

        /// <summary>
        /// 是否为路径
        /// </summary>
        bool IsDirectory { get; }

        /// <summary>
        /// 当前路径是否存在
        /// </summary>
        bool Exists => IsFile || IsDirectory;

        /// <summary>
        /// 当前路径的根路径
        /// </summary>
        IFilePath Root { get; }

        /// <summary>
        /// 上层路径
        /// </summary>
        IFilePath Up { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetChildren().GetEnumerator();
        }

        IEnumerator<IFilePath> IEnumerable<IFilePath>.GetEnumerator()
        {
            return GetChildren().GetEnumerator();
        }

        /// <summary>
        /// 比较两路径
        /// </summary>
        /// <param name="other">另一个路径</param>
        /// <returns></returns>
        bool IEquatable<IFilePath>.Equals(IFilePath other)
        {
            return PathName == other?.PathName;
        }

        /// <summary>
        /// 获取子路径
        /// </summary>
        /// <returns></returns>
        IEnumerable<IFilePath> GetChildren();

        /// <summary>
        /// 获取子路径下的所有文件
        /// </summary>
        /// <returns></returns>
        IEnumerable<IFilePath> GetFiles()
        {
            return GetChildren().Where(path => path.IsFile);
        }

        /// <summary>
        /// 获取子路径下的所有目录
        /// </summary>
        /// <returns></returns>
        IEnumerable<IFilePath> GetDirectories()
        {
            return GetChildren().Where(path => path.IsDirectory);
        }

        /// <summary>
        /// 获取上层所有目录
        /// </summary>
        /// <returns></returns>
        IEnumerable<IFilePath> GetUpDirectories()
        {
            var filePath = this;
            IFilePath upFilePath;
            while (!Equals(upFilePath = filePath.Up, filePath))
                yield return filePath = upFilePath;
        }

        /// <summary>
        /// 获取此路径是否为指定路径的父路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        bool IsParentOf(IFilePath filePath)
        {
            return filePath.GetUpDirectories().Contains(this);
        }

        /// <summary>
        /// 获取此路径是否为指定路径的子路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        bool IsChildOf(IFilePath filePath)
        {
            return GetUpDirectories().Contains(filePath);
        }

        /// <summary>
        /// 打开读取流
        /// </summary>
        /// <returns></returns>
        Stream OpenRead();

        /// <summary>
        /// 读取所有字节
        /// </summary>
        /// <returns></returns>
        byte[] ReadAllBytes()
        {
            using var stream = OpenRead();
            var buffer = new Span<byte>(new byte[stream.Length]);
            stream.Read(buffer);
            return buffer.ToArray();
        }

        /// <summary>
        /// 打开写入流
        /// </summary>
        /// <returns></returns>
        Stream OpenWrite();

        /// <summary>
        /// 打开文本读取器
        /// </summary>
        /// <returns></returns>
        TextReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        /// <summary>
        /// 读取所有文本
        /// </summary>
        /// <returns></returns>
        string ReadAllText()
        {
            using var reader = OpenText();
            return reader.ReadToEnd();
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <returns></returns>
        string GetFileName()
        {
            var path = PathName;
            var index = path.LastIndexOf('/');
            string temp;
            return index == -1
                ? path
                : index + 1 == path.Length
                    ? (temp = path[..index])[(temp.LastIndexOf('/') + 1)..]
                    : path[(index + 1)..];
        }

        /// <summary>
        /// 获取目录名
        /// </summary>
        /// <returns></returns>
        string GetDirectoryName()
        {
            var path = PathName;
            return path[..(path.LastIndexOf('/') + 1)];
        }
    }
}