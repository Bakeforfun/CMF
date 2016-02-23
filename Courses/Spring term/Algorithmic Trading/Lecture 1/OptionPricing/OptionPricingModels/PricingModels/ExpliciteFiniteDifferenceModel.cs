// (c) Konstantin Brownstein 2016, except implementation

namespace OptionPricingModels.PricingModels {
    internal class ExpliciteFiniteDifferenceModel : IOptionsPricingModel {
        public void Compute(Option option) {
            // TODO : IMPLEMENT
            option.OptionPrice = 3;
            option.Delta = 4;
            option.Gamma = 5;
            option.Vega = 6;
            option.Theta = 7;
            option.Rho = 8;
        }
    }
}