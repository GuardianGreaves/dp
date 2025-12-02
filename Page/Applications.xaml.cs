using diplom_loskutova.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace diplom_loskutova.Page
{
    public partial class Applications : System.Windows.Controls.Page
    {
        private DP_2025_LoskutovaDataSetTableAdapters.ЗАЯВКАTableAdapter adapter = new DP_2025_LoskutovaDataSetTableAdapters.ЗАЯВКАTableAdapter();
        private DP_2025_LoskutovaDataSet db = new DP_2025_LoskutovaDataSet();   // Объект для работы с данными из базы (DataSet)
        public Applications(string _role)
        {
            InitializeComponent();
            LoadData();
            LoadToComboBox();

            var visibilityManager = new Class.RoleVisibilityManager(_role);
            visibilityManager.SetButtonVisibility(btnDelete, btnAdd, btnChange);

        }

        // Загружает данные из базы в DataSet и привязывает к ListView.
        private void LoadData()
        {
            try
            {
                adapter.FillBy(db.ЗАЯВКА);
                listViewApplication.ItemsSource = db.ЗАЯВКА.DefaultView;
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
                        adapter.Update(db.ЗАЯВКА);
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
            selectedRowView = listViewApplication.SelectedItem as DataRowView;
            return selectedRowView != null;
        }

        // Открывает страницу добавления или изменения записи.
        private void OpenPage(bool isChangeOrAdd, DataRowView rowView = null)
        {
            diplom_loskutova.Page.AddOrChange.ApplicationsAOC page;
            if (rowView != null)
                page = new diplom_loskutova.Page.AddOrChange.ApplicationsAOC(rowView);
            else
                page = new diplom_loskutova.Page.AddOrChange.ApplicationsAOC();

            page.ChangeOrAdd = isChangeOrAdd;
            page.DataChanged += (s, ev) => LoadData();
            NavigationService.Navigate(page);
        }

        private void ApplyFilter()
        {
            if (db.ЗАЯВКА.Rows.Count == 0)
            {
                listViewApplication.ItemsSource = null;
                return;
            }

            List<string> filters = new List<string>();

            if (ComboBoxSearchFIO.SelectedValue != null)
            {
                filters.Add($"ID_Гражданина = {ComboBoxSearchFIO.SelectedValue}");
            }

            if (ComboBoxSearchStatus.SelectedValue != null)
            {
                filters.Add($"ID_Статуса = {ComboBoxSearchStatus.SelectedValue}");
            }

            if (ComboBoxSearchEvent.SelectedValue != null)
            {
                filters.Add($"ID_Мероприятия = {ComboBoxSearchEvent.SelectedValue}");
            }

            if (DatePickerSearch.SelectedDate.HasValue)
            {
                string selectedDateStr = DatePickerSearch.SelectedDate.Value.ToString("yyyy-MM-dd");
                filters.Add($"Дата_Создания = '{selectedDateStr}'");
            }

            string filter = string.Join(" AND ", filters);

            DataView dv = db.ЗАЯВКА.DefaultView;
            dv.RowFilter = filter;
            listViewApplication.ItemsSource = dv;
        }


        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxSearchEvent.SelectedIndex = -1;
            ComboBoxSearchFIO.SelectedIndex = -1;
            ComboBoxSearchStatus.SelectedIndex = -1;
            ApplyFilter();
        }

        private void ComboBoxSearchFIO_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();

        }

        private void ComboBoxSearchStatus_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();

        }

        private void ComboBoxSearchEvent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();

        }

        private void DatePickerSearch_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void LoadToComboBox()
        {
            var userAdapter = new DP_2025_LoskutovaDataSetTableAdapters.ГРАЖДАНИНTableAdapter();
            var usersTable = userAdapter.GetData();
            usersTable.Columns.Add("FullName", typeof(string), "Фамилия + ' ' + Имя + ' ' + Отчество");
            ComboBoxHelper.LoadData(ComboBoxSearchFIO, usersTable, "FullName", "ID_Гражданина");

            var statusAdapter = new DP_2025_LoskutovaDataSetTableAdapters.СТАТУСTableAdapter();
            ComboBoxHelper.LoadData(ComboBoxSearchStatus, statusAdapter.GetData(), "Название", "ID_Статуса");

            var eventAdapter = new DP_2025_LoskutovaDataSetTableAdapters.МЕРОПРИЯТИЕTableAdapter();
            ComboBoxHelper.LoadData(ComboBoxSearchEvent, eventAdapter.GetData(), "Название", "ID_Мероприятия");
        }

    }
}