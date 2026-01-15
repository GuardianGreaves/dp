using diplom_loskutova.Helpers;
using ScottPlot;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace diplom_loskutova.Page
{
    public partial class Events : System.Windows.Controls.Page
    {
        private DP_2025_LoskutovaDataSetTableAdapters.МЕРОПРИЯТИЕTableAdapter adapter = new DP_2025_LoskutovaDataSetTableAdapters.МЕРОПРИЯТИЕTableAdapter();
        private DP_2025_LoskutovaDataSet db = new DP_2025_LoskutovaDataSet();   // Объект для работы с данными из базы (DataSet)

        public Events(string _role)
        {
            InitializeComponent();
            LoadData();
            LoadToComboBox();
            LoadUserStats();

            var visibilityManager = new Class.RoleVisibilityManager(_role);
            visibilityManager.SetButtonVisibility(btnDelete, btnAdd, btnChange);

        }

        private void LoadUserStats()
        {
            try
            {
                // Загружаем МЕРОПРИЯТИЕ
                adapter.Fill(db.МЕРОПРИЯТИЕ);
                tbTotalEvent.Text = db.МЕРОПРИЯТИЕ.Count.ToString();
                listViewEvents.ItemsSource = db.МЕРОПРИЯТИЕ.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
                return;
            }

            // Строим гистограмму типов мероприятий
            BuildEventTypeHistogram();
        }

        private void BuildEventTypeHistogram()
        {
            var eventData = GetEventTypeStatistics();

            if (eventData.Rows.Count == 0)
                return;

            // Очищаем график
            WpfPlot1.Plot.Clear();

            // Динамически извлекаем данные
            var values = eventData.AsEnumerable()
                .Select(row => Convert.ToDouble(row["Count"]))
                .ToArray();

            var labels = eventData.AsEnumerable()
                .Select(row => row["TypeName"].ToString())
                .ToArray();

            double[] positions = Enumerable.Range(1, values.Length).Select(x => (double)x).ToArray();

            // Добавляем столбцы динамически
            for (int i = 0; i < values.Length; i++)
            {
                double[] xs = { positions[i] };
                double[] ys = { values[i] };

                var bar = WpfPlot1.Plot.Add.Bars(xs, ys);
                bar.LegendText = labels[i];
            }

            WpfPlot1.Plot.Title("Распределение мероприятий по типам");
            WpfPlot1.Plot.ShowLegend(Alignment.UpperLeft);
            WpfPlot1.Plot.Axes.Margins(bottom: 0);
            WpfPlot1.Refresh();
        }
        private DataTable GetEventTypeStatistics()
        {
            string sql = @"
        SELECT 
            t.Название as TypeName,
            ISNULL(COUNT(m.ID_Мероприятия), 0) as Count
        FROM [dbo].[ТИП_МЕРОПРИЯТИЯ] t
        LEFT JOIN [dbo].[МЕРОПРИЯТИЕ] m ON t.ID_Типа = m.ID_Типа
        GROUP BY t.ID_Типа, t.Название
        ORDER BY t.ID_Типа";

            DataTable dt = new DataTable();
            using (var adapter = new SqlDataAdapter(sql, ConfigurationManager.ConnectionStrings["diplom_loskutova.Properties.Settings.DP_2025_LoskutovaConnectionString"].ConnectionString))
            {
                adapter.Fill(dt);
            }
            return dt;
        }


        // Загружает данные из базы в DataSet и привязывает к ListView.
        private void LoadData()
        {
            try
            {
                adapter.FillBy(db.МЕРОПРИЯТИЕ);
                listViewEvents.ItemsSource = db.МЕРОПРИЯТИЕ.DefaultView;
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
                        adapter.Update(db.МЕРОПРИЯТИЕ);
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

        // Универсальный метод для выбора строки из ListView как DataRowView.
        private bool TryGetSelectedRow(out DataRowView selectedRowView)
        {
            selectedRowView = listViewEvents.SelectedItem as DataRowView;
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
            diplom_loskutova.Page.AddOrChange.EventsAOC page;
            if (rowView != null)
                page = new diplom_loskutova.Page.AddOrChange.EventsAOC(rowView);
            else
                page = new diplom_loskutova.Page.AddOrChange.EventsAOC();

            page.ChangeOrAdd = isChangeOrAdd;
            page.DataChanged += (s, ev) => LoadData();
            NavigationService.Navigate(page);
        }

        private void ApplyFilter()
        {
            if (db.МЕРОПРИЯТИЕ.Rows.Count == 0)
            {
                listViewEvents.ItemsSource = null;
                return;
            }

            string filter = "";

            // Фильтр по ФИО
            var fio = ComboBoxSearchFIO.Text.Trim();
            if (!string.IsNullOrEmpty(fio))
            {
                if (filter.Length > 0)
                    filter += " AND ";
                filter += $"(Фамилия LIKE '%{fio}%' OR Имя LIKE '%{fio}%' OR Отчество LIKE '%{fio}%')";
            }

            if (ComboBoxSearchTypeEvent.SelectedValue != null)
            {
                filter += $"ID_Типа = {ComboBoxSearchTypeEvent.SelectedValue}";
            }

            // Фильтр по логину
            var login = TextBoxSearchName.Text.Trim();
            if (!string.IsNullOrEmpty(login))
            {
                if (filter.Length > 0)
                    filter += " AND ";
                filter += $"Название LIKE '%{login}%'";
            }

            // Фильтр по дате
            if (DatePickerSearchDate.SelectedDate.HasValue)
            {
                if (filter.Length > 0)
                    filter += " AND ";
                string selectedDateStr = DatePickerSearchDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                filter += $"Дата_Мероприятия = #{selectedDateStr}#";
            }

            // Фильтр по бюджету
            if (decimal.TryParse(TextBoxMinBudget.Text.Trim(), out decimal minBudget))
            {
                if (filter.Length > 0)
                    filter += " AND ";
                filter += $"Бюджет >= {minBudget.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            }

            if (decimal.TryParse(TextBoxMaxBudget.Text.Trim(), out decimal maxBudget))
            {
                if (filter.Length > 0)
                    filter += " AND ";
                filter += $"Бюджет <= {maxBudget.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            }

            DataView dv = db.МЕРОПРИЯТИЕ.DefaultView;
            dv.RowFilter = filter;
            listViewEvents.ItemsSource = dv;
        }

        private void ComboBoxSearchTypeEvent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void TextBoxSearchName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void TextBoxSearchDate_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ComboBoxSearchDate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxSearchTypeEvent.SelectedIndex = -1;
            ComboBoxSearchFIO.SelectedIndex = -1;
            TextBoxSearchName.Text = "";
            DatePickerSearchDate.SelectedDate = null;
            TextBoxMinBudget.Text = "";
            TextBoxMaxBudget.Text = "";
            ApplyFilter();
        }

        private void ComboBoxSearchFIO_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ComboBoxSearchMoney_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void TextBoxMinBudget_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void TextBoxMaxBudget_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter();
        }
        private void DatePickerSearchDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void LoadToComboBox()
        {
            var userAdapter = new DP_2025_LoskutovaDataSetTableAdapters.ПОЛЬЗОВАТЕЛЬTableAdapter();

            var usersTable = userAdapter.GetData();
            usersTable.Columns.Add("FullName", typeof(string), "Фамилия + ' ' + Имя + ' ' + Отчество");
            ComboBoxHelper.LoadData(ComboBoxSearchFIO, usersTable, "FullName", "ID_Пользователя");

            var typeEventAdapter = new DP_2025_LoskutovaDataSetTableAdapters.ТИП_МЕРОПРИЯТИЯTableAdapter();
            ComboBoxHelper.LoadData(ComboBoxSearchTypeEvent, typeEventAdapter.GetData(), "Название", "ID_Типа");
        }
    }
}