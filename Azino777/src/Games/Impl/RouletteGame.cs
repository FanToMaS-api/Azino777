using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Games.Services;
using Games.User;

namespace Games.Games.Impl
{
    /// <summary>
    ///     Игра рулетка
    /// </summary>
    public class RouletteGame : IGame
    {
        #region Fileds

        private readonly Random _random;

        private readonly IUser _user;

        #endregion

        #region .ctor

        /// <inheritdoc cref="RouletteGame"/>
        public RouletteGame(IUser user, IInOutHandler inOutHandler)
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
        ///     Оставшиеся в игре монеты
        /// </summary>
        public double Coin { get; private set; }

        /// <inheritdoc />
        public IUser User => _user;

        /// <summary>
        ///     Название игры
        /// </summary>
        public string Name => "Австралийская Рулетка";

        /// <summary>
        ///      Описание игры
        /// </summary>
        public static string Description =>
            "Испытай удачу! Собери 6 одинаковых цифр выйграй джекпот!";

        /// <summary>
        ///     Правила игры
        /// </summary>
        public static string GameRules =>
            "Твоя задача выбить как можно больше повторяющихся цифр!\n" +
            "На все вопросы разрешено отвечать ДА, да, д, Yes, yes, YES и иными другими способами.\n" +
            "Все иные символы будут восприняты как отказ";

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task StartGameAsync(double bid, CancellationToken token = default)
        {
            await InOutHandler.PrintAsync(ToString(), _user.ChatId, token);

            if (_user.GetBalance() - bid < 0)
            {
                await InOutHandler.PrintAsync("Недостаточно денег на счете", _user.ChatId, token);
                OnGameUpdated?.Invoke(this, EventArgs.Empty, token);
                return;
            }

            _user.AddBalance(-bid);
            Coin = bid;
            OnGameUpdated?.Invoke(this, EventArgs.Empty, token);

            await InOutHandler.PrintAsync($"Твои монеты: {Coin}", _user.ChatId, token);

            await InOutHandler.PrintAsync("Хотите продолжить?", _user.ChatId, token);

            // Вызов метода получения сообщения, нужен для консоли, в телеге просто проходит мимо
            await InOutHandler.InputAsync(token);
        }

        /// <inheritdoc />
        public async Task LogicAsync(string input, CancellationToken token)
        {
            Coin -= 10;

            var sb = new StringBuilder();
            var curNum = new List<int>();

            for (var i = 0; i < 6; i++)
            {
                var num = _random.Next(1, 10);
                sb.Append(num);
                curNum.Add(num);
            }

            await InOutHandler.PrintAsync($"{sb}", _user.ChatId, token);

            var tempList = curNum.GroupBy(x => x)
                .Select(y => y.Count())
                .ToList();
            var max = tempList.Max();

            await InOutHandler.PrintAsync("Поздравляем! Вам начислено ", _user.ChatId, token);

            double price;
            switch (max)
            {
                case 6:
                    {
                        price = 50;
                        Coin += price;
                        break;
                    }
                case 5:
                    {
                        price = 30;
                        Coin += price;
                        break;
                    }
                case 4:
                    {
                        price = 20;
                        Coin += price;
                        break;
                    }
                case 3:
                    {
                        price = 10;
                        Coin += price;
                        break;
                    }
                case 2:
                    {
                        price = 5;
                        Coin += price;
                        break;
                    }
                default:
                    {
                        price = 0;
                        break;
                    }
            }

            await InOutHandler.PrintAsync($"{price} монет!", _user.ChatId, token);
            OnGameUpdated?.Invoke(this, EventArgs.Empty, token);
        }

        /// <inheritdoc />
        public async Task<double> EndGameAsync(CancellationToken token)
        {
            InOutHandler.OnMessageReceived -= OnMessageReceived;
            await InOutHandler.PrintAsync("Отличная игра! Возвращайся ещё!", _user.ChatId, token);

            return Coin;
        }

        /// <inheritdoc />
        public bool IsGameOver()
        {
            return Coin - 10 < 0;
        }

        public override string ToString()
        {
            return $"Название игры: {Name}\nОписание: {Description}\nПравила: {GameRules}";
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик получения сообщений от пользователя
        /// </summary>
        private async Task OnMessageReceived(object? sender, string message, CancellationToken token)
        {
            if (InputValidator.CheckInput(message))
            {
                await LogicAsync("", token);
                await InOutHandler.PrintAsync($"Твои монеты: {Coin}", _user.ChatId, token);

                if (IsGameOver())
                {
                    _user.AddBalance(await EndGameAsync(token));
                    OnGameEnded?.Invoke(this, EventArgs.Empty, token);

                    return;
                }

                await InOutHandler.PrintAsync("Хотите продолжить?", _user.ChatId, token);
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

        #endregion
    }
}