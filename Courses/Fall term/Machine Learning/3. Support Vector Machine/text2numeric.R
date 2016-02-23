dat <- read.csv("elections_usa96_train.csv", header = TRUE, stringsAsFactors = FALSE)

ClinLR <- dat$ClinLR
head(ClinLR)

polit <- c(extLib = 1, Lib = 2, sliLib = 3, Mod = 4, sliCon = 5, Con = 6, extCon = 7)
polit

for (i in 1:length(polit)) {
  repl <- ClinLR == names(polit[i])
  ClinLR[repl] <- replace(ClinLR, repl, polit[i])[repl]
}
ClinLR <- as.numeric(ClinLR)