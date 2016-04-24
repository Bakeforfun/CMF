// (c) Konstantin Brownstein 2016

using OptionPricing.Views;

namespace OptionPricing {
    public class Bootstrapper {
        public void Start() {
            var shell = new PricingShell();
            shell.Show();
        }
    }
}