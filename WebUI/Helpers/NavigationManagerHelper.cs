using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace WebUI.Helpers
{
    /// <summary>
    ///     Вспомогательный класс менеджера навигации
    /// </summary>
    internal static class NavigationManagerHelper
    {
        #region

        /// <summary>
        ///     Возвращает параметр из запроса
        /// </summary>
        public static bool TryGetQueryParametr<T>(this NavigationManager manager, string paramName, out T result, T defaultParam = default(T))
        {
            var absoluteUri = manager.ToAbsoluteUri(manager.Uri);
            result = defaultParam;
            if (QueryHelpers.ParseQuery(absoluteUri.Query).TryGetValue(paramName, out var stringValue))
            {
                if (typeof(T) == typeof(string) && !string.IsNullOrEmpty(stringValue))
                {
                    result = (T)(object)stringValue.ToString();
                    return true;
                }

                if (typeof(T) == typeof(int) && !string.IsNullOrEmpty(stringValue))
                {
                    if (int.TryParse(stringValue, out var res))
                    {
                        result = (T)(object)res;
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}
