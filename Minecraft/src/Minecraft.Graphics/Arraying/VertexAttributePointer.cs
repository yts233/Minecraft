namespace Minecraft.Graphics.Arraying
{
    /// <summary>
    ///     顶点堆指针属性
    /// </summary>
    public struct VertexAttributePointer
    {
        /// <summary>
        ///     索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     大小
        /// </summary>
        /// <remarks>只能是1,2,3,4</remarks>
        public int Size { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        public VertexAttribePointerType Type { get; set; }

        /// <summary>
        ///     是否单位化
        /// </summary>
        public bool Normalized { get; set; }

        /// <summary>
        ///     偏移（字节）
        /// </summary>
        public int Offset { get; set; }
    }
}