using Minecraft.Protocol;
using Minecraft.Protocol.Packets.Client;
using Minecraft.Protocol.Packets.Server;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Minecraft.Extensions;
using Minecraft.Data.Nbt.Tags;
using Minecraft.Data.Numerics;
using ClientChatMessagePacket = Minecraft.Protocol.Packets.Client.ChatMessagePacket;
using ServerChatMessagePacket = Minecraft.Protocol.Packets.Server.ChatMessagePacket;

namespace Minecraft.Client
{
    public class MinecraftClientAdapter
    {
        private readonly string _hostname;
        private readonly ushort _port;
        private readonly int _protocolVersion;
        private bool _stopping = false;
        private readonly TcpClient _tcpClient;
        private ProtocolAdapter _protocolAdapter;
        private readonly MinecraftClient _client;

        public MinecraftClientAdapter(string hostname, ushort port, MinecraftClient client)
        {
            _hostname = hostname;
            _port = port;
            _protocolVersion = client._protocolVersion;
            _tcpClient = new TcpClient();
            _client = client;
        }

        #region Events

        public event EventHandler<(string hostname, uint port)> Connected;
        public event EventHandler<(string userName, Uuid uuid)> Logined;
        public event EventHandler<(int entityId, bool isHardcore, Gamemode gamemode, Gamemode previousGamemode, int worldCount, NamedIdentifier[] worldNames, NbtCompound dimensionCodec, NbtCompound dimension, NamedIdentifier worldName, long hashedSeed, int maxPlayers, int viewDistance, bool reducedDebugInfo, bool enableRespawnScreen, bool isDebug, bool isFlat)> Joined;
        public event EventHandler<(Difficulty difficulty, bool locked)> ServerDiffculty;
        public event EventHandler<(PlayerAbilitiy Flags, float flyingSpeed, float fieldOfViewModifier)> PlayerAbilities;
        public event EventHandler<HotbarSlot> HeldItemChange;
        public event EventHandler<(Vector3d position, Rotation rotation, CoordKind xKind, CoordKind yKind, CoordKind zKind, CoordKind yRotKind, CoordKind xRotKind, int teleportId, bool dismountVehicle)> PlayerPositionAndLook;
        public event EventHandler<string> Disconnected;
        public event EventHandler<Exception> Exception;

        #endregion

        #region Properties

        public (string locale, sbyte viewDistance, ChatMode chatMode, bool chatColors, SkinPart displayedSkinParts, Hand mainHand, bool disableTextFiltering) ClientSettings { get; set; } = ("en_US", 4, ChatMode.Enabled, true, SkinPart.All, Hand.Right, true);

        public async Task SubmitClientSettings()
        {
            var (locale, viewDistance, chatMode, chatColors, displayedSkinParts, mainHand, disableTextFiltering) = ClientSettings;
            await _protocolAdapter.SendPacket(new ClientSettingsPacket
            {
                Locale = locale,
                ViewDistance = viewDistance,
                ChatMode = chatMode,
                ChatColors = chatColors,
                DisplayedSkinParts = displayedSkinParts,
                MainHand = mainHand,
                DisableTextFiltering = disableTextFiltering
            });
        }

        #endregion

        #region Send Packets
        
        public async Task SendChatPacket(string message)
        {
            await _protocolAdapter.SendPacket(new ClientChatMessagePacket { Message = message });
        }

        #endregion

        #region Connection

        public bool IsConnected { get; private set; }
        public bool IsLogined { get; private set; }
        public bool IsJoined { get; private set; }

        public async Task Connect()
        {
            if (IsConnected) throw new InvalidOperationException("The adapter is already connected");
            IsConnected = true;
            try
            {
                await _tcpClient.ConnectAsync(_hostname, _port);
                _protocolAdapter = new ProtocolAdapter(_tcpClient.GetStream(), Protocol.Packets.PacketBoundTo.Server);
                _protocolAdapter.Started += ProtocolAdapter_Started;
                _protocolAdapter.Stopped += ProtocolAdapter_Stopped;
                _protocolAdapter.Exception += (sender, ex) =>
                {
                    _ = Logger.Warn<MinecraftClientAdapter>(ex.ToString());
                };
                await Task.Run(_protocolAdapter.Start);
            }
            catch (Exception ex)
            {
                _ = Logger.Warn<MinecraftClientAdapter>($"Adapter stopped with exception: {ex.Message}");
                await Task.Run(() =>
                {
                    Exception?.Invoke(this, ex);
                    Disconnected?.Invoke(this, ex.Message);
                });
            }
            finally
            {
                IsConnected = false;
                IsLogined = false;
                IsJoined = false;
            }
        }

