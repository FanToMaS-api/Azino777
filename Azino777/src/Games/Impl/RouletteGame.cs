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

        private bool _firstCheck;

        private readonly IUser _user;

        #endregion

        #region .ctor

        /// <inheritdoc cref="RouletteGame"/>
        public RouletteGame(IUser user, InOutHandlerBase inOutHandler)
        {
            _user = user;
            InOutHandler = inOutHandler;
            Name = "Австралийская Рулетка";
            Description = "Испытай удачу! Собери 6 одинаковых цифр выйграй джекпот!";
            GameRules = "Игра простая до ужаса, проиграть в нее невозможно!\n" +
                "Твоя задача выбить как можно больше повторяющихся цифр!\n" +
                "На все вопросы разрешено отвечать ДА, да, д, Yes, yes, YES и иными другими способами.\n" +
                "Все иные символы будут восприняты как отказ";

            _random = new();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public InOutHandlerBase InOutHandler { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public string GameRules { get; }

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

            while (!await GameOverAsync(token))
            {
                if (_firstCheck)
                {
                    await InOutHandler.PrintAsync("Хотите продолжить?", token);
                }

                _firstCheck = true;

                if (InputValidator.CheckInput(await InOutHandler.InputAsync(token)))
                {
                    await LogicAsync("", token);

                    await InOutHandler.PrintAsync($"Твои монеты: {_coin}", token);
                }
                else
                {
                    _user.AddBalance(await EndGameAsync(token));
                    return;
                }
            }

            _user.AddBalance(await EndGameAsync(token));
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

            Console.Write("Поздравляем! Вам начислено ");

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
            await InOutHandler.PrintAsync("Отличная игра! Возвращайся ещё!", token);
            return _coin;
        }

        /// <inheritdoc />
        public async Task<bool> GameOverAsync(CancellationToken token)
        {
            return _coin - 10 < 0;
        }

        public override string ToString()
        {
            return $"Название игры: {Name}\nОписание: {Description}\nПравила: {GameRules}";
        }

        #endregion

        #region Private methods

        #endregion
    }
}
