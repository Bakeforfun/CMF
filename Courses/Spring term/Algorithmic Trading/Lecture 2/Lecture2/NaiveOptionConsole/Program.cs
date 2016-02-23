using System;

namespace NaiveOptionConsole {
    internal class Program {
        private static void Main(string[] args) {
            var option1 = new NaiveOption {
                UnderlyingPrice = 100,
                Type = OptionType.Call,
                StrikePrice = 100,
                TimeToExpiration = 90.0/365,
                Volatility = 0.3,
                InterestRate = 0.02
            };

            Console.WriteLine("=== Option1 ===");
            Console.WriteLine($"Price:\t{option1.Price.ToString("##.00")}");
            Console.WriteLine($"Delta:\t{option1.Delta.ToString("##.00")},\tCashDelta:\t{option1.CashDelta.ToString("##.00")}");
            Console.WriteLine($"Gamma:\t{option1.Gamma.ToString("##.00")},\tCashGamma:\t{option1.CashGamma.ToString("##.00")}");
            Console.WriteLine($"Theta:\t{option1.Theta.ToString("##.00")},\tCashTheta:\t{option1.CashTheta.ToString("##.00")}");
            Console.WriteLine($"Vega:\t{option1.Vega.ToString("##.00")},\tCashVega:\t{option1.CashVega.ToString("##.00")}");
            Console.WriteLine($"Rho:\t{option1.Rho.ToString("##.00")},\tCashRho:\t{option1.CashRho.ToString("##.00")}");

            Console.ReadLine();
        }
    }
}