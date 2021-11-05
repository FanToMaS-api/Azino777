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
    internal class RouletteGame : IGame
    {
        #region Fileds

        private double _coin;

        private readonly Random _random;

        private readonly IUser _user;

        #endregion

        #region .ctor

        /// <inheritdoc cref="RouletteGame"/>
        public RouletteGame(IUser user, InOutHandlerBase inOutHandler)
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
            await InOutHandler.PrintAsync(ToString(), token);

            if (_user.GetBalance() - bid < 0)
            {
                await InOutHandler.PrintAsync("Недостаточно денег на счете", token);
                return;
            }

            _user.AddBalance(-bid);
            _coin = bid;

            await InOutHandler.PrintAsync($"Твои монеты: {_coin}", token);

            await InOutHandler.PrintAsync("Хотите продолжить?", token);

            // Вызов метода получения сообщения, нужен для консоли, в телеге просто проходит мимо
            InOutHandler.InputAsync(CancellationToken.None).GetAwaiter();
        }

        /// <inheritdoc />
        public async Task LogicAsync(string input, CancellationToken token)
        {
            _coin -= 10;

            var sb = new StringBuilder();
            var curNum = new List<int>();

            for (var i = 0; i < 6; i++)
            {
                var num = _random.Next(1, 10);
                sb.Append(num);
                curNum.Add(num);
            }

            await InOutHandler.PrintAsync($"{sb}", token);

            var tempList = curNum.GroupBy(x => x)
                .Select(y => y.Count())
                .ToList();
            var max = tempList.Max();

            await InOutHandler.PrintAsync("Поздравляем! Вам начислено ", token);

            double price;
            switch (max)
            {
                case 6:
                    {
                        price = 50;
                        _coin += price;
                        break;
                    }
                case 5:
                    {
                        price = 30;
                        _coin += price;
                        break;
                    }
                case 4:
                    {
                        price = 20;
                        _coin += price;
                        break;
                    }
                case 3:
                    {
                        price = 10;
                        _coin += price;
                        break;
                    }
                case 2:
                    {
                        price = 5;
                        _coin += price;
                        break;
                    }
                default:
                    {
                        price = 0;
                        break;
                    }
            }

            await InOutHandler.PrintAsync($"{price} монет!", token);
        }

        /// <inheritdoc />
        public async Task<double> EndGameAsync(CancellationToken token)
        {
            InOutHandler.OnMessageReceived -= OnMessageReceived;
            await InOutHandler.PrintAsync("Отличная игра! Возвращайся ещё!", token);

            return _coin;
        }

        /// <inheritdoc />
        public bool IsGameOver()
        {
            return _coin - 10 < 0;
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
        private void OnMessageReceived(object? sender, string message)
        {
            if (InputValidator.CheckInput(message))
            {
                LogicAsync("", CancellationToken.None).GetAwaiter();
                InOutHandler.PrintAsync($"Твои монеты: {_coin}", CancellationToken.None).GetAwaiter();

                if (IsGameOver())
                {
                    _user.AddBalance(EndGameAsync(CancellationToken.None).GetAwaiter().GetResult());
                    return;
                }

                InOutHandler.PrintAsync("Хотите продолжить?", CancellationToken.None).GetAwaiter();

                // Вызов метода получения сообщения, нужен для консоли, в телеге просто проходит мимо
                InOutHandler.InputAsync(CancellationToken.None).GetAwaiter();
            }
            else
            {
                _user.AddBalance(EndGameAsync(CancellationToken.None).GetAwaiter().GetResult());
            }
        }

        #endregion
    }
}