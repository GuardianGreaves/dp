using diplom_lib_loskutova.Export;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace diplom_loskutova
{
    public partial class UserControlReport : UserControl
    {
        private readonly string queryReport1 = @"
        SELECT 
            г.Фамилия + ' ' + г.Имя AS Гражданин,
            COUNT(z.ID_Заявки) AS [Количество заявок]
        FROM dbo.ГРАЖДАНИН г
        LEFT JOIN dbo.ЗАЯВКА z ON г.ID_Гражданина = z.ID_Гражданина
        GROUP BY г.Фамилия, г.Имя
        ORDER BY [Количество заявок] DESC;";

        private readonly string queryReport2 = @"
        SELECT 
            м.[Название] AS Название_Мероприятия,
            г.[Фамилия] + ' ' + г.[Имя] + ' ' + г.[Отчество] AS ФИО_Гражданина
        FROM [ФИКСАЦИЯ_ЯВКИ] ф
        LEFT JOIN [МЕРОПРИЯТИЕ] м ON ф.[ID_Мероприятия] = м.[ID_Мероприятия]
        LEFT JOIN [ГРАЖДАНИН] г ON ф.[ID_Гражданина] = г.[ID_Гражданина]";

        private readonly string queryReport3 = @"
        SELECT 
            m.Название,
            t.Название AS [Тип мероприятия],
            p.Имя + ' ' + p.Фамилия AS Пользователь,
            m.Описание,
            m.Дата_Мероприятия AS Дата,
            m.Бюджет
        FROM dbo.МЕРОПРИЯТИЕ m
        LEFT JOIN dbo.ТИП_МЕРОПРИЯТИЯ t ON m.ID_Типа = t.ID_Типа
        LEFT JOIN dbo.ПОЛЬЗОВАТЕЛЬ p ON m.ID_Пользователя = p.ID_Пользователя
        WHERE m.Дата_Мероприятия >= CAST(GETDATE() AS DATE)
        ORDER BY m.Дата_Мероприятия";

        private readonly string queryReport3_2 = @"
        SELECT 
            m.Название,
            t.Название AS [Тип мероприятия],
            p.Имя + ' ' + p.Фамилия AS Пользователь,
            m.Описание,
            m.Дата_Мероприятия AS Дата,
            m.Бюджет
        FROM dbo.МЕРОПРИЯТИЕ m
        LEFT JOIN dbo.ТИП_МЕРОПРИЯТИЯ t ON m.ID_Типа = t.ID_Типа
        LEFT JOIN dbo.ПОЛЬЗОВАТЕЛЬ p ON m.ID_Пользователя = p.ID_Пользователя
        WHERE m.Дата_Мероприятия >= @ДатаНачала
        ORDER BY m.Дата_Мероприятия";


        private readonly string queryReport4 = @"
        SELECT 
            С.Название AS Статус,
            CAST(SUM(CASE WHEN З.ID_Статуса = С.ID_Статуса THEN 1 ELSE 0 END) AS FLOAT) / 
            (SELECT COUNT(*) FROM ЗАЯВКА) AS ДоляЗаявок
        FROM СТАТУС С
        LEFT JOIN ЗАЯВКА З ON З.ID_Статуса = С.ID_Статуса
        GROUP BY С.Название";

        private readonly string queryReport5 = @"
        SELECT 
            СТАТУС.Название,
            ROUND(CAST(COUNT(ЗАЯВКА.ID_Заявки) AS FLOAT) / (SELECT COUNT(*) FROM [ЗАЯВКА]) * 100, 2) AS Процент
        FROM [ЗАЯВКА]
        JOIN [СТАТУС] ON ЗАЯВКА.ID_Статуса = СТАТУС.ID_Статуса
        GROUP BY СТАТУС.Название
        ORDER BY Процент DESC";

        private readonly string queryReport6 = @"
        SELECT 
            ГРАЖДАНИН.Фамилия + ' ' + ГРАЖДАНИН.Имя + ' ' + ГРАЖДАНИН.Отчество AS ФИО,
            COUNT(*) AS Всего_заявок,
            SUM(CASE WHEN ЗАЯВКА.ID_Статуса = 4 THEN 1 ELSE 0 END) AS Выполнено_услуг,
            ROUND(
                CAST(SUM(CASE WHEN ЗАЯВКА.ID_Статуса = 4 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100, 2
            ) AS Доля_выполненных_услуг_в_процентах
        FROM [ЗАЯВКА]
        JOIN [ГРАЖДАНИН] ON ЗАЯВКА.ID_Гражданина = ГРАЖДАНИН.ID_Гражданина
        GROUP BY ГРАЖДАНИН.Фамилия, ГРАЖДАНИН.Имя, ГРАЖДАНИН.Отчество
        ORDER BY Доля_выполненных_услуг_в_процентах DESC";

        private string currentSqlQuery;
        public UserControlReport(int numberReport)
        {
            InitializeComponent();
            InitializeReport(numberReport);
        }

        private void LoadDataGrid(DateTime датаНачала)
        {
            string query = queryReport3_2;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["diplom_loskutova.Properties.Settings.DP_2025_LoskutovaConnectionString"].ConnectionString); // Полная типизация
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ДатаНачала", датаНачала.Date);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            conn.Open();
            adapter.Fill(dt);
            conn.Close();

            dataGridReport.ItemsSource = dt.DefaultView;
        }

        private void InitializeReport(int _numberReport)
        {
            DbHelper dbHelper = new DbHelper();
            DataTable dt;

            switch (_numberReport)
            {
                case 1:
                    NameReport.Text = "Уровень активности граждан";
                    currentSqlQuery = queryReport1;
                    break;
                case 2:
                    NameReport.Text = "Реестр участников мероприятий";
                    currentSqlQuery = queryReport2;
                    break;
                case 3:
                    currentSqlQuery = queryReport3;
                    NameReport.Text = "Список текущих мероприятий";
                    filter.Visibility = Visibility.Visible;
                    break;
                case 4:
                    NameReport.Text = "Статистика удовлетворенности заявок";
                    currentSqlQuery = queryReport4;
                    break;
                case 5:
                    NameReport.Text = "Анализ персональных заявок";
                    currentSqlQuery = queryReport5;
                    break;
                case 6:
                    NameReport.Text = "Статистика предоставленных услуг";
                    currentSqlQuery = queryReport6;
                    break;
                default:
                    currentSqlQuery = "";
                    filter.Visibility = Visibility.Collapsed;
                    break;
            }

            if (!string.IsNullOrEmpty(currentSqlQuery))
            {
                dt = dbHelper.ExecuteQuery(currentSqlQuery);
                FillListViewWPF(dataGridReport, dt);
            }
        }

        public void FillListViewWPF(System.Windows.Controls.DataGrid listView, DataTable dt)
        {
            listView.ItemsSource = dt.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExportWord exportWord = new ExportWord();
            exportWord.export(currentSqlQuery, ""); // передаем текущий SQL-запрос
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ExportExcel exportExcel = new ExportExcel();
            exportExcel.export(currentSqlQuery); // передаем текущий SQL-запрос
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == true)
            {
                // Создаем FlowDocument для форматированной таблицы
                FlowDocument doc = new FlowDocument();
                Table table = new Table();
                table.CellSpacing = 0;
                table.Margin = new Thickness(10); // Отступы таблицы внутри полей

                // Настройка документа под страницу
                doc.PagePadding = new Thickness(20);
                double pageWidth = printDlg.PrintableAreaWidth -
                                  doc.PagePadding.Left - doc.PagePadding.Right -
                                  table.Margin.Left - table.Margin.Right;
                doc.ColumnWidth = pageWidth;

                // Заголовки столбцов
                TableRowGroup header = new TableRowGroup();
                TableRow headerRow = new TableRow();
                foreach (var col in dataGridReport.Columns)
                {
                    // Пропорциональная ширина колонок от DataGrid
                    double colWidth = col.ActualWidth / dataGridReport.ActualWidth;
                    table.Columns.Add(new TableColumn()
                    {
                        Width = new GridLength(colWidth, GridUnitType.Star)
                    });

                    TableCell cell = new TableCell(new Paragraph(new Run(col.Header.ToString())))
                    {
                        Background = System.Windows.Media.Brushes.LightGray,
                        FontWeight = FontWeights.Bold,
                        BorderBrush = System.Windows.Media.Brushes.Black,
                        BorderThickness = new Thickness(0.5),
                        Padding = new Thickness(3),
                        FontSize = 10,
                        TextAlignment = TextAlignment.Center
                    };
                    headerRow.Cells.Add(cell);
                }
                header.Rows.Add(headerRow);
                table.RowGroups.Add(header);

                // Данные из DataGrid
                TableRowGroup body = new TableRowGroup();
                foreach (var item in dataGridReport.Items)
                {
                    if (item is DataRowView rowView)
                    {
                        TableRow dataRow = new TableRow();
                        for (int colIndex = 0; colIndex < dataGridReport.Columns.Count; colIndex++)
                        {
                            string cellValue = rowView.Row.ItemArray[colIndex]?.ToString() ?? "";
                            TableCell cell = new TableCell(new Paragraph(new Run(cellValue)))
                            {
                                BorderBrush = System.Windows.Media.Brushes.Black,
                                BorderThickness = new Thickness(0.5),
                                Padding = new Thickness(2),
                                FontSize = 9,
                                TextAlignment = TextAlignment.Left
                            };
                            dataRow.Cells.Add(cell);
                        }
                        body.Rows.Add(dataRow);
                    }
                }
                table.RowGroups.Add(body);
                doc.Blocks.Add(table);

                // Печать документа
                IDocumentPaginatorSource paginator = doc;
                printDlg.PrintDocument(paginator.DocumentPaginator, "Данные из DataGrid");
            }
        }

        private void filterDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDataGrid(filterDate.SelectedDate ?? DateTime.Now.Date);
        }
    }
}