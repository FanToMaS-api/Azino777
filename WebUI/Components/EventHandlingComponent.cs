using System;
using Microsoft.AspNetCore.Components;

namespace WebUI.Components
{
    /// <summary>
    ///     Управляет вызовом функций подписок и отписок и освобождения данных
    /// </summary>
    public abstract class EventHandlingComponent : ComponentBase, IDisposable
    {
        private bool _hasEventSubscription;

        /// <inheritdoc />
        protected override void OnAfterRender(bool firstRender)
        {
            if (!_hasEventSubscription)
            {
                _hasEventSubscription = true;
                Subscribe();
            }

            base.OnAfterRender(firstRender);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_hasEventSubscription)
            {
                _hasEventSubscription = false;
                Unsubscribe();
            }

            OnDisposing();
        }

        /// <summary>
        ///     Подписаться на событие
        /// </summary>
        protected abstract void Subscribe();

        /// <summary>
        ///     Отписаться от события
        /// </summary>
        protected abstract void Unsubscribe();

        /// <summary>
        ///     Функция вызываемая после Dispose объекта
        /// </summary>
        protected abstract void OnDisposing();
    }
}
