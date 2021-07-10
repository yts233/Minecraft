using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Tool.AssetResolver
{
    public class AssetFileInfo
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public int Size { get; set; }

        public static IEnumerable<AssetFileInfo> Resolve(string filepath)
        {
            using var jsonDocument = JsonDocument.Parse(File.ReadAllText(filepath));
            var root = jsonDocument.RootElement;
            var objects = root.GetProperty("objects");
            foreach (var obj in objects.EnumerateObject())
                yield return new AssetFileInfo
                {
                    Name = obj.Name,
                    Hash = obj.Value.GetProperty("hash").GetString(),
                    Size = obj.Value.GetProperty("size").GetInt32()
                };
        }
    }
}