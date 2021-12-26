using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using NLog;

namespace WebUI.Components
{
    /// <summary>
    ///     Компонент для регулирования подписки на событие изменения адреса
    /// </summary>
    public abstract class LocationAwareComponent : EventHandlingComponent
    {
        private ILogger Logger = LogManager.GetCurrentClassLogger();

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Функция вызываемая при изменении пути
        /// </summary>
        protected abstract Task OnLocationChangedAsync();

        /// <inheritdoc />
        protected override void Subscribe()
        {
            NavigationManager.LocationChanged += LocationChangedEventHandler;
        }

        /// <inheritdoc />
        protected override void Unsubscribe()
        {
            NavigationManager.LocationChanged -= LocationChangedEventHandler;
        }

        /// <inheritdoc />
        protected override void OnDisposing()
        {
        }

        /// <summary>
        ///     Обаботчик изменения пути
        /// </summary>
        private void LocationChangedEventHandler(object sender, LocationChangedEventArgs e)
        {
            try
            {
                InvokeAsync(OnLocationChangedAsync);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Component may has already been disposed");
            }
        }
    }
}
