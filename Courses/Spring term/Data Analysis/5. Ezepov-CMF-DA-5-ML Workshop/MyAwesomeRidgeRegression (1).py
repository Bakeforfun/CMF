import numpy as np


def norm(x):
    return np.sqrt(np.sum(np.square(x)))


class MyAwesomeRidgeRegression:
    """Class for linear regression with L2-regularization (aka Ridge).
    """

    def __init__(self, num_of_features, reg_constant):
        self.M = num_of_features
        self.reg_constant = reg_constant
        self.init_weights()

    def init_weights(self, seed=None):
        np.random.seed(seed)
        self.W = np.random.normal(size=self.M)
        self.b = 1

    def predict(self, X):
        return np.dot(X, self.W) + self.b

    def loss(self, X, y):
        return np.sum(np.square(y - self.predict(X)))/2/len(y) +\
    self.reg_constant * np.sum(np.square(self.W))

    def loss_grad(self, X, y):
        diff = y - self.predict(X)
        dW = -np.dot(X.T, diff)/len(y) + 2 * self.reg_constant * self.W
        db = -np.sum(diff)/len(y)
        return dW, db

    def check_grad(self, X, y, eps=1e-6):
        """Numerical gradient checking
        """
        # Calculate real gradients
        my_dW, my_db = self.loss_grad(X, y)

        # Backup parameters
        W0 = self.W.copy()
        b0 = self.b

        # Calculate numerical dW
        dW = []
        for i in range(len(W0)):
            self.W[i] = W0[i] + eps
            l1 = self.loss(X, y)

            self.W[i] = W0[i] - eps
            l2 = self.loss(X, y)

            dW.append( (l1-l2)/2/eps )
            self.W[i] = W0[i]
        dW = np.array(dW)

        # Calculate numerical db
        self.b = b0 + eps
        l1 = self.loss(X, y)

        self.b = b0 - eps
        l2 = self.loss(X, y)

        db = (l1-l2)/2/eps
        self.b = b0

        # Check difference
        print('Difference in dW: %3.3g' % (norm(dW - my_dW)/norm(dW + my_dW)))
        print('Difference in db: %3.3g' % (norm(db - my_db)/norm(db + my_db)))

    def train(self, X, y, learning_rate, Xval, yval, early_stopping=10):
        """Train regression with early stopping by validation data.
        Returns training history.
        """
        err = {
            'train': [],
            'val': [],
        }
        assert early_stopping > 1, 'Early stopping must be more than 1!'

        t = 0
        while t < 100000:
            t += 1

            err['train'].append(self.loss(X, y))
            err['val'].append(self.loss(Xval, yval))

            # Early stopping magic
            if t > early_stopping and err['val'][-1] > err['val'][-(early_stopping+1)]:
                print('Stopped at epoch %d.' % t)
                break

            # Gradient descent
            dW, db = self.loss_grad(X, y)
            self.W -= dW * learning_rate
            self.b -= db * learning_rate

        err['train'].append(self.loss(X, y))
        err['val'].append(self.loss(Xval, yval))
        return err
