using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Games.Impl.MoneyService;
using Games.Interfaces.Game;
using Games.Interfaces.MoneyService;
using Games.Interfaces.User;

namespace Games.Impl.Games
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

        private readonly IMoneyHandler _moneyHandler;

        #endregion

        #region .ctor

        /// <inheritdoc cref="RouletteGame"/>
        public RouletteGame(IUser user)
        {
            _moneyHandler = new MoneyHandler();
            _user = user;
            Name = "Австралийская Рулетка";
            Description = "Испытай удачу! Собери 6 одинаковых цифр выйграй джекпот!";
            GameRules = "Игра простая до ужаса, проиграть в нее невозможно!\n" +
                "Твоя задача выбить как можно больше повторяющихся цифр!";

            _random = new();
        }

        #endregion

        #region Properties

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
            Console.WriteLine(ToString());

            if (_user.GetBalance() - bid < 0)
            {
                Console.WriteLine("Недостаточно денег на счете");
                return;
            }

            _moneyHandler.AddBalance(_user, -bid);
            _coin = bid;

            Console.WriteLine($"Твои монеты: {_coin}");

            while (!await GameOverAsync(token))
            {
                if (_firstCheck)
                {
                    Console.WriteLine("Хотите закончить и забрать выйгрыш? (ENTER)");
                }

                _firstCheck = true;

                if (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    await LogicAsync("", token);

                    Console.WriteLine($"Твои монеты: {_coin}");
                }
                else
                {
                    _moneyHandler.AddBalance(_user, await EndGameAsync(token));
                    return;
                }
            }

            _moneyHandler.AddBalance(_user, await EndGameAsync(token));
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
            Console.WriteLine($"{sb}");

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

            Console.WriteLine($"{price} монет!");
        }

        /// <inheritdoc />
        public async Task<double> EndGameAsync(CancellationToken token)
        {
            Console.WriteLine("Отличная игра! Возвращайся ещё!");
            return _coin;
        }

        /// <inheritdoc />
        public async Task<bool> GameOverAsync(CancellationToken token)
        {
            return _coin - 10 < 0;
        }

        public override string ToString()
        {
            return $"Название игры: {Name}\n Описание: {Description}\n Правила: {GameRules}";
        }

        #endregion

        #region Private methods

        #endregion
    }
}
