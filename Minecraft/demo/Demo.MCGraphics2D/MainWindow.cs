using Minecraft;
using Minecraft.Extensions;
using Minecraft.Input;
using Minecraft.Data;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Windowing;
using Minecraft.Resources;
using Minecraft.Resources.Vanilla.MC117Resource;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Linq;
using Minecraft.Graphics.Renderers.UI;
using Minecraft.Graphics.Rendering;
using Minecraft.Resources.Fonts;
using System.Collections.Generic;

namespace Demo.MCGraphics2D
{

    public class Player
    {
        public Vector2d Position { get; set; } = (10D, 64D);
        public bool FaceLeft { get; set; } = false;
        public Vector2d Velocity { get; set; } = (0D, 0D);
        public double Mass { get; set; } = 1D;
        public double GravityScale { get; set; } = 1D;
        public bool OnGround { get; set; }
        public Box2d LocalBound { get; set; } = new Box2d(-.3D, 0D, .3D, 1.7D);

        public Box2d GetBoxBound()
        {
            return new Box2d(LocalBound.Min + Position, LocalBound.Max + Position);
        }

        public void Update(IEnumerable<Box2d> aabbs)
        {
            Velocity -= (0D, GravityScale * .01D);
            var org = Velocity;
            var a = org;
            var boxes = aabbs.ToArray();
            foreach (var box in boxes)
                a.Y = box.ClipYCollide(GetBoxBound(), a.Y);
            Position += (0D, a.Y);

            foreach (var box in boxes)
                a.X = box.ClipXCollide(GetBoxBound(), a.X);
            Position += (a.X, 0D);
            OnGround = org.Y != a.Y && org.Y < .0D;

            var scale = new Vector2d(.91D, .98D);
            if (OnGround)
                scale.X = .8D;
            if (org.X != a.X)
                scale.X = 0D;
            if (org.Y != a.Y)
                scale.Y = 0D;

            Velocity *= scale;
        }
    }

    public class MainWindow : RenderWindow
    {
        private readonly VanillaResource _resource;
        private readonly Font _font;
        private readonly Chunk2D _chunk;
        private readonly ISmoothAxisInput _viewInput;
        private readonly IAxisInput _playerMoveInput;
        private readonly HudRenderer _hud;
        private readonly Player _player;
        private readonly BoxObject _boxObj;
        private ITexture2DAtlas _atlases;
        private Tex2dShader _texShader;
        private Col2dShader _colShader;
        private Vector2 _viewCenter;
        private float _scale = 2F;
        private TextureAtlasBuilder _textureBuilder;

        public MainWindow()
        {
            _resource = new VanillaResource();
            _font = new Font(_resource, "default");

            _chunk = new Chunk2D();
            _chunk.Fill(0, 0, 0, 256, 0, 0, "bedrock");
            _chunk.Fill(0, 1, 0, 256, 60, 0, "stone");
            _chunk.Fill(0, 61, 0, 256, 62, 0, "dirt");
            _chunk.Fill(0, 63, 0, 256, 63, 0, "grass_block_side");

            var rand = new Random();
            for (int i = 0; i < 800; i++)
            {
                switch (rand.Next(11))
                {
                    case 0:
                        _chunk.SetBlock(rand.Next(256), rand.Next(13) + 1, "diamond_ore");
                        break;
                    case 1:
                    case 2:
                        _chunk.SetBlock(rand.Next(256), rand.Next(30) + 3, "gold_ore");
                        break;
                    case 3:
                    case 4:
                    case 5:
                        _chunk.SetBlock(rand.Next(256), rand.Next(40) + 10, "iron_ore");
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        _chunk.SetBlock(rand.Next(256), rand.Next(45) + 15, "coal_ore");
                        break;
                }
            }

            _hud = new HudRenderer(this, () => _atlases, _font);
            _hud.Add(new TextHudObject { Text = "Hello, World!" });

            _viewInput = this.CreateKeyAxisInput(negativeXKey: Keys.A, positionXKey: Keys.D, negativeYKey: Keys.S, positionYKey: Keys.W, negativeZKey: Keys.Minus, positionZKey: Keys.Equal).CreateScaledAxisInput(4F).CreateSmoothAxisInput();
            _playerMoveInput = this.CreateKeyAxisInput(negativeXKey: Keys.Left, positionXKey: Keys.Right, negativeYKey: Keys.Down, positionYKey: Keys.Up);

            _player = new Player();
            _boxObj = new BoxObject();

            //KeyDown += (sender, e) => Console.WriteLine(e.Key);

            this.AddCompletedRenderer(_hud);
        }

