using System;
using System.Collections.Generic;
using Minecraft.Data.Common.Blocking;
using Minecraft.Data.Common.Chunking;
using Minecraft.Data.Nbt.Serialization;

namespace Test.Data.Nbt.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var nbt = NbtSerializer.Serialize(new Chuck
            {
                Version = 1,
                Data = new ChuckData
                {
                    X = 0,
                    Z = 0,
                    Biomes = new sbyte[] {1, 2, 3, 4, 5, 6, 7},
                    InhabitedTime = 0,
                    LastUpdate = 0,
                    HeightMaps = new ChuckHeightMaps
                    {
                        MotionBlocking = new long[] {1, 2, 3},
                        OceanFloor = new long[] {4, 5, 6},
                        WorldSurface = new long[] {7, 8, 9},
                        OceanFloorWg = new long[] {1, 3, 5},
                        WorldSurfaceWg = new long[] {2, 4, 6},
                        MotionBlockingNoLeaves = new long[] {3, 5, 7}
                    },
                    Sections = new List<ChuckSection>
                    {
                        new ChuckSection
                        {
                            Y = 0,
                            Palette = new List<BlockState>
                            {
                                new BlockState
                                {
                                    Name = "minecraft:stone",
                                    Properties = new Dictionary<string, string>()
                                }
                            },
                            BlockLight = new sbyte[] {0},
                            BlockStates = new long[] {0},
                            SkyLight = new sbyte[] {0}
                        }
                    }
                }
            });
            Console.WriteLine(nbt.ToString());
        }
    }
}