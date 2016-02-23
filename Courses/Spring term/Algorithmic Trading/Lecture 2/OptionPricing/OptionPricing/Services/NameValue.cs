// (c) Konstantin Brownstein 2016

namespace OptionPricing.Services {
    public class NameValue<T> {
        public NameValue(string name, T value) {
            Name = name;
            Value = value;
        }

        public string Name { get; protected set; }
        public T Value { get; protected set; }
    }
}