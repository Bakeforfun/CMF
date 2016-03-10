// (c) Konstantin Brownstein 2016, except implementation

using BaseEntities;
using System;

namespace OptionPricingModels.PricingModels {
    internal class BinomialModel : IOptionsPricingModel {
        public void Compute(OptionPosition option) {

            // initialise option parameters
            double S = option.UnderlyingPrice;
            double K = option.Strike;

            double sigma = option.Volatility;
            double r = option.InterestRate;

            OptionType type = option.Type;
            OptionExerciseType exerciseType = option.ExerciseType;

            int nSteps = 1000; // number of steps in the binomial model

            double t = option.TimeToExpiry.Days / 365.0;
            double dt = t / nSteps; // time interval between steps

            

            double[,] lattice = createLattice(S, K, sigma, r, type, exerciseType, nSteps, dt);

            var sign = option.Side == Side.Buy ? 1 : -1;

            double upFactor = Math.Pow(Math.E, sigma * Math.Sqrt(dt));
            double downFactor = 1 / upFactor;

            option.OptionPrice = lattice[0, 0];

            option.Delta = sign * (lattice[1, 1] - lattice[1, 0]) / (option.UnderlyingPrice * (upFactor - downFactor));

            option.Gamma = sign * (((lattice[2, 2] - lattice[2, 1]) / (option.UnderlyingPrice * (upFactor * upFactor - upFactor * downFactor))) -
                                   ((lattice[2, 1] - lattice[2, 0]) / (option.UnderlyingPrice * (upFactor * downFactor - downFactor * downFactor)))) /
                                    (0.5 * option.UnderlyingPrice * (upFactor * upFactor - downFactor * downFactor)); 

            option.Theta = sign * (lattice[2, 1] - lattice[0, 0]) / (2 * dt);


            double delta = 0.001;
            double priceDeltaVol = createLattice(S, K, (sigma + delta), r, type, exerciseType, nSteps, dt)[0, 0]; // option price with a higher volatily for vega calculation
            option.Vega = sign * (priceDeltaVol - option.OptionPrice) / delta; // estimate option price sensitivity to volatility

            double priceDeltaR = createLattice(S, K, sigma, (r + delta), type, exerciseType, nSteps, dt)[0, 0]; // option price with a higher rate for rho calculation
            option.Rho = sign * (priceDeltaR - option.OptionPrice) / delta; // estimate option price sensitivity to percent rate
        }

        // Returns option prices lattice as a two-dimensional array
        private static double[,] createLattice(double S, double K, double sigma, double r, OptionType type, OptionExerciseType exerciseType, int nSteps, double dt) {
            double upFactor = Math.Pow(Math.E, sigma * Math.Sqrt(dt));
            double downFactor = 1 / upFactor;
            double p = (Math.Pow(Math.E, r * dt) - downFactor) / (upFactor - downFactor); // probability of an up-move

            double[,] underlyingPrice = new double[nSteps + 1, nSteps + 1]; // a matrix containing underlying prices of the underlying at each step
            double[,] lattice = new double[nSteps + 1, nSteps + 1]; // a matrix containing option prices of the underlying at each step

            // Build binomial tree
            for (int i = 0; i <= nSteps; i++) {
                for (int j = 0; j <= i; j++) {
                    underlyingPrice[i, j] = S * Math.Pow(upFactor, j) * Math.Pow(downFactor, i - j);
                }
            }

            // Initialise option prices at maturity
            for (int i = 0; i <= nSteps; i++) {
                if (type == OptionType.Call) {
                    lattice[nSteps, i] = Math.Max(underlyingPrice[nSteps, i] - K, 0);
                }
                else {
                    lattice[nSteps, i] = Math.Max(K - underlyingPrice[nSteps, i], 0);
                }
            }

            // Step back through the tree
            for (int i = nSteps - 1; i >= 0; i--) {
                for (int j = 0; j <= i; j++) {
                    if (exerciseType == OptionExerciseType.European) { // if European
                        lattice[i, j] = Math.Pow(Math.E, -r * dt) * (p * lattice[i + 1, j + 1] + (1 - p) * lattice[i + 1, j]);
                    }
                    else { // if American
                        if (type == OptionType.Call) // if a call
                            lattice[i, j] = Math.Max(underlyingPrice[i, j] - K, Math.Pow(Math.E, -r * dt) * (p * lattice[i + 1, j + 1] + (1 - p) * lattice[i + 1, j]));
                        else // if a put
                            lattice[i, j] = Math.Max(K - underlyingPrice[i, j], Math.Pow(Math.E, -r * dt) * (p * lattice[i + 1, j + 1] + (1 - p) * lattice[i + 1, j]));
                    }
                }
            }
            return lattice;
        }
    }
}