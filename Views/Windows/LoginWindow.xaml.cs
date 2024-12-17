namespace TestItemTemplate.ViewModels.Windows
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    [ObservableObject]
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        [ObservableProperty]
        private string _password;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool loginSuccessful = ValidateLogin( Password);
            if (loginSuccessful)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("登录失败，请检查用户名和密码！");
            }
        }

        private bool ValidateLogin( string password)
        {
            if (password == "123456")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Wpf.Ui.Controls.PasswordBox pw = (Wpf.Ui.Controls.PasswordBox)sender;
            Password = pw.Password;
        }
    }
}
