using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Minecraft.Protocol.Client
{
    /// <summary>
    /// 服务器列表Ping结果
    /// </summary>
    public class ServerListPingResult
    {
        /// <summary>
        /// Delay
        /// </summary>
        /// <remarks>毫秒级</remarks>
        public long Delay { get; set; }

        public string Description { get; set; }

        public int MaxPlayerCount { get; set; }

        public int OnlinePlayerCount { get; set; }

        public string VersionName { get; set; }

        public int ProtocolVersion { get; set; }

        public byte[] Favicon { get; set; }

        public IReadOnlyCollection<(string playerName, Uuid uuid)> SamplePlayers { get; set; }

        public void LoadContent(string content)
        {
            using var json = JsonDocument.Parse(content);
            var root = json.RootElement;
            root.TryGetProperty("description", out JsonElement description);
            Description = description.GetRawText();
            root.TryGetProperty("players", out var players);
            players.TryGetProperty("max", out var max);
            max.TryGetInt32(out var maxPlayerCount);
            MaxPlayerCount = maxPlayerCount;
            players.TryGetProperty("online", out var online);
            online.TryGetInt32(out var onlinePlayerCount);
            OnlinePlayerCount = onlinePlayerCount;
            players.TryGetProperty("sample", out var sample);
            var samples = new List<(string playerName, Uuid uuid)>();
            if (sample.ValueKind == JsonValueKind.Array)
                foreach (var player in sample.EnumerateArray())
                {
                    player.TryGetProperty("name", out var name);
                    var playerName = name.GetString();
                    player.TryGetProperty("id", out var id);
                    id.TryGetGuid(out var guid);
                    var uuid = new Uuid(guid);
                    samples.Add((playerName, uuid));
                }
            SamplePlayers = samples;
            root.TryGetProperty("version", out var version);
            version.TryGetProperty("name", out var versionName);
            VersionName = versionName.GetString();
            version.TryGetProperty("protocol", out var versionProtocol);
            versionProtocol.TryGetInt32(out var protocolVersion);
            ProtocolVersion = protocolVersion;
            root.TryGetProperty("favicon", out var favicon);
            if (favicon.ValueKind == JsonValueKind.String)
            {
                var faviconData = favicon.GetString() + "=";
                if (faviconData != null && faviconData.StartsWith("data:image/png;base64,"))
                {
                    Favicon = Convert.FromBase64CharArray(faviconData.ToCharArray(), 22, faviconData.Length - 23);
                }
            }
        }
    }
}