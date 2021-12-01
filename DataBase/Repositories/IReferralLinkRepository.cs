using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;

namespace DataBase.Repositories
{
    /// <summary>
    ///     Репозиторий реферальных ссылок
    /// </summary>
    public interface IReferralLinkRepository
    {
        /// <summary>
        ///     Создать запрос к таблице реферальных ссылок
        /// </summary>
        IQueryable<ReferralLinkEntity> CreateQuery();

        /// <summary>
        ///     Получить реферальную ссылку
        /// </summary>
        Task<ReferralLinkEntity> GetAsync(string link, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать запись в таблице реферальных ссылок
        /// </summary>
        Task<ReferralLinkEntity> CreateAsync(Action<ReferralLinkEntity> action, CancellationToken cancellationToken = default);
    }
}
