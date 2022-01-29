using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Shading;
using Minecraft.Graphics.Texturing;
using Minecraft.Resources.Fonts;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Graphics.Renderers.UI
{
    public class HudRenderer : ICompletedRenderer
    {
        private readonly IRenderContainer _container;
        private readonly Queue<IHudObject> _disposeObjects = new Queue<IHudObject>();
        private readonly Func<ITexture2DAtlas> _fontTextureProvider;
        private readonly Font _defaultFont;
        private ITexture2DAtlas _fontTexture;
        private HudShader _shader;
        private readonly List<IHudObject> _hudObjects = new List<IHudObject>();

        public IReadOnlyCollection<IHudObject> Objects => _hudObjects;

        public HudRenderer(IRenderContainer container, Func<ITexture2DAtlas> fontTextureProvider, Font defaultFont)
        {
            _container = container;
            _fontTextureProvider = fontTextureProvider;
            _defaultFont = defaultFont;
        }

        private void Container_RenderClientSizeChanged(object sender, Vector2i e)
        {
            _shader.Use();
            _shader.Projection = Matrix4.CreateOrthographicOffCenter(0F, e.X, e.Y, 0, -10F, 100F);
            //_shader.Projection = Matrix4.CreateOrthographicOffCenter(-10F, 10F, -10F, 10F, -100F, 300F);
        }

        ~HudRenderer()
        {
            _container.RenderClientSizeChanged -= Container_RenderClientSizeChanged;
        }

        public void Dispose()
        {
            ((IShader)_shader).Dispose();
            _container.RenderClientSizeChanged -= Container_RenderClientSizeChanged;
        }

        public void Initialize()
        {
            _container.RenderClientSizeChanged += Container_RenderClientSizeChanged;
            _fontTexture = _fontTextureProvider();
            _shader = new HudShader();
        }

        public void Render()
        {
            _shader.Use();
            while (_disposeObjects.TryDequeue(out var dObj))
            {
                switch (dObj)
                {
                    case TextHudObject texObj:
                        texObj.EAH?.Dispose();
                        texObj.TVP = null;
                        break;
                }
            }
            foreach (var hudObj in _hudObjects)
            {
                switch (hudObj)
                {
                    case TextHudObject texObj:
                        {
                            if (texObj.VertexUpdated)
                            {
                                texObj.EAH?.DisposeAll();
                                texObj.EAH = texObj.TVP.ToElementArray().GetHandle();
                                texObj.TVP.Clear();
                                texObj.VertexUpdated = false;
                                texObj.TextUpdated = false;
                            }


                            if (texObj.EAH == null)
                                continue;

                            _shader.Model = texObj.Model;
                            _fontTexture.Bind();
                            texObj.EAH.Bind();
                            texObj.EAH.Render();
                            break;
                        }
                    default:
                        throw new RenderException($"Unknown HudObject: {hudObj.GetType().FullName}");
                }
            }
        }

        public void Update()
        {
            foreach (var hudObj in _hudObjects)
            {
                switch (hudObj)
                {
                    case TextHudObject texObj:
                        if (_fontTexture != null)
                        {
                            if (texObj.TextUpdated)
                            {
                                if (texObj.TVP == null)
                                    texObj.TVP = new TextVertexProvider(texObj);
                                texObj.TVP.Calculate(_fontTexture);
                                texObj.VertexUpdated = true;
                            }
                            if (texObj.ModelUpdated)
                            {
                                texObj.Model = Matrix4.CreateTranslation(texObj.Position);
                                texObj.ModelUpdated = false;
                            }
                        }
                        break;
                    default:
                        throw new RenderException($"Unknown HudObject: {hudObj.GetType().FullName}");
                }
            }
        }

        public void Tick()
        {

        }

        public void Add(IHudObject hudObject)
        {
            if (hudObject is TextHudObject textHudObject)
            {
                if (textHudObject.Font == null)
                    textHudObject.Font = _defaultFont;
            }
            _hudObjects.Add(hudObject);
        }

        public void Remove(IHudObject hudObject)
        {
            if (!_hudObjects.Remove(hudObject))
                return;
            _disposeObjects.Enqueue(hudObject);
        }
    }
}
