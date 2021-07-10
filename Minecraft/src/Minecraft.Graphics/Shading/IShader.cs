using System;
using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Shading
{
    /// <summary>
    ///     着色器
    /// </summary>
    public interface IShader : IHandle,IDisposable
    {
        void IBindable.Bind()
        {
            Use();
        }

        /// <summary>
        ///     使用着色器
        /// </summary>
        void Use();

        /// <summary>
        ///     获取变量位置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetLocation(string name);

        #region Sets

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        void SetFloat(int location, float value);

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        void SetDouble(int location, double value);

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        void SetInt(int location, int value);

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="vector"></param>
        void SetVector2(int location, Vector2 vector);

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="vector"></param>
        void SetVector3(int location, Vector3 vector);

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="vector"></param>
        void SetVector4(int location, Vector4 vector);

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="matrix"></param>
        void SetMatrix4(int location, ref Matrix4 matrix);

        #endregion

        #region Gets

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        float GetFloat(int location);

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        double GetDouble(int location);

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        int GetInt(int location);

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Vector2 GetVector2(int location);

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Vector3 GetVector3(int location);

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Vector4 GetVector4(int location);

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Matrix4 GetMatrix4(int location);

        #endregion

        void IDisposable.Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}