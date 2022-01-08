﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataBase.Repositories.Impl
{
    /// <summary>
    ///     Репозиторий пользователей сайта
    /// </summary>
    internal class UserRepository : IUserRepository
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        private readonly AppDbContext _dbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="UserRepository"/>
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<IdentityUser> CreateQuery() => _dbContext.Users.AsQueryable();

        /// <inheritdoc />
        public void Remove(IdentityUser entity)
        {
            _dbContext.Users.Remove(entity);
        }

        /// <inheritdoc />
        public async Task<IdentityUser> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                .Where(_ => _.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<IdentityUser> CreateAsync(Action<IdentityUser> action, CancellationToken cancellationToken = default)
        {
            var user = new IdentityUser();
            action(user);

            var conflictingUser = await _dbContext.Users
                .Where(_ => _.Id == user.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (conflictingUser != null)
            {
                Log.Error("User with this id is already exist");
                return user;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<IdentityUser> UpdateAsync(string id, Action<IdentityUser> action, CancellationToken cancellationToken = default)
        {
            var user = await GetAsync(id, cancellationToken);
            if (user is null)
            {
                return user;
            }

            action(user);

            var conflictingUser = await _dbContext.Users
                .Where(_ => _.Id == id && _.Id != user.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictingUser != null)
            {
                Log.Error("User with this id is already exist");
                return user;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return user;
        }

        #endregion
    }
}
