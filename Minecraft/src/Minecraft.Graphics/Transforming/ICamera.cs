using OpenTK.Mathematics;
using static OpenTK.Mathematics.MathHelper;

namespace Minecraft.Graphics.Transforming
{
    public interface ICamera
    {
        Vector3 Position { get; set; }

        /// <summary>
        /// Rotation angle (in degree)
        /// </summary>
        Vector2 Rotation { get; set; }

        Vector3 Front => (
            (float)(Sin(DegreesToRadians(Rotation.X)) * Cos(DegreesToRadians(Rotation.Y))),
            (float)Sin(DegreesToRadians(Rotation.Y)),
            (float)(-Cos(DegreesToRadians(Rotation.X)) * Cos(DegreesToRadians(Rotation.Y))));

        Vector3 Target => Position + Front;
        Vector3 Up => Vector3.UnitY;
        Vector3 Right => Vector3.Cross(Front, Up).Normalized();

        void LookAt(Vector3 target)
        {
            var delta = (target - Position).Normalized();
            var horz = new Vector3(delta.X, 0, delta.Z).Normalized();
            var x = (float)RadiansToDegrees(Acos(horz.X));
            var y = (float)RadiansToDegrees(Acos(Vector3.Dot(delta, horz)));
            Rotation = (x, y);
        }
    }
}