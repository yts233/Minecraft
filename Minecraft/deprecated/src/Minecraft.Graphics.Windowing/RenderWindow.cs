using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Minecraft.Graphics.Rendering;
using Minecraft.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using KeyboardKeyEventArgs = OpenTK.Windowing.Common.KeyboardKeyEventArgs;
using MKeyboardKeyEventArgs = Minecraft.Input.KeyboardKeyEventArgs;
using MKeyModifiers = Minecraft.Input.KeyModifiers;
using MKeys = Minecraft.Input.Keys;

namespace Minecraft.Graphics.Windowing
{
    public class RenderWindow : IRenderContainer, IPointerContainer, IKeyboardContainer, IGameTickContainer, IDisposable
    {
        private GameWindow _gameWindow;
        private WindowPointerState _pointerState;
        private double _renderFreq = 60, _updateFreq = 120;

        public Vector2i Location => _gameWindow.Location;
        public event Action<MKeyboardKeyEventArgs> KeyDown;
        public event Action<MKeyboardKeyEventArgs> KeyUp;

        public IKeyboardState KeyboardState { get; private set; }

        public event Action<PointerButtonEventArgs> PointerDown;
        public event Action<PointerButtonEventArgs> PointerUp;
        public event Action<PointerMoveEventArgs> PointerMove;

        public bool PointerGrabbed
        {
            get => _pointerGrabbed;
            set
            {
                _pointerGrabbed = value;
                if (_gameWindow != null)
                    _gameWindow.CursorGrabbed = value;
            }
        }

        public IPointerState PointerState => _pointerState;

        public ICollection<IInitializer> Initializers { get; } = new List<IInitializer>();

        public ICollection<IRenderable> Renderers { get; } = new List<IRenderable>();

        public ICollection<IUpdatable> Updaters { get; } = new List<IUpdatable>();

        public ICollection<ITickable> Tickers { get; } = new List<ITickable>();

        public Vector2i ClientSize => _gameWindow.ClientSize;

        public double PreviousRenderTime { get; private set; }
        public double PreviousUpdateTime { get; private set; }

