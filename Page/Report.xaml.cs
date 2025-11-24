using System;
using System.Data;
using System.Windows;

using diplom_lib_loskutova.Export;

namespace diplom_loskutova.Page
{
    public partial class Report : System.Windows.Controls.Page
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
        SELECT *
        FROM dbo.МЕРОПРИЯТИЕ
        WHERE Дата_Мероприятия >= CAST(GETDATE() AS DATE)";

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
        public Report(int numberReport)
        {
            InitializeComponent();
            InitializeReport(numberReport);
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
                    NameReport.Text = "Список текущих мероприятий";
                    currentSqlQuery = queryReport3;
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
    }
}