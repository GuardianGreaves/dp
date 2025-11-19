using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace diplom_loskutova.Page
{
    public partial class Report : System.Windows.Controls.Page
    {
        public event EventHandler OpenPageReports;

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
                    // Используется строка подключения из App.config (MyConnectionString)
                    dt = dbHelper.ExecuteQuery(@"
                        SELECT 
                            г.Фамилия + ' ' + г.Имя AS Гражданин,
                            COUNT(z.ID_Заявки) AS [Количество заявок]
                        FROM 
                            dbo.ГРАЖДАНИН г
                        LEFT JOIN 
                            dbo.ЗАЯВКА z ON г.ID_Гражданина = z.ID_Гражданина
                        GROUP BY 
                            г.Фамилия, г.Имя
                        ORDER BY 
                            [Количество заявок] DESC;");
                    FillListViewWPF(dataGridReport, dt);
                    break;
                case 2:
                    NameReport.Text = "Реестр участников мероприятий";
                    // Используется строка подключения из App.config (MyConnectionString)
                    dt = dbHelper.ExecuteQuery(@"
                        SELECT 
                            м.[Название] AS Название_Мероприятия,
                            г.[Фамилия] + ' ' + г.[Имя] + ' ' + г.[Отчество] AS ФИО_Гражданина
                        FROM [ФИКСАЦИЯ_ЯВКИ] ф
                        LEFT JOIN [МЕРОПРИЯТИЕ] м
                            ON ф.[ID_Мероприятия] = м.[ID_Мероприятия]
                        LEFT JOIN [ГРАЖДАНИН] г
                            ON ф.[ID_Гражданина] = г.[ID_Гражданина]"
                    );
                    FillListViewWPF(dataGridReport, dt);
                    break;
                case 3:
                    NameReport.Text = "Список текущих мероприятий";
                    // Используется строка подключения из App.config (MyConnectionString)
                    dt = dbHelper.ExecuteQuery(@"
                        SELECT *
                        FROM dbo.МЕРОПРИЯТИЕ
                        WHERE Дата_Мероприятия >= CAST(GETDATE() AS DATE)"
                    );
                    FillListViewWPF(dataGridReport, dt);
                    break;
                case 4:
                    NameReport.Text = "Статистика удовлетворенности заявок";
                    // Используется строка подключения из App.config (MyConnectionString)
                    dt = dbHelper.ExecuteQuery(@"
                        SELECT 
                            С.Название AS Статус,
                            CAST(SUM(CASE WHEN З.ID_Статуса = С.ID_Статуса THEN 1 ELSE 0 END) AS FLOAT) / 
                            (SELECT COUNT(*) FROM ЗАЯВКА) AS ДоляЗаявок
                        FROM СТАТУС С
                        LEFT JOIN ЗАЯВКА З ON З.ID_Статуса = С.ID_Статуса
                        GROUP BY С.Название"
                    );
                    FillListViewWPF(dataGridReport, dt);
                    break;
                case 5:
                    NameReport.Text = "Анализ персональных заявок";
                    // Используется строка подключения из App.config (MyConnectionString)
                    dt = dbHelper.ExecuteQuery(@"
                        SELECT 
                            СТАТУС.Название,
                            ROUND(CAST(COUNT(ЗАЯВКА.ID_Заявки) AS FLOAT) / (SELECT COUNT(*) FROM [ЗАЯВКА]) * 100, 2) AS Процент
                        FROM [ЗАЯВКА]
                        JOIN [СТАТУС] ON ЗАЯВКА.ID_Статуса = СТАТУС.ID_Статуса
                        GROUP BY СТАТУС.Название
                        ORDER BY Процент DESC"
                    );
                    FillListViewWPF(dataGridReport, dt);
                    break;
                case 6:
                    NameReport.Text = "Статистика предоставленных услуг";
                    // Используется строка подключения из App.config (MyConnectionString)
                    dt = dbHelper.ExecuteQuery(@"
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
                        ORDER BY Доля_выполненных_услуг_в_процентах DESC"
                    );
                    FillListViewWPF(dataGridReport, dt);
                    break;
                default:
                    break;
            }
        }

        // Пример запроса и заполнения WPF ListView через ItemsSource
        public void FillListViewWPF(System.Windows.Controls.DataGrid listView, DataTable dt)
        {
            listView.ItemsSource = dt.DefaultView;
        }

    }
}
