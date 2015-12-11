##### Initialisation #####
setwd("~/CMF/Courses/Applied Financial Econometrics/4. Low loss VaR curves using GARCH/Homework 4")
library(quantmod)
library(ggplot2)
library(ghyp)
library(FinTS)
library(copula)
library(fGarch)
library(tseries)
Sys.setlocale("LC_ALL","English")

##### Loading data #####
tickers = c("SPY","DIA") # specify the tickers to load

e <- new.env() # environment where data will be stored
options("getSymbols.warning4.0"=FALSE) # suppress warning about future change in the function behaviour
getSymbols(tickers, src = "google", from="2014-01-01", env=e) # load data from Yahoo Finance

data = do.call(merge, eapply(e, Cl)[tickers]) # merge data into a single xts taking only adjusted prices
names(data) = tickers

##### Calculate returns #####
rets = apply(data, 2, function(x) diff(x)/x[-length(x)])
rets = xts(rets, order.by = index(data)[-1])

##### LM-test #####
ArchTest(rets$SPY,lags=12)
ArchTest(rets$DIA,lags=12)

##### GARCH modelling #####
#SPY
SPY.gfit <- garchFit(formula=~aparch(1,1),data=rets$SPY,delta=2,
                     include.delta=FALSE,leverage=TRUE,cond.dist="sged",
                     shape=1.2,include.shape=TRUE,trace=FALSE)

plot(SPY.gfit,which=13)
plot(SPY.gfit,which=10)

#DIA
DIA.gfit <- garchFit(formula=~aparch(1,1),data=rets$DIA,delta=2,
                     include.delta=FALSE,leverage=TRUE,cond.dist="sged",
                     shape=1.2,include.shape=TRUE,trace=FALSE)

plot(DIA.gfit,which=13)
plot(DIA.gfit,which=10)

##### Stationarity tests #####
adf.test(rets$SPY)
adf.test(rets$DIA)

pp.test(rets$SPY)
pp.test(rets$DIA)

kpss.test(rets$SPY, null="Level")
kpss.test(rets$DIA, null="Level")

##### Standardized residuals #####
z <- matrix(nrow=length(rets$SPY),ncol=2)
z[,1] <- SPY.gfit@residuals / SPY.gfit@sigma.t
z[,2] <- DIA.gfit@residuals / DIA.gfit@sigma.t

##### Residuals partial distributions #####
mean <- c(0,0); sd <- c(1,1); nu <- c(1.2,1.2)

xi <- c(SPY.gfit@fit$par["skew"], DIA.gfit@fit$par["skew"])
cdf <- matrix(nrow=length(rets$SPY),ncol=2)
for (i in 1:2) cdf[,i] = psged(z[,i],mean=mean[i],sd=sd[i],nu=nu[i],xi=xi[i])

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

##### Monte-Carlo
N = 100000
cdf.sim <- rcopula(n=N,copula=stud.fit@copula)
z.sim <- matrix(nrow=N,ncol=2)
for (i in 1:2) z.sim[,i] <- qsged(cdf.sim[,i],
                                  mean=mean[i],sd=sd[i],nu=nu[i],xi=xi[i])
frc1 <- predict(SPY.gfit,n.ahead=1)
frc2 <- predict(DIA.gfit,n.ahead=1)
mu <- c(frc1[,1],frc2[,1])
sigma <- c(frc1[,3],frc2[,3])

##### Modelled portfolio returns #####
w = c(0.5, 0,5)
prt.sim <- w[1]*(mu[1]+sigma[1]*z.sim[,1]) +
  w[2]*(mu[2]+sigma[2]*z.sim[,2])

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

  SPY.gfit <- garchFit(formula=~aparch(1,1),data=h.rets$SPY,delta=2,
                       include.delta=FALSE,leverage=TRUE,cond.dist="sged",
                       shape=1.2,include.shape=TRUE,trace=FALSE)
  
  DIA.gfit <- garchFit(formula=~aparch(1,1),data=h.rets$DIA,delta=2,
                       include.delta=FALSE,leverage=TRUE,cond.dist="sged",
                       shape=1.2,include.shape=TRUE,trace=FALSE)
  
  z <- matrix(nrow=length(h.rets$SPY),ncol=2)
  z[,1] <- SPY.gfit@residuals / SPY.gfit@sigma.t
  z[,2] <- DIA.gfit@residuals / DIA.gfit@sigma.t
  
  mean <- c(0,0); sd <- c(1,1); nu <- c(1.2,1.2)
  xi <- c(SPY.gfit@fit$par["skew"], DIA.gfit@fit$par["skew"])
  cdf <- matrix(nrow=length(h.rets$SPY),ncol=2)
  for (j in 1:2) cdf[,j] = psged(z[,j],mean=mean[j],sd=sd[j],nu=nu[j],xi=xi[j])
  
  stud.fit <- fitCopula(cdf,copula=stud.cop)
  
  N = 100000
  cdf.sim <- rcopula(n=N,copula=stud.fit@copula)
  z.sim <- matrix(nrow=N,ncol=2)
  for (j in 1:2) z.sim[,j] <- qsged(cdf.sim[,j],
                                    mean=mean[j],sd=sd[j],nu=nu[j],xi=xi[j])
  frc1 <- predict(SPY.gfit,n.ahead=1)
  frc2 <- predict(DIA.gfit,n.ahead=1)
  h.mu <- c(frc1[,1],frc2[,1])
  h.sigma <- c(frc1[,3],frc2[,3])
  
  h.prt.sim <- w[1]*(h.mu[1]+h.sigma[1]*z.sim[,1]) +
    w[2]*(h.mu[2]+h.sigma[2]*z.sim[,2])
  
  h.prt.sim <- sort(prt.sim)
  
  VaR_curve[i-h] <- h.prt.sim[alpha*N]
}

fact <- prt.sim[(h+1):length(prt.sim)] # FFFIIIIIINIIIIISSHHHHH
plot(as.numeric(fact),type="l", ylab = "Return")
lines(VaR_curve,col="red")

##### Kupiec test #####
K <- sum(fact<VaR_curve); alpha0 <- K/length(rets)
S <- -2*log((1-alpha)^(length(rets)-K)*alpha^K)+
  2*log((1-alpha0)^(length(rets)-K)*alpha0^K)
p.value <- 1-pchisq(S,df=1)















##### VaR curve #####

#T1 <- 6*260; T2 <- T - T1 # обучающая и экзаменующая выборки

# на пространстве экзаменующей выборки построим набор
# последовательных значений VaR
VaR_curve <- numeric()
h <- 0.5*260
for (i in (h+1):length(rets)) {
  h.rets <- rets[(i-h):(i-1)]
  rets.gfit <- garchFit(formula=~aparch(1,1),data=h.rets,
                       delta=2,include.delta=FALSE,leverage=TRUE,cond.dist="sged",
                       shape=1.5,include.shape=FALSE,trace=FALSE)
  rets.frc <- predict(rets.gfit,n.ahead=1)
  VaR_curve[i-h] <- rets.frc[1,1]+rets.frc[1,3]*qsged(alpha,mean=0,sd=1,
                                               nu=1.5,xi=rets.gfit@fit$par["skew"])
}

fact <- rets[(h+1):length(rets)]
plot(as.numeric(fact),type="l", ylab = "Return")
lines(VaR_curve,col="red")

##### Kupiec test #####
K <- sum(fact<VaR_curve); alpha0 <- K/length(rets)
S <- -2*log((1-alpha)^(length(rets)-K)*alpha^K)+
  2*log((1-alpha0)^(length(rets)-K)*alpha0^K)
p.value <- 1-pchisq(S,df=1)

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
