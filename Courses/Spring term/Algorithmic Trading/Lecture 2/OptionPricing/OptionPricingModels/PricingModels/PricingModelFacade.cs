// (c) Konstantin Brownstein 2016, except implementation

using System;

namespace OptionPricingModels.PricingModels {
    internal static class PricingModelFacade {
        public static void Compute(OptionPosition option) {
            IOptionsPricingModel pricingModel;
            switch (option.Model) {
                case OptionPricingModel.Binomial:
                    pricingModel = new BinomialModel();
                    break;
                case OptionPricingModel.BlackScholes:
                    pricingModel = new BlackScholesModel();
                    break;
                case OptionPricingModel.ExpliciteFiniteDifference:
                    pricingModel = new ExpliciteFiniteDifferenceModel();
                    break;
                case OptionPricingModel.Trinomial:
                    pricingModel = new TrinomialModel();
                    break;
                default:
                    throw new InvalidOperationException($"No implementation for {option.Model}");
            }
            pricingModel.Compute(option);
        }
    }
}