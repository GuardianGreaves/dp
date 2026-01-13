using ScottPlot;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace diplom_loskutova.Page
{
    public partial class Citizens : System.Windows.Controls.Page
    {
        private DP_2025_LoskutovaDataSetTableAdapters.ГРАЖДАНИНTableAdapter adapter = new DP_2025_LoskutovaDataSetTableAdapters.ГРАЖДАНИНTableAdapter();
        private DP_2025_LoskutovaDataSet db = new DP_2025_LoskutovaDataSet();   // Объект для работы с данными из базы (DataSet)
        public Citizens(string _role)
        {
            InitializeComponent();
            LoadUserStats();
            LoadData();

            var visibilityManager = new Class.RoleVisibilityManager(_role);
            visibilityManager.SetButtonVisibility(btnDelete, btnAdd, btnChange);
        }

        private void LoadUserStats()
        {
            try
            {
                // Загружаем ПОЛЬЗОВАТЕЛЬ
                adapter.Fill(db.ГРАЖДАНИН);

                // 1. Всего пользователей
                tbTotalCitizen.Text = db.ПОЛЬЗОВАТЕЛЬ.Count.ToString();

                listViewCitizen.ItemsSource = db.ПОЛЬЗОВАТЕЛЬ.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }




            // Создаем DataTable с агрегированными данными
            DataTable dtCitizen = new DataTable();

            // Запрос со всеми ролями в одном запросе
            string sqlRoles = @"
    SELECT 
        ISNULL([40-50], 0) as [40-50],
        ISNULL([51-60], 0) as [51-60], 
        ISNULL([61-70], 0) as [61-70],
        ISNULL([71-80], 0) as [71-80],
        ISNULL([81-90], 0) as [81-90],
        ISNULL([91-100], 0) as [91-100],
        ISNULL([Другие], 0) as [Другие]
    FROM (
        SELECT 
            CASE 
                WHEN DATEDIFF(YEAR, Дата_Рождения, GETDATE()) BETWEEN 40 AND 50 THEN '40-50'
                WHEN DATEDIFF(YEAR, Дата_Рождения, GETDATE()) BETWEEN 51 AND 60 THEN '51-60'
                WHEN DATEDIFF(YEAR, Дата_Рождения, GETDATE()) BETWEEN 61 AND 70 THEN '61-70'
                WHEN DATEDIFF(YEAR, Дата_Рождения, GETDATE()) BETWEEN 71 AND 80 THEN '71-80'
                WHEN DATEDIFF(YEAR, Дата_Рождения, GETDATE()) BETWEEN 81 AND 90 THEN '81-90'
                WHEN DATEDIFF(YEAR, Дата_Рождения, GETDATE()) BETWEEN 91 AND 100 THEN '91-100'
                ELSE 'Другие'
            END as Возрастная_группа,
            1 as cnt
        FROM [dbo].[ГРАЖДАНИН]
    ) AS SourceTable
    PIVOT (
        COUNT(cnt) 
        FOR Возрастная_группа IN ([40-50], [51-60], [61-70], [71-80], [81-90], [91-100], [Другие])
    ) AS PivotTable";

            SqlDataAdapter adapter2 = new SqlDataAdapter(sqlRoles, ConfigurationManager.ConnectionStrings["diplom_loskutova.Properties.Settings.DP_2025_LoskutovaConnectionString"].ConnectionString);
            adapter2.Fill(dtCitizen);

            // Безопасное чтение значений
            int age1 = dtCitizen.Rows.Count > 0 ? Convert.ToInt32(dtCitizen.Rows[0]["40-50"]) : 0;
            int age2 = dtCitizen.Rows.Count > 0 ? Convert.ToInt32(dtCitizen.Rows[0]["51-60"]) : 0;
            int age3 = dtCitizen.Rows.Count > 0 ? Convert.ToInt32(dtCitizen.Rows[0]["61-70"]) : 0;
            int age4 = dtCitizen.Rows.Count > 0 ? Convert.ToInt32(dtCitizen.Rows[0]["71-80"]) : 0;
            int age5 = dtCitizen.Rows.Count > 0 ? Convert.ToInt32(dtCitizen.Rows[0]["81-90"]) : 0;
            int age6 = dtCitizen.Rows.Count > 0 ? Convert.ToInt32(dtCitizen.Rows[0]["91-100"]) : 0;
            int age7 = dtCitizen.Rows.Count > 0 ? Convert.ToInt32(dtCitizen.Rows[0]["Другие"]) : 0;

            double[] values = { age1, age2, age3, age4, age5, age6, age7 };
            var barPlot = WpfPlot1.Plot.Add.Bars(values);

            // bars may be styled after they have been added
            barPlot.Bars[0].FillColor = Colors.Orange;
            barPlot.Bars[1].FillColor = Colors.Green;
            barPlot.Bars[2].FillColor = Colors.Navy;

            barPlot.Bars[0].FillHatch = new ScottPlot.Hatches.Striped();
            barPlot.Bars[1].FillHatch = new ScottPlot.Hatches.Dots();
            barPlot.Bars[2].FillHatch = new ScottPlot.Hatches.Checker();

            foreach (var bar in barPlot.Bars)
            {
                bar.LineWidth = 2;
                bar.LineColor = bar.FillColor.Darken(0.5);
                bar.FillHatchColor = bar.FillColor.Lighten(0.1);
            }

            // tell the plot to autoscale with no padding beneath the bars
            WpfPlot1.Plot.Axes.Margins(bottom: 0);

            WpfPlot1.Refresh();
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