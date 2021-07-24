using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Data.Nbt
{
    public class NbtException : Exception
    {
        internal NbtException(string message) : base(message)
        {
        }
    }
}
