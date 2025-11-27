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
using System.Windows.Shapes;

namespace diplom_loskutova
{
    public partial class WindowAuthorization : Window
    {
        public WindowAuthorization()
        {
            InitializeComponent();
            diplom_loskutova.Page.Authorization page = new diplom_loskutova.Page.Authorization();
            mainFrame.Navigate(new diplom_loskutova.Page.Authorization());
        }
    }
}
