// (c) Konstantin Brownstein 2016, except implementation

using BaseEntities;
using System;

namespace OptionPricingModels.PricingModels {
    internal class BinomialModel : IOptionsPricingModel {
        public void Compute(OptionPosition option) {
            int nSteps = 1000; // number of steps in the binomial model

            double t = option.TimeToExpiry.Days / 365.0;
            double dt = t / nSteps; // time interval between steps

            double S = option.UnderlyingPrice;
            double K = option.Strike;

            double sigma = option.Volatility;
            double r = option.InterestRate;

            OptionType type = option.Type;

            double[,] lattice = createLattice(nSteps, dt, S, K, sigma, r, type);

            var sign = option.Side == Side.Buy ? 1 : -1;

            double upFactor = Math.Pow(Math.E, sigma * Math.Sqrt(dt));
            double downFactor = 1 / upFactor;

            option.OptionPrice = lattice[0, 0];

            option.Delta = sign * (lattice[1, 1] - lattice[1, 0]) / (option.UnderlyingPrice * (upFactor - downFactor));

            option.Gamma = sign * (((lattice[2, 2] - lattice[2, 1]) / (option.UnderlyingPrice * (upFactor * upFactor - upFactor * downFactor))) -
                                   ((lattice[2, 1] - lattice[2, 0]) / (option.UnderlyingPrice * (upFactor * downFactor - downFactor * downFactor)))) /
                                    (0.5 * option.UnderlyingPrice * (upFactor * upFactor - downFactor * downFactor)); 

            option.Theta = sign * (lattice[2, 1] - lattice[0, 0]) / (2 * dt);

            double priceDeltaVol = createLattice(nSteps, dt, S, K, (sigma + 0.001), r, type)[0, 0]; // option price with a higher volatily for vega calculation
            option.Vega = sign * (priceDeltaVol - option.OptionPrice) / 0.001; // estimate option price sensitivity to volatility

            double priceDeltaR = createLattice(nSteps, dt, S, K, sigma, (r + 0.001), type)[0, 0]; // option price with a higher rate for rho calculation
            option.Rho = sign * (priceDeltaR - option.OptionPrice) / 0.001; // estimate option price sensitivity to percent rate
        }

        private static double[,] createLattice(int nSteps, double dt, double S, double K, double sigma, double r, OptionType type)
        {
            double upFactor = Math.Pow(Math.E, sigma * Math.Sqrt(dt));
            double downFactor = 1 / upFactor;
            double p = (Math.Pow(Math.E, r * dt) - downFactor) / (upFactor - downFactor); // probability of an up-move

            double[,] underlyingPrice = new double[nSteps + 1, nSteps + 1]; // an array containing underlying prices of the underlying at each step
            underlyingPrice[0, 0] = S; // price at the root of the tree is equal to the current price
            underlyingPrice[nSteps, 0] = underlyingPrice[0, 0] * Math.Pow(downFactor, nSteps);

            for (int i = 1; i <= nSteps; i++) // initialise asset prices at maturity
            {
                underlyingPrice[nSteps, i] = underlyingPrice[nSteps, i - 1] * upFactor / downFactor;
            }

            double[,] lattice = new double[nSteps + 1, nSteps + 1]; // an array containing option prices of the underlying at each step

            for (int i = 0; i <= nSteps; i++) // initialise option prices at maturity
            {
                if (type == OptionType.Call)
                {
                    lattice[nSteps, i] = Math.Max(underlyingPrice[nSteps, i] - K, 0);
                }
                else
                {
                    lattice[nSteps, i] = Math.Max(K - underlyingPrice[nSteps, i], 0);
                }
            }

            for (int i = nSteps - 1; i >= 0; i--) // step back through the tree
            {
                for (int j = 0; j <= i; j++)
                {
                    lattice[i, j] = Math.Pow(Math.E, -r * dt) * (p * lattice[i + 1, j + 1] + (1 - p) * lattice[i + 1, j]);
                }
            }

            return lattice;
        }
    }
}