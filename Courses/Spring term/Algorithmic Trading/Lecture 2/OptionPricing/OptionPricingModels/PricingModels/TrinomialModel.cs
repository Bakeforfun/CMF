// (c) Konstantin Brownstein 2016, except implementation

using BaseEntities;
using System;

namespace OptionPricingModels.PricingModels {
    internal class TrinomialModel : IOptionsPricingModel {
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
            
            double[,] optionPrice = createLattice(S, K, sigma, r, type, exerciseType, nSteps, dt);

            var sign = option.Side == Side.Buy ? 1 : -1;

            double upFactor = Math.Pow(Math.E, sigma * Math.Sqrt(2 * dt));
            double downFactor = 1 / upFactor;

            option.OptionPrice = optionPrice[0, 0];
                        
            option.Delta = sign * (optionPrice[1, 2] - optionPrice[1, 0]) / (S * (upFactor - downFactor));

            option.Gamma = sign * ((optionPrice[1, 2] - optionPrice[1, 1]) / (S * (upFactor - 1)) -
                                   (optionPrice[1 ,1] - optionPrice[1, 0]) / (S * (1 - downFactor))) /
                                   (0.5 * S * (upFactor - downFactor));

            option.Theta = sign * (optionPrice[1, 1] - optionPrice[0, 0]) / dt;
            
            double delta = 0.001;
            double priceDeltaVol = createLattice(S, K, (sigma + delta), r, type, exerciseType, nSteps, dt)[0, 0]; // option price with a higher volatily for vega calculation
            option.Vega = sign * (priceDeltaVol - option.OptionPrice) / delta; // estimate option price sensitivity to volatility

            double priceDeltaR = createLattice(S, K, sigma, (r + delta), type, exerciseType, nSteps, dt)[0, 0]; // option price with a higher rate for rho calculation
            option.Rho = sign * (priceDeltaR - option.OptionPrice) / delta; // estimate option price sensitivity to percent rate

        }

        // Returns option prices lattice as a two-dimensional array
        private static double[,] createLattice(double S, double K, double sigma, double r, OptionType type, OptionExerciseType exerciseType, int nSteps, double dt) {
            double upFactor = Math.Pow(Math.E, sigma * Math.Sqrt(2 * dt));
            double downFactor = 1 / upFactor;

            double temp_1 = 0.5 * r * dt;
            double temp_2 = sigma * Math.Sqrt(0.5 * dt);
            double pu = Math.Pow((Math.Pow(Math.E, temp_1) - Math.Pow(Math.E, -temp_2)) / (Math.Pow(Math.E, temp_2) - Math.Pow(Math.E, -temp_2)), 2); // probability of an up-move
            double pd = Math.Pow((Math.Pow(Math.E, temp_2) - Math.Pow(Math.E, temp_1)) / (Math.Pow(Math.E, temp_2) - Math.Pow(Math.E, -temp_2)), 2); // probability of a down-move
            double pm = 1.0 - pu - pd; // probability that the price will stay the same

            double[,] underlyingPrice = new double[nSteps + 1, 2 * nSteps + 1]; // a matrix containing underlying prices of the underlying at each step
            double[,] lattice = new double[nSteps + 1, 2 * nSteps + 1]; // a matrix containing option prices of the underlying at each step

            // Build trinomial tree
            for (int i = 0; i <= nSteps; i++) {
                for (int j = 0; j <= 2 * i; j++) {
                    underlyingPrice[i, j] = S * Math.Pow(upFactor, j - i);
                }
            }

            // Initialise option prices at maturity
            for (int i = 0; i <= 2 * nSteps; i++) {
                if (type == OptionType.Call) {
                    lattice[nSteps, i] = Math.Max(underlyingPrice[nSteps, i] - K, 0);
                }
                else {
                    lattice[nSteps, i] = Math.Max(K - underlyingPrice[nSteps, i], 0);
                }
            }

            // Step back through the tree
            for (int i = nSteps - 1; i >= 0; i--) {
                for (int j = 0; j <= 2 * i; j++) {
                    if (exerciseType == OptionExerciseType.European) { // if European
                        lattice[i, j] = Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 2] + pm * lattice[i + 1, j + 1] + pd * lattice[i + 1, j]);
                    }
                    else
                    { // if American
                        if (type == OptionType.Call) // if a call
                            lattice[i, j] = Math.Max(underlyingPrice[i, j] - K, Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 2] + pm * lattice[i + 1, j + 1] + pd * lattice[i + 1, j]));
                        else // if a put
                            lattice[i, j] = Math.Max(K - underlyingPrice[i, j], Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 2] + pm * lattice[i + 1, j + 1] + pd * lattice[i + 1, j]));
                    }
                }
            }
            return lattice;
        }
    }
}