using System;

namespace NaiveOptionConsole {
    public class NaiveOption {
        public OptionType Type;
        public double UnderlyingPrice;
        public double StrikePrice;
        public double TimeToExpiration;
        public double InterestRate;
        public double Volatility;

        private double _d1 => (Math.Log(UnderlyingPrice / StrikePrice) +
                          (InterestRate + Volatility * Volatility / 2) * TimeToExpiration) /
                         (Volatility * Math.Sqrt(TimeToExpiration));
        private double _d2 => _d1 - Volatility * Math.Sqrt(TimeToExpiration);

        public double Price
        {
            get
            {
                if(Type == OptionType.Call)
                    return UnderlyingPrice * NormalDistribution.Phi(_d1) -
                            StrikePrice * Math.Pow(Math.E, -InterestRate * TimeToExpiration) * NormalDistribution.Phi(_d2);
                else
                    return -UnderlyingPrice * NormalDistribution.Phi(-_d1) +
                            StrikePrice * Math.Pow(Math.E, -InterestRate * TimeToExpiration) * NormalDistribution.Phi(-_d2);
            }
        }

        public double Delta
        {
            get
            {
                if(Type == OptionType.Call)
                    return NormalDistribution.Phi(_d1);
                else
                    return (NormalDistribution.Phi(_d1) - 1);
            }
        }

        public double CashDelta => Delta * UnderlyingPrice;

        public double Gamma => NormalDistribution.Density(_d1) / (UnderlyingPrice * Volatility * Math.Sqrt(TimeToExpiration));

        public double CashGamma => Gamma * UnderlyingPrice * UnderlyingPrice / 100;

        public double Theta
        {
            get
            {
                if(Type == OptionType.Call)
                    return (-(UnderlyingPrice * NormalDistribution.Density(_d1) * Volatility) / (2 * Math.Sqrt(TimeToExpiration)) -
                           InterestRate * StrikePrice * Math.Pow(Math.E, -InterestRate * TimeToExpiration) *
                           NormalDistribution.Phi(_d2));
                else
                    return (-(UnderlyingPrice * NormalDistribution.Density(_d1) * Volatility) / (2 * Math.Sqrt(TimeToExpiration)) +
                           InterestRate * StrikePrice * Math.Pow(Math.E, -InterestRate * TimeToExpiration) *
                           NormalDistribution.Phi(-_d2));
            }
        }

        public double CashTheta => Theta / 365;

        public double Vega => (UnderlyingPrice * Math.Sqrt(TimeToExpiration) * NormalDistribution.Density(_d1));

        public double CashVega => Vega / 100;

        public double Rho
        {
            get
            {
                if(Type == OptionType.Call)
                    return StrikePrice * TimeToExpiration * Math.Pow(Math.E, -InterestRate * TimeToExpiration) *
                           NormalDistribution.Phi(_d2);
                else
                    return -StrikePrice * TimeToExpiration * Math.Pow(Math.E, -InterestRate * TimeToExpiration) *
                           NormalDistribution.Phi(-_d2);
            }
        }

        public double CashRho => Rho / 100;

    }
}