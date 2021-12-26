using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Games.Services;
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
    public class TelegramService : ITelegramService
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
            _client = new TelegramBotClient(_token);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Событие получения нового сообщения
        /// </summary>
        private event Func<Message, string, CancellationToken, Task> OnMessageReceived;

        /// <inheritdoc />
        public ServiceStatusType Status { get; private set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void Initialize()
        {
            _messageService = new MessageService(_client);
            _bot = new TelegramBot(_messageService);
        }

        /// <inheritdoc />
        public async Task StartAsync()
        {
            if (Status == ServiceStatusType.Running)
            {
                Log.Warn("Сancel start: the service is already running");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            OnMessageReceived += _bot.HandleUpdateAsync;
            OnMessageReceived += _messageService.HandleUpdateAsync;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };
            _updateReceiver = new QueuedUpdateReceiver(_client, receiverOptions);

            Status = ServiceStatusType.Running;

            try
            {
                Log.Info("Successful connection to bot");

                await foreach (var update in _updateReceiver.WithCancellation(_cancellationTokenSource.Token))
                {
                    await OnMessageReceived?.Invoke(update.Message, update.Message.Text, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException exception)
            {
                Log.Warn(exception, "The service was stopped by token cancellation");
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
            if (Status == ServiceStatusType.Stopped)
            {
                Log.Warn("Сancel stop: the service has already been stopped");
                return;
            }

            _cancellationTokenSource.Cancel();
            OnMessageReceived -= _messageService.HandleUpdateAsync;
            OnMessageReceived -= _bot.HandleUpdateAsync;

            Status = ServiceStatusType.Stopped;
            Log.Info("The service was stopped");
        }

        /// <inheritdoc />
        public async Task RestartAsync()
        {
            if (Status != ServiceStatusType.Running)
            {
                Log.Warn("Сancel restart: the service has not been started yet");
                return;
            }

            Status = ServiceStatusType.NotLaunch;

            Stop();
            Dispose();
            await StartAsync();
        }

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }

        #endregion
    }
}
