using System;
using System.Threading;
using System.Threading.Tasks;
using Games.DefaultTexts;
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
        public BlackjackGame(IUser user, ITelegramService telegramService)
        {
            _user = user;
            TelegramService = telegramService;
            _random = new();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public ITelegramService TelegramService { get; }

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
            Bid = bid;
            TelegramService.OnMessageReceived += OnMessageReceived;
            await TelegramService.PrintAsync(ToString(), _user.ChatId, token);

            if (_user.GetBalance() - bid < 0)
            {
                await TelegramService.PrintAsync(BlackjackDefaultText.EndOfMoneyText, _user.ChatId, token);
                TelegramService.OnMessageReceived -= OnMessageReceived;
                OnGameEnded?.Invoke(this, EventArgs.Empty, token);

                return;
            }

            SetFirstCardDeal();
            if (DialerScope == 21)
            {
                await TelegramService.PrintAsync(GetInformation(), _user.ChatId, token);

                _user.AddBalance(await EndGameAsync(token));
                OnGameEnded?.Invoke(this, EventArgs.Empty, token);

                return;
            }

            await TelegramService.PrintAsync(GetInformation(), _user.ChatId, token);
            await TelegramService.PrintAsync(BlackjackDefaultText.IsNeededNewCardText, _user.ChatId, token);
        }

        /// <inheritdoc />
        public async Task LogicAsync(string input, CancellationToken token)
        {
            // 15 чтобы игра не была простой
            // (шанс выпадения туза при такой расстановке равен вероятности при игре с 52 картами)
            var userNum = _random.Next(5, 15);

            var dealerNum = _random.Next(1, 12);
            if (userNum < 11)
            {
                UserScope += userNum;
                await TelegramService.PrintAsync($"Выпало {userNum} {GetRightDeclension(userNum)}", _user.ChatId, token);
            }
            else if (userNum == 11)
            {
                // у туза значение 1 или 11 (11 пока общая сумма не больше 21, далее 1)
                if (UserScope + DialerScope > 21)
                {
                    UserScope += 1;
                    await TelegramService.PrintAsync($"Выпало 1 очко", _user.ChatId, token);
                }
                else
                {
                    UserScope += 11;
                    await TelegramService.PrintAsync("Выпало 11 очков", _user.ChatId, token);
                }
            }
            else
            {
                userNum = _random.Next(8, 10);
                UserScope += userNum;
                await TelegramService.PrintAsync($"Выпало {userNum} {GetRightDeclension(userNum)}", _user.ChatId, token);
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
            TelegramService.OnMessageReceived -= OnMessageReceived;

            if (UserScope > 21)
            {
                DialerScope = new Random().Next(16, 22);
                await TelegramService.PrintAsync($"К сожалению, у дилера {DialerScope} {GetRightDeclension(DialerScope)}.\n" +
                    "Но не стоит расстраиваться, в следующий раз обязательно повезет!", _user.ChatId, token);

                return -Bid;
            }
            else if (DialerScope > 21 && UserScope <= 21)
            {
                var prize = Math.Ceiling(Bid / 2);
                await TelegramService.PrintAsync($"Отличная игра! Твой выйгрыш {prize}", _user.ChatId, token);

                return prize;
            }
            else if (DialerScope > UserScope)
            {
                await TelegramService.PrintAsync($"К сожалению, очков у дилера {DialerScope} {GetRightDeclension(DialerScope)}.\n" +
                    "Но не стоит расстраиваться, в следующий раз обязательно повезет!", _user.ChatId, token);

                return -Bid;
            }
            else if (UserScope > DialerScope)
            {
                var prize = Math.Ceiling(Bid / 2);
                await TelegramService.PrintAsync($"Отличная игра! У дилера {DialerScope} {GetRightDeclension(DialerScope)}.\n" +
                    $"Твой выйгрыш {prize}", _user.ChatId, token);

                return prize;
            }
            else
            {
                await TelegramService.PrintAsync(BlackjackDefaultText.EqualScoreText, _user.ChatId, token);
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
        ///     Создает первую раздачу
        /// </summary>
        private void SetFirstCardDeal()
        {
            UserScope = _random.Next(1, 6) + _random.Next(1, 6) + _random.Next(1, 6) + _random.Next(1, 7);
            DialerScope = _random.Next(1, 6) + _random.Next(1, 6) + _random.Next(1, 6) + _random.Next(1, 7);
        }

        /// <summary>
        ///     Обработчик прихода сообщения от пользователя
        /// </summary>
        private async Task OnMessageReceived(object sender, string message, CancellationToken token = default)
        {
            if (InputValidator.CheckInput(message))
            {
                await LogicAsync("unused_input_text", token);
                await TelegramService.PrintAsync(GetInformation(), _user.ChatId, token);

                if (IsGameOver())
                {
                    _user.AddBalance(await EndGameAsync(token));
                    OnGameEnded?.Invoke(this, EventArgs.Empty, token);

                    return;
                }
                await TelegramService.PrintAsync(BlackjackDefaultText.IsNeededNewCardText, _user.ChatId, token);
                OnGameUpdated?.Invoke(this, EventArgs.Empty, token);
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
