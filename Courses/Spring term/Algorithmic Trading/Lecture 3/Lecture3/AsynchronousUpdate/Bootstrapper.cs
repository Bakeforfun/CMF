using AsynchronousUpdate.Views;

namespace AsynchronousUpdate {
    public class Bootstrapper {
        public void Start() {
            var shell = new ShellView();
            shell.Show();
        }
    }
}