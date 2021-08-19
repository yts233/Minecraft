using System;

namespace Minecraft
{
    /// <summary>表示通用唯一标识符 (UUID)。</summary>
    public struct Uuid : IEquatable<Uuid>, IComparable, IComparable<Uuid>, IFormattable
    {
        private readonly Guid _value;

        /// <summary>
        /// <see cref="T:System.Uuid" /> 结构的只读实例，其值均为零。
        /// </summary>
        public static readonly Uuid Empty;

        public Uuid(Guid guid)
        {
            _value = guid;
        }

        /// <summary>使用指定的字节数组初始化 <see cref="T:System.Uuid" /> 类的新实例。</summary>
        /// <param name="b">包含用于初始化 UUID 的值的 16 元素字节数组。</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="b" /> 为 <see langword="null" />。
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="b" /> 的长度不是 16 个字节。
        /// </exception>
        public Uuid(byte[] b)
        {
            _value = new Guid(b);
        }

        /// <summary>使用指定的整数和字节初始化 <see cref="T:System.Uuid" /> 类的新实例。</summary>
        /// <param name="a">UUID 的前 4 个字节。</param>
        /// <param name="b">UUID 的下两个字节。</param>
        /// <param name="c">UUID 的下两个字节。</param>
        /// <param name="d">UUID 的下一个字节。</param>
        /// <param name="e">UUID 的下一个字节。</param>
        /// <param name="f">UUID 的下一个字节。</param>
        /// <param name="g">UUID 的下一个字节。</param>
        /// <param name="h">UUID 的下一个字节。</param>
        /// <param name="i">UUID 的下一个字节。</param>
        /// <param name="j">UUID 的下一个字节。</param>
        /// <param name="k">UUID 的下一个字节。</param>
        public Uuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            _value = new Guid(a, b, c, d, e, f, g, h, i, j, k);
        }

        /// <summary>使用指定的整数和字节数组初始化 <see cref="T:System.Uuid" /> 类的新实例。</summary>
        /// <param name="a">UUID 的前 4 个字节。</param>
        /// <param name="b">UUID 的下两个字节。</param>
        /// <param name="c">UUID 的下两个字节。</param>
        /// <param name="d">UUID 的其余 8 个字节。</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="d" /> 为 <see langword="null" />。
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="d" /> 的长度不是 8 个字节。
        /// </exception>
        public Uuid(int a, short b, short c, byte[] d)
        {
            _value = new Guid(a, b, c, d);
        }

        /// <summary>通过使用指定的只读字节范围所表示的值来初始化 <see cref="T:System.Uuid" /> 结构的新实例。</summary>
        /// <param name="b">包含表示 UUID 的字节的只读范围。 范围的长度必须正好为 16 个字节。</param>
        /// <exception cref="T:System.ArgumentException">范围的长度必须正好为 16 个字节。</exception>
        public Uuid(ReadOnlySpan<byte> b)
        {
            _value = new Guid(b);
        }

        /// <summary>使用指定字符串所表示的值初始化 <see cref="T:System.Uuid" /> 类的新实例。</summary>
        /// <param name="g">
        /// 包含下面任一格式的 UUID 的字符串（“d”表示忽略大小写的十六进制数字）：
        /// 32 个连续的数字：
        /// dddddddddddddddddddddddddddddddd
        /// 或
        /// 8、4、4、4 和 12 位数字的分组，各组之间有连线符。 也可以用一对大括号或者圆括号将整个 UUID 括起来：
        /// dddddddd-dddd-dddd-dddd-dddddddddddd
        /// 或
        /// {dddddddd-dddd-dddd-dddd-dddddddddddd}
        /// 或
        /// (dddddddd-dddd-dddd-dddd-dddddddddddd)
        /// 或
        /// 8、4 和 4 位数字的分组，和一个 8 组 2 位数字的子集，每组都带有前缀“0x”或“0X”，以逗号分隔。 整个 UUID 和子集用大括号括起来：
        /// {0xdddddddd, 0xdddd, 0xdddd,{0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd}}
        /// 所有大括号、逗号和“0x”前缀都是必需的。 所有内置的空格都将被忽略。 组中的所有前导零都将被忽略。
        /// 组中显示的数字为可在该组显示的有意义数字的最大数目。 你可以指定从 1 到为组显示的位数。 指定的位数被认为是该组低序位的位数。
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="g" /> 为 <see langword="null" />。
        /// </exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="g" /> 的格式无效。
        /// </exception>
        /// <exception cref="T:System.OverflowException">
        /// <paramref name="g" /> 的格式无效。
        /// </exception>
        public Uuid(string g)
        {
            _value = new Guid(g);
        }

