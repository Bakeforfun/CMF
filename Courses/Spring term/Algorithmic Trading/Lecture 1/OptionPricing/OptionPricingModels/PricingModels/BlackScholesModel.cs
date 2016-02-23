// (c) Konstantin Brownstein 2016, except implementation

namespace OptionPricingModels.PricingModels {
    internal class BlackScholesModel : IOptionsPricingModel {
        public void Compute(Option option) {
            // TODO : IMPLEMENT
            option.OptionPrice = 2;
            option.Delta = 3;
            option.Gamma = 4;
            option.Vega = 5;
            option.Theta = 6;
            option.Rho = 7;
        }
    }
}