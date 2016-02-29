// (c) Konstantin Brownstein 2016, except implementation

using System;

namespace OptionPricingModels.PricingModels {
    internal class BinomialModel : IOptionsPricingModel {
        public void Compute(OptionPosition option) {
            int nSteps = 10; // number of steps in the binomial model

            var t = option.TimeToExpiry.Days / 365.0;
            double dt = t / nSteps; // time interval between steps

            double upFactor = Math.Pow(Math.E, option.Volatility * Math.Sqrt(dt));
            double downFactor = 1 / upFactor;
            double p = (Math.Pow(Math.E, option.InterestRate * dt) - downFactor) / (upFactor - downFactor); // probability of an up-move

            double[, ] underlyingPrice = new double[nSteps + 1, nSteps + 1]; // an array containing underlying prices of the underlying at each step
            underlyingPrice[0, 0] = option.UnderlyingPrice; // price at the root of the tree is equal to the current price
            underlyingPrice[nSteps, 0] = underlyingPrice[0, 0] * Math.Pow(downFactor, nSteps);

            for (int i = 1; i <= nSteps; i++) // initialise asset prices at maturity
            {
                underlyingPrice[nSteps, i] = underlyingPrice[nSteps, i - 1] * upFactor / downFactor;
            }

            double[, ] optionPrice = new double[nSteps + 1, nSteps + 1]; // an array containing option prices of the underlying at each step

            for (int i = 0; i <= nSteps; i++) // initialise option prices at maturity
            {
                if (option.Type == OptionType.Call)
                {
                    optionPrice[nSteps, i] = Math.Max(underlyingPrice[nSteps, i] - option.Strike, 0);
                }
                else
                {
                    optionPrice[nSteps, i] = Math.Max(option.Strike - underlyingPrice[nSteps, i], 0);
                }
            }

            for (int i = nSteps - 1; i >= 0; i--) // step back through the tree
            {
                for (int j = 0; j <= i; j++)
                {
                    optionPrice[i, j] = Math.Pow(Math.E, -option.InterestRate * dt) * (p * optionPrice[i + 1, j + 1] + (1 - p) * optionPrice[i + 1, j]);
                }
            }

            option.OptionPrice = optionPrice[0, 0];
            //option.Delta = 
            //option.Gamma = 
            //option.Vega = 
            //option.Theta = 
            //option.Rho = 
        }
    }
}