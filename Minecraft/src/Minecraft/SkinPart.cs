using System;

namespace Minecraft
{
    [Flags]
    public enum SkinPart : byte
    {
        Cape = 0x01,
        Jacket = 0x02,
        LeftSleeve = 0x04,
        RightSleeve = 0x08,
        LeftPartsLeg = 0x10,
        RightPartsLeg = 0x20,
        Hat = 0x40,
        All = 0xFF
    }
}