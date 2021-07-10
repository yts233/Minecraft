namespace Minecraft.Data.Nbt
{
    public enum NbtTagType
    {
        /// <summary>
        ///     表明<see cref="Compound" />的结尾。
        /// </summary>
        /// <remarks>它只会在<see cref="Compound" />内部使用，而且即使在<see cref="Compound" />中也不会被指名。</remarks>
        End = 0,

        /// <summary>
        ///     单个有符号字节
        /// </summary>
        Byte = 1,

        /// <summary>
        ///     单个有符号的大字节序16位整型
        /// </summary>
        Short = 2,

        /// <summary>
        ///     单个有符号的大字节序32位整型
        /// </summary>
        Int = 3,

        /// <summary>
        ///     单个有符号的大字节序64位整型
        /// </summary>
        Long = 4,

        /// <summary>
        ///     单个大字节序的IEEE-754单精度浮点数
        /// </summary>
        /// <remarks>可能为NaN</remarks>
        Float = 5,

        /// <summary>
        ///     单个大字节序的IEEE-754双精度浮点数（可能为NaN）
        /// </summary>
        Double = 6,

        /// <summary>
        ///     一个包含长度前缀的有符号字节数组。
        /// </summary>
        /// <remarks>前缀是一个有符号整型（即4字节）</remarks>
        ByteArray = 7,

        /// <summary>
        ///     一个包含长度前缀的modified UTF-8字符串。
        /// </summary>
        /// <remarks>
        ///     前缀是一个无符号短整型（即2字节）来表明字符串以字节为单位的长度。
        /// </remarks>
        String = 8,

        /// <summary>
        ///     一个相同类型无名标签的列表。
        /// </summary>
        /// <remarks>
        ///     列表的前缀是它包含项目的类型ID（即1字节）列表的长度就是一个有符号整型（即4字节）。
        ///     如果列表的长度为0或为负，则类型可能是0（<see cref="End" />），但反之它必须是其他的什么类型。
        ///     （Notch式的实现在这种情况下使用了<see cref="End" />，但在另一个Mojang参考实现中使用了1，总之解析器应该在长度&lt;=0时接受任何类型）
        /// </remarks>
        List = 9,

        /// <summary>
        ///     有效地列出named标签。
        /// </summary>
        /// <remarks>不能保证顺序。</remarks>
        Compound = 10,

        /// <summary>
        ///     一个带长度前缀的有符号整型数组。
        /// </summary>
        /// <remarks>前缀是一个有符号整型（即4字节），表示4字节整型的数量。</remarks>
        IntArray = 11,

        /// <summary>
        ///     一个带长度前缀的有符号整型数组。
        /// </summary>
        /// <remarks>前缀是一个有符号整型（即4字节），表示4字节整型的数量。</remarks>
        LongArray = 12
    }
}