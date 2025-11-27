using System;
using System.Windows;

namespace diplom_loskutova
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
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
            diplom_loskutova.Page.Applications page = new diplom_loskutova.Page.Applications();
            mainFrame.Navigate(page);
        }



        // ОТКРЫТИЕ СТРАНИЦЫ МЕРОПРИЯТИЯ
        private void ButtonOpenPageEvent(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Events page = new diplom_loskutova.Page.Events();
            mainFrame.Navigate(page);
        }


        // ОТКРЫТИЕ СТРАНИЦЫ ГРАЖДАНЕ
        private void ButtonOpenPageCitizen(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Citizens page = new diplom_loskutova.Page.Citizens();
            mainFrame.Navigate(page);
        }

        // ОТКРЫТИЕ СТРАНИЦЫ ПОЛЬЗОВАТЕЛИ
        private void ButtonOpenPageUsers(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Users page = new diplom_loskutova.Page.Users();
            mainFrame.Navigate(page);
        }


        // ОТКРЫТИЕ СТРАНИЦЫ СТАТУС
        private void ButtonOpenPageStatus(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Status page = new diplom_loskutova.Page.Status();
            mainFrame.Navigate(page);
        }

        // ОТКРЫТИЕ СТРАНИЦЫ ТИП МЕРОПРИЯТИЙ
        private void ButtonOpenPageTypeEvent(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.TypeEvent page = new diplom_loskutova.Page.TypeEvent();
            mainFrame.Navigate(page);
        }

        // ОТКРЫТИЕ СТРАНИЦЫ РОЛЬ ПОЛЬЗОВАТЕЛЯ
        private void ButtonOpenPageRoleUser(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.RoleUser page = new diplom_loskutova.Page.RoleUser();
            mainFrame.Navigate(page);
        }

        // ОТКРЫТИЕ СТРАНИЦЫ ОТЧЕТЫ
        private void ButtonOpenPageReports(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Reports page = new diplom_loskutova.Page.Reports();
            mainFrame.Navigate(page);
        }
    }
}
