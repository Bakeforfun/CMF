##### Initialisation #####
setwd("~/CMF/Courses/Machine Learning/2. Time series decomposition, seasonal, trend & cycle/Homework 2")
library(ggplot2)
library(stats)
library(mFilter)
library(forecast)
Sys.setlocale("LC_ALL","English")

##### Loading data #####
tmp = read.csv("Data/prod_indx_train.csv", header = TRUE)[,1]
y <- ts(log(tmp), start = c(1959, 8), freq = 12)

##### Time series decompostion #####
plot(y)

## decompose ts into trend, seasonal and remainder data
stl.y <- stl(y, s.window="periodic", robust=FALSE)
plot(stl.y)
ts = stl.y$time.series

## separate trend from cycles
# lambda = 6.25 for yearly data
# lambda = 1600 for quarterly data
# lambda = 129600 for monthly data
# freq = NULL to choose lambda automatically
hp.trend <- hpfilter(stl.y$time.series[,"trend"], type = "lambda", freq = 129600, drift = FALSE)
plot(hp.trend)

## repeat H-P for smoother cycle
cycle <- hpfilter(hp.trend$cycle,type="lambda",freq=1600)$trend
plot(cycle)

##### Predict new values for the series decompositions #####
## use ARIMA to predict cycle
fit = auto.arima(cycle)
pred.cycle = forecast(fit,63)
plot(pred.cycle)
res.cycle = c(cycle,pred.cycle$mean)
res.cycle <- ts(res.cycle, start = c(1959, 8), freq = 12)

## predict seasonal component
pred.season = forecast(ts[,1],63)
plot(pred.season)
res.season = c(ts[,1],pred.season$mean)
res.season <- ts(res.season, start = c(1959, 8), freq = 12)

## predict cycle
pred.trend = forecast(ts[,2],63)
plot(pred.trend)
res.trend = c(ts[,2],pred.trend$mean)
res.trend <- ts(res.trend, start = c(1959, 8), freq = 12)

##### Combine decompositions #####
result = res.trend + res.season + res.cycle
result = exp(result)

##### Write the result to a file #####
result.pred = pred.cycle$mean + pred.season$mean + pred.trend$mean
result.pred = exp(result.pred)
write.csv(result.pred, "result.csv")
