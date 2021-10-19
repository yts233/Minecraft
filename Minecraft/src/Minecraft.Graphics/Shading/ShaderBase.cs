using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Shading
{
    /// <summary>
    /// 提供没有Get、Set方法的着色器基类
    /// </summary>
    public abstract class ShaderBase : IShader
    {
        /// <summary>
        /// 创建着色器基类
        /// </summary>
        /// <param name="shaderProgram">着色器程序</param>
        protected ShaderBase(int shaderProgram)
        {
            ShaderProgram = shaderProgram;
        }

        /// <summary>
        /// 创建着色器基类
        /// </summary>
        /// <param name="shader">着色器</param>
        protected ShaderBase(IHandle shader) : this(shader.Handle)
        {
        }

        /// <summary>
        /// 着色器程序
        /// </summary>
        protected virtual int ShaderProgram { get; }

        int IHandle.Handle => ShaderProgram;

        /// <summary>
        /// 使用着色器
        /// </summary>
        public virtual void Use()
        {
            GL.UseProgram(ShaderProgram);
        }

        /// <summary>
        /// 获取变量位置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual int GetLocation(string name)
        {
            var value = GL.GetUniformLocation(ShaderProgram, name);
            //if (value == 0)
            //    throw new ShaderException($"Can't get \"{name}\" location.");
            return value;
        }

        #region Sets

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        protected virtual void SetFloat(int location, float value)
        {
            GL.Uniform1(location, value);
        }

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        protected virtual void SetDouble(int location, double value)
        {
            GL.Uniform1(location, value);
        }

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        protected virtual void SetInt(int location, int value)
        {
            GL.Uniform1(location, value);
        }

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="vector"></param>
        protected virtual void SetVector2(int location, Vector2 vector)
        {
            GL.Uniform2(location, vector);
        }

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="vector"></param>
        protected virtual void SetVector3(int location, Vector3 vector)
        {
            GL.Uniform3(location, vector);
        }

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="vector"></param>
        protected virtual void SetVector4(int location, Vector4 vector)
        {
            GL.Uniform4(location, vector);
        }

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="matrix"></param>
        protected virtual void SetMatrix4(int location, ref Matrix4 matrix)
        {
            GL.UniformMatrix4(location, false, ref matrix);
        }

        #endregion

        #region Gets

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected virtual float GetFloat(int location)
        {
            GL.GetUniform(ShaderProgram, location, out float value);
            return value;
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected virtual double GetDouble(int location)
        {
            GL.GetUniform(ShaderProgram, location, out double value);
            return value;
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected virtual int GetInt(int location)
        {
            GL.GetUniform(ShaderProgram, location, out int value);
            return value;
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected virtual Vector2 GetVector2(int location)
        {
            var buffer = new float[2];
            GL.GetnUniform(ShaderProgram, location, sizeof(float) * 2, buffer);
            return new Vector2(buffer[0], buffer[1]);
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected virtual Vector3 GetVector3(int location)
        {
            var buffer = new float[3];
            GL.GetnUniform(ShaderProgram, location, sizeof(float) * 3, buffer);
            return new Vector3(buffer[0], buffer[1], buffer[3]);
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected virtual Vector4 GetVector4(int location)
        {
            var buffer = new float[4];
            GL.GetnUniform(ShaderProgram, location, sizeof(float) * 4, buffer);
            return new Vector4(buffer[0], buffer[1], buffer[2], buffer[3]);
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected virtual Matrix4 GetMatrix4(int location)
        {
            var buffer = new float[16];
            GL.GetnUniform(ShaderProgram, location, sizeof(float) * 16, buffer);
            return new Matrix4(
                buffer[0], buffer[1], buffer[2], buffer[3],
                buffer[4], buffer[5], buffer[6], buffer[7],
                buffer[8], buffer[9], buffer[10], buffer[11],
                buffer[12], buffer[13], buffer[14], buffer[15]
            );
        }

        #endregion

        #region IShader

        void IShader.Use()
        {
            Use();
        }

        int IShader.GetLocation(string name)
        {
            return GetLocation(name);
        }

        void IShader.SetFloat(int location, float value)
        {
            SetFloat(location, value);
        }

        void IShader.SetDouble(int location, double value)
        {
            SetDouble(location, value);
        }

        void IShader.SetInt(int location, int value)
        {
            SetInt(location, value);
        }

        void IShader.SetVector2(int location, Vector2 vector)
        {
            SetVector2(location, vector);
        }

        void IShader.SetVector3(int location, Vector3 vector)
        {
            SetVector3(location, vector);
        }

        void IShader.SetVector4(int location, Vector4 vector)
        {
            SetVector4(location, vector);
        }

        void IShader.SetMatrix4(int location, ref Matrix4 matrix)
        {
            SetMatrix4(location, ref matrix);
        }

        float IShader.GetFloat(int location)
        {
            return GetFloat(location);
        }

        double IShader.GetDouble(int location)
        {
            return GetDouble(location);
        }

        int IShader.GetInt(int location)
        {
            return GetInt(location);
        }

        Vector2 IShader.GetVector2(int location)
        {
            return GetVector2(location);
        }

        Vector3 IShader.GetVector3(int location)
        {
            return GetVector3(location);
        }

        Vector4 IShader.GetVector4(int location)
        {
            return GetVector4(location);
        }

        Matrix4 IShader.GetMatrix4(int location)
        {
            return GetMatrix4(location);
        }

        #endregion
    }
}