using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace diplom_loskutova.Page.AddOrChange
{

    public partial class CitizensAOC : System.Windows.Controls.Page
    {
        private string fileName;
        private string fullPath;

        // DataSet для работы с таблицей 
        private DP_2025_LoskutovaDataSet db = new DP_2025_LoskutovaDataSet();

        // События для уведомления об изменениях данных и открытии страницы 
        public event EventHandler DataChanged;
        public event EventHandler OpenPageTypeEvent;

        // Флаг для определения операции: изменение (true) или добавление (false)
        public bool ChangeOrAdd = false;

        // Представление текущей строки данных, редактируемой на форме
        private DataRowView currentRow = null;

        // Адаптер для работы с таблицей
        private DP_2025_LoskutovaDataSetTableAdapters.ГРАЖДАНИНTableAdapter adapter = new DP_2025_LoskutovaDataSetTableAdapters.ГРАЖДАНИНTableAdapter();

        // Конструктор страницы с передачей строки таблицы для редактирования.
        // Если строка передана, заполняет поля формы значениями из нее.
        public CitizensAOC(DataRowView row = null)
        {
            InitializeComponent();
            if (row != null)
            {
                ChangeOrAdd = true;
                SetCurrentRow(row); // Заполнение формы данными строки
            }
            else
            {
                ChangeOrAdd = false;
                NamePage.Text = "Добавление записи в таблицу \"Граждане\"";
            }
        }

        // Заполнить форму по переданной строке данных
        private void SetCurrentRow(DataRowView row)
        {
            currentRow = row;
            DatePickerBirthday.Text = currentRow["Дата_Рождения"].ToString();
            TextBoxTelephone.Text = currentRow["Контактный_Телефон"].ToString();
            TextBoxName.Text = currentRow["Имя"].ToString();
            TextBoxSurname.Text = currentRow["Фамилия"].ToString();
            TextBoxPatronymic.Text = currentRow["Отчество"].ToString();
            fileName = currentRow["Фото"].ToString();
            fullPath = System.IO.Path.Combine(baseDirectory, photosSubdirectory, fileName);
            PhotoImage.Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            NamePage.Text = $"Редактирование записи №{Convert.ToInt32(currentRow["ID_Гражданина"])}";
        }

        // Проверка на дубликат записи
        private bool ExistsDuplicate(string telephone)
        {
            var existingRecords = adapter.GetData()
                    .Where(row => row.Контактный_Телефон == telephone);

            // При редактировании исключаем текущую запись по ее уникальному идентификатору, если он есть
            if (ChangeOrAdd && currentRow != null)
            {
                existingRecords = existingRecords.Where(row => row.ID_Гражданина != Convert.ToInt32(currentRow["ID_Гражданина"]));
            }

            return existingRecords.Any();
        }

        /// Обработчик кнопки сохранения.
        /// При изменении обновляет текущую строку DataRowView,
        /// При добавлении создает новую строку и сохраняет изменения.
        private void BtnSave(object sender, RoutedEventArgs e)
        {
            DateTime birthday = DatePickerBirthday.DisplayDate;
            string telephone = TextBoxTelephone.Text;
            string name = TextBoxName.Text;
            string surname = TextBoxSurname.Text;
            string patronymic = TextBoxPatronymic.Text;
            string photo = fileName;

            if (ExistsDuplicate(telephone))
            {
                MessageBox.Show("Запись с таким номером уже существует.", "Ошибка");
                return;
            }

            try
            {
                if (ChangeOrAdd)
                {
                    // Изменяем выбранную запись
                    currentRow["Дата_Рождения"] = birthday;
                    currentRow["Контактный_Телефон"] = telephone;
                    currentRow["Имя"] = name;
                    currentRow["Фамилия"] = surname;
                    currentRow["Отчество"] = patronymic;
                    currentRow["Фото"] = photo;
                    // Сохраняем изменения в базе
                    adapter.Update((DP_2025_LoskutovaDataSet.ГРАЖДАНИНDataTable)currentRow.DataView.Table);
                }
                else
                {
                    // Создаем новую запись в таблице
                    var newRow = db.ГРАЖДАНИН.NewГРАЖДАНИНRow();
                    newRow.Дата_Рождения = birthday;
                    newRow.Контактный_Телефон = telephone;
                    newRow.Имя = name;
                    newRow.Фамилия = surname;
                    newRow.Отчество = patronymic;
                    newRow.Фото = photo;
                    db.ГРАЖДАНИН.Rows.Add(newRow);

                    // Сохраняем новую запись в базе
                    adapter.Update(db.ГРАЖДАНИН);
                }
                MessageBox.Show("Данные успешно сохранены", "Успех");
                DataChanged?.Invoke(this, EventArgs.Empty); // Вызываем событие, что данные изменились, чтобы загрузить их заново на предыдущей странице
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            NavigationService.GoBack();
        }

        // Кнопка назад, возвращает на предыдущую страницу
        private void BtnBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string photosSubdirectory = "Image";
        string selectedPhotoFullPath;  // полный путь к выбранному файлу

        private void BtnSelectPhoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.png; *.bmp)|*.jpg; *.png; *.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                selectedPhotoFullPath = openFileDialog.FileName;

                string destinationFolder = System.IO.Path.Combine(baseDirectory, photosSubdirectory);
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                // Создаем уникальное имя файла с датой-временем
                string ext = System.IO.Path.GetExtension(selectedPhotoFullPath);
                fileName = $"photo_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";

                string destinationPath = System.IO.Path.Combine(destinationFolder, fileName);

                try
                {
                    File.Copy(selectedPhotoFullPath, destinationPath, true);

                    PhotoImage.Source = new BitmapImage(new Uri(destinationPath));

                    // Сохраняйте destinationPath (или только relative path) в запись/базу
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании или загрузке изображения: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
