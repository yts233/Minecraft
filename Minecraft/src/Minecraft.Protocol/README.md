# Minecraft.Protocol

此项目实现协议版本号为755,对应Minecraft版本为1.17.1

### What's the normal login sequence for a client?

See Authentication for communication with Mojang's servers.

The recommended login sequence looks like this, where C is the client and S is the
server:

1. Client connects to server
2. C→S: Handshake State=2
3. C→S: Login Start
4. S→C: Encryption Request
5. Client auth
6. C→S: Encryption Response
7. Server auth, both enable encryption
8. S→C: Set Compression (Optional, enables compression)
9. S→C: Login Success
10. S→C: Join Game
11. S→C: Plugin Message: minecraft:brand with the server's brand (Optional)
12. S→C: Server Difficulty (Optional)
13. S→C: Player Abilities (Optional)
14. C→S: Plugin Message: minecraft:brand with the client's brand (Optional)
15. C→S: Client Settings
16. S→C: Held Item Change
17. S→C: Declare Recipes
18. S→C: Tags
19. S→C: Entity Status
20. S→C: Declare Commands
21. S→C: Unlock Recipes
22. S→C: Player Position And Look
23. S→C: Player Info (Add Player action)
24. S→C: Player Info (Update latency action)
25. S→C: Update View Position
26. S→C: Update Light (One sent for each chunk in a square centered on the player's position)
27. S→C: Chunk Data (One sent for each chunk in a square centered on the player's position)
28. S→C: World Border (Once the world is finished loading)
29. S→C: Spawn Position (“home” spawn, not where the client will spawn on login)
30. S→C: Player Position And Look (Required, tells the client they're ready to spawn)
31. C→S: Teleport Confirm
32. C→S: Player Position And Look (to confirm the spawn position)
33. C→S: Client Status (sent either before or while receiving chunks, further testing needed, server handles correctly if not sent)
34. S→C: inventory, entities, etc

### Offline mode

If the server is in offline mode, it will not send the Encryption Request packet, and
likewise, the client should not send Encryption Response. In this case, encryption is
never enabled, and no authentication is performed.

Clients can tell that a server is in offline mode if the server sends a Login Success
without sending Encryption Request.

I think I've done everything right, but…

…my player isn't spawning!

After sending the common-sense packets (Handshake, Login Start, inventory,
compass, and chunks), you need to finally send the player their initial position for
them to leave the “Loading Map” screen.

Note that if the following steps are taken, a Minecraft client will spawn the player:

1 Do Handshake (see Protocol Encryption)
2 Send Spawn Position packet
3 Send Player Position And Look (clientbound) packet

While the above steps are sufficient for Minecraft 1.4.5, it is good form to send
packets that inform the client about the world around the player before allowing the
player to spawn.

…my client isn't receiving complete map chunks!

Main article: How to Write a Client

The standard Minecraft server sends full chunks only when your client is sending
player status update packets (any of Player (0x03) through Player Position And Look
(0x06)).

…all connecting clients spasm and jerk uncontrollably!

For newer clients, your server needs to send 49 chunks ahead of time, not just one.
Send a 7×7 square of chunks, centered on the connecting client's position, before
spawning them.

…the client is trying to send an invalid packet that begins with 0xFE01

The client is attempting a legacy ping, this happens if your server did not respond to
the Server List Ping properly, including if it sent malformed JSON.

…the client disconnects after some time with a "Timed out" error

The server is expected to send a Keep Alive packet every second, and the client
should respond with the serverbound version of that packet. If either party does not
receive keep alives for some period of time, they will disconnect.

How do I open/save a command block?

The process to actually open the command block window clientside is somewhat
complex; the client actually uses the Update Block Entity (0x09) packet to open it.

First, the client must have at least an OP permission level of 2, or else the client will
refuse to open the command block. (The op permission level is set with the Entity
Status packet)

To actually open the command block:

1. C→S: Player Block Placement (0x1C), with the position being the command block
that was right-clicked.
2. S→C: Update Block Entity (0x09), with the NBT of the command block.