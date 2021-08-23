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
    /// <summary>
    /// Minecraft 客户端适配器
    /// </summary>
    public class MinecraftClientAdapter
    {
        private delegate void Chat();
        private readonly string _hostname;
        private readonly ushort _port;
        private readonly int _protocolVersion;
        private bool _stopping = false;
        private readonly TcpClient _tcpClient;
        private ProtocolAdapter _protocolAdapter;
        private readonly MinecraftClient _client;
        private static readonly Logger<MinecraftClientAdapter> _logger = Logger.GetLogger<MinecraftClientAdapter>();
        private static readonly Logger<Chat> _chatLogger = Logger.GetLogger<Chat>();

        public MinecraftClientAdapter(string hostname, ushort port, MinecraftClient client)
        {
            _hostname = hostname;
            _port = port;
            _protocolVersion = client._protocolVersion;
            _tcpClient = new TcpClient();
            _client = client;
        }

        #region Events

        /// <summary>
        /// 连接到服务器
        /// </summary>
        public event EventHandler<(string hostname, uint port)> Connected;
        /// <summary>
        /// 登录成功
        /// </summary>
        public event EventHandler<(string userName, Uuid uuid)> Logined;
        /// <summary>
        /// 加入到服务器
        /// </summary>
        public event EventHandler<(int entityId, bool isHardcore, Gamemode gamemode, Gamemode previousGamemode, int worldCount, NamedIdentifier[] worldNames, NbtCompound dimensionCodec, NbtCompound dimension, NamedIdentifier worldName, long hashedSeed, int maxPlayers, int viewDistance, bool reducedDebugInfo, bool enableRespawnScreen, bool isDebug, bool isFlat)> Joined;
        /// <summary>
        /// 服务器难度
        /// </summary>
        public event EventHandler<(Difficulty difficulty, bool locked)> ServerDiffculty;
        /// <summary>
        /// 玩家能力
        /// </summary>
        public event EventHandler<(PlayerAbilitiy Flags, float flyingSpeed, float fieldOfViewModifier)> PlayerAbilities;
        /// <summary>
        /// 切换物品栏
        /// </summary>
        public event EventHandler<HotbarSlot> HeldItemChange;
        /// <summary>
        /// 玩家位置变化
        /// </summary>
        public event EventHandler<(Vector3d position, CoordKind xKind, CoordKind yKind, CoordKind zKind, int teleportId, bool dismountVehicle)> PlayerPosition;
        /// <summary>
        /// 玩家朝向变化
        /// </summary>
        public event EventHandler<(Rotation rotation, CoordKind yRotKind, CoordKind xRotKind, int teleportId)> PlayerLook;
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <remarks>附带原因</remarks>
        public event EventHandler<string> Disconnected;
        /// <summary>
        /// 发生异常
        /// </summary>
        public event EventHandler<Exception> Exception;

        #endregion

        #region Properties

        /// <summary>
        /// 客户端设置
        /// </summary>
        public (string locale, sbyte viewDistance, ChatMode chatMode, bool chatColors, SkinPart displayedSkinParts, Hand mainHand, bool disableTextFiltering) ClientSettings { get; set; } = ("en_US", 4, ChatMode.Enabled, true, SkinPart.All, Hand.Right, true);

        /// <summary>
        /// 提交客户端设置
        /// </summary>
        /// <returns></returns>
        public async Task SubmitClientSettings()
        {
            var (locale, viewDistance, chatMode, chatColors, displayedSkinParts, mainHand, disableTextFiltering) = ClientSettings;
            await Task.Yield();
            _protocolAdapter.WritePacket(new ClientSettingsPacket
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

        /// <summary>
        /// 发送聊天
        /// </summary>
        /// <param name="message">内容</param>
        /// <returns></returns>
        public async Task SendChatPacket(string message)
        {
            await Task.Yield();
            _protocolAdapter.WritePacket(new ClientChatMessagePacket { Message = message });
        }

        #endregion

        #region Connection

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected { get; private set; }
        /// <summary>
        /// 是否已登录
        /// </summary>
        public bool IsLogined { get; private set; }
        /// <summary>
        /// 是否已加入
        /// </summary>
        public bool IsJoined { get; private set; }

        private bool _connecting = false;

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
            if (IsConnected) throw new InvalidOperationException("The adapter is already connected");
            if (_connecting) throw new InvalidOperationException("The adapter is connecting");
            _connecting = true;

            await _tcpClient.ConnectAsync(_hostname, _port);
            IsConnected = true;
            _connecting = false;
            _protocolAdapter = new ProtocolAdapter(_tcpClient.GetStream(), Protocol.Packets.PacketBoundTo.Server);
            Connected?.Invoke(this, (_hostname, _port));

            async Task LoginStart()
            {
                await Task.Yield();

                // C→S: Handshake State=2
                await _protocolAdapter.WritePacketAsync(new HandshakePacket
                {
                    NextState = ProtocolState.Login,
                    ProtocolVersion = _protocolVersion,
                    ServerAddress = _hostname,
                    ServerPort = _port,
                });

                // C→S: Login Start
                await _protocolAdapter.WritePacketAsync(new LoginStartPacket
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
            }

            _ = PacketHandleTask();
            _ = LoginStart().HandleException(ex =>
            {
                Exception?.Invoke(this, ex);
                _logger.Warn($"Adapter stopped with exception: {(ex is SocketException ? ex.Message : ex)}");
            });
        }

        private async Task PacketHandleTask()
        {
            await Task.Yield();
            try
            {
                while (IsConnected)
                {
                    var p = await _protocolAdapter.ReadPacketAsync();
                    if (p == null)
                        break;
                    switch (p)
                    {
                        case LoginDisconnectPacket packet:
                            _logger.Info($"Disconnect. Reason: {packet.Reason}");
                            IsConnected = false;
                            Disconnected?.Invoke(this, packet.Reason);
                            break;
                        case ServerChatMessagePacket packet:
                            _logger.Info(packet.JsonData);
                            break;
                        case LoginSuccessPacket packet:
                            Logined?.Invoke(this, (packet.Username, packet.Uuid));
                            _logger.Info($"User {packet.Username} ({packet.Uuid}) logined");
                            IsLogined = true;
                            break;
                        case DisconnectPacket packet:
                            _logger.Info($"Disconnect. Reason: {packet.Reason}");
                            IsConnected = false;
                            Disconnected?.Invoke(this, packet.Reason);
                            break;
                        case JoinGamePacket packet:
                            Joined?.Invoke(this, (packet.EntityId, packet.IsHardcore, packet.Gamemode, packet.PreviousGamemode, packet.WorldCount, packet.WorldNames, packet.DimensionCodec, packet.Dimension, packet.WorldName, packet.HashedSeed, packet.MaxPlayers, packet.ViewDistance, packet.ReducedDebugInfo, packet.EnableRespawnScreen, packet.IsDebug, packet.IsFlat));
                            await SubmitClientSettings();
                            break;
                        case ServerDifficultyPacket packet:
                            ServerDiffculty?.Invoke(this, (packet.Difficulty, packet.DifficultyLocked));
                            break;
                        case PlayerAbilitiesPacket packet:
                            PlayerAbilities?.Invoke(this, (packet.Flags, packet.FlyingSpeed, packet.FieldOfViewModifier));
                            break;
                        case HeldItemChangePacket packet:
                            HeldItemChange?.Invoke(this, packet.Slot);
                            break;
                        case PlayerPositionAndLookPacket packet:
                            _ = _protocolAdapter.WritePacketAsync(new TeleportConfirmPacket { TeleportId = packet.TeleportId });
                            if (!IsJoined)
                            {
                                IsJoined = true;
                                _ = _protocolAdapter.WritePacketAsync(new PlayerPositionAndRotationPacket
                                {
                                    Position = packet.Position,
                                    Rotation = packet.Rotation,
                                    OnGround = true
                                });
                                _ = _protocolAdapter.WritePacketAsync(new ClientStatusPacket
                                {
                                    ActionId = ClientStatusAction.PerformRespawn
                                });
                            }
                            PlayerPosition?.Invoke(this, (packet.Position, packet.XKind, packet.YKind, packet.ZKind, packet.TeleportId, packet.DismountVehicle));
                            PlayerLook?.Invoke(this, (packet.Rotation, packet.YRotKind, packet.XRotKind, packet.TeleportId));
                            break;
                    }
                }
            }
            catch (SocketException ex)
            {
                IsConnected = false;
                Disconnected?.Invoke(this, $"{ex.GetType().FullName}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Exception?.Invoke(this, ex);
            }
            finally
            {
                if (IsConnected)
                {
                    Disconnected?.Invoke(this, "Connection closed");
                }
                IsConnected = false;
                IsLogined = false;
                IsJoined = false;
            }
        }

        public async Task Disconnect()
        {
            if (!IsConnected || _stopping) return;
            _stopping = true;
            IsConnected = false;
            _protocolAdapter.Dispose();
            _tcpClient.Dispose();
            _stopping = false;
            await Task.CompletedTask;
        }

        #endregion
    }
}
