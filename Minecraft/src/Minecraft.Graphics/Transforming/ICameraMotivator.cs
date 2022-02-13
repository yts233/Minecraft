using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public interface ICameraMotivator : IUpdatable
    {
        ICamera GetCamera();

        Vector3 GlobalVelocity { get; set; }
        Vector3 LocalVelocity { get; set; }

        Vector3 ResultantVelocity { get; set; }

        CameraType Type { get; set; }

        float MovementSpeed { get; set; }
    }
}