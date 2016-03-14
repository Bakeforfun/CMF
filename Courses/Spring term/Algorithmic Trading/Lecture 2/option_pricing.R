library(RQuantLib)
library(ggplot2)

p = EuropeanOption(type = "call", underlying = 100, strike = 100, dividendYield = 0,
                   riskFreeRate = 0.05, maturity = 90/365, volatility = 0.3)

ECall = vector()
ACall = vector()
EPut = vector()
APut = vector()
i = 1
vola = seq(0.01, 1.0, by = 0.01)
for (v in vola) {
  ECall[i] = EuropeanOption(type = "call", underlying = 100, strike = 100, dividendYield = 0,
                         riskFreeRate = 0.05, maturity = 90/365, volatility = v)$value
  EPut[i] = EuropeanOption(type = "put", underlying = 100, strike = 100, dividendYield = 0,
                            riskFreeRate = 0.05, maturity = 90/365, volatility = v)$value
  ACall[i] = AmericanOption(type = "call", underlying = 100, strike = 100, dividendYield = 0,
                            riskFreeRate = 0.05, maturity = 90/365, volatility = v)$value
  APut[i] = AmericanOption(type = "put", underlying = 100, strike = 100, dividendYield = 0,
                            riskFreeRate = 0.05, maturity = 90/365, volatility = v)$value
  i = i + 1
}

plot(vola, ECall, type = "l", xlab = "Volatility", ylab = "European Call option price", 
     lwd = 2, ylim = c(0,20), panel.first = grid())
lines(vola, EPut)
