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
    public class Blackjack : IGame
    {
        #region Fields

        private readonly Random _random;

        private bool _firstCheck;

        private readonly IUser _user;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Blackjack"/>
        public Blackjack(IUser user, InOutHandlerBase inOutHandler)
        {
            _user = user;
            InOutHandler = inOutHandler;
            InOutHandler.OnMessageReceived += OnMessageReceived;
            _random = new();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public InOutHandlerBase InOutHandler { get; }

        /// <summary>
        ///     Очки пользователя
        /// </summary>
        public int UserScope { get; private set; }

        /// <summary>
        ///     Очки дилера
        /// </summary>
        public int DialerScope { get; set; }

        /// <summary>
        ///     Ставка игры
        /// </summary>
        public double Bid { get; set; }

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
            await InOutHandler.PrintAsync(ToString(), token);

            if (_user.GetBalance() - Bid < 0)
            {
                await InOutHandler.PrintAsync("Недостаточно денег на счете", token);
                return;
            }

            _user.AddBalance(-bid);
            Bid = bid;

            // Первоначальная настройка
            UserScope = _random.Next(1, 14) + _random.Next(1, 14);
            DialerScope = _random.Next(1, 14) + _random.Next(1, 14);
            while (DialerScope > 21)
            {
                DialerScope = 0;
                DialerScope = _random.Next(1, 14) + _random.Next(1, 14);
            }

            if (DialerScope == 21)
            {
                DialerScope = 0;
                _user.AddBalance(await EndGameAsync(token));
                return;
            }

            await InOutHandler.PrintAsync(GetInformation(), token);
            await InOutHandler.PrintAsync("Хочешь взять еще карту?", token);

            // Вызов метода получения сообщения, нужен для консоли, в телеге просто проходит мимо
            await InOutHandler.InputAsync(token);
        }

        /// <inheritdoc />
        public async Task LogicAsync(string input, CancellationToken token)
        {
            var userNum = _random.Next(1, 14);
            var dealerNum = _random.Next(1, 14);

            if (userNum < 11)
            {
                UserScope += userNum;
                await InOutHandler.PrintAsync($"Выпало {userNum} {GetRightDeclension(userNum)}", token);
            }
            else if (userNum <= 13)
            {
                UserScope += 10;
                await InOutHandler.PrintAsync("Выпало 10 очков", token);
            }
            else
            {
                if (UserScope + DialerScope > 21)
                {
                    UserScope += 1;
                }
            }

            DialerScope += dealerNum;
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
            if (UserScope > 21 && DialerScope <= 21)
            {
                await InOutHandler.PrintAsync($"К сожалению, очков у дилера {DialerScope} {GetRightDeclension(DialerScope)}.\n" +
                    "Но не стоит расстраиваться, в следующий раз обязательно повезет!", token);

                return -Bid;
            }
            else if (DialerScope > 21 && UserScope <= 21)
            {
                await InOutHandler.PrintAsync($"Отличная игра! Твой выйгрыш {Bid * 1.5}", token);

                return Bid * 1.5;
            }
            else if (DialerScope > UserScope)
            {
                await InOutHandler.PrintAsync($"К сожалению, очков у дилера {DialerScope} {GetRightDeclension(DialerScope)}.\n" +
                    "Но не стоит расстраиваться, в следующий раз обязательно повезет!", token);

                return -Bid;
            }
            else if (UserScope > DialerScope)
            {
                await InOutHandler.PrintAsync($"Отличная игра! Очков у дилера {DialerScope} {GetRightDeclension(DialerScope)}.\n" +
                    $"Твой выйгрыш {Bid * 1.5}", token);

                return Bid * 1.5;
            }
            else
            {
                await InOutHandler.PrintAsync("Удивительно, очков у дилера столько же сколько и у тебя! Придется перераздать", token);
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
        private void OnMessageReceived(object? sender, string message)
        {
            if (InputValidator.CheckInput(message))
            {
                LogicAsync("unused_input_text", CancellationToken.None).GetAwaiter().GetResult();
                InOutHandler.PrintAsync(GetInformation(), CancellationToken.None).GetAwaiter().GetResult();

                if (IsGameOver())
                {
                    _user.AddBalance(EndGameAsync(CancellationToken.None).GetAwaiter().GetResult());
                    return;
                }
                InOutHandler.PrintAsync("Хочешь взять еще карту?", CancellationToken.None).GetAwaiter();

                // Вызов метода получения сообщения, нужен для консоли, в телеге просто проходит мимо
                InOutHandler.InputAsync(CancellationToken.None).GetAwaiter();
            }
        }

        /// <summary>
        ///     Возвращает информацию о текущем состоянии игры
        /// </summary>
        private string GetInformation()
        {
            if (!_firstCheck)
            {
                _firstCheck = true;

                return $"Текущий счет: Дилер: {DialerScope} {GetRightDeclension(DialerScope)}\n" +
                  $"У тебя {UserScope} {GetRightDeclension(UserScope)}";
            }

            return $"Твои очки: {UserScope}";
        }

        /// <summary>
        ///     Возвращает верное склонение слова
        /// </summary>
        private string GetRightDeclension(int num)
        {
            switch (num)
            {
                case 1 or 21:
                    {
                        return "очко";
                    }
                case 2 or 3 or 4:
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
