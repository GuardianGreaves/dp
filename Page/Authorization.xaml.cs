using diplom_lib_loskutova.Encryption;
using System.Windows;

namespace diplom_loskutova.Page
{
    public partial class Authorization : System.Windows.Controls.Page
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Используем новый класс шифрования скремблер
            ScramblerEncryptor encryptor = new ScramblerEncryptor("13371337");
            var encryptLogin = encryptor.Encrypt(TextBoxLogin.Text.Trim());
            var encryptPassword = encryptor.Encrypt(TextBoxPassword.Password.Trim());

            if (string.IsNullOrEmpty(encryptLogin) || string.IsNullOrEmpty(encryptPassword))
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }

            DbHelper db = new DbHelper();
            var result = db.CheckUser(encryptLogin, encryptPassword);

            if (result.count > 0)
            {
                MessageBox.Show("Авторизация успешна.");
                MainWindow newWindow = new MainWindow(result.name, result.role);
                newWindow.Show();
                Application.Current.MainWindow.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }

        private void TextBoxLogin_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBlockLogin.Text) && TextBoxLogin.Text.Length > 0)
            {
                TextBlockLogin.Visibility = Visibility.Hidden;
            }
            else
            {
                TextBlockLogin.Visibility = Visibility.Visible;
            }
        }

        private void TextBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBlockPassword.Text) && TextBoxPassword.Password.Length > 0)
            {
                TextBlockPassword.Visibility = Visibility.Hidden;
            }
            else
            {
                TextBlockPassword.Visibility = Visibility.Visible;
            }
        }
    }
}
