using System;
using System.Collections;
using Minecraft.Graphics.Rendering;

namespace Minecraft.Graphics.Arraying
{
    /// <summary>
    /// 顶点数组
    /// </summary>
    public interface IVertexArray : IHandle, IBindable, IRenderable, IEnumerable, IDisposable
    {
        /// <summary>
        /// 渲染
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="count">数量</param>
        void Render(int index, int count);
    }
}