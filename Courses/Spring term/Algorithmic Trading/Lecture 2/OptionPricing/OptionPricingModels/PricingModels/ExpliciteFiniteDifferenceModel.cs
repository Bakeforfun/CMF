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
            OptionType type = option.Type;
            OptionExerciseType exerciseType = option.ExerciseType;

            int nTimeSteps = 1000; // number of time steps in the lattice
            int nPriceSteps = 200; // number of price steps in the lattice

            double t = option.TimeToExpiry.Days / 365.0;
            double dt = t / nTimeSteps; // interval between time steps

            double S_max = 2 * K;
            double dS = S_max / nPriceSteps;
                    
            double optionPrice = createLattice(S, K, sigma, r, type, exerciseType, nTimeSteps, dt, nPriceSteps, dS);

            var sign = option.Side == Side.Buy ? 1 : -1;



            option.OptionPrice = optionPrice;
            // option.Delta = 
            // option.Gamma = 
            // option.Vega = 
            // option.Theta = 
            // option.Rho = 
        }

        // Returns option prices lattice as a two-dimensional array
        private static double createLattice(double S, double K, double sigma, double r, OptionType type, OptionExerciseType exerciseType, int nTimeSteps, double dt, int nPriceSteps, double dS) {
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
                    pu = 0.5 * (sigma * sigma * j * j + r * j )* dt;
                    pd = 0.5 * (sigma * sigma * j * j - r * j) * dt;
                    pm = 1 - sigma * sigma * j * j * dt;

                    if (exerciseType == OptionExerciseType.European) {
                        if (type == OptionType.Call) { // if European call
                            //lattice[i, 0] = 0; // boundary condition at S = 0
                            //lattice[i, nPriceSteps] = underlyingPrice[nPriceSteps] - K * Math.Pow(Math.E, -r * (nTimeSteps - i) * dt); // boundary condition at S = nPriceSteps
                            lattice[i, 0] = (1 - r * dt) * lattice[i + 1, 0];
                            lattice[i, nPriceSteps] = 2 * lattice[i, nPriceSteps - 1] - lattice[i, nPriceSteps - 2];
                            lattice[i, j] = Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 1] + pm * lattice[i + 1, j] + pd * lattice[i + 1, j - 1]);
                        }
                        else { // if European put
                            //lattice[i, 0] = K * Math.Pow(Math.E, -r * (nTimeSteps - i) * dt); // boundary condition at S = 0
                            //lattice[i, nPriceSteps] = 0; // boundary condition at S = nPriceSteps
                            lattice[i, 0] = (1 - r * dt) * lattice[i + 1, 0];
                            lattice[i, nPriceSteps] = 2 * lattice[i, nPriceSteps - 1] - lattice[i, nPriceSteps - 2];
                            lattice[i, j] = Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 1] + pm * lattice[i + 1, j] + pd * lattice[i + 1, j - 1]);
                        }
                    }
                    else {
                        if (type == OptionType.Call) { // if American call
                            //lattice[i, 0] = 0; // boundary condition at S = 0
                            //lattice[i, nPriceSteps] = underlyingPrice[nPriceSteps] - K * Math.Pow(Math.E, -r * (nTimeSteps - i) * dt); // boundary condition at S = nPriceSteps
                            lattice[i, 0] = (1 - r * dt) * lattice[i + 1, 0];
                            lattice[i, nPriceSteps] = 2 * lattice[i, nPriceSteps - 1] - lattice[i, nPriceSteps - 2];
                            lattice[i, j] = Math.Max(underlyingPrice[j] - K, Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 1] + pm * lattice[i + 1, j] + pd * lattice[i + 1, j - 1]));
                        }
                        else { // if American put
                            //lattice[i, 0] = K * Math.Pow(Math.E, -r * (nTimeSteps - i) * dt); // boundary condition at S = 0
                            //lattice[i, nPriceSteps] = 0; // boundary condition at S = nPriceSteps
                            lattice[i, 0] = (1 - r * dt) * lattice[i + 1, 0];
                            lattice[i, nPriceSteps] = 2 * lattice[i, nPriceSteps - 1] - lattice[i, nPriceSteps - 2];
                            lattice[i, j] = Math.Max(K - underlyingPrice[j], Math.Pow(Math.E, -r * dt) * (pu * lattice[i + 1, j + 1] + pm * lattice[i + 1, j] + pd * lattice[i + 1, j - 1]));
                        }
                    }
                }
            }

            // get j* such that S_0 \in [ j*dS , (j*+1)dS ]
            int jstar;
            jstar = (int) Math.Floor(S / dS);
            double sum = 0;
            // run 2 point Lagrange polynomial interpolation
            sum = sum + (S - underlyingPrice[jstar + 1]) / (underlyingPrice[jstar] - underlyingPrice[jstar + 1]) * lattice[0, jstar];
            sum = sum + (S - underlyingPrice[jstar]) / (underlyingPrice[jstar + 1] - underlyingPrice[jstar]) * lattice[0, jstar + 1];
            return sum;
            // return lattice;
        }
    }
}