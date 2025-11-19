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
            page.OpenPageApplicationAOC += OpenPageApplicationAOC;
            mainFrame.Navigate(page);
        }
        private void OpenPageApplicationAOC(object sender, EventArgs e)
        {
            mainFrame.Navigate(new diplom_loskutova.Page.AddOrChange.ApplicationsAOC());
        }



        // ОТКРЫТИЕ СТРАНИЦЫ МЕРОПРИЯТИЯ
        private void ButtonOpenPageEvent(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Events page = new diplom_loskutova.Page.Events();
            page.OpenPageEventAOC += OpenPageEventAOC;
            mainFrame.Navigate(page);
        }
        private void OpenPageEventAOC(object sender, EventArgs e)
        {
            mainFrame.Navigate(new diplom_loskutova.Page.AddOrChange.EventsAOC());
        }



        // ОТКРЫТИЕ СТРАНИЦЫ ГРАЖДАНЕ
        private void ButtonOpenPageCitizen(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Citizens page = new diplom_loskutova.Page.Citizens();
            page.OpenPageCitizenAOC += OpenPageCitizenAOC;
            mainFrame.Navigate(page);
        }
        private void OpenPageCitizenAOC(object sender, EventArgs e)
        {
            mainFrame.Navigate(new diplom_loskutova.Page.AddOrChange.CitizensAOC());
        }



        // ОТКРЫТИЕ СТРАНИЦЫ ПОЛЬЗОВАТЕЛИ
        private void ButtonOpenPageUsers(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Users page = new diplom_loskutova.Page.Users();
            page.OpenPageUsersAOC += OpenPageUsersAOC;
            mainFrame.Navigate(page);
        }
        private void OpenPageUsersAOC(object sender, EventArgs e)
        {
            mainFrame.Navigate(new diplom_loskutova.Page.AddOrChange.UsersAOC());
        }



        // ОТКРЫТИЕ СТРАНИЦЫ СТАТУС
        private void ButtonOpenPageStatus(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Status page = new diplom_loskutova.Page.Status();
            page.OpenPageStatusAOC += OpenPageStatusAOC;
            mainFrame.Navigate(page);
        }
        private void OpenPageStatusAOC(object sender, EventArgs e)
        {
            mainFrame.Navigate(new diplom_loskutova.Page.AddOrChange.StatusAOC());
        }



        // ОТКРЫТИЕ СТРАНИЦЫ ТИП МЕРОПРИЯТИЙ
        private void ButtonOpenPageTypeEvent(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.TypeEvent page = new diplom_loskutova.Page.TypeEvent();
            page.OpenPageTypeEventAOC += OpenPageTypeEventAOC;
            mainFrame.Navigate(page);
        }
        private void OpenPageTypeEventAOC(object sender, EventArgs e)
        {
            mainFrame.Navigate(new diplom_loskutova.Page.AddOrChange.TypeEventAOC());
        }



        // ОТКРЫТИЕ СТРАНИЦЫ РОЛЬ ПОЛЬЗОВАТЕЛЯ
        private void ButtonOpenPageRoleUser(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.RoleUser page = new diplom_loskutova.Page.RoleUser();
            page.OpenPageRoleUserAOC += OpenPageRoleUserAOC;
            mainFrame.Navigate(page);
        }
        private void OpenPageRoleUserAOC(object sender, EventArgs e)
        {
            mainFrame.Navigate(new diplom_loskutova.Page.AddOrChange.RoleUserAOC());
        }



        // ОТКРЫТИЕ СТРАНИЦЫ ОТЧЕТЫ
        private void ButtonOpenPageReports(object sender, RoutedEventArgs e)
        {
            diplom_loskutova.Page.Reports page = new diplom_loskutova.Page.Reports();
            page.OpenPageReport += OpenPageReportsAOC;
            mainFrame.Navigate(page);
        }
        private void OpenPageReportsAOC(object sender, EventArgs e)
        {
            mainFrame.Navigate(new diplom_loskutova.Page.Reports());
        }

    }
}
