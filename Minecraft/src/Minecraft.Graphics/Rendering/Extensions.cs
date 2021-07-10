using System;

namespace Minecraft.Graphics.Rendering
{
    public static class Extensions
    {
        public static IRenderContainer AddObject(this IRenderContainer renderContainer, object obj)
        {
            if (obj is IInitializer initializer) renderContainer.AddInitializer(initializer);

            if (obj is IUpdatable updater) renderContainer.AddUpdater(updater);

            if (obj is IRenderable renderer) renderContainer.AddRenderer(renderer);

            return renderContainer;
        }

        public static IRenderContainer AddInitializer(this IRenderContainer renderContainer, IInitializer initializer)
        {
            renderContainer.Initializers.Add(initializer);
            return renderContainer;
        }

        public static IRenderContainer AddUpdater(this IRenderContainer renderContainer, IUpdatable updater)
        {
            renderContainer.Updaters.Add(updater);
            return renderContainer;
        }

        public static IRenderContainer AddRenderer(this IRenderContainer renderContainer, IRenderable renderer)
        {
            renderContainer.Renderers.Add(renderer);
            return renderContainer;
        }
        
        public static IGameTickContainer AddTicker(this IGameTickContainer gameTickContainer, ITickable ticker)
        {
            gameTickContainer.Tickers.Add(ticker);
            return gameTickContainer;
        }
        
        public static IRenderContainer AddInitializer(this IRenderContainer renderContainer, Action action)
        {
            renderContainer.AddInitializer(new CustomInitializer(action));
            return renderContainer;
        }

        public static IRenderContainer AddUpdater(this IRenderContainer renderContainer, Action action)
        {
            renderContainer.AddUpdater(new CustomUpdater(action));
            return renderContainer;
        }

        public static IRenderContainer AddRenderer(this IRenderContainer renderContainer, Action action)
        {
            renderContainer.AddRenderer(new CustomRenderer(action));
            return renderContainer;
        }

        public static IGameTickContainer AddTicker(this IGameTickContainer gameTickContainer, Action action)
        {
            gameTickContainer.AddTicker(new CustomTicker(action));
            return gameTickContainer;
        }
    }
}