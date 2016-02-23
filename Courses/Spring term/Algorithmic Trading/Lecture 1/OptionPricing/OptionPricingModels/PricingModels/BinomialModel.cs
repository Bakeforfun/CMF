// (c) Konstantin Brownstein 2016, except implementation

namespace OptionPricingModels.PricingModels {
    internal class BinomialModel : IOptionsPricingModel {
        public void Compute(Option option) {
            // TODO : Implement
            option.OptionPrice = 1;
            option.Delta = 2;
            option.Gamma = 3;
            option.Vega = 4;
            option.Theta = 5;
            option.Rho = 6;
        }
    }
}