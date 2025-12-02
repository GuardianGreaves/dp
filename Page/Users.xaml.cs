using diplom_loskutova.Helpers;
using System;
using System.Data;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace diplom_loskutova.Page
{
    public partial class Users : System.Windows.Controls.Page
    {
        private DP_2025_LoskutovaDataSetTableAdapters.ПОЛЬЗОВАТЕЛЬTableAdapter adapter = new DP_2025_LoskutovaDataSetTableAdapters.ПОЛЬЗОВАТЕЛЬTableAdapter();
        private DP_2025_LoskutovaDataSet db = new DP_2025_LoskutovaDataSet();   // Объект для работы с данными из базы (DataSet)
        public Users(string _role)
        {
            InitializeComponent();
            LoadData();
            LoadRolesToComboBox();

            var visibilityManager = new Class.RoleVisibilityManager(_role);
            visibilityManager.SetButtonVisibility(btnDelete, btnAdd, btnChange);
        }

        // Загружает данные из базы в DataSet и привязывает к ListView.
        private void LoadData()
        {
            try
            {
                adapter.FillBy(db.ПОЛЬЗОВАТЕЛЬ);
                listViewUsers.ItemsSource = db.ПОЛЬЗОВАТЕЛЬ.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        // Открывает страницу создания новой записи.
        private void BtnAdd(object sender, RoutedEventArgs e)
        {
            OpenPage(false);
        }

        // Проверяет выбран ли элемент, удаляет его из DataTable, обновляет базу и перезагружает данные.
        private void BtnDelete(object sender, RoutedEventArgs e)
        {
            if (TryGetSelectedRow(out DataRowView selectedRowView))
            {
                var result = MessageBox.Show("Вы уверены что хотите удалить запись ?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    selectedRowView.Row.Delete();

                    try
                    {
                        adapter.Update(db.ПОЛЬЗОВАТЕЛЬ);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.");
            }
        }

        // Навигирует на страницу редактирования выбранного элемента.
        private void BtnChange(object sender, RoutedEventArgs e)
        {
            NavigatePageSelectedRow();
        }

        // Переходит на страницу редактирования выбранной записи, если она выбрана.
        private void ListViewStatus_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigatePageSelectedRow();
        }

        private void NavigatePageSelectedRow()
        {
            if (TryGetSelectedRow(out DataRowView selectedRowView))
                OpenPage(true, selectedRowView);
            else
                MessageBox.Show("Выберите строку для редактирования.");
        }

        // Универсальный метод для выбора строки из ListView как DataRowView.
        private bool TryGetSelectedRow(out DataRowView selectedRowView)
        {
            selectedRowView = listViewUsers.SelectedItem as DataRowView;
            return selectedRowView != null;
        }

        // Открывает страницу добавления или изменения записи.
        private void OpenPage(bool isChangeOrAdd, DataRowView rowView = null)
        {
            diplom_loskutova.Page.AddOrChange.UsersAOC page;
            if (rowView != null)
            {
                page = new diplom_loskutova.Page.AddOrChange.UsersAOC(rowView);
            }
            else
            {
                page = new diplom_loskutova.Page.AddOrChange.UsersAOC();
            }

            page.ChangeOrAdd = isChangeOrAdd;
            page.DataChanged += (s, ev) => LoadData();
            NavigationService.Navigate(page);
        }

        private void LoadRolesToComboBox()
        {
            var roleAdapter = new DP_2025_LoskutovaDataSetTableAdapters.РОЛЬTableAdapter();
            ComboBoxHelper.LoadData(ComboBoxSearchRole, roleAdapter.GetData(), "Название", "ID_Роли");
        }

        private void ComboBoxSearchRole_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (db.ПОЛЬЗОВАТЕЛЬ.Rows.Count == 0)
            {
                listViewUsers.ItemsSource = null;
                return;
            }

            string filter = "";

            // Фильтр по выбранной роли
            if (ComboBoxSearchRole.SelectedValue != null)
            {
                filter += $"ID_Роли = {ComboBoxSearchRole.SelectedValue}";
            }

            // Фильтр по ФИО
            var fio = TextBoxSearchFIO.Text.Trim();
            if (!string.IsNullOrEmpty(fio))
            {
                if (filter.Length > 0)
                    filter += " AND ";
                filter += $"(Фамилия LIKE '%{fio}%' OR Имя LIKE '%{fio}%' OR Отчество LIKE '%{fio}%')";
            }

            // Фильтр по логину
            var login = TextBoxSearchLogin.Text.Trim();
            if (!string.IsNullOrEmpty(login))
            {
                if (filter.Length > 0)
                    filter += " AND ";
                filter += $"Логин LIKE '%{login}%'";
            }

            DataView dv = db.ПОЛЬЗОВАТЕЛЬ.DefaultView;
            dv.RowFilter = filter;
            listViewUsers.ItemsSource = dv;
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxSearchRole.SelectedIndex = -1; // Сброс выбора роли
            TextBoxSearchFIO.Text = "";             // Очистка поиска по ФИО
            TextBoxSearchLogin.Text = "";           // Очистка поиска по логину
            ApplyFilter();                          // Применить фильтр 
        }

        private void TextBoxSearchFIO_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void TextBoxSearchLogin_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter();
        }
    }
}