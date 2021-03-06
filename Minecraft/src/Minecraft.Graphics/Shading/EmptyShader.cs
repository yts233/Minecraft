using OpenTK.Graphics.OpenGL;

namespace Minecraft.Graphics.Shading
{
    /// <summary>
    ///     着色器连接器
    /// </summary>
    public class ShaderBuilder
    {
        /// <summary>
        ///     创建着色器连接器
        /// </summary>
        public ShaderBuilder()
        {
            Logger.Info<ShaderBuilder>($"Create shader: {ShaderProgram}");
        }

        /// <summary>
        ///     着色器程序
        /// </summary>
        public int ShaderProgram { get; } = GL.CreateProgram();

        /// <summary>
        ///     获取该着色器连接器是否已被链接
        /// </summary>
        public bool Linked { get; private set; }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private int _attachShader(ShaderType type, string source, int oldValue)
        {
            _checkProgramLinked();
            if (oldValue != -1) throw new ShaderException("Shader has already been created.");

            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var success);
            if (success == 0) throw new ShaderException($"{type}: {GL.GetShaderInfoLog(shader)}");

            GL.AttachShader(ShaderProgram, shader);
            GL.DeleteShader(shader);
            Logger.Info<ShaderBuilder>($"{type}: {shader} attached");
            return shader;
        }

        private int _detachShader(int shader)
        {
            _checkProgramLinked();
            if (shader != -1) GL.DetachShader(ShaderProgram, shader);

            Logger.Info<ShaderBuilder>($"Shader: {shader} detached");
            return -1;
        }

        private void _checkProgramLinked()
        {
            if (Linked) throw new ShaderException("Program has already been linked.");
        }

        /// <summary>
        ///     链接着色器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ShaderException">着色器程序链接失败</exception>
        public Shader Link()
        {
            _checkProgramLinked();
            GL.LinkProgram(ShaderProgram);
            GL.GetProgram(ShaderProgram, GetProgramParameterName.LinkStatus, out var success);
            if (success == 0) throw new ShaderException($"Program: {GL.GetProgramInfoLog(ShaderProgram)}");

            Linked = true;
            Logger.Info<ShaderBuilder>($"Shader program: {ShaderProgram} linked");
            return new Shader(this);
        }

        #region Shaders

        public int ComputeShaderHandle { get; private set; } = -1;
        public int FragmentShaderHandle { get; private set; } = -1;
        public int GeometryShaderHandle { get; private set; } = -1;
        public int VertexShaderHandle { get; private set; } = -1;
        public int FragmentShaderArbHandle { get; private set; } = -1;
        public int GeometryShaderExtHandle { get; private set; } = -1;
        public int TessControlShaderHandle { get; private set; } = -1;
        public int TessEvaluationShaderHandle { get; private set; } = -1;
        public int VertexShaderArbHandle { get; private set; } = -1;

        #endregion

        #region Attach

        public ShaderBuilder AttachComputeShader(string source)
        {
            return (ComputeShaderHandle = _attachShader(ShaderType.ComputeShader, source, ComputeShaderHandle)) != -1
                ? this
                : null;
        }

        public ShaderBuilder AttachFragmentShader(string source)
        {
            return (FragmentShaderHandle = _attachShader(ShaderType.FragmentShader, source, FragmentShaderHandle)) != -1
                ? this
                : null;
        }

        public ShaderBuilder AttachGeometryShader(string source)
        {
            return (GeometryShaderHandle = _attachShader(ShaderType.GeometryShader, source, GeometryShaderHandle)) != -1
                ? this
                : null;
        }

        public ShaderBuilder AttachVertexShader(string source)
        {
            return (VertexShaderHandle = _attachShader(ShaderType.VertexShader, source, VertexShaderHandle)) != -1
                ? this
                : null;
        }

        public ShaderBuilder AttachFragmentShaderArb(string source)
        {
            return (FragmentShaderArbHandle =
                _attachShader(ShaderType.FragmentShaderArb, source, FragmentShaderArbHandle)) != -1
                ? this
                : null;
        }

        public ShaderBuilder AttachGeometryShaderExt(string source)
        {
            return (GeometryShaderExtHandle =
                _attachShader(ShaderType.GeometryShaderExt, source, GeometryShaderExtHandle)) != -1
                ? this
                : null;
        }

        public ShaderBuilder AttachTessControlShader(string source)
        {
            return (TessControlShaderHandle =
                _attachShader(ShaderType.TessControlShader, source, TessControlShaderHandle)) != -1
                ? this
                : null;
        }

        public ShaderBuilder AttachTessEvaluationShader(string source)
        {
            return (TessEvaluationShaderHandle =
                _attachShader(ShaderType.TessEvaluationShader, source, TessEvaluationShaderHandle)) != -1
                ? this
                : null;
        }

        public ShaderBuilder AttachVertexShaderArb(string source)
        {
            return (VertexShaderArbHandle = _attachShader(ShaderType.VertexShaderArb, source, VertexShaderArbHandle)) !=
                   -1
                ? this
                : null;
        }

        #endregion

        #region Detach

        public ShaderBuilder DetachComputeShader()
        {
            return (ComputeShaderHandle = _detachShader(ComputeShaderHandle)) == -1 ? this : null;
        }

        public ShaderBuilder DetachFragmentShader()
        {
            return (FragmentShaderHandle = _detachShader(FragmentShaderHandle)) == -1 ? this : null;
        }

        public ShaderBuilder DetachGeometryShader()
        {
            return (GeometryShaderHandle = _detachShader(GeometryShaderHandle)) == -1 ? this : null;
        }

        public ShaderBuilder DetachVertexShader()
        {
            return (VertexShaderHandle = _detachShader(VertexShaderHandle)) == -1 ? this : null;
        }

        public ShaderBuilder DetachFragmentShaderArb()
        {
            return (FragmentShaderArbHandle = _detachShader(FragmentShaderArbHandle)) == -1 ? this : null;
        }

        public ShaderBuilder DetachGeometryShaderExt()
        {
            return (GeometryShaderExtHandle = _detachShader(GeometryShaderExtHandle)) == -1 ? this : null;
        }

        public ShaderBuilder DetachTessControlShader()
        {
            return (TessControlShaderHandle = _detachShader(TessControlShaderHandle)) == -1 ? this : null;
        }

        public ShaderBuilder DetachTessEvaluationShader()
        {
            return (TessEvaluationShaderHandle = _detachShader(TessEvaluationShaderHandle)) == -1 ? this : null;
        }

        public ShaderBuilder DetachVertexShaderArb()
        {
            return (VertexShaderArbHandle = _detachShader(VertexShaderArbHandle)) == -1 ? this : null;
        }

        #endregion
    }
}