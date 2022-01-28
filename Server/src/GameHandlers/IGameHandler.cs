using System.Threading;
using System.Threading.Tasks;
using DataBase.Repositories;

namespace Server.GameHandlers
{
    /// <summary>
    ///     Общий интерфейс обработчиков игр
    /// </summary>
    public interface IGameHandler
    {
        /// <summary>
        ///     Создает новую игру
        /// </summary>
        Task StartGameAsync(ITelegramDbContext dbContext, long userId, double bid, CancellationToken cancellationToken);
    }
}
