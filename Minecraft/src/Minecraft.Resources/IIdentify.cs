using System;

namespace Minecraft.Resources
{
    /// <summary>
    /// 可识别的
    /// </summary>
    public interface IIdentify : IEquatable<IIdentify>
    {
        /// <summary>
        /// 标识符
        /// </summary>
        public Uuid Id { get; }

        bool IEquatable<IIdentify>.Equals(IIdentify other)
        {
            return Id == other?.Id;
        }
    }
}