        public Uuid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            _value = new Guid(a, b, c, d, e, f, g, h, i, j, k);
        }

        /// <summary>将此实例与指定 <see cref="T:System.Uuid" /> 对象进行比较并返回它们的相对值。</summary>
        /// <param name="value">要与此实例进行比较的对象。</param>
        /// <returns>
        /// 一个带符号数字，指示此实例和 <paramref name="value" /> 的相对值。
        /// 返回值
        /// 说明
        /// 负整数
        /// 此实例小于 <paramref name="value" />。
        /// 零
        /// 此实例等于 <paramref name="value" />。
        /// 正整数
        /// 此实例大于 <paramref name="value" />。
        /// </returns>
        public int CompareTo(Uuid value)
        {
            return _value.CompareTo(value);
        }

        /// <summary>将此实例与指定对象进行比较并返回一个对二者的相对值的指示。</summary>
        /// <param name="value">要比较的对象，或为 <see langword="null" />。</param>
        /// <returns>
        /// 一个带符号数字，指示此实例和 <paramref name="value" /> 的相对值。
        /// 返回值
        /// 说明
        /// 负整数
        /// 此实例小于 <paramref name="value" />。
        /// 零
        /// 此实例等于 <paramref name="value" />。
        /// 正整数
        /// 此实例大于 <paramref name="value" />，或 <paramref name="value" /> 为 <see langword="null" />。
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="value" /> 不是 <see cref="T:System.Uuid" />。
        /// </exception>
        public int CompareTo(object value)
        {
            return _value.CompareTo(value);
        }

        /// <summary>返回一个值，该值指示此实例和指定的 <see cref="T:System.Uuid" /> 对象是否表示相同的值。</summary>
        /// <param name="g">要与此实例进行比较的对象。</param>
        /// <returns>如果 <see langword="true" /> 与此实例相等，则为 <paramref name="g" />；否则为 <see langword="false" />。</returns>
        public bool Equals(Uuid g)
        {
            return _value.Equals(g);
        }

        /// <summary>返回一个值，该值指示此实例是否与指定的对象相等。</summary>
        /// <param name="obj">与该实例进行比较的对象。</param>
        /// <returns>
        /// 如果 <paramref name="obj" /> 是值与此实例相等的 <see cref="T:System.Uuid" />，则为 <see langword="true" />；否则为
        /// <see langword="false" />。
        /// </returns>
        public override bool Equals(object obj)
        {
            return _value.Equals(obj);
        }

        /// <summary>返回此实例的哈希代码。</summary>
        /// <returns>此实例的哈希代码。</returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>初始化 <see cref="T:System.Uuid" /> 结构的新实例。</summary>
        /// <returns>一个新的 UUID 对象。</returns>
        public static Uuid NewUuid()
        {
            return new Uuid(Guid.NewGuid());
        }

        /// <summary>指示两个指定的 <see cref="T:System.Uuid" /> 对象的值是否相等。</summary>
        /// <param name="a">要比较的第一个对象。</param>
        /// <param name="b">要比较的第二个对象。</param>
        /// <returns>如果 <paramref name="a" /> 和 <paramref name="b" /> 相等，则为 <see langword="true" />；否则为 <see langword="false" />。</returns>
        public static bool operator ==(Uuid a, Uuid b)
        {
            return a._value == b._value;
        }

        /// <summary>指示两个指定的 <see cref="T:System.Uuid" /> 对象的值是否不相等。</summary>
        /// <param name="a">要比较的第一个对象。</param>
        /// <param name="b">要比较的第二个对象。</param>
        /// <returns>如果 <paramref name="a" /> 和 <paramref name="b" /> 不相等，则为 <see langword="true" />；否则为 <see langword="false" />。</returns>
        public static bool operator !=(Uuid a, Uuid b)
        {
            return a._value != b._value;
        }

        /// <summary>将表示 UUID 的只读字符范围转换为等效的 <see cref="T:System.Uuid" /> 结构。</summary>
        /// <param name="input">包含表示 UUID 的字节的只读范围。</param>
        /// <returns>一个包含已分析的值的结构。</returns>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> 的格式无法识别。
        /// 或
        /// 剪裁后，只读字符范围的长度为 0。
        /// </exception>
        public static Uuid Parse(ReadOnlySpan<char> input)
        {
            return new Uuid(Guid.Parse(input));
        }

        /// <summary>将 UUID 的字符串表示形式转换为等效的 <see cref="T:System.Uuid" /> 结构。</summary>
        /// <param name="input">要转换的字符串。</param>
        /// <returns>一个包含已分析的值的结构。</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> 为 <see langword="null" />。
        /// </exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> 的格式无法识别。
        /// </exception>
        public static Uuid Parse(string input)
        {
            return new Uuid(Guid.Parse(input));
        }

        /// <summary>如果字符串采用指定格式，则将 UUID 的字符范围表示形式转换为等效的 <see cref="T:System.Uuid" /> 结构。</summary>
        /// <param name="input">包含表示要转换的 UUID 的字符的只读范围。</param>
        /// <param name="format">表示以下某个说明符的字符的只读范围，指示解释 <paramref name="input" /> 时要使用的确切格式：“N”、“D”、“B”、“P”或“X”。</param>
        /// <returns>一个包含已分析的值的结构。</returns>
        public static Uuid ParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format)
        {
            return new Uuid(Guid.ParseExact(input, format));
        }

        /// <summary>将 UUID 的字符串表示形式转换为等效的 <see cref="T:System.Uuid" /> 结构，前提是该字符串采用的是指定格式。</summary>
        /// <param name="input">要转换的 UUID。</param>
        /// <param name="format">下列说明符之一，指示解释 <paramref name="input" /> 时要使用的确切格式：“N”、“D”、“B”、“P”或“X”。</param>
        /// <returns>一个包含已分析的值的结构。</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> 或 <paramref name="format" /> 为 <see langword="null" />。
        /// </exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> 未采用 <paramref name="format" /> 指定的格式。
        /// </exception>
        public static Uuid ParseExact(string input, string format)
        {
            return new Uuid(Guid.ParseExact(input, format));
        }

        /// <summary>返回包含此实例的值的 16 元素字节数组。</summary>
        /// <returns>16 元素字节数组。</returns>
        public byte[] ToByteArray()
        {
            return _value.ToByteArray();
        }

        /// <summary>返回注册表格式的此实例值的字符串表示形式。</summary>
        /// <returns>
        /// 这 <see cref="T:System.Uuid" />的值，格式化通过使用“D”格式说明符如下所示:
        /// <c>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</c>
        /// 其中 UUID 的值表示为一系列小写的十六进制位，这些十六进制位分别以 8 个、4 个、4 个、4 个和 12 个位为一组并由连字符分隔开。
        /// 例如，返回值可以是“382c74c3-721d-4f34-80e5-57657b6cbc27”。 若要将从 a 到 f 的十六进制数转换为大写，请对返回的字符串调用
        /// <see cref="M:System.String.ToUpper" /> 方法。
        /// </returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        /// <summary>根据所提供的格式说明符，返回此 <see cref="T:System.Uuid" /> 实例值的字符串表示形式。</summary>
        /// <param name="format">
        /// 一个单格式说明符，它指示如何格式化此 <see cref="T:System.Uuid" /> 的值。 <paramref name="format" />
        /// 参数可以是“N”、“D”、“B”、“P”或“X”。 如果 <paramref name="format" /> 为 <see langword="null" /> 或空字符串 ("")，则使用“D”。
        /// </param>
        /// <returns>此 <see cref="T:System.Uuid" /> 的值，用一系列指定格式的小写十六进制位表示。</returns>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="format" /> 的值不是 <see langword="null" />、空字符串 ("")、“N”、“D”、“B”、“P”或“X”。
        /// </exception>
        public string ToString(string format)
        {
            return _value.ToString(format);
        }

        /// <summary>根据所提供的格式说明符和区域性特定的格式信息，返回 <see cref="T:System.Uuid" /> 类的此实例值的字符串表示形式。</summary>
        /// <param name="format">
        /// 一个单格式说明符，它指示如何格式化此 <see cref="T:System.Uuid" /> 的值。 <paramref name="format" />
        /// 参数可以是“N”、“D”、“B”、“P”或“X”。 如果 <paramref name="format" /> 为 <see langword="null" /> 或空字符串 ("")，则使用“D”。
        /// </param>
        /// <param name="provider">（保留）一个对象，用于提供区域性特定的格式设置信息。</param>
        /// <returns>此 <see cref="T:System.Uuid" /> 的值，用一系列指定格式的小写十六进制位表示。</returns>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="format" /> 的值不是 <see langword="null" />、空字符串 ("")、“N”、“D”、“B”、“P”或“X”。
        /// </exception>
        public string ToString(string format, IFormatProvider provider)
        {
            return _value.ToString(format, provider);
        }

        public static explicit operator Guid(Uuid value)
        {
            return value._value;
        }

        public static explicit operator Uuid(Guid value)
        {
            return new Uuid(value);
        }
    }
}