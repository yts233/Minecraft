using System.Diagnostics.CodeAnalysis;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Shading
{
    public class DerivedShader : IShader
    {
        [MaybeNull] public IShader BaseShader { get; set; }

        public int Handle => BaseShader?.Handle ?? 0;

        public double GetDouble(int location)
        {
            return BaseShader?.GetDouble(location) ?? default;
        }

        public float GetFloat(int location)
        {
            return BaseShader?.GetFloat(location) ?? default;
        }

        public int GetInt(int location)
        {
            return BaseShader?.GetInt(location) ?? default;
        }

        public int GetLocation(string name)
        {
            return BaseShader?.GetLocation(name) ?? default;
        }

        public Matrix4 GetMatrix4(int location)
        {
            return BaseShader?.GetMatrix4(location) ?? default;
        }

        public Vector2 GetVector2(int location)
        {
            return BaseShader?.GetVector2(location) ?? default;
        }

        public Vector3 GetVector3(int location)
        {
            return BaseShader?.GetVector3(location) ?? default;
        }

        public Vector4 GetVector4(int location)
        {
            return BaseShader?.GetVector4(location) ?? default;
        }

        public void SetDouble(int location, double value)
        {
            BaseShader?.SetDouble(location, value);
        }

        public void SetFloat(int location, float value)
        {
            BaseShader?.SetFloat(location, value);
        }

        public void SetInt(int location, int value)
        {
            BaseShader?.SetInt(location, value);
        }

        public void SetMatrix4(int location, ref Matrix4 matrix)
        {
            BaseShader?.SetMatrix4(location, ref matrix);
        }

        public void SetVector2(int location, Vector2 vector)
        {
            BaseShader?.SetVector2(location, vector);
        }

        public void SetVector3(int location, Vector3 vector)
        {
            BaseShader?.SetVector3(location, vector);
        }

        public void SetVector4(int location, Vector4 vector)
        {
            BaseShader?.SetVector4(location, vector);
        }

        public void Use()
        {
            BaseShader?.Use();
        }
    }
}