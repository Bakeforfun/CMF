##### Initialisation #####
setwd("C:/Users/Internet/Google Drive/QF collaboration/CMF/Courses/Applied Financial Econometrics/3. Portfolio modeling, multivariate GHD & Copulas/Homework 3")
library(quantmod)
library(ggplot2)
library(ghyp)
Sys.setlocale("LC_ALL","English")

##### Loading data #####
tickers = c("GS","BAC") # specify the tickers to load

e <- new.env() # environment where data will be stored
options("getSymbols.warning4.0"=FALSE) # suppress warning about future change in the function behaviour
getSymbols(tickers, src = "yahoo", from="2014-01-01", env=e) # load data from Yahoo Finance

data = do.call(merge, eapply(e, Ad)[tickers]) # merge data into a single xts taking only adjusted prices
names(data) = tickers

##### Calculate returns #####
rets = apply(data, 2, function(x) diff(x)/x[-length(x)])
rets = xts(rets, order.by = index(data)[-1])

##### Initiate portfolio #####
prt = rets

##### Fit multivariate GHD and assess portfolio risks #####
prt.fit <- fit.ghypmv(prt,symmetric=FALSE,silent=TRUE)

w <- c(0.5,0.5) # assets' weights in portfolio
N <- 10^6; alpha <- 0.1
sim <- rghyp(n=N,object=prt.fit)
prt.sim <- w[1]*sim[,1]+w[2]*sim[,2]
prt.sim <- sort(prt.sim)
VaR <- prt.sim[alpha*N]
ES <- mean(prt.sim[1:(alpha*N-1)])

##### Find optimal portfolio weights #####
opt <- portfolio.optimize(prt.fit,
                          risk.measure="value.at.risk",type="minimum.risk",
                          target.return=NULL,risk.free=NULL,level=0.95,silent=TRUE)



##### Calculate VaR curve #####
VaR_curve <- numeric()
h <- 0.5 * 260 # window length

for (i in (h+1):length(rets)) 
{
  h.rets <- rets[(i-h):(i-1)]
  rets.fit <- stepAIC.ghyp(h.rets,dist=c("ghyp", "hyp", "NIG", "VG", "t", "gauss"),
                          symmetric=NULL,silent=TRUE)
  VaR_curve[i-h] <- qghyp(alpha,object=rets.fit$best.model)
}

fact <- rets[(h+1):length(rets)]
plot(as.numeric(fact),type="l", ylab = "Return")
lines(VaR_curve,col="red")

##### Kupiec test #####
K <- sum(fact<VaR_curve); alpha0 <- K/length(rets)
S <- -2*log((1-alpha)^(length(rets)-K)*alpha^K)+
  2*log((1-alpha0)^(length(rets)-K)*alpha0^K)
p.value <- 1-pchisq(S,df=1)

##### Loss functions #####
L.Lo <- sum((fact-VaR)^2*(fact<VaR))/K
L.BI <- sum((fact-VaR)/VaR*(fact<VaR))/K
