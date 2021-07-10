using OpenTK.Mathematics;

namespace Minecraft.Graphics.Shading
{
    /// <summary>
    ///     着色器
    /// </summary>
    public sealed class Shader : ShaderBase
    {
        /// <summary>
        ///     从已存在的着色器创建
        /// </summary>
        /// <param name="shader"></param>
        /// <exception cref="ShaderException"></exception>
        public Shader(ShaderBuilder shader) : base(shader.ShaderProgram)
        {
            if (!shader.Linked) throw new ShaderException("The shader program hadn't been linked yet.");

            BaseShader = shader;
        }

        /// <summary>
        ///     基着色器
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ShaderBuilder BaseShader { get; }

        /// <summary>
        ///     着色器程序
        /// </summary>
        public new int ShaderProgram => base.ShaderProgram;

        /// <summary>
        ///     使用着色器
        /// </summary>
        public new void Use()
        {
            base.Use();
        }

        #region GetLocation

        /// <summary>
        ///     获取变量位置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new int GetLocation(string name)
        {
            return base.GetLocation(name);
        }

        #endregion

        #region Sets

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        public new void SetFloat(int location, float value)
        {
            base.SetFloat(location, value);
        }

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        public new void SetDouble(int location, double value)
        {
            base.SetDouble(location, value);
        }

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        public new void SetInt(int location, int value)
        {
            base.SetInt(location, value);
        }

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="vector"></param>
        public new void SetVector2(int location, Vector2 vector)
        {
            base.SetVector2(location, vector);
        }

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="vector"></param>
        public new void SetVector3(int location, Vector3 vector)
        {
            base.SetVector3(location, vector);
        }

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="vector"></param>
        public new void SetVector4(int location, Vector4 vector)
        {
            base.SetVector4(location, vector);
        }

        /// <summary>
        ///     设置变量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="matrix"></param>
        public new void SetMatrix4(int location, ref Matrix4 matrix)
        {
            base.SetMatrix4(location, ref matrix);
        }

        #endregion

        #region Gets

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public new float GetFloat(int location)
        {
            return base.GetFloat(location);
        }

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public new double GetDouble(int location)
        {
            return base.GetDouble(location);
        }

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public new int GetInt(int location)
        {
            return base.GetInt(location);
        }

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public new Vector2 GetVector2(int location)
        {
            return base.GetVector2(location);
        }

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public new Vector3 GetVector3(int location)
        {
            return base.GetVector3(location);
        }

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public new Vector4 GetVector4(int location)
        {
            return base.GetVector4(location);
        }

        /// <summary>
        ///     获取变量
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public new Matrix4 GetMatrix4(int location)
        {
            return base.GetMatrix4(location);
        }

        #endregion
    }
}