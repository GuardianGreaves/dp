using diplom_loskutova.Class;
using diplom_loskutova.Page;
using System;
using System.Web.Configuration;
using System.Windows;

namespace diplom_loskutova
{
    public partial class MainWindow : Window
    {
        private string role;
        public MainWindow(string _name, string _role)
        {
            InitializeComponent();
            nameUser.Text = _name;
            role = _role;
            RoleManager roleManager = new RoleManager();
            nameRole.Text = "(" + roleManager.GetNameById(Convert.ToInt32(_role)) + ")";

            if (_role == "3")
            {
                BtnOpenPageApplication.Visibility = Visibility.Collapsed;
                BtnOpenPageCitizen.Visibility = Visibility.Collapsed;
                BtnOpenPageUsers.Visibility = Visibility.Collapsed;
                BtnOpenPageStatus.Visibility = Visibility.Collapsed;
                BtnOpenPageTypeEvent.Visibility = Visibility.Collapsed;
                BtnOpenPageRoleUser.Visibility = Visibility.Collapsed;
                BtnOpenPageReports.Visibility = Visibility.Collapsed;
            }
        }



        // КНОПКИ ОКНА
        private Window GetWindow() => Window.GetWindow(this);
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            var window = GetWindow();
            if (window != null) window.WindowState = WindowState.Minimized;
        }
        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            var window = GetWindow();
            if (window != null)
            {
                window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var window = GetWindow();
            if (window != null) window.Close();
        }



        // ОТКРЫТИЕ СТРАНИЦЫ ЗАЯВКИ
        private void ButtonOpenPageApplication(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Applications page = new diplom_loskutova.Page.Applications(role);
            mainFrame.Navigate(page);
        }



        // ОТКРЫТИЕ СТРАНИЦЫ МЕРОПРИЯТИЯ
        private void ButtonOpenPageEvent(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Events page = new diplom_loskutova.Page.Events(role);
            mainFrame.Navigate(page);
        }


        // ОТКРЫТИЕ СТРАНИЦЫ ГРАЖДАНЕ
        private void ButtonOpenPageCitizen(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Citizens page = new diplom_loskutova.Page.Citizens(role);
            mainFrame.Navigate(page);
        }

        // ОТКРЫТИЕ СТРАНИЦЫ ПОЛЬЗОВАТЕЛИ
        private void ButtonOpenPageUsers(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Users page = new diplom_loskutova.Page.Users(role);
            mainFrame.Navigate(page);
        }


        // ОТКРЫТИЕ СТРАНИЦЫ СТАТУС
        private void ButtonOpenPageStatus(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Status page = new diplom_loskutova.Page.Status(role);
            mainFrame.Navigate(page);
        }

        // ОТКРЫТИЕ СТРАНИЦЫ ТИП МЕРОПРИЯТИЙ
        private void ButtonOpenPageTypeEvent(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.TypeEvent page = new diplom_loskutova.Page.TypeEvent(role);
            mainFrame.Navigate(page);
        }

        // ОТКРЫТИЕ СТРАНИЦЫ РОЛЬ ПОЛЬЗОВАТЕЛЯ
        private void ButtonOpenPageRoleUser(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.RoleUser page = new diplom_loskutova.Page.RoleUser(role);
            mainFrame.Navigate(page);
        }

        // ОТКРЫТИЕ СТРАНИЦЫ ОТЧЕТЫ
        private void ButtonOpenPageReports(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Reports page = new diplom_loskutova.Page.Reports();
            mainFrame.Navigate(page);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowAuthorization newWindow = new WindowAuthorization();
            newWindow.Show();
            this.Close();
        }
    }
}
