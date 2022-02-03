using System;

namespace Minecraft.Graphics.Rendering
{
    public static class Extensions
    {
        public static IRenderContainer AddRenderObject(this IRenderContainer renderContainer, object obj)
        {
            if (obj is IInitializer initializer) renderContainer.AddInitializer(initializer);

            if (obj is IUpdatable updater) renderContainer.AddUpdater(updater);

            if (obj is IRenderable renderer) renderContainer.AddRenderer(renderer);

            return renderContainer;
        }

        public static IRenderContainer AddCompletedRenderer(this ICompletedContainer container, ICompletedRenderer renderer)
        {
            container.AddInitializer(renderer).AddUpdater(renderer).AddRenderer(renderer);
            container.AddTicker(renderer);
            return container;
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

        public static IRenderContainer AddInitializer(this IRenderContainer renderContainer, Action callback)
        {
            renderContainer.AddInitializer(new CustomInitializer(callback));
            return renderContainer;
        }

        public static IRenderContainer AddUpdater(this IRenderContainer renderContainer, Action callback)
        {
            renderContainer.AddUpdater(new CustomUpdater(callback));
            return renderContainer;
        }

        public static IRenderContainer AddRenderer(this IRenderContainer renderContainer, Action callback)
        {
            renderContainer.AddRenderer(new CustomRenderer(callback));
            return renderContainer;
        }

        public static IGameTickContainer AddTicker(this IGameTickContainer gameTickContainer, Action callback)
        {
            gameTickContainer.AddTicker(new CustomTicker(callback));
            return gameTickContainer;
        }

        public static IGameTickContainer AddIntervalTicker(this IGameTickContainer gameTickContainer, int interval, ITickable ticker)
        {
            gameTickContainer.AddTicker(new TimerTicker(interval, ticker.Tick));
            return gameTickContainer;
        }

        public static IGameTickContainer AddIntervalTicker(this IGameTickContainer gameTickContainer, int interval, Action callback)
        {
            gameTickContainer.AddTicker(new TimerTicker(interval, callback));
            return gameTickContainer;
        }
    }
}