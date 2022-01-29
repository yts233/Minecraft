using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Rendering
{
    public interface IRenderContainer
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <remarks>请在此处设置其他属性</remarks>
        ICollection<IInitializer> Initializers { get; }

        /// <summary>
        /// 渲染器
        /// </summary>
        ICollection<IRenderable> Renderers { get; }

        /// <summary>
        /// 更新器
        /// </summary>
        /// <remarks>请不要在这里运行有关渲染和着色器相关代码</remarks>
        ICollection<IUpdatable> Updaters { get; }

        Vector2i ClientSize { get; }
        string Title { get; set; }
        double PreviousRenderTime { get; }
        double PreviousUpdateTime { get; }
        double RenderFrequency { get; set; }
        double UpdateFrequency { get; set; }
        void Close();

        #region Events

        event EventHandler ContainerInitalized;
        event EventHandler BeforeInitalizers;
        event EventHandler AfterInitalizers;
        event EventHandler BeforeRenderers;
        event EventHandler AfterRenderers;
        event EventHandler BeforeUpdaters;
        event EventHandler AfterUpdaters;
        event EventHandler ContainerStarted;
        event EventHandler ContainerClosed;
        event EventHandler<Vector2i> ClientSizeChanged;
        event EventHandler<Vector2i> RenderClientSizeChanged;

        #endregion
    }
}