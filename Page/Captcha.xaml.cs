using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace diplom_loskutova.Page
{
    /// <summary>
    /// Логика взаимодействия для Captcha.xaml
    /// </summary>
    public partial class Captcha : System.Windows.Controls.Page
    {
        string CaptchaText;

        public Captcha()
        {
            InitializeComponent();
            CapthaGenerate();
        }

        void CapthaGenerate()
        {
            diplom_lib_loskutova.Captcha.Captcha captchaGenerator = new diplom_lib_loskutova.Captcha.Captcha(120, 36);
            CaptchaText = captchaGenerator.CaptchaText;
            BitmapImage _captchaImage = captchaGenerator.GenerateCaptchaBitmapImage();
            captchaImage.Source = _captchaImage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CapthaGenerate();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (CaptchaText == textBoxCaptcha.Text)
            {
                MessageBox.Show("Каптча введена верно");
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Каптча введена неверно");
                CapthaGenerate();
            }
        }
    }
}
