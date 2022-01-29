using Minecraft.Client.Handlers;
using System;

namespace Minecraft.Client
{
    public interface IMinecraftClient
    {
        bool IsConnected { get; }
        bool IsJoined { get; }
        MinecraftClientState State { get; }

        event EventHandler<string> ChatReceived;
        event EventHandler<string> Disconnected;

        void Connect(string hostname, ushort port);
        void Disconnect();
        IMinecraftClientAdapter GetAdapter();
        IClientPlayerHandler GetPlayer();
        IWorldHandler GetWorld();
        void Reconnect();
        void SendChatMessage(string message);
        ServerListPingResult ServerListPing(string hostname, ushort port);
    }
}