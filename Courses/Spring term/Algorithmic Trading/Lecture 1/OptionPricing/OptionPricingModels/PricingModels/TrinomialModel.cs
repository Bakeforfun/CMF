// (c) Konstantin Brownstein 2016, except implementation

namespace OptionPricingModels.PricingModels {
    internal class TrinomialModel : IOptionsPricingModel {
        public void Compute(Option option) {
            // TODO : Implement
            option.OptionPrice = 11;
            option.Delta = 21;
            option.Gamma = 31;
            option.Vega = 41;
            option.Theta = 51;
            option.Rho = 61;
        }
    }
}