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
    /// <summary>
    /// 一个图形窗口
    /// </summary>
    /// <remarks>
    /// <para>请在<see cref="GlfwThread"/>内创建<see cref="RenderWindow"/>实例</para>
    /// </remarks>
    public class RenderWindow : IRenderWindowContainer
    {
        /// <summary>
        /// Glfw线程
        /// </summary>
        /// <remarks>提供一个可以创建<see cref="RenderWindow"/>实例的Glfw线程。若关闭此线程，将无法创建渲染窗体</remarks>
        public static IThreadDispatcher GlfwThread { get; } = ThreadHelper.CreateDispatcher(threadName: "GlfwThread");

        public static T InvokeOnGlfwThread<T>(Func<T> callback) where T : class
        {
            T value = null;
            GlfwThread.Invoke(() => value = callback());
            return value;
        }

        public static void InvokeOnGlfwThread(Action callback)
        {
            GlfwThread.Invoke(callback);
        }

        private GameWindow _gameWindow;
        private WindowPointerState _pointerState;
        private double _renderFreq = 60, _updateFreq = 60;

        public Vector2i Location => _gameWindow.Location;

        public IKeyboardState KeyboardState { get; private set; }

        #region Events

        public event EventHandler<MKeyboardKeyEventArgs> KeyDown;
        public event EventHandler<MKeyboardKeyEventArgs> KeyUp;
        public event EventHandler<PointerButtonEventArgs> PointerDown;
        public event EventHandler<PointerButtonEventArgs> PointerUp;
        public event EventHandler<PointerMoveEventArgs> PointerMove;
        public event EventHandler ContainerInitalized;
        public event EventHandler BeforeInitalizers;
        public event EventHandler AfterInitalizers;
        public event EventHandler BeforeRenderers;
        public event EventHandler AfterRenderers;
        public event EventHandler BeforeUpdaters;
        public event EventHandler AfterUpdaters;
        public event EventHandler ContainerStarted;
        public event EventHandler ContainerClosed;
        public event EventHandler TickTimerStarted;
        public event EventHandler BeforeTickers;
        public event EventHandler AfterTickers;
        public event EventHandler<Vector2i> ClientSizeChanged;
        public event EventHandler<Vector2i> RenderClientSizeChanged;

        #endregion

        #region EventMethods

        protected virtual void OnKeyDown(object sender, MKeyboardKeyEventArgs e)
        {
            KeyDown?.Invoke(sender, e);
        }
        protected virtual void OnKeyUp(object sender, MKeyboardKeyEventArgs e)
        {
            KeyUp?.Invoke(sender, e);
        }
        protected virtual void OnPointerDown(object sender, PointerButtonEventArgs e)
        {
            PointerDown?.Invoke(sender, e);
        }
        protected virtual void OnPointerUp(object sender, PointerButtonEventArgs e)
        {
            PointerUp?.Invoke(sender, e);
        }
        protected virtual void OnPointerMove(object sender, PointerMoveEventArgs e)
        {
            PointerMove?.Invoke(sender, e);
        }
        protected virtual void OnContainerInitalized(object sender, EventArgs e)
        {
            ContainerInitalized?.Invoke(sender, e);
        }
        protected virtual void OnBeforeInitalizers(object sender, EventArgs e)
        {
            BeforeInitalizers?.Invoke(sender, e);
        }
        protected virtual void OnAfterInitalizers(object sender, EventArgs e)
        {
            AfterInitalizers?.Invoke(sender, e);
        }
        protected virtual void OnBeforeRenderers(object sender, EventArgs e)
        {
            BeforeRenderers?.Invoke(sender, e);
        }
        protected virtual void OnAfterRenderers(object sender, EventArgs e)
        {
            AfterRenderers?.Invoke(sender, e);
        }
        protected virtual void OnBeforeUpdaters(object sender, EventArgs e)
        {
            BeforeUpdaters?.Invoke(sender, e);
        }
        protected virtual void OnAfterUpdaters(object sender, EventArgs e)
        {
            AfterUpdaters?.Invoke(sender, e);
        }
        protected virtual void OnContainerStarted(object sender, EventArgs e)
        {
            ContainerStarted?.Invoke(sender, e);
        }
        protected virtual void OnContainerClosed(object sender, EventArgs e)
        {
            ContainerClosed?.Invoke(sender, e);
        }
        protected virtual void OnTickTimerStarted(object sender, EventArgs e)
        {
            TickTimerStarted?.Invoke(sender, e);
        }
        protected virtual void OnBeforeTickers(object sender, EventArgs e)
        {
            BeforeTickers?.Invoke(sender, e);
        }
        protected virtual void OnAfterTickers(object sender, EventArgs e)
        {
            AfterTickers?.Invoke(sender, e);
        }
        protected virtual void OnClientSizeChanged(object sender, Vector2i e)
        {
            ClientSizeChanged?.Invoke(sender, e);
        }
        protected virtual void OnRenderClientSizeChanged(object sender, Vector2i e)
        {
            RenderClientSizeChanged?.Invoke(sender, e);
        }

        #endregion

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

        public Vector2i ClientSize => _gameWindow.ClientRectangle.Size;

        public double PreviousRenderTime { get; private set; }
        public double PreviousUpdateTime { get; private set; }

        private string _title = "Render Window";
        private bool _isFullscreen;
        private bool _pointerGrabbed;
        private bool _initalized = false;
        private bool _disposed;
        private Vector2i _size = new(854, 480);

        public Vector2i Size
        {
            get => _gameWindow?.Size ?? _size;
            set
            {
                _size = value;
                if (_gameWindow != null)
                    _gameWindow.Size = value;
            }
        }

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

        public bool PointerActivated { get; private set; }

        private readonly Timer _gameTickTimer = new(50);

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <remarks>将会堵塞线程，请勿在<see cref="GlfwThread"/>中执行</remarks>
        public void Run()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RenderWindow));

            //_initalized = true;
            GlfwThread.Invoke(() => _gameWindow?.Close());
            _gameTickTimer?.Stop();
            //_gameWindow?.Dispose();

            //GlfwThread.Invoke(() => _gameWindow =
            //    new GameWindow(
            //            new GameWindowSettings
            //            {
            //                RenderFrequency = _renderFreq,
            //                UpdateFrequency = _updateFreq,
            //                IsMultiThreaded = true
            //            },
            //            new NativeWindowSettings
            //            { Title = _title, Size = _size, NumberOfSamples = 4 })
            //    { VSync = VSyncMode.Off, CursorGrabbed = _pointerGrabbed });
            //
            _gameWindow =
               new GameWindow(
                       new GameWindowSettings
                       {
                           RenderFrequency = _renderFreq,
                           UpdateFrequency = _updateFreq,
                           IsMultiThreaded = true
                       },
                       new NativeWindowSettings
                       { Title = _title, Size = _size, NumberOfSamples = 0 })
               { VSync = VSyncMode.Off, CursorGrabbed = _pointerGrabbed };

            OnContainerInitalized(this, EventArgs.Empty);
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
            _gameWindow.MouseEnter += GameWindow_MouseEnter;
            _gameWindow.MouseLeave += GameWindow_MouseLeave;
            _gameTickTimer.Elapsed += GameTickTimerOnElapsed;
            var logger = Logger.GetLogger<RenderWindow>();
            logger.Info("Reload window");
            _gameWindow.Run();
        }

        private void GameWindow_MouseLeave()
        {
            PointerActivated = false;
        }

        private void GameWindow_MouseEnter()
        {
            PointerActivated = true;
        }

        private void GameTickTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Logger.SetThreadName("ClientTickThread");
            OnBeforeTickers(this, EventArgs.Empty);
            foreach (var ticker in Tickers)
                ticker.Tick();

            OnAfterTickers(this, EventArgs.Empty);
        }

        private void GameWindow_RenderThreadStarted()
        {
            Logger.SetThreadName("RenderThread");

            OnBeforeInitalizers(this, EventArgs.Empty);
            foreach (var initializer in Initializers) initializer.Initialize();
            OnAfterInitalizers(this, EventArgs.Empty);

            _initalized = true;
        }

        private void GameWindow_KeyUp(KeyboardKeyEventArgs obj)
        {
            OnKeyUp(this, new MKeyboardKeyEventArgs((MKeys)obj.Key, obj.ScanCode, (MKeyModifiers)obj.Modifiers,
                obj.IsRepeat));
        }

        private void GameWindow_KeyDown(KeyboardKeyEventArgs obj)
        {
            OnKeyDown(this, new MKeyboardKeyEventArgs((MKeys)obj.Key, obj.ScanCode, (MKeyModifiers)obj.Modifiers,
                obj.IsRepeat));
        }

        private void GameWindow_MouseMove(MouseMoveEventArgs obj)
        {
            _pointerState.PreviousPosition = _pointerState.Position;
            _pointerState.Position = obj.Position;
            _pointerState.Delta = obj.Delta;
            OnPointerMove(this, new PointerMoveEventArgs(obj.Position, obj.Delta));
        }

        private void GameWindow_MouseUp(MouseButtonEventArgs obj)
        {
            OnPointerUp(this, new PointerButtonEventArgs((PointerButton)obj.Button, (InputAction)obj.Action,
                (MKeyModifiers)obj.Modifiers));
        }

        private void GameWindow_MouseDown(MouseButtonEventArgs obj)
        {
            OnPointerDown(this, new PointerButtonEventArgs((PointerButton)obj.Button, (InputAction)obj.Action,
                (MKeyModifiers)obj.Modifiers));
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
            _gameWindow.MouseEnter -= GameWindow_MouseEnter;
            _gameWindow.MouseLeave -= GameWindow_MouseLeave;
            _gameTickTimer.Elapsed -= GameTickTimerOnElapsed;
            _gameTickTimer.Stop();

            _pointerState = null;
            KeyboardState = null;

            //foreach (var obj in Initializers.Cast<object>().Concat(Renderers).Concat(Updaters))
            //    if (obj is IDisposable disposable)
            //        disposable.Dispose();
        }

        private Vector2i _prevClientSize;
        private bool _clientSizeChanged = false;
        private void GameWindow_UpdateFrame(FrameEventArgs obj)
        {
            if (!_initalized)
                return;
            if (_prevClientSize != _gameWindow.ClientSize)
            {
                _prevClientSize = _gameWindow.ClientSize;
                OnClientSizeChanged(this, _prevClientSize);
                _clientSizeChanged = true;
            }
            PreviousUpdateTime = obj.Time;
            OnBeforeUpdaters(this, EventArgs.Empty);
            foreach (var updatable in Updaters) updatable.Update();
            OnAfterUpdaters(this, EventArgs.Empty);

            _pointerState.Delta = Vector2.Zero;
            _pointerState.PreviousPosition = _pointerState.Position;
        }

        private void GameWindow_RenderFrame(FrameEventArgs obj)
        {
            PreviousRenderTime = obj.Time;

            if (_clientSizeChanged)
            {
                OnRenderClientSizeChanged(this, _prevClientSize);
                _clientSizeChanged = false;
            }

            OnBeforeRenderers(this, EventArgs.Empty);
            foreach (var renderable in Renderers) renderable.Render();
            OnAfterRenderers(this, EventArgs.Empty);

            _gameWindow.SwapBuffers();
        }

        private void GameWindow_Load()
        {
            _gameWindow.CenterWindow();
            _pointerState = new WindowPointerState(_gameWindow.MouseState);
            KeyboardState = new WindowKeyboardState(_gameWindow.KeyboardState);
            _gameWindow.WindowState = _isFullscreen ? WindowState.Fullscreen : WindowState.Normal;

            OnContainerStarted(this, EventArgs.Empty);

            _gameTickTimer.Start();
            OnTickTimerStarted(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            GC.SuppressFinalize(this);
            _gameWindow?.Dispose();
            _gameTickTimer?.Dispose();
        }

        // TODO: fix bugs
        public void Close()
        {
            if (_gameWindow == null || _gameWindow.Exists)
                return;
            _gameWindow.Close();
            _gameTickTimer.Stop();
            OnContainerClosed(this, EventArgs.Empty);
        }
    }
}