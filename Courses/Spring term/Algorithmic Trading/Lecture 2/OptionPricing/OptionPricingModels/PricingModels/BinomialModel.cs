// (c) Konstantin Brownstein 2016, except implementation

using System;

namespace OptionPricingModels.PricingModels {
    internal class BinomialModel : IOptionsPricingModel {
        public void Compute(OptionPosition option) {
            int nSteps = 100; // number of steps in the binomial model

            double[][] price = new double[nSteps][]; // an array containing prices of the underlying at each step
            price[0][0] = option.UnderlyingPrice; // price at the root of the tree is equal to the current price

            var t = option.TimeToExpiry.Days / 365.0;
            double dt = t / nSteps; // time interval between steps

            double upFactor = Math.Pow(Math.E, option.Volatility * Math.Sqrt(dt));
            double downFactor = 1 / upFactor;

            double p = 5; // probability of an up-move

            // TODO : Implement
            //option.OptionPrice = 
            //option.Delta = 
            //option.Gamma = 
            //option.Vega = 
            //option.Theta = 
            //option.Rho = 
        }
    }
}