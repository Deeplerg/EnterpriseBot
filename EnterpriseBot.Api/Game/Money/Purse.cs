using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Money;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.MarketSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Game.Money
{
    public class Purse
    {
        protected Purse() { }

        #region model
        public long Id { get; protected set; }

        public IReadOnlyCollection<Money> Money 
        {
            get => new ReadOnlyCollection<Money>(money);
            protected set => money = value.ToList(); 
        }
        private List<Money> money = new List<Money>();
        #endregion

        #region actions
        public static GameResult<Purse> Create(PurseCreationParams pars)
        {
            var units = Game.Money.Money.Create(new MoneyCreationParams(pars.UnitsAmount, Currency.Units));
            var businessCoins = Game.Money.Money.Create(new MoneyCreationParams(pars.UnitsAmount, Currency.BusinessCoins));

            if (units.LocalizedError != null) return units.LocalizedError;
            if (businessCoins.LocalizedError != null) return businessCoins.LocalizedError;

            return new Purse
            {
                Money = new List<Money> { units, businessCoins }
            };
        }


        public GameResult<decimal> Add(decimal amount, Currency currency)
        {
            var money = Money.Single(m => m.Currency == currency);

            return money.Add(amount);
        }

        public EmptyGameResult AddRange(IEnumerable<Money> range)
        {
            foreach (var m in range)
            {
                var addingResult = Add(m.Amount, m.Currency);
                if (addingResult.LocalizedError != null)
                    return addingResult.LocalizedError;
            }

            return new EmptyGameResult();
        }

        public GameResult<decimal> Reduce(decimal amount, Currency currency)
        {
            var money = Money.Single(m => m.Currency == currency);

            return money.Reduce(amount);
        }

        public EmptyGameResult ReduceRange(IEnumerable<Money> range)
        {
            foreach (var m in range)
            {
                var reducingResult = Reduce(m.Amount, m.Currency);
                if (reducingResult.LocalizedError != null)
                    return reducingResult.LocalizedError;
            }

            return new EmptyGameResult();
        }


        public EmptyGameResult TransferFrom(Purse purse, decimal amount, Currency currency)
        {
            return Transfer(from: purse, to: this, amount, currency);
        }

        public EmptyGameResult TransferTo(Purse purse, decimal amount, Currency currency)
        {
            return Transfer(from: this, to: purse, amount, currency);
        }

        private static EmptyGameResult Transfer(Purse from, Purse to,
                                                decimal amount, Currency currency)
        {
            var receivingPurseMoney = to.Money.Single(m => m.Currency == currency);
            var givingPurseMoney = from.Money.Single(m => m.Currency == currency);

            if (givingPurseMoney.Amount - amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Not enough money to transfer",
                    RussianMessage = "Недостаточно денег для перевода"
                };
            }

            givingPurseMoney.Reduce(amount);
            receivingPurseMoney.Add(amount);

            return new EmptyGameResult();
        }

        public decimal GetMoneyAmount(Currency currency)
        {
            return Money.Single(m => m.Currency == currency).Amount;
        }

        public EmptyGameResult BuyBusinessCoins(decimal amount, MarketSettings marketSettings)
        {
            var units = Money.Single(m => m.Currency == Currency.Units);
            var bc    = Money.Single(m => m.Currency == Currency.BusinessCoins);

            var bcPrice = marketSettings.BusinessCoinPricePerUnit;
            var overallPrice = bcPrice * amount;

            if (units.Amount < overallPrice)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,

                    EnglishMessage = $"Not enough units to buy {amount}bc. " +
                                     $"You have: {units.Amount}, but needed: {overallPrice}." +
                                     $"1bc price in units: {bcPrice}",

                    RussianMessage = $"Недостаточно юнитов, чтобы купить {amount}bc. " +
                                     $"У тебя есть: {units.Amount}, необходимо: {overallPrice}." +
                                     $"Стоимость 1bc в юнитах: {bcPrice}"
                };
            }

            units.Reduce(overallPrice);
            bc.Add(amount);

            return new EmptyGameResult();
        }
        #endregion
    }
}