        private void ProtocolAdapter_Stopped(object sender, EventArgs e)
        {
            Disconnected?.Invoke(this, "Lose connection.");
        }

        private async void ProtocolAdapter_Started(object sender, EventArgs e)
        {
            _ = Task.Run(() => Connected?.Invoke(this, (_hostname, _port))).LogException<MinecraftClientAdapter>();
            await Start().LogException<MinecraftClientAdapter>();
        }

        private delegate void Chat();
        private async Task Start()
        {
            // _protocolAdapter.PacketReceived += (_, p) => _ = Logger.Info<Chat>($"Packet S->C\n{p.GetPropertyInfoString()}");
            _protocolAdapter.PacketSent += (_, p) => _ = Logger.Info<Chat>($"Packet C->S\n{p.GetPropertyInfoString()}");
            _protocolAdapter.HandlePacket<ServerChatMessagePacket>(p => _ = Logger.Info<Chat>(p.JsonData));
            await Login();
            await Join();
        }

        private async Task Login()
        {
            /*
             * Client connects to server
             * C→S: Handshake State=2
             * C→S: Login Start
             * S→C: Encryption Request
             * Client auth
             * C→S: Encryption Response
             * Server auth, both enable encryption
             * S→C: Set Compression (Optional, enables compression)
             * S→C: Login Success
             */


            _protocolAdapter.HandlePacket<LoginDisconnectPacket>(packet =>
            {
                _ = Logger.Info<MinecraftClientAdapter>($"Disconnect. Reason: {packet.Reason}");
                Disconnected?.Invoke(this, packet.Reason);
            });

            // C→S: Handshake State=2
            await _protocolAdapter.SendPacket(new HandshakePacket
            {
                NextState = ProtocolState.Login,
                ProtocolVersion = _protocolVersion,
                ServerAddress = _hostname,
                ServerPort = _port,
            });

            // C→S: Login Start
            await _protocolAdapter.SendPacket(new LoginStartPacket
            {
                Name = _client._playerName
            });

            if (!_client._offlineMode)
            {
                // S→C: Encryption Request
                // Client auth

                // C→S: Encryption Response
                // Server auth, both enable encryption
            }

            // S→C: Set Compression (Optional, enables compression)

            var loginSuccessPacket = await _protocolAdapter.ReceiveSinglePacket<LoginSuccessPacket>();
            _ = Task.Run(() => Logined?.Invoke(this, (loginSuccessPacket.Username, loginSuccessPacket.Uuid))).LogException<MinecraftClientAdapter>();
            _ = Logger.Info<MinecraftClientAdapter>($"User {loginSuccessPacket.Username} ({loginSuccessPacket.Uuid}) logined");

            IsLogined = true;
        }

