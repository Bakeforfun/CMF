##### Initialisation #####
setwd("~/CMF/Courses/Applied Financial Econometrics/5. Extreme value theory/Homework 5")
library("quantmod")
library("fGarch")
library("zoo")
library("xts")
library("ghyp")
Sys.setlocale("LC_ALL","English")

##### Loading data #####
tickers = c("SPY") # specify the tickers to load

e <- new.env() # environment where data will be stored
options("getSymbols.warning4.0"=FALSE) # suppress warning about future change in the function behaviour
getSymbols(tickers, src = "yahoo", from="2013-01-01", env=e) # load data from Yahoo Finance

data = do.call(merge, eapply(e, Ad)[tickers]) # merge data into a single xts taking only adjusted prices
names(data) = tickers

##### Calculate returns #####
rets = apply(data, 2, function(x) diff(x)/x[-length(x)])
rets = xts(rets, order.by = index(data)[-1])

##### GARCH modelling #####
rets.gfit <- garchFit(formula=~aparch(1,1),data=rets,leverage=TRUE,cond.dist="ged", trace=FALSE)
rets.forecast <- predict(rets.gfit,n.ahead=10^4)

##### VaR and ES calculation
alpha <- 0.05
VaR <- rets.forecast[1,1]+rets.forecast[1,3]*qged(alpha,mean=0,sd=1, nu=rets.gfit@fit$par["shape"])
rets.forecast <- sort(rets.forecast[,1])
ES <- mean(rets.forecast[1:(alpha*10^4-1)])

##### VaR curve #####
VaR_curve <- numeric()
h <- 0.5*260

for (i in (h+1):length(rets)) 
{
  h.rets <- rets[(i-h):(i-1)]
#  rets.gfit <- garchFit(formula=~aparch(1,1), data=h.rets, leverage=TRUE, cond.dist="ged", trace=FALSE)
  rets.gfit <- garchFit(formula=~aparch(1,1),data=h.rets,
                        delta=2,include.delta=FALSE,leverage=TRUE,cond.dist="sged",
                        shape=1.5,include.shape=FALSE,trace=FALSE)
  rets.forecast <- predict(rets.gfit,n.ahead=1)
  VaR_curve[i-h] <- rets.forecast[1,1]+rets.forecast[1,3]*qged(alpha,mean=0,sd=1, nu=rets.gfit@fit$par["shape"])
}

fact <- rets[(h+1):length(rets)]
plot(as.numeric(fact),type="l", ylab = "Return")
lines(VaR_curve,col="red")

##### Kupiec test #####
K <- sum(fact<VaR_curve); alpha0 <- K/length(rets)
S <- -2*log((1-alpha)^(length(rets)-K)*alpha^K)+
  2*log((1-alpha0)^(length(rets)-K)*alpha0^K)
p.value <- 1-pchisq(S,df=1)