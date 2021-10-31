using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Impl.MoneyService;
using Games.Impl.Services;
using Games.Interfaces.Game;
using Games.Interfaces.MoneyService;
using Games.Interfaces.User;

namespace Games.Impl.Games
{
    /// <summary>
    ///     Игра блэкджек
    /// </summary>
    public class Blackjack : IGame
    {
        #region Fields

        private int _userScope;

        private int _dialerScope;

        private double _bid;

        private readonly Random _random;

        private bool _firstCheck;

        private readonly IUser _user;

        private readonly IMoneyHandler _moneyHandler;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Blackjack"/>
        public Blackjack(IUser user, InOutHandlerBase inOutHandler)
        {
            _moneyHandler = new MoneyHandler();
            _user = user;
            InOutHandler = inOutHandler;

            // TODO: Вынести инициализацию описания игр в отдельные фейлы txt
            Name = "21 очко";
            Description = "Набери максимальное количество очков, но не больше 21, и получи гарантированный приз";
            GameRules = "Твоя задача набрать очков больше, чем у дилера, но не превысить 21 очко. Делай ставку, и давай побеждать!\n" +
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

            if (_user.GetBalance() - _bid < 0)
            {
                await InOutHandler.PrintAsync("Недостаточно денег на счете", token);
                return;
            }

            _moneyHandler.AddBalance(_user, -bid);
            _bid = bid;

            // Первоначальная настройка
            _userScope = _random.Next(1, 14) + _random.Next(1, 14);
            _dialerScope = _random.Next(1, 14) + _random.Next(1, 14);
            while (_dialerScope > 21)
            {
                _dialerScope = 0;
                _dialerScope = _random.Next(1, 14) + _random.Next(1, 14);
            }

            if (_dialerScope == 21)
            {
                _dialerScope = 0;
                _moneyHandler.AddBalance(_user, await EndGameAsync(token));
                return;
            }

            await InOutHandler.PrintAsync(GetInformation(), token);

            while (!await GameOverAsync(token))
            {
                await InOutHandler.PrintAsync("Хочешь взять еще карту?", token);

                if (InputValidator.CheckInput(await InOutHandler.InputAsync(token)))
                {
                    await LogicAsync("input_text", token);
                    await InOutHandler.PrintAsync(GetInformation(), token);
                }
                else
                {
                    _moneyHandler.AddBalance(_user, await EndGameAsync(token));
                    return;
                }
            }
        }

        /// <inheritdoc />
        public async Task LogicAsync(string input, CancellationToken token)
        {
            var userNum = _random.Next(1, 14);
            var dealerNum = _random.Next(1, 14);

            if (userNum < 11)
            {
                _userScope += userNum;
                await InOutHandler.PrintAsync($"Выпало {userNum} {GetRightDeclension(userNum)}", token);
            }
            else if (userNum <= 13)
            {
                _userScope += 10;
                await InOutHandler.PrintAsync("Выпало 10 очков", token);
            }
            else
            {
                if (_userScope + _dialerScope > 21)
                {
                    _userScope += 1;
                }
            }

            _dialerScope += dealerNum;
        }
        /// <inheritdoc />
        public async Task<bool> GameOverAsync(CancellationToken token)
        {
            return _userScope > 21;
        }

        /// <inheritdoc />
        public async Task<double> EndGameAsync(CancellationToken token)
        {
            if (_userScope > 21 && _dialerScope <= 21)
            {
                await InOutHandler.PrintAsync($"К сожалению, очков у дилера {_dialerScope} {GetRightDeclension(_dialerScope)}.\n" +
                    "Но не стоит расстраиваться, в следующий раз обязательно повезет!", token);

                return -_bid;
            }
            else if (_dialerScope > 21 && _userScope <= 21)
            {
                await InOutHandler.PrintAsync($"Отличная игра! Твой выйгрыш {_bid * 1.5}", token);

                return _bid * 1.5;
            }
            else if (_dialerScope > _userScope)
            {
                await InOutHandler.PrintAsync($"К сожалению, очков у дилера {_dialerScope} {GetRightDeclension(_dialerScope)}.\n" +
                    "Но не стоит расстраиваться, в следующий раз обязательно повезет!", token);

                return -_bid;
            }
            else if (_userScope > _dialerScope)
            {
                await InOutHandler.PrintAsync($"Отличная игра! Очков у дилера {_dialerScope} {GetRightDeclension(_dialerScope)}.\n" +
                    $"Твой выйгрыш {_bid * 1.5}", token);

                return _bid * 1.5;
            }
            else
            {
                await InOutHandler.PrintAsync("Удивительно, очков у дилера столько же сколько и у тебя! Придется перераздать", token);
                _moneyHandler.AddBalance(_user, _bid);
                await StartGameAsync(_bid, token);
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
        ///     Возвращает информацию о текущем состоянии игры
        /// </summary>
        private string GetInformation()
        {
            if (!_firstCheck)
            {
                _firstCheck = true;

                return $"Текущий счет: Дилер: {_dialerScope} {GetRightDeclension(_dialerScope)}\n" +
                  $"У тебя {_userScope} {GetRightDeclension(_userScope)}";
            }
            else
            {
                return $"Твои очки: {_userScope}";
            }
        }

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
