using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Graphics.Renderers.Environments.Skybox
{
    internal class SkyboxShader : ShaderBase
    {
        private readonly int _viewLocation;
        private readonly int _modelLocation;
        private readonly int _projectionLocation;

        public SkyboxShader() : base(new ShaderBuilder().AttachVertexShader(EnvironmentShaders.SkyboxVertexShaderSource).AttachFragmentShader(EnvironmentShaders.SkyboxFragmentShaderSource))
        {
            _modelLocation = GetLocation("model");
            _viewLocation = GetLocation("view");
            _projectionLocation = GetLocation("projection");
        }

        public Matrix4 Model { get => GetMatrix4(_modelLocation); set => SetMatrix4(_modelLocation, ref value); }
        public Matrix4 View { get => GetMatrix4(_viewLocation); set => SetMatrix4(_viewLocation, ref value); }
        public Matrix4 Projection { get => GetMatrix4(_projectionLocation); set => SetMatrix4(_projectionLocation, ref value); }
    }
}
