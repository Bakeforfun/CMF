##### Initialisation #####
setwd("C:/Users/Internet/Google Drive/QF collaboration/CMF/Courses/Applied Financial Econometrics/1. Introduction to R/Homework 1")
library(quantmod)
Sys.setlocale("LC_ALL","English")

##### Loading data #####
tickers = c("SPY", "AAPL", "IBM") # specify the tickers to load

e <- new.env() # environment where data will be stored
options("getSymbols.warning4.0"=FALSE) # suppress warning about future change in the function behaviour
getSymbols(tickers, src = "yahoo", from="2002-01-01", env=e) # load data from Yahoo Finance

df = do.call(merge, eapply(e, Ad)[tickers]) # merge data into a single xts taking only adjusted prices
names(df) = tickers

##### Calculate returns #####
rets = apply(df, 2, function(x) diff(x)/x[-length(x)])
rets = xts(rets, order.by = index(df)[-1])

##### Plot prices and returns #####
for (i in 1:ncol(df))
{
  par(mfrow=c(2,1))
  plot(df[,i], main = paste(tickers[i], "price"))
  plot(rets[,i], main = paste(tickers[i], "returns"))
}
par(mfrow=c(1,1))

###### Calculate mean return and return s.d. #####
stats = function(r)
{
  x = c(mean(r), sd(r))
  names(x) = c("Mean", "SD")
  return(x)
}

options(digits=4)
apply(rets, 2, stats)