        protected override void OnContainerInitalized(object sender, EventArgs e)
        {
            var assets = _resource.GetAssets().Where(asset =>
                asset.Type == AssetType.Texture
            && (asset.NamedIdentifier.Name.StartsWith("block/")
            //|| asset.NamedIdentifier.Name.StartsWith("font/")
            ) && asset.NamedIdentifier.Name.EndsWith(".png"));
            _textureBuilder = new TextureAtlasBuilder();
            var i = 0;
            foreach (var asset in assets)
            {
                using var stream = asset.OpenRead();
                Logger.GetLogger<MainWindow>().Info(asset.NamedIdentifier.FullName);
                var bImg = new Image(stream);
                _textureBuilder.Add(asset.NamedIdentifier.FullName, bImg);
                //var isSingle = bImg.FrameCount == 1;
                //var q = 0;
                //foreach (var image in bImg)
                //{
                //    _textureBuilder.Add(isSingle ? asset.NamedIdentifier.FullName : $"{asset.NamedIdentifier.FullName}{{{q}}}", image);
                //    i++;
                //    q++;
                //    if (i == 4096)
                //        break;
                //}

                if (i == 4096)
                    break;
            }

            base.OnContainerStarted(sender, e);
        }

        protected override void OnBeforeInitalizers(object sender, EventArgs e)
        {
            _atlases = _textureBuilder.Build();

            _chunk.GenerateMeshes(_atlases);
            _boxObj.GenerateMeshes();
            _texShader = new Tex2dShader();
            _colShader = new Col2dShader();

            GL.Enable(EnableCap.DepthTest);
            GL.LineWidth(2F);

            CenterPlayer();
            base.OnBeforeInitalizers(sender, e);
        }

        //Vector2 _lastMoveVelocity;

        protected override void OnBeforeUpdaters(object sender, EventArgs e)
        {
            // player
            var moveVelocity = (Vector2d)_playerMoveInput.Value.Xy;
            _playerMoveInput.Update();
            _player.Velocity += _player.OnGround ? (moveVelocity.X * .02D, _playerMoveInput.Value.Y > 0F ? .17D : 0D) : (moveVelocity.X * .014D, 0D);//(Vector2d)(moveVelocity - _lastMoveVelocity) * .1D;
            _player.Update(_chunk.GetCubes(_player.GetBoxBound().Inflated(_player.Velocity)));

            //_player.Velocity -= (0D, _player.GravityScale / 600D);
            //_player.Position += _player.Velocity;
            //_lastMoveVelocity = moveVelocity;

            // camera
            _viewInput.Update();
            _scale += _viewInput.Value.Z * .01F;
            _scale = Math.Max(Math.Min(_scale, 5F), 1F);

            if (KeyboardState.IsKeyDown(Keys.GraveAccent))
                CenterPlayer();
            else _viewCenter += _viewInput.Value.Xy / _scale * 1.5F;

            base.OnBeforeUpdaters(sender, e);
        }

        protected override void OnBeforeRenderers(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var viewMat = Matrix4.CreateScale(16F) * Matrix4.CreateTranslation(-new Vector3(_viewCenter)) * Matrix4.CreateScale(_scale);

            var eah = _chunk.GetElementArrayHandle();
            if (eah != null)
            {
                _texShader.Use();
                _texShader.Model = Matrix4.Identity;
                _texShader.View = viewMat;
                _texShader.Depth = -1F;
                eah.Bind();
                eah.Render();
            }

            //player box
            var playerBoxBound = _player.GetBoxBound();
            _colShader.Use();
            _colShader.Model = Matrix4.CreateScale(new Vector3((Vector2)playerBoxBound.Size)) * Matrix4.CreateTranslation(new Vector3((Vector2)playerBoxBound.Min));
            _colShader.View = viewMat;
            _colShader.Depth = 10F;
            eah = _boxObj.GetElementArrayHandle();
            eah.Bind();
            eah.Render(PrimitiveType.LineLoop);

            ////center
            //_colShader.Use();
            //_colShader.Model = Matrix4.CreateScale(100F);
            //_colShader.View = Matrix4.Identity;
            //_colShader.Depth = 99F;
            //eah = _boxObj.GetElementArrayHandle();
            //eah.Bind();
            //eah.Render(PrimitiveType.LineLoop);


            base.OnBeforeRenderers(sender, e);
        }

        protected override void OnRenderClientSizeChanged(object sender, Vector2i e)
        {
            GL.Viewport(0, 0, e.X, e.Y);
            var mat = Matrix4.CreateOrthographic(e.X, e.Y, -100F, 100F);
            _texShader.Use();
            _texShader.Projection = mat;
            _colShader.Use();
            _colShader.Projection = mat;
            base.OnRenderClientSizeChanged(sender, e);
        }

        private void CenterPlayer()
        {
            _viewCenter = (Vector2)_player.Position * 16;
        }
    }
}
