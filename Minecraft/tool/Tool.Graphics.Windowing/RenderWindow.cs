using Minecraft.Graphics.Rendering;
using OpenTK.Windowing.Desktop;
using System.Collections.Generic;

namespace Tool.Graphics.Windowing
{
    public class RenderWindow
    {
        private GameWindow _gameWindow;

        public ICollection<IInitializer> Initializers { get; } = new List<IInitializer>();

        public ICollection<IRenderable> RenderObjects { get; } = new List<IRenderable>();

        public ICollection<IUpdatable> Updaters { get; } = new List<IUpdatable>();


        public void ReloadWindow()
        {
            _gameWindow?.Dispose();

            _gameWindow = new GameWindow(new GameWindowSettings
            {
                RenderFrequency = 30,
                UpdateFrequency = 30,
                IsMultiThreaded = true
            }, new NativeWindowSettings
            {
                Title = "Render Window"
            });

            _gameWindow.Load += () =>
            {
                foreach (var initializer in Initializers)
                {
                    initializer.Initialize();
                }
            };
            _gameWindow.RenderFrame += e =>
            {
                foreach (var renderable in RenderObjects)
                {
                    renderable.Render();
                }

                _gameWindow.SwapBuffers();
            };
            _gameWindow.UpdateFrame += e =>
            {
                foreach (var updatable in Updaters)
                {
                    updatable.Update();
                }
            };
            _gameWindow.Run();
        }
    }
}