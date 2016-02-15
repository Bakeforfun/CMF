X <- rbind(cbind(rnorm(100),rnorm(100)), cbind(rnorm(100, mean = 3), rnorm(100, mean = 3)))
class <- c(rep(1,100),rep(2,100))
plot(X, pch = 16)

smpl <- sample(1:nrow(X))
X <- X[smpl,]; class <- class[smpl]

K <- 2
n <- ncol(X); m <- nrow(X)

initMeans <- function(X, K) X[sample(1:nrow(X), size = K),]
initMeans(X, K)

Mu <- initMeans(X, K)
points(Mu, pch = 4, col = "red", lwd = 3)

covMatr <- function(X, class) {
  u <- unique(class)
  K <- length(u)
  cm <- vector("list", K)
  for (i in 1:K) cm[[i]] <- cov(X[class == class[i],])
  cm
}


distEuclid <- function(x, y, Sigma) (sum((x-y)^2))^0.5
X[1:2,]
distEuclid(X[1,], X[2,])
distEuclid(X[2,], X[2,])

distMahalanobis <- function(x, y, Sigma) {
  solveSigma <- try(solve(Sigma), silent = TRUE)
  if (class(solveSigma) == "try-error") solveSigma <- diag(nrow(Sigma))
  as.vector((rbind(x-y) %*% solveSigma %*% cbind(x-y))^0.5)
}
Sigma <- list(diag(rep(1,n)), diag(rep(1,n)))
distMahalanobis(X[1,], X[2,], Sigma[[1]])

distMatr <- function(X, Mu, distFunc, Sigma) {
  K <- nrow(Mu); m <- nrow(X); n <- ncol(X)
  dist <- vector("list", K)
  for (j in 1:K) dist[[j]] <- function(x) distFunc(x, Mu[j,], Sigma[[j]])
  Matr <- matrix(nrow = m, ncol = K)
  for (j in 1:K) Matr[,j] <- apply(X, 1, dist[[j]])
  Matr
}

head(distMatr(X, Mu, distMahalanobis, Sigma))

assignClass <- function(X, Mu, distFunc, Sigma) {
  Matr <- distMatr(X, Mu, distFunc, Sigma)
  class <- apply(Matr, 1, which.min)
  u <- unique(class)
  for (i in 1:length(u)) class[class == u[i]] <- i
  class
}

calcMeans <- function(X, class) {
  u <- unique(class)
  K <- length(u); m <- nrow(X); n <- ncol(X)
  Mu <- matrix(nrow = K, ncol = n)
  for (i in 1:length(u)) Mu[i,] <- apply(rbind(X[class == u[i],]), 2, mean)
  Mu
}
Mu <- calcMeans(X, class)
points(Mu, pch = 4, col = "green", lwd = 3)


calcTSS <- function(X) {
  m <- nrow(X); n <- ncol(X)
  mu <- apply(X, 2, mean)
  TSS <- matrix(0, nrow = n, ncol = n)
  for (i in 1:m) TSS <- TSS + cbind(X[i,]-mu) %*% rbind(X[i,]-mu)
  TSS
}
calcTSS(X)

calcWSS <- function(X, Mu, class) {
  m <- nrow(X); n <- ncol(X)
  WSS <- matrix(0, nrow = n, ncol = n)
  for (i in 1:m) WSS <- WSS + cbind(X[i,]-Mu[class[i],]) %*% rbind(X[i,]-Mu[class[i],])
  WSS
}
calcWSS(X, Mu, class)

calcBSS <- function(X, Mu, class) calcTSS(X) - calcWSS(X, Mu, class)
calcBSS(X, Mu, class)
  


KMeans <- function(X, K, distFunc = distEuclid, nstart = 10, maxIter = 20, tol = 0.001) {
  
  model <- NULL; err <- Inf
  m<- nrow(X); n <- ncol(X)

  for (s in 1:nstart) {

    Mu <- initMeans(X, K)
    Sigma <- vector("list", K); for (k in 1:K) Sigma[[k]] <- diag(rep(1,n))
    class <- assignClass(X, Mu, distFunc, Sigma)

    iter <- 0
    TSS <- calcTSS(X); WSS <- calcWSS(X, Mu, class)
    dTSS <- det(TSS); dWSS <- det(WSS)
    ratio <- dWSS / dTSS; deltaRatio <- -Inf
  
    optTrace <- rbind(c(0, dWSS, round(ratio*100,1)))
    dimnames(optTrace) <- list(NULL, c("iteration", "wss", "wss.tss"))

    while (iter <= maxIter & deltaRatio < -tol) {
      iter <- iter + 1
      Mu <- calcMeans(X, class)
      Sigma <- covMatr(X, class)
      class <- assignClass(X, Mu, distFunc, Sigma)
      WSS <- calcWSS(X, Mu, class); dWSS <- det(WSS)
      newRatio <- dWSS / dTSS; deltaRatio <- newRatio - ratio
      ratio <- newRatio
      optTrace <- rbind(optTrace, c(iter, dWSS, round(ratio*100,1)))
    }

    if (dWSS < err) {
      model <- list(optTrace = optTrace, means = Mu, class = class, err = dWSS)
      err <- dWSS
    }

  }

  model
  
}

KMeans(X, 2)

km <- KMeans(X, 2)
library(ggplot2)
qplot(X[,1], X[,2], color = km$class, lwd = 3)


