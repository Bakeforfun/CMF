##### Initialisation #####
setwd("~/CMF/Courses/Applied Financial Econometrics/5. Extreme value theory/Homework 5")
library("evd")
Sys.setlocale("LC_ALL","English")

##### Loading and processing data #####
data = read.csv("Data/loss_train.csv")
debt = rowSums(data)
T = length(debt)

##### Set debt levels #####
u = numeric()
alpha = 1-1/1000;
u[1] = sort(debt)[0.99 * T]
u[2] = sort(debt)[0.95 * T]
u[3] = sort(debt)[0.90 * T]

##### Fit GPD and find levels #####
result = numeric()
for (i in 1:3)
{
  gpd.fit = fpot(debt,threshold=u[i],model="gpd",method="SANN")
  beta = gpd.fit$estimate[1]
  xi = gpd.fit$estimate[2]
  Fu = gpd.fit$pat
  result[i] = u[i]+beta/xi*(((1-alpha)/Fu)^(-xi)-1)
}

write.csv(result, "result.csv")
