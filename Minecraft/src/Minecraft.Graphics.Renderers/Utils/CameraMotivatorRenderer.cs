using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Transforming;
using Minecraft.Input;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Utils
{
    public class CameraMotivatorRenderer : IUpdatable, ICameraMotivator
    {
        private readonly ICamera _camera;

        public CameraMotivatorRenderer(ICamera camera)
        {
            _camera = camera;
        }

        private Vector3 _local, _local2Global, _global, _resultant;
        private Vector2 _lastCameraRotation;

        public bool Controlable { get; set; }
        public IAxisInput PositionInput { get; set; }
        public IAxisInput RotationInput { get; set; }
        public float Speed { get; set; } = 0.0125F;
        public Vector3 GlobalVelocity
        {
            get => _global;
            set
            {
                _global = value;
                _resultant = _global + _local2Global;
            }
        }

        public Vector3 LocalVelocity
        {
            get => _local;
            set
            {
                if (_local == value && _lastCameraRotation == _camera.Rotation)
                    return; // avoid to recalculate
                _local = value;
                _lastCameraRotation = _camera.Rotation;
                if (_type == CameraType.Flight)
                    _local2Global = Matrix3.CreateRotationY(MathHelper.DegreesToRadians(_camera.Rotation.Y)) * Matrix3.CreateRotationX(MathHelper.DegreesToRadians(_camera.Rotation.X)) * value;
                else
                {
                    var tmp = Matrix2.CreateRotation(_camera.Rotation.X) * new Vector2(value.X, -value.Z);
                    _local2Global = new Vector3(tmp.X, value.Y, -tmp.Y);
                }
                _resultant = _global + _local2Global;
            }
        }

        public Vector3 ResultantVelocity
        {
            get => _resultant;
            set
            {
                if (_global == value)
                    return; // avoid to recalculate
                _local = Vector3.Zero;
                _global = value;
                _resultant = value;
            }
        }

        private CameraType _type = CameraType.Flight;

        public CameraType Type { get; set; } = CameraType.Flight;

        public ICamera GetCamera()
        {
            return _camera;
        }

        public void Update()
        {
            if (Controlable)
            {
                if (PositionInput != null)
                {
                    PositionInput.Update();
                    LocalVelocity = PositionInput.Value;
                }
                if (RotationInput != null)
                {
                    RotationInput.Update();
                    _camera.Rotation += RotationInput.Value.Xy;
                }
            }
            _camera.Position += _resultant * Speed;
        }
    }
}
