using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using Games.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using Server.Telegram.Bot;
using Server.Telegram.Bot.Impl;
using Server.Telegram.Service;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Server.Telegram.Impl
{
    /// <summary>
    ///     Сервис управляющий работой бота
    /// </summary>
    internal class TelegramService : ITelegramService
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        private IMessageService _messageService;

        private ITelegramBotClient _client;

        private ITelegramBot _bot;

        private QueuedUpdateReceiver _updateReceiver;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly string _token;

        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramService"/>
        public TelegramService(string token)
        {
            _token = token;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Событие получения нового сообщения
        /// </summary>
        private event Func<Message, string, CancellationToken, Task> OnMessageReceived;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _client = new TelegramBotClient(_token);
            _messageService = new MessageService(_client);

            _bot = new TelegramBot(_messageService);
            OnMessageReceived += _bot.HandleUpdateAsync;
            OnMessageReceived += _messageService.HandleUpdateAsync;

            ApplyMigrations();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };
            _updateReceiver = new QueuedUpdateReceiver(_client, receiverOptions);
        }

        /// <inheritdoc />
        public async Task StartAsync()
        {
            try
            {
                await foreach (var update in _updateReceiver.WithCancellation(_cancellationTokenSource.Token))
                {
                    await OnMessageReceived?.Invoke(update.Message, update.Message.Text, _cancellationTokenSource.Token);
                }

                Log.Info("Successful connection to bot");
            }
            catch (OperationCanceledException exception)
            {
                // TODO: Проверить попадает ли сюда поток при отмене
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                await RestartAsync();
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            OnMessageReceived -= _messageService.HandleUpdateAsync;
            OnMessageReceived -= _bot.HandleUpdateAsync;
        }

        /// <inheritdoc />
        public async Task RestartAsync()
        {
            Stop();
            Dispose();
            Initialize();
            await StartAsync();
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Применяет миграции
        /// </summary>
        private static void ApplyMigrations()
        {
            Log.Info("Applying database migrations...");

            using var database = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());

            database.Database.Migrate();

            Log.Info("Database migrations successfully applied");
        }

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        #endregion
    }
}
