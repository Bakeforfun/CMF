##### Initialisation #####
setwd("~/CMF/Courses/Machine Learning/1. Logistic regression/Homework")
library(ggplot2)
Sys.setlocale("LC_ALL","English")

# library(caTools)
# set.seed(123)
# split = sample.split(data$target_variable, SplitRatio = 0.7)
# train = subset(data, split==TRUE)
# test = subset(data, split==FALSE)


##### Loading data #####
y = read.csv("Data/is_spam_train.csv")
X = read.csv("Data/mail_features_train.csv")
X_test = read.csv("Data/mail_features_test.csv")

##### Formatting data #####
y <- cbind(as.numeric(y$x)); X <- as.matrix(X)
X <- cbind(1, X) # add an intercept column
m <- nrow(X); n <- ncol(X) - 1


##### Logistic function #####
g <- function(z) 1/(1+exp(-z))

J <- function(theta) {
  m <- nrow(X)
  # h hypothesis calculation
  # theta and y are column vectors, X is a matrix
  h.theta <- g(X%*%theta)
  -t(y)%*%log(h.theta)/m - t(1-y)%*%log(1-h.theta)/m
}

gradJ <- function(theta) {
  m <- nrow(X)
  t(X)%*%(g(X%*%theta)-y)/m
}


##### Numeric optimisation #####
theta0 <- cbind(rep(0,times=n+1)) # initial values
opt <- optim(fn=J, gr=gradJ, par=theta0, method="BFGS")
theta <- opt$par; Jval <- opt$value
list(theta=as.vector(theta),J=Jval)

##### Results #####
y.result <- X %*% theta
y.result <- as.numeric(y.result >= 0)



##### Visualise #####

# head(iris)
# 
# 
# qplot(iris$Sepal.Length, iris$Sepal.Width, color = iris$Species, lwd = 5)
# qplot(iris$Petal.Length, iris$Petal.Width, color = iris$Species, lwd = 5)
# 
# pairs(iris, pch = 16)


# panel.cor <- function(x, y, digits = 2, cex.cor, ...)
# {
#   usr <- par("usr"); on.exit(par(usr))
#   par(usr = c(0, 1, 0, 1))
#   # correlation coefficient
#   r <- cor(x, y)
#   txt <- format(c(r, 0.123456789), digits = digits)[1]
#   txt <- paste("cor = ", txt, sep = "")
#   text(0.5, 0.6, txt, cex = 1.5)
#   
#   # p-value calculation
#   p <- cor.test(x, y)$p.value
#   txt2 <- format(c(p, 0.123456789), digits = digits)[1]
#   txt2 <- paste("p.val = ", txt2, sep = "")
#   if(p<0.01) txt2 <- paste("p.val ", "< 0.01", sep = "")
#   text(0.5, 0.4, txt2, cex = 1.5)
# }
# 
# pairs(iris, upper.panel = panel.cor, pch = 16)

x1 <- c(0,-theta[1]/theta[3])
x2 <- c(-theta[1]/theta[2],0)
lines(x1,x2,type="l",lwd=3)
