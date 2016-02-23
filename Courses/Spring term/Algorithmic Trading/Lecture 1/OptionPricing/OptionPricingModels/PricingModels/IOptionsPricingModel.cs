// (c) Konstantin Brownstein 2016, except implementation

namespace OptionPricingModels.PricingModels {
    public interface IOptionsPricingModel {
        void Compute(Option option);
    }
}