        private async Task Join()
        {
            /*
            * S→C: Join Game
            * S→C: Plugin Message: minecraft:brand with the server's brand (Optional)
            * S→C: Server Difficulty (Optional)
            * S→C: Player Abilities (Optional)
            * C→S: Plugin Message: minecraft:brand with the client's brand (Optional)
            * C→S: Client Settings
            * S→C: Held Item Change
            * S→C: Declare Recipes
            * S→C: Tags
            * S→C: Entity Status
            * S→C: Declare Commands
            * S→C: Unlock Recipes
            * S→C: Player Position And Look
            * S→C: Player Info (Add Player action)
            * S→C: Player Info (Update latency action)
            * S→C: Update View Position
            * S→C: Update Light (One sent for each chunk in a square centered on the player's position)
            * S→C: Chunk Data (One sent for each chunk in a square centered on the player's position)
            * S→C: World Border (Once the world is finished loading)
            * S→C: Spawn Position (“home” spawn, not where the client will spawn on login)
            * S→C: Player Position And Look (Required, tells the client they're ready to spawn)
            * C→S: Teleport Confirm
            * C→S: Player Position And Look (to confirm the spawn position)
            * C→S: Client Status (sent either before or while receiving chunks, further testing needed, server handles correctly if not sent)
            * S→C: inventory, entities, etc
            */

            _protocolAdapter.HandlePacket<DisconnectPacket>(packet =>
            {
                _ = Logger.Info<MinecraftClientAdapter>($"Disconnect. Reason: {packet.Reason}");
                Disconnected?.Invoke(this, packet.Reason);
            });

            // S→C: Join Game
            _ = _protocolAdapter.HandleReceiveSinglePacket<JoinGamePacket>(joinGamePacket =>
            {
                Joined?.Invoke(this, (joinGamePacket.EntityId, joinGamePacket.IsHardcore, joinGamePacket.Gamemode, joinGamePacket.PreviousGamemode, joinGamePacket.WorldCount, joinGamePacket.WorldNames, joinGamePacket.DimensionCodec, joinGamePacket.Dimension, joinGamePacket.WorldName, joinGamePacket.HashedSeed, joinGamePacket.MaxPlayers, joinGamePacket.ViewDistance, joinGamePacket.ReducedDebugInfo, joinGamePacket.EnableRespawnScreen, joinGamePacket.IsDebug, joinGamePacket.IsFlat));
            }).LogException<MinecraftClientAdapter>();

            // TODO: S->C: Plugin Message
            //

            // S→C: Server Difficulty (Optional)
            _ = _protocolAdapter.HandlePacket<ServerDifficultyPacket>(p => ServerDiffculty?.Invoke(this, (p.Difficulty, p.DifficultyLocked)));

            // S→C: Player Abilities (Optional)
            _ = _protocolAdapter.HandlePacket<PlayerAbilitiesPacket>(p => PlayerAbilities?.Invoke(this, (p.Flags, p.FlyingSpeed, p.FieldOfViewModifier)));

            // TODO: C->S: Plugin Message
            //

            // C→S: Client Settings
            _ = SubmitClientSettings();

            // S→C: Held Item Change
            _ = _protocolAdapter.HandlePacket<HeldItemChangePacket>(p => HeldItemChange?.Invoke(this, p.Slot));

            // S→C: Declare Recipes


            // S→C: Tags


            // S→C: Entity Status


            // S→C: Declare Commands


            // S→C: Unlock Recipes


            // S→C: Player Position And Look
            // ingore

            // S→C: Player Info (Add Player action)


            // S→C: Player Info (Update latency action)


            // S→C: Update View Position


            // S→C: Update Light (One sent for each chunk in a square centered on the player's position)


            // S→C: Chunk Data (One sent for each chunk in a square centered on the player's position)


            // S→C: World Border (Once the world is finished loading)


            // S→C: Spawn Position (“home” spawn, not where the client will spawn on login)


            // S→C: Player Position And Look (Required, tells the client they're ready to spawn)
            // C→S: Teleport Confirm
            // C→S: Player Position And Rotation (to confirm the spawn position)
            _ = _protocolAdapter.HandlePacket<PlayerPositionAndLookPacket>(p =>
            {
                _ = _protocolAdapter.SendPacket(new TeleportConfirmPacket { TeleportId = p.TeleportId }).LogException<MinecraftClientAdapter>();
                PlayerPositionAndLook?.Invoke(this, (p.Position, p.Rotation, p.XKind, p.YKind, p.ZKind, p.YRotKind, p.XRotKind, p.TeleportId, p.DismountVehicle));
            });
            _ = _protocolAdapter.HandleReceiveSinglePacket<PlayerPositionAndLookPacket>(p =>
            {
                IsJoined = true;
                _ = _protocolAdapter.SendPacket(new PlayerPositionAndRotationPacket
                {
                    Position = p.Position,
                    Rotation = p.Rotation,
                    OnGround = true
                });
            });

            // C→S: Client Status (sent either before or while receiving chunks, further testing needed, server handles correctly if not sent)
            // ????

            // S→C: inventory, entities, etc




        }

        public async Task Disconnect()
        {
            if (!IsConnected || _stopping) return;
            _stopping = true;
            await _protocolAdapter.Stop();
            _tcpClient.Close();
            IsConnected = false;
            _stopping = false;
        }

        #endregion

    }
}
