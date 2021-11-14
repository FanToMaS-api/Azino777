using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Services;
using Games.User;

namespace Games.Games.Impl
{
    /// <summary>
    ///     Игра блэкджек
    /// </summary>
    public class BlackjackGame : IGame
    {
        #region Fields

        private readonly Random _random;

        private readonly IUser _user;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BlackjackGame"/>
        public BlackjackGame(IUser user, IInOutHandler inOutHandler)
        {
            _user = user;
            InOutHandler = inOutHandler;
            InOutHandler.OnMessageReceived += OnMessageReceived;
            _random = new();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IInOutHandler InOutHandler { get; }

        /// <inheritdoc />
        public event Func<IGame, EventArgs, CancellationToken, Task> OnGameUpdated;

        /// <inheritdoc />
        public event Func<IGame, EventArgs, CancellationToken, Task> OnGameEnded;

        /// <summary>
        ///     Очки пользователя
        /// </summary>
        public int UserScope { get; private set; }

        /// <summary>
        ///     Очки дилера
        /// </summary>
        public int DialerScope { get; private set; }

        /// <summary>
        ///     Ставка игры
        /// </summary>
        public double Bid { get; private set; }

        /// <inheritdoc />
        public IUser User => _user;

        /// <summary>
        ///     Название игры
        /// </summary>
        public string Name => "21 очко";

        /// <summary>
        ///      Описание игры
        /// </summary>
        public static string Description =>
            "Набери максимальное количество очков, но не больше 21, и получи гарантированный приз";

        /// <summary>
        ///     Правила игры
        /// </summary>
        public static string GameRules =>
            "Твоя задача набрать очков больше, чем у дилера, но не превысить 21 очко. Делай ставку, и давай побеждать!\n" +
            "На все вопросы разрешено отвечать ДА, да, д, Yes, yes, YES и иными другими способами.\n" +
            "Все иные символы будут восприняты как отказ";

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task StartGameAsync(double bid, CancellationToken token = default)
        {
            await InOutHandler.PrintAsync(ToString(), _user.ChatId, token);

            if (_user.GetBalance() - Bid < 0)
            {
                await InOutHandler.PrintAsync("Недостаточно денег на счете", _user.ChatId, token);
                OnGameEnded?.Invoke(this, EventArgs.Empty, token);

                return;
            }

            _user.AddBalance(-bid);
            Bid = bid;

            // Первоначальная настройка
            UserScope = _random.Next(1, 10) + _random.Next(1, 11);
            DialerScope = _random.Next(1, 5) + _random.Next(1, 5) + _random.Next(1, 5) + _random.Next(1, 6);

            if (DialerScope == 21)
            {
                DialerScope = 0;
                _user.AddBalance(await EndGameAsync(token));

                OnGameEnded?.Invoke(this, EventArgs.Empty, token);
                return;
            }

            await InOutHandler.PrintAsync(GetInformation(), _user.ChatId, token);
            await InOutHandler.PrintAsync("Хочешь взять еще карту?", _user.ChatId, token);


            // Вызов метода получения сообщения, нужен для консоли, в телеге просто проходит мимо
            await InOutHandler.InputAsync(token);
        }

        /// <inheritdoc />
        public async Task LogicAsync(string input, CancellationToken token)
        {
            var userNum = _random.Next(1, 14);
            var dealerNum = _random.Next(1, 7);

            if (userNum < 11)
            {
                UserScope += userNum;
                await InOutHandler.PrintAsync($"Выпало {userNum} {GetRightDeclension(userNum)}", _user.ChatId, token);
            }
            else if (userNum <= 13)
            {
                UserScope += 10;
                await InOutHandler.PrintAsync("Выпало 10 очков", _user.ChatId, token);
            }
            else
            {
                if (UserScope + DialerScope > 21)
                {
                    UserScope += 1;
                }
            }

            DialerScope += dealerNum;
            OnGameUpdated?.Invoke(this, EventArgs.Empty, token);
        }

        /// <inheritdoc />
        public bool IsGameOver()
        {
            return UserScope > 21;
        }

        /// <inheritdoc />
        public async Task<double> EndGameAsync(CancellationToken token)
        {
            InOutHandler.OnMessageReceived -= OnMessageReceived;

            if (UserScope > 21)
            {
                DialerScope = new Random().Next(16, 22);
                await InOutHandler.PrintAsync($"К сожалению, у дилера {DialerScope} {GetRightDeclension(DialerScope)}.\n" +
                    "Но не стоит расстраиваться, в следующий раз обязательно повезет!", _user.ChatId, token);

                return -Bid;
            }
            else if (DialerScope > 21 && UserScope <= 21)
            {
                await InOutHandler.PrintAsync($"Отличная игра! Твой выйгрыш {Bid * 1.5}", _user.ChatId, token);

                return Bid * 1.5;
            }
            else if (DialerScope > UserScope)
            {
                await InOutHandler.PrintAsync($"К сожалению, очков у дилера {DialerScope} {GetRightDeclension(DialerScope)}.\n" +
                    "Но не стоит расстраиваться, в следующий раз обязательно повезет!", _user.ChatId, token);

                return -Bid;
            }
            else if (UserScope > DialerScope)
            {
                await InOutHandler.PrintAsync($"Отличная игра! У дилера {DialerScope} {GetRightDeclension(DialerScope)}.\n" +
                    $"Твой выйгрыш {Bid * 1.5}", _user.ChatId, token);

                return Bid * 1.5;
            }
            else
            {
                await InOutHandler.PrintAsync("Удивительно, очков у дилера столько же сколько и у тебя! Придется перераздать", _user.ChatId, token);
                _user.AddBalance(Bid);
                await StartGameAsync(Bid, token);
            }

            return 0;
        }

        public override string ToString()
        {
            return $"Название игры: {Name}\nОписание: {Description}\nПравила: {GameRules}";
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик прихода сообщения от пользователя
        /// </summary>
        private async Task OnMessageReceived(object? sender, string message, CancellationToken token = default)
        {
            if (InputValidator.CheckInput(message))
            {
                await LogicAsync("unused_input_text", token);
                await InOutHandler.PrintAsync(GetInformation(), _user.ChatId, token);

                if (IsGameOver())
                {
                    _user.AddBalance(await EndGameAsync(token));
                    OnGameEnded?.Invoke(this, EventArgs.Empty, token);

                    return;
                }
                await InOutHandler.PrintAsync("Хочешь взять еще карту?", _user.ChatId, token);
                OnGameUpdated?.Invoke(this, EventArgs.Empty, token);

                // Вызов метода получения сообщения, нужен для консоли, в телеге просто проходит мимо
                await InOutHandler.InputAsync(token);
            }
            else
            {
                _user.AddBalance(await EndGameAsync(token));
                OnGameEnded?.Invoke(this, EventArgs.Empty, token);
            }
        }

        /// <summary>
        ///     Возвращает информацию о текущем состоянии игры
        /// </summary>
        private string GetInformation()
        {
            return $"У тебя {UserScope} {GetRightDeclension(UserScope)}";
        }

        /// <summary>
        ///     Возвращает верное склонение слова
        /// </summary>
        private static string GetRightDeclension(int num)
        {
            switch (num)
            {
                case 1 or 21:
                    {
                        return "очко";
                    }
                case 2 or 3 or 4 or >= 22 and < 25:
                    {
                        return "очка";
                    }
                case > 3 and <= 10:
                    {
                        return "очков";
                    }
                default:
                    {
                        return "очков";
                    }
            }
        }

        #endregion
    }
}
