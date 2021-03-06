##### Initialisation #####
setwd("~/CMF/Courses/Applied Financial Econometrics/3. Portfolio modeling, multivariate GHD & Copulas/Homework 3")
library(quantmod)
library(ggplot2)
library(ghyp)
library(copula)
Sys.setlocale("LC_ALL","English")

##### Loading data #####
tickers = c("AAPL","GOOGL") # specify the tickers to load

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

##### Find optimal portfolio weights and assess portfolio risks #####
opt <- portfolio.optimize(prt.fit,
                          risk.measure="value.at.risk",type="minimum.risk",
                          target.return=NULL,risk.free=NULL,level=0.95,silent=TRUE)

w <- opt$opt.weights # assets' weights in portfolio
N <- 10^6; alpha <- 0.1
sim <- rghyp(n=N,object=prt.fit)
prt.sim <- w[1]*sim[,1]+w[2]*sim[,2]
prt.sim <- sort(prt.sim)
VaR_new <- prt.sim[alpha*N]
ES_new <- mean(prt.sim[1:(alpha*N-1)])

##### Calculate VaR curve #####
VaR_curve <- numeric()
h <- 0.5 * 260 # window length
h.w <- matrix(ncol = 2, nrow = length(prt[,1]) - h)

for (i in (h+1):length(prt[,1])) 
{
  h.prt <- prt[(i-h):(i-1),]
  
  h.prt.fit <- fit.ghypmv(h.prt,symmetric=FALSE,silent=TRUE)
  
  h.opt <- portfolio.optimize(h.prt.fit,
                            risk.measure="value.at.risk",type="minimum.risk",
                            target.return=NULL,risk.free=NULL,level=0.95,silent=TRUE)
  
  h.w[i-h,] <- h.opt$opt.weights # assets' weights in portfolio

  sim <- rghyp(n=N,object=h.prt.fit)
  h.prt.sim <- h.w[i-h,1]*sim[,1]+h.w[i-h,2]*sim[,2]
  h.prt.sim <- sort(h.prt.sim)
  VaR_curve[i-h] <- h.prt.sim[alpha*N]
}

fact <- prt[(h+1):length(prt[,1]),1]*h.w[,1] + prt[(h+1):length(prt[,2]),2]*h.w[,2]
plot(as.numeric(fact),type="l", ylab = "Return")
lines(VaR_curve,col="red")

##### Kupiec test #####
K <- sum(fact<VaR_curve); alpha0 <- K/length(rets)
S <- -2*log((1-alpha)^(length(rets)-K)*alpha^K)+
  2*log((1-alpha0)^(length(rets)-K)*alpha0^K)
p.value <- 1-pchisq(S,df=1)

##### Fit assets' distributions #####
aapl.fit <- stepAIC.ghyp(rets$AAPL,dist=c("gauss","t","ghyp"),
                        symmetric=NULL,silent=TRUE)$best.model
googl.fit <- stepAIC.ghyp(rets$GOOGL,dist=c("gauss","t","ghyp"),
                        symmetric=NULL,silent=TRUE)$best.model

aapl.cdf <- pghyp(rets$AAPL,object=aapl.fit)
googl.cdf <- pghyp(rets$GOOGL,object=googl.fit)
cdf <- cbind(as.numeric(aapl.cdf), as.numeric(googl.cdf))

##### Initialise copulas #####
norm.cop <- normalCopula(dim=2,param=0.5,dispstr="un")
stud.cop <- tCopula(dim=2,param=0.5,df=5,df.fixed=TRUE,dispstr="un")
gumb.cop <- gumbelCopula(dim=2,param=2)
clay.cop <- claytonCopula(dim=2,param=2)

##### Fit copulas #####
norm.fit <- fitCopula(cdf,copula=norm.cop)
stud.fit <- fitCopula(cdf,copula=stud.cop)
gumb.fit <- fitCopula(cdf,copula=gumb.cop)
clay.fit <- fitCopula(cdf,copula=clay.cop)

##### Get assets' returns #####
N <- 10^4
stud.sim <- rCopula(n=N,copula=stud.fit@copula)

aapl.sim <- qghyp(stud.sim[,1],object=aapl.fit)
googl.sim <- qghyp(stud.sim[,2],object=googl.fit)
w_copula <- c(0.5,0.5)
prt.sim <- w_copula[1]*aapl.sim + w_copula[2]*googl.sim

##### VaR and ES #####
alpha <- 0.10
prt.sim <- sort(prt.sim)
VaR <- prt.sim[alpha*N]
ES <- mean(prt.sim[1:(alpha*N-1)])

##### Calculate VaR curve #####
VaR_curve <- numeric()
h <- 0.5 * 260 # window length


for (i in (h+1):length(rets[,1])) 
{
  h.rets <- rets[(i-h):(i-1),]
  
  aapl.fit <- stepAIC.ghyp(h.rets$AAPL,dist=c("t","ghyp"),
                           symmetric=NULL,silent=TRUE)$best.model
  googl.fit <- stepAIC.ghyp(h.rets$GOOGL,dist=c("t","ghyp"),
                            symmetric=NULL,silent=TRUE)$best.model
  
  aapl.cdf <- pghyp(h.rets$AAPL,object=aapl.fit)
  googl.cdf <- pghyp(h.rets$GOOGL,object=googl.fit)
  cdf <- cbind(as.numeric(aapl.cdf), as.numeric(googl.cdf))
  
  stud.fit <- fitCopula(cdf,copula=stud.cop)
  
  aapl.sim <- qghyp(stud.sim[,1],object=aapl.fit)
  googl.sim <- qghyp(stud.sim[,2],object=googl.fit)
  
  h.prt.sim <- w[1]*aapl.sim + w[2]*googl.sim
  
  h.prt.sim <- sort(h.prt.sim)
  VaR_curve[i-h] <- h.prt.sim[alpha*N]
}

fact <- prt[(h+1):length(prt[,1]),1]*h.w[,1] + prt[(h+1):length(prt[,2]),2]*h.w[,2]
plot(as.numeric(fact),type="l", ylab = "Return")
lines(VaR_curve,col="red")

##### Kupiec test #####
K <- sum(fact<VaR_curve); alpha0 <- K/length(rets)
S <- -2*log((1-alpha)^(length(rets)-K)*alpha^K)+
  2*log((1-alpha0)^(length(rets)-K)*alpha0^K)
p.value <- 1-pchisq(S,df=1)