        private string _title = "Render Window";
        private bool _isFullscreen;
        private bool _pointerGrabbed;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (_gameWindow != null)
                    _gameWindow.Title = value;
            }
        }

        public double RenderFrequency
        {
            get => _renderFreq;
            set
            {
                _renderFreq = value;
                if (_gameWindow != null)
                    _gameWindow.RenderFrequency = value;
            }
        }

        public double UpdateFrequency
        {
            get => _updateFreq;
            set
            {
                _updateFreq = value;
                if (_gameWindow != null)
                    _gameWindow.UpdateFrequency = value;
            }
        }

        public bool IsFullScreen
        {
            get => _isFullscreen;
            set
            {
                _isFullscreen = value;
                if (_gameWindow != null)
                    _gameWindow.WindowState = value ? WindowState.Fullscreen : WindowState.Normal;
            }
        }

        private readonly Timer _gameTickTimer = new Timer(50);

        public void ReloadWindow()
        {
            _gameWindow?.Dispose();

            _gameWindow =
                new GameWindow(
                        new GameWindowSettings
                        {
                            RenderFrequency = _renderFreq, UpdateFrequency = _updateFreq, IsMultiThreaded = true
                        },
                        new NativeWindowSettings
                            {Title = _title, Size = (854, 480), NumberOfSamples = 4})
                    {VSync = VSyncMode.Off, CursorGrabbed = _pointerGrabbed};
            _gameWindow.Load += GameWindow_Load;
            _gameWindow.RenderThreadStarted += GameWindow_RenderThreadStarted;
            _gameWindow.RenderFrame += GameWindow_RenderFrame;
            _gameWindow.UpdateFrame += GameWindow_UpdateFrame;
            _gameWindow.Unload += GameWindow_Unload;
            _gameWindow.MouseDown += GameWindow_MouseDown;
            _gameWindow.MouseUp += GameWindow_MouseUp;
            _gameWindow.MouseMove += GameWindow_MouseMove;
            _gameWindow.KeyDown += GameWindow_KeyDown;
            _gameWindow.KeyUp += GameWindow_KeyUp;
            _gameTickTimer.Elapsed += GameTickTimerOnElapsed;
            _gameWindow.Run();
        }

        private void GameTickTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Logger.SetThreadName("ClientTickThread");
            foreach (var ticker in Tickers)
                ticker.Tick();
        }

        private static void GameWindow_RenderThreadStarted()
        {
            Logger.SetThreadName("RenderThread");
        }

        private void GameWindow_KeyUp(KeyboardKeyEventArgs obj)
        {
            KeyUp?.Invoke(new MKeyboardKeyEventArgs((MKeys) obj.Key, obj.ScanCode, (MKeyModifiers) obj.Modifiers,
                obj.IsRepeat));
        }

        private void GameWindow_KeyDown(KeyboardKeyEventArgs obj)
        {
            KeyDown?.Invoke(new MKeyboardKeyEventArgs((MKeys) obj.Key, obj.ScanCode, (MKeyModifiers) obj.Modifiers,
                obj.IsRepeat));
        }

        private void GameWindow_MouseMove(MouseMoveEventArgs obj)
        {
            _pointerState.PreviousPosition = _pointerState.Position;
            _pointerState.Position = obj.Position;
            _pointerState.Delta = obj.Delta;
            PointerMove?.Invoke(new PointerMoveEventArgs(obj.Position, obj.Delta));
        }

        private void GameWindow_MouseUp(MouseButtonEventArgs obj)
        {
            PointerUp?.Invoke(new PointerButtonEventArgs((PointerButton) obj.Button, (InputAction) obj.Action,
                (KeyModifiers) obj.Modifiers));
        }

        private void GameWindow_MouseDown(MouseButtonEventArgs obj)
        {
            PointerDown?.Invoke(new PointerButtonEventArgs((PointerButton) obj.Button, (InputAction) obj.Action,
                (KeyModifiers) obj.Modifiers));
        }

        private void GameWindow_Unload()
        {
            _gameWindow.Load -= GameWindow_Load;
            _gameWindow.RenderThreadStarted -= GameWindow_RenderThreadStarted;
            _gameWindow.RenderFrame -= GameWindow_RenderFrame;
            _gameWindow.UpdateFrame -= GameWindow_UpdateFrame;
            _gameWindow.Unload -= GameWindow_Unload;
            _gameWindow.MouseDown -= GameWindow_MouseDown;
            _gameWindow.MouseUp -= GameWindow_MouseUp;
            _gameWindow.MouseMove -= GameWindow_MouseMove;
            _gameWindow.KeyDown -= GameWindow_KeyDown;
            _gameWindow.KeyUp -= GameWindow_KeyUp;
            _gameTickTimer.Elapsed -= GameTickTimerOnElapsed;
            _gameTickTimer.Stop();

            _pointerState = null;
            KeyboardState = null;
            
            static void DisposeObject(object obj)
            {
                if (obj is IDisposable disposable)
                    disposable.Dispose();
            }

            foreach (var obj in Initializers.Cast<object>().Concat(Renderers).Concat(Updaters))
                DisposeObject(obj);
        }

        private void GameWindow_UpdateFrame(FrameEventArgs obj)
        {
            PreviousUpdateTime = obj.Time;
            foreach (var updatable in Updaters) updatable.Update();
            _pointerState.Delta = Vector2.Zero;
            _pointerState.PreviousPosition = _pointerState.Position;
        }

        private void GameWindow_RenderFrame(FrameEventArgs obj)
        {
            PreviousRenderTime = obj.Time;
            foreach (var renderable in Renderers) renderable.Render();

            _gameWindow.SwapBuffers();
        }

        private void GameWindow_Load()
        {
            _gameWindow.CenterWindow();
            _pointerState = new WindowPointerState(_gameWindow.MouseState);
            KeyboardState = new WindowKeyboardState(_gameWindow.KeyboardState);
            _gameWindow.WindowState = _isFullscreen ? WindowState.Fullscreen : WindowState.Normal;
            foreach (var initializer in Initializers) initializer.Initialize();

            _gameTickTimer.Start();
        }

        public void Dispose()
        {
            _gameWindow?.Dispose();
            _gameTickTimer.Dispose();
        }

        ~RenderWindow()
        {
            Dispose();
        }

        public void Close()
        {
            _gameWindow.Close();
        }
    }
}