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
using Minecraft.Numerics;
using ClientChatMessagePacket = Minecraft.Protocol.Packets.Client.ChatMessagePacket;
using ServerChatMessagePacket = Minecraft.Protocol.Packets.Server.ChatMessagePacket;
using Minecraft.Protocol.Packets;

namespace Minecraft.Client
{
    /// <summary>
    /// Minecraft 客户端适配器
    /// </summary>
    public class MinecraftClientAdapter
    {
        private readonly string _hostname;
        private readonly ushort _port;

        private readonly int _protocolVersion;
        private bool _stopping = false;
        private readonly TcpClient _tcpClient;
        private ProtocolAdapter _protocolAdapter;
        private readonly MinecraftClient _client;
        private static readonly Logger<MinecraftClientAdapter> _logger = Logger.GetLogger<MinecraftClientAdapter>();

        public MinecraftClientAdapter(string hostname, ushort port, MinecraftClient client)
        {
            _hostname = hostname;
            _port = port;
            _protocolVersion = client._protocolVersion;
            _tcpClient = new TcpClient();
            _client = client;
        }

        public ProtocolAdapter GetProtocol()
        {
            if (!IsConnected)
                return null;
            return _protocolAdapter;
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
        /// 玩家生成
        /// </summary>
        public event EventHandler<(int entityId, Uuid playerUuid, Vector3d position, Rotation rotation)> SpawnPlayer;
        /// <summary>
        /// 接收聊天
        /// </summary>
        public event EventHandler<(string jsonData, ChatMessagePosition position, Uuid sender)> Chat;
        /// <summary>
        /// 实体移动8格以内距离
        /// </summary>
        public event EventHandler<(int entityId, Vector3d delta, bool onGround)> EntityDeltaMove;
        /// <summary>
        /// 实体朝向改变
        /// </summary>
        public event EventHandler<(int entityId, Rotation rotation, bool onGround)> EntityRotation;
        /// <summary>
        /// 实体移动8格以外距离
        /// </summary>
        public event EventHandler<(int entityId, Vector3d position, Rotation rotation, bool onGround)> EntityTeleport;
        /// <summary>
        /// 实体在客户端中被移除
        /// </summary>
        public event EventHandler<(int count, int[] entityIds)> EntitiesDestroyed;
        /// <summary>
        /// 客户端所在区块位置更新
        /// </summary>
        public event EventHandler<(int chunkX, int chunkZ)> UpdateViewPosition;
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
        public (string locale, sbyte viewDistance, ChatMode chatMode, bool chatColors, SkinPart displayedSkinParts, HandSide mainHand, bool disableTextFiltering) ClientSettings { get; set; } = ("en_US", 4, ChatMode.Enabled, true, SkinPart.All, HandSide.Right, true);

        /// <summary>
        /// 提交客户端设置
        /// </summary>
        /// <returns></returns>
        public void SubmitClientSettings()
        {
            var (locale, viewDistance, chatMode, chatColors, displayedSkinParts, mainHand, disableTextFiltering) = ClientSettings;
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
        public void SendChatPacket(string message)
        {
            _protocolAdapter.WritePacket(new ClientChatMessagePacket { Message = message });
        }

        /// <summary>
        /// 当玩家用一个速度移动时发送
        /// </summary>
        /// <remarks>使用绝对坐标</remarks>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public void SendVehicleMovePacket(Vector3d position, Rotation rotation)
        {
            _protocolAdapter.WritePacket(new VehicleMovePacket { Position = position, Rotation = rotation });
        }

        /// <summary>
        /// 发送玩家位置数据包
        /// </summary>
        /// <remarks>使用绝对坐标</remarks>
        /// <param name="position">玩家脚部的坐标，FeetY一般为HeadY-1.62</param>
        /// <param name="onGround"></param>
        /// <returns></returns>
        public void SendPlayerPositionPacket(Vector3d position, bool onGround)
        {
            _protocolAdapter.WritePacket(new PlayerPositionPacket { Position = position, OnGround = onGround });
        }

        /// <summary>
        /// 发送玩家朝向数据包
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public void SendPlayerRotationPacket(Rotation rotation, bool onGround)
        {
            _protocolAdapter.WritePacket(new PlayerRotationPacket { Rotation = rotation, OnGround = onGround });
        }

        /// <summary>
        /// 发送玩家坐标朝向数据包
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="onGround"></param>
        /// <returns></returns>
        public void SendPlayerPositionAndRotationPacket(Vector3d position, Rotation rotation, bool onGround)
        {
            _protocolAdapter.WritePacket(new PlayerPositionAndRotationPacket { Position = position, Rotation = rotation, OnGround = onGround });
        }

        /// <summary>
        /// 发送玩家移动数据包
        /// </summary>
        /// <param name="onGround"></param>
        /// <returns></returns>
        public void SendPlayerMovementPacket(bool onGround)
        {
            _protocolAdapter.WritePacket(new PlayerMovementPacket { OnGround = onGround });
        }

        /// <summary>
        /// 发送交互数据包
        /// </summary>
        /// <remarks>交互类型为攻击</remarks>
        /// <param name="entityId"></param>
        /// <param name="sneaking"></param>
        /// <returns></returns>
        public void SendInteractEntityPacket(int entityId, bool sneaking)
        {
            _protocolAdapter.WritePacket(new InteractEntityPacket { EntityId = entityId, Type = InteractType.Attack, Sneaking = sneaking });
        }

        /// <summary>
        /// 发送交互数据包
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="hand"></param>
        /// <param name="sneaking"></param>
        /// <returns></returns>
        public void SendInteractEntityPacket(int entityId, Hand hand, bool sneaking)
        {
            _protocolAdapter.WritePacket(new InteractEntityPacket { EntityId = entityId, Type = InteractType.Interact, Hand = hand, Sneaking = sneaking });
        }

        /// <summary>
        /// 发送交互数据包
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="target"></param>
        /// <param name="hand"></param>
        /// <param name="sneaking"></param>
        /// <returns></returns>
        public void SendInteractEntityPacket(int entityId, Vector3f target, Hand hand, bool sneaking)
        {
            _protocolAdapter.WritePacket(new InteractEntityPacket { EntityId = entityId, Type = InteractType.InteractAt, Target = target, Hand = hand, Sneaking = sneaking });
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
        public void Connect()
        {
            if (IsConnected) throw new InvalidOperationException("The adapter is already connected");
            if (_connecting) throw new InvalidOperationException("The adapter is connecting");
            _connecting = true;

            _tcpClient.Connect(_hostname, _port);
            IsConnected = true;
            _connecting = false;
            _protocolAdapter = new ProtocolAdapter(_tcpClient.GetStream(), PacketBoundTo.Server);
            _protocolAdapter.Start();
            Connected?.Invoke(this, (_hostname, _port));

            // C→S: Handshake State=2
            _protocolAdapter.WritePacket(new HandshakePacket
            {
                NextState = ProtocolState.Login,
                ProtocolVersion = _protocolVersion,
                ServerAddress = _hostname,
                ServerPort = _port,
            });

            // C→S: Login Start
            _protocolAdapter.WritePacket(new LoginStartPacket
            {
                Name = _client._playerName
            });

            /*if (!_client._offlineMode)
            {
                // S→C: Encryption Request
                // Client auth

                // C→S: Encryption Response
                // Server auth, both enable encryption
            }*/

            ThreadHelper.StartThread(HandlePackets, "NetworkPacketWorker", true);
        }

        public void Disconnect()
        {
            if (!IsConnected || _stopping) return;
            _stopping = true;
            IsConnected = false;
            _protocolAdapter.Close();
            _tcpClient.Dispose();
            _stopping = false;
        }

        #endregion

        private void HandlePackets()
        {
            try
            {
                while (IsConnected)
                {
                    var p = _protocolAdapter.ReadPacket();
                    if (p == null)
                        break;
                    switch (p)
                    {
                        // handle packets here
                        case LoginDisconnectPacket packet:
                            IsConnected = false;
                            Disconnected?.Invoke(this, packet.Reason);
                            break;
                        case ServerChatMessagePacket packet:
                            Chat?.Invoke(this, (packet.JsonData, packet.Position, packet.Sender));
                            break;
                        case LoginSuccessPacket packet:
                            Logined?.Invoke(this, (packet.Username, packet.Uuid));
                            _logger.Info($"User {packet.Username} ({packet.Uuid}) logined");
                            IsLogined = true;
                            break;
                        case DisconnectPacket packet:
                            IsConnected = false;
                            Disconnected?.Invoke(this, packet.Reason);
                            break;
                        case JoinGamePacket packet:
                            Joined?.Invoke(this, (packet.EntityId, packet.IsHardcore, packet.Gamemode, packet.PreviousGamemode, packet.WorldCount, packet.WorldNames, packet.DimensionCodec, packet.Dimension, packet.WorldName, packet.HashedSeed, packet.MaxPlayers, packet.ViewDistance, packet.ReducedDebugInfo, packet.EnableRespawnScreen, packet.IsDebug, packet.IsFlat));
                            SubmitClientSettings();
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
                            _protocolAdapter.WritePacket(new TeleportConfirmPacket { TeleportId = packet.TeleportId });
                            if (!IsJoined)
                            {
                                IsJoined = true;
                                _protocolAdapter.WritePacket(new PlayerPositionAndRotationPacket
                                {
                                    Position = packet.Position,
                                    Rotation = packet.Rotation,
                                    OnGround = true
                                });
                                _protocolAdapter.WritePacket(new ClientStatusPacket
                                {
                                    ActionId = ClientStatusAction.PerformRespawn
                                });
                            }
                            PlayerPosition?.Invoke(this, (packet.Position, packet.XKind, packet.YKind, packet.ZKind, packet.TeleportId, packet.DismountVehicle));
                            PlayerLook?.Invoke(this, (packet.Rotation, packet.YRotKind, packet.XRotKind, packet.TeleportId));
                            break;
                        case SpawnPlayerPacket packet:
                            SpawnPlayer?.Invoke(this, (packet.EntityId, packet.PlayerUuid, packet.Position, packet.Rotation));
                            break;
                        case EntityPositionPacket packet:
                            EntityDeltaMove?.Invoke(this, (packet.EntityId, packet.Delta, packet.OnGround));
                            break;
                        case EntityRotationPacket packet:
                            EntityRotation?.Invoke(this, (packet.EntityId, packet.Rotation, packet.OnGround));
                            break;
                        case EntityPositionAndRotationPacket packet:
                            EntityDeltaMove?.Invoke(this, (packet.EntityId, packet.Delta, packet.OnGround));
                            EntityRotation?.Invoke(this, (packet.EntityId, packet.Rotation, packet.OnGround));
                            break;
                        case EntityTeleportPacket packet:
                            EntityTeleport?.Invoke(this, (packet.EntityId, packet.Position, packet.Rotation, packet.OnGround));
                            break;
                        case DestroyEntitiesPacket packet:
                            EntitiesDestroyed?.Invoke(this, (packet.Count, packet.EntityIds));
                            break;
                        case UpdateViewPositionPacket packet:
                            UpdateViewPosition?.Invoke(this, (packet.ChunkX, packet.ChunkZ));
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
                _logger.Warn(ex);
                Exception?.Invoke(this, ex);
            }
            finally
            {
                _tcpClient.Dispose();
                if (IsConnected)
                {
                    Disconnected?.Invoke(this, "Connection closed");
                }
                IsConnected = false;
                IsLogined = false;
                IsJoined = false;
            }
        }
    }
}
