// (c) Konstantin Brownstein 2016, except implementation

using BaseEntities;
using System;

namespace OptionPricingModels.PricingModels {
    internal class ExpliciteFiniteDifferenceModel : IOptionsPricingModel {
        public void Compute(OptionPosition option) {
            // initialise option parameters
            double S = option.UnderlyingPrice;
            double K = option.Strike;
            double sigma = option.Volatility;
            double r = option.InterestRate;
            double q = option.DividendRate;
            OptionType type = option.Type;
            OptionExerciseType exerciseType = option.ExerciseType;

            int nPriceSteps, nTimeSteps; // number of price and time steps in the lattice

            nPriceSteps = 300;
            double S_max = 2 * K;
            double dS = S_max / nPriceSteps;

            double t = option.TimeToExpiry.Days / 365.0;
            double dt = 0.9 / sigma / sigma / nPriceSteps / nPriceSteps; // for stability
            nTimeSteps = (int) Math.Floor(t / dt) + 1;
            dt = t / nTimeSteps; // interval between time steps

            double[,] optionPrice = createLattice(S, K, sigma, r, q, type, exerciseType, nTimeSteps, dt, nPriceSteps, dS);

            var sign = option.Side == Side.Buy ? 1 : -1;

            // find index such that S lies in [ index * dS, (index + 1) * dS ]
            int index = (int)Math.Floor(S / dS);
            double price = 0;
            // run 2-point Lagrange polynomial interpolation
            price = price + (S - dS * (index + 1)) / (dS * index - dS * (index + 1)) * optionPrice[0, index];
            price = price + (S - dS * index) / (dS * (index + 1) - dS * (index)) * optionPrice[0, index + 1];

            option.OptionPrice = price;

            option.Delta = sign * (optionPrice[0, index + 1] - optionPrice[0, index - 1]) / (2 * dS);
            option.Gamma = sign * (optionPrice[0, index + 1] - 2 * optionPrice[0, index] + optionPrice[0, index - 1]) / (dS * dS);
            option.Theta = (optionPrice[1, index] - optionPrice[0, index]) / dt;

            double delta = 0.001;

            double[,] priceDeltaVol = createLattice(S, K, (sigma + delta), r, q, type, exerciseType, nTimeSteps, dt, nPriceSteps, dS); // option price with a higher volatily for vega calculation
            price = (S - dS * (index + 1)) / (dS * index - dS * (index + 1)) * priceDeltaVol[0, index] + (S - dS * index) / (dS * (index + 1) - dS * (index)) * priceDeltaVol[0, index + 1]; // interpolate
            option.Vega = sign * (price - option.OptionPrice) / delta; // estimate option price sensitivity to volatility

            double[,] priceDeltaR = createLattice(S, K, sigma, (r + delta), q, type, exerciseType, nTimeSteps, dt, nPriceSteps, dS); // option price with a higher rate for rho calculation
            price = (S - dS * (index + 1)) / (dS * index - dS * (index + 1)) * priceDeltaR[0, index] + (S - dS * index) / (dS * (index + 1) - dS * (index)) * priceDeltaR[0, index + 1]; // interpolate
            option.Rho = sign * (price - option.OptionPrice) / delta; // estimate option price sensitivity to percent rate
        }

        // Returns option prices lattice as a two-dimensional array
        private static double[,] createLattice(double S, double K, double sigma, double r, double q, OptionType type, OptionExerciseType exerciseType, int nTimeSteps, double dt, int nPriceSteps, double dS) {
            double[] underlyingPrice = new double[nPriceSteps + 1]; // an array containing underlying prices of the underlying at each step
            double[,] lattice = new double[nTimeSteps + 1, nPriceSteps + 1]; // a matrix containing option prices of the underlying at each step

            double pu, pd, pm; // probabilities of an up-move, a down-move and that the price will stay the same

            // Build stock price grid
            for (int i = 0; i <= nPriceSteps; i++) {
                underlyingPrice[i] = i * dS;
            }

            // Initialise option prices at maturity
            for (int i = 0; i <= nPriceSteps; i++) {
                if (type == OptionType.Call) {
                    lattice[nTimeSteps, i] = Math.Max(underlyingPrice[i] - K, 0);
                }
                else {
                    lattice[nTimeSteps, i] = Math.Max(K - underlyingPrice[i], 0);
                }
            }

            // Step back through the tree
            for (int i = nTimeSteps - 1; i >= 0; i--) {
                for (int j = 1; j <= nPriceSteps - 1; j++) { // loop from S = 1 to S = (nPriceSteps - 1), because S = 0 and S = nPriceSteps will have boundary conditions
                    pu = 0.5 * (sigma * sigma * j * j + (r - q) * j )* dt;
                    pd = 0.5 * (sigma * sigma * j * j - (r - q) * j) * dt;
                    pm = 1 - sigma * sigma * j * j * dt;

                    lattice[i, 0] = 0; // boundary condition at S = 0
                    lattice[i, nPriceSteps] = underlyingPrice[nPriceSteps] - K * Math.Pow(Math.E, -r * (nTimeSteps - i) * dt); // boundary condition at S = nPriceSteps
                    
                    // Alternative boundary conditions from Paul Wilmott (do not work yet)
                    //lattice[i, 0] = (1 - r * dt) * lattice[i + 1, 0];
                    //lattice[i, nPriceSteps] = 2 * lattice[i, nPriceSteps - 1] - lattice[i, nPriceSteps - 2];

                    if (exerciseType == OptionExerciseType.European)
                        lattice[i, j] = Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 1] + pm * lattice[i + 1, j] + pd * lattice[i + 1, j - 1]);
                    else {
                        if (type == OptionType.Call) // if American call
                            lattice[i, j] = Math.Max(underlyingPrice[j] - K, Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 1] + pm * lattice[i + 1, j] + pd * lattice[i + 1, j - 1]));
                        else // if American put
                            lattice[i, j] = Math.Max(K - underlyingPrice[j], Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 1] + pm * lattice[i + 1, j] + pd * lattice[i + 1, j - 1]));
                    }
                }
            }
            return lattice;
        }
    }
}