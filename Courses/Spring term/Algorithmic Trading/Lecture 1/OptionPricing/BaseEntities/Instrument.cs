// (c) Konstantin Brownstein 2016

using System;

namespace BaseEntities {
    [Serializable]
    public class Instrument {
        public Instrument() { }

        public Instrument(string name) {
            Name = name;
        }

        public string Name { get; set; }

        public override string ToString() {
            return Name;
        }
    }
}