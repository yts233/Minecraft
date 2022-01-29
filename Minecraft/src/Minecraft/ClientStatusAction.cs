namespace Minecraft
{
    public enum ClientStatusAction : int
    {
        /// <summary>
        /// Perform respawn action
        /// </summary>
        /// <remarks>Sent when the client is ready to complete login and when the client is ready to respawn after death.</remarks>
        PerformRespawn = 0,
        /// <summary>
        /// Request stats action
        /// </summary>
        /// <remarks>Sent when the client opens the Statistics menu.</remarks>
        RequestStats = 1
    }
}
