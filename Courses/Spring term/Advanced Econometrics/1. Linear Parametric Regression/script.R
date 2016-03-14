library(fBasics) # Normality tests
library(lmtest)  # Heteroscedasticity tests
library(tseries) # Autoregressions

# initial data
library(datasets)
ozone <- airquality$Ozone
rad <- airquality$Solar.R
rem <- is.na(ozone) | is.na(rad)
ozone <- ozone[!rem]; rad <- rad[!rem]

# split data into learning and test sample
N <- length(ozone); E <- 20; T <- N-E
train.obs <- (1:T)
eval.obs <- (T+1):N
t.rad <- rad[train.obs]; t.ozone <- ozone[train.obs]
e.rad <- rad[eval.obs]; e.ozone <- ozone[eval.obs]

# regression
fit.par <- lm(ozone ~ radiation, data=data.frame(radiation=t.rad,ozone=t.ozone), weights=NULL)
# alternative regression call
# fit.par <- lm(t.ozone ~ t.rad)

##### Model quality analysis #####
summary(fit.par)

fit.par$coefficients
fit.par$residuals
fit.par$fitted.values

plot(t.rad,t.ozone,pch=16,xlab="radiation",ylab="ozone")
z <- order(t.rad)
lines(t.rad[z],fit.par$fitted.values[z],col="blue",lwd=3)

##### Residual analysis #####
res <- fit.par$residuals
hist(res)
plot(res,type="l")

# normality tests
shapiro.test(res)
jarqueberaTest(res)

##### Reforlmulation of the model #####
fit.par <- lm(log(ozone) ~ rad + rad2,data=data.frame(rad=t.rad,rad2=t.rad^2,ozone=t.ozone))
plot(t.rad,t.ozone,pch=16,xlab="radiation",ylab="ozone")
z <- order(t.rad)
lines(t.rad[z],exp(fit.par$fitted.values[z]),col="blue",lwd=3)

##### Residual analysis #####
res <- fit.par$residuals
hist(res)
plot(res,type="l")

# normality tests
shapiro.test(res)
jarqueberaTest(res)

##### Heteroscedasticity tests ####
bptest(fit.par,varformula=NULL,data=NULL,studentize=FALSE)
gqtest(fit.par,fraction=25,alternative="two.sided")





##### ASSUME THIS MODEL WITH HETEROSCEDASTICITY #####
oz <- lm(t.ozone ~ t.rad)
bptest(oz,varformula=NULL,data=NULL,studentize=FALSE)

# calculate estimates os standard deivatiations
e.sq <- oz$residuals^2
sigma.hat <- lm(e.sq ~ t.rad)$fitted.values ^ 0.5

# use weighted OLS
oz.wgt <- lm(t.ozone ~ t.rad, weights = 1/sigma.hat)




##### Autocorrelation test #####
dwtest(fit.par,alternative="two.sided")


##### Remove autoregressive part #####
adf.test(log(t.ozone))
ar1 <- arma(log(t.ozone), order = c(1,0))
adf.test(ar1$residuals[2:T])

fit.par <- lm(ozone ~ rad + rad2,data = data.frame(ozone = ar1$residuals[2:T],rad = t.rad[2:T], rad2 = t.rad[2:T]^2))
summary(fit.par)

##### Prediction #####
predict.arma <- function(model, data, newdata, alpha = 0.05) {
  # your code here
  list(fit = fit, lower = fit + delta*qt(alpha/2,df=df),
       upper = fit - delta*qt(alpha/2,df=df))
}
ar1.frc <- predict.arma(model = ar1,
                        data = log(t.ozone)[1:(T-1)],
                        newdata = log(e.ozone), alpha = 0.1)

par.frc <- predict(fit.par,
                   newdata=data.frame(rad=e.rad[2:E],rad2=e.rad[2:E]^2),
                   se.fit=TRUE,interval="prediction",level=0.90)

frc <- exp(ar1.frc$fit[2:E] + par.frc$fit[,"fit"])