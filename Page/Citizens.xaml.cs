using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace diplom_loskutova.Page
{
    public partial class Citizens : System.Windows.Controls.Page
    {
        private DP_2025_LoskutovaDataSetTableAdapters.ГРАЖДАНИНTableAdapter adapter = new DP_2025_LoskutovaDataSetTableAdapters.ГРАЖДАНИНTableAdapter();
        private DP_2025_LoskutovaDataSet db = new DP_2025_LoskutovaDataSet();   // Объект для работы с данными из базы (DataSet)
        public Citizens()
        {
            InitializeComponent();
            LoadData();
        }

        // Загружает данные из базы в DataSet и привязывает к ListView.
        private void LoadData()
        {
            try
            {
                adapter.Fill(db.ГРАЖДАНИН);
                listViewCitizen.ItemsSource = db.ГРАЖДАНИН.DefaultView;
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


        // Адаптер для связанной таблицы 
        private DP_2025_LoskutovaDataSetTableAdapters.ЗАЯВКАTableAdapter relatedAdapter = new DP_2025_LoskutovaDataSetTableAdapters.ЗАЯВКАTableAdapter();

        // Проверяет выбран ли элемент, удаляет его из DataTable, обновляет базу и перезагружает данные.
        private void BtnDelete(object sender, RoutedEventArgs e)
        {
            if (TryGetSelectedRow(out DataRowView selectedRowView))
            {
                int typeEventId = Convert.ToInt32(selectedRowView["ID_Гражданина"]);

                // Получаем все связанные записи из таблицы мероприятий
                var relatedRows = relatedAdapter.GetData();

                // Проверяем есть ли связанные записи
                bool hasRelated = false;
                foreach (var row in relatedRows)
                {
                    if (row.ID_Гражданина == typeEventId)
                    {
                        hasRelated = true;
                        break;  // если нашли, дальше проверять нет смысла
                    }
                }

                if (hasRelated)
                {
                    var result = MessageBox.Show("Существуют связанные мероприятия. Вы уверены, что хотите удалить эту запись?",
                        "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (result != MessageBoxResult.OK)
                        return; // Отмена удаления
                }

                else
                {
                    var result = MessageBox.Show("Вы уверены что хотите удалить запись ?", "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (result != MessageBoxResult.OK)
                        return; // Отмена удаления
                }

                try
                {
                    adapter.Update(db.ГРАЖДАНИН);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка");
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

        // Универсальный метод для выбора строки из ListView как DataRowView.
        private bool TryGetSelectedRow(out DataRowView selectedRowView)
        {
            selectedRowView = listViewCitizen.SelectedItem as DataRowView;
            return selectedRowView != null;
        }

        private void NavigatePageSelectedRow()
        {
            if (TryGetSelectedRow(out DataRowView selectedRowView))
                OpenPage(true, selectedRowView);
            else
                MessageBox.Show("Выберите строку для редактирования.");
        }

        // Открывает страницу добавления или изменения записи.
        private void OpenPage(bool isChangeOrAdd, DataRowView rowView = null)
        {
            diplom_loskutova.Page.AddOrChange.CitizensAOC page;
            if (rowView != null)
                page = new diplom_loskutova.Page.AddOrChange.CitizensAOC(rowView);
            else
                page = new diplom_loskutova.Page.AddOrChange.CitizensAOC();

            page.ChangeOrAdd = isChangeOrAdd;
            page.DataChanged += (s, ev) => LoadData();
            NavigationService.Navigate(page);
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            DatePickerSearch.Text = "";
            TextBoxSearchFIO.Text = "";             
            TextBoxSearchTelephone.Text = "";          
            ApplyFilter();                          
        }
        private void ApplyFilter()
        {
            if (db.ГРАЖДАНИН.Rows.Count == 0)
            {
                listViewCitizen.ItemsSource = null;
                return;
            }

            string filter = "";

            // Фильтр по ФИО
            var fio = TextBoxSearchFIO.Text.Trim();
            if (!string.IsNullOrEmpty(fio))
            {
                if (filter.Length > 0)
                    filter += " AND ";
                filter += $"(Фамилия LIKE '%{fio}%' OR Имя LIKE '%{fio}%' OR Отчество LIKE '%{fio}%')";
            }

            // Фильтр по ФИО
            var telephone = TextBoxSearchTelephone.Text.Trim();
            if (!string.IsNullOrEmpty(telephone))
            {
                if (filter.Length > 0)
                    filter += " AND ";
                filter += $"(Контактный_Телефон LIKE '%{telephone}%')";
            }

            // Фильтр по дате
            if (DatePickerSearch.SelectedDate.HasValue)
            {
                if (filter.Length > 0)
                    filter += " AND ";
                string selectedDateStr = DatePickerSearch.SelectedDate.Value.ToString("yyyy-MM-dd");
                filter += $"Дата_Рождения = #{selectedDateStr}#";
            }

            DataView dv = db.ГРАЖДАНИН.DefaultView;
            dv.RowFilter = filter;
            listViewCitizen.ItemsSource = dv;
        }

        private void DatePickerSearch_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void TextBoxSearchTelephone_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void TextBoxSearchFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }
    }
}