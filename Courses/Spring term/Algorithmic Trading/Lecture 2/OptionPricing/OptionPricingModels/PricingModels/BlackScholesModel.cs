// (c) Konstantin Brownstein 2016, except implementation

using BaseEntities;

namespace OptionPricingModels.PricingModels {
    internal class BlackScholesModel : IOptionsPricingModel {
        public void Compute(OptionPosition option) {
            var t = option.TimeToExpiry.Days/365.0;
            // var d1 = 
            // var d2 = 

            var sign = option.Side == Side.Buy ? 1 : -1;
            // when calculating option greeks keep in mind that we calculate greek for position, 
            // and that's why BS formulas for greeks must be multiplied by -1 in case we're selling options
            // so formulas for greeks are [sign * BSFormula]

            if (option.Type == OptionType.Call) {
                // option.OptionPrice = 
                //option.Delta = 
                //option.Theta = 
                //option.Rho = 
            }

            //option.Gamma = 
            //option.Vega = 
        }
    }
}