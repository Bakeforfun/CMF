// (c) Konstantin Brownstein 2016, except implementation

using BaseEntities;
using OptionPricingModels.Services;
using System;

namespace OptionPricingModels.PricingModels {
    internal class BlackScholesModel : IOptionsPricingModel {
        public void Compute(OptionPosition option) {
            var t = option.TimeToExpiry.Days / 365.0;
            var d1 = (Math.Log(option.UnderlyingPrice / option.Strike) + (option.InterestRate - option.DividendRate + option.Volatility * option.Volatility / 2) * t) / (option.Volatility * Math.Sqrt(t));
            var d2 = d1 - option.Volatility * Math.Sqrt(t);

            var sign = option.Side == Side.Buy ? 1 : -1;
            // when calculating option greeks keep in mind that we calculate greek for position, 
            // and that's why BS formulas for greeks must be multiplied by -1 in case we're selling options
            // so formulas for greeks are [sign * BSFormula]

            if (option.Type == OptionType.Call) {
                option.OptionPrice = option.UnderlyingPrice * Math.Pow(Math.E, -option.DividendRate * t) * NormalDistribution.Phi(d1) -
                                     option.Strike * Math.Pow(Math.E, -option.InterestRate * t) * NormalDistribution.Phi(d2);
                option.Delta = sign * Math.Pow(Math.E, -option.DividendRate * t) * NormalDistribution.Phi(d1);
                option.Theta = sign * (-(option.UnderlyingPrice * Math.Pow(Math.E, -option.DividendRate * t) * NormalDistribution.Density(d1) * option.Volatility) / (2 * Math.Sqrt(t)) -
                               option.InterestRate * option.Strike * Math.Pow(Math.E, -option.InterestRate * t) * NormalDistribution.Phi(d2));
                option.Rho = sign * option.Strike * t * Math.Pow(Math.E, -option.InterestRate * t) * NormalDistribution.Phi(d2);
            }
            else {
                option.OptionPrice = -option.UnderlyingPrice * Math.Pow(Math.E, -option.DividendRate * t) * NormalDistribution.Phi(-d1) +
                                     option.Strike * Math.Pow(Math.E, -option.InterestRate * t) * NormalDistribution.Phi(-d2);
                option.Delta = sign * Math.Pow(Math.E, -option.DividendRate * t) * (NormalDistribution.Phi(d1) - 1);
                option.Theta = sign * (-(option.UnderlyingPrice * Math.Pow(Math.E, -option.DividendRate * t) * NormalDistribution.Density(d1) * option.Volatility) / (2 * Math.Sqrt(t)) +
                                option.InterestRate * option.Strike * Math.Pow(Math.E, -option.InterestRate * t) * NormalDistribution.Phi(-d2));
                option.Rho = sign * (-1) * option.Strike * t * Math.Pow(Math.E, -option.InterestRate * t) * NormalDistribution.Phi(-d2);
            }

            option.Gamma = sign * Math.Pow(Math.E, -option.DividendRate * t) * NormalDistribution.Density(d1) / (option.UnderlyingPrice * option.Volatility * Math.Sqrt(t));
            option.Vega = sign * option.UnderlyingPrice * Math.Sqrt(t) * Math.Pow(Math.E, -option.DividendRate * t) * NormalDistribution.Density(d1);
        }
    }
}