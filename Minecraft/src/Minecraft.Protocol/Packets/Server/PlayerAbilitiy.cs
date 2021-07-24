using System;

namespace Minecraft.Protocol.Packets.Server
{
    [Flags]
    public enum PlayerAbilitiy : sbyte
    {
        Invulnerable = 0x01,
        Flying = 0x02,
        AllowFlying = 0x04,
        /// <summary>
        /// Creative mode (Instant Break)
        /// </summary>
        CreativeMode = 0x08
    }
}
