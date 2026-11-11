using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Поликлиника
{
    public partial class AddPatientWindow : Window
    {
        public AddPatientWindow()
        {
            InitializeComponent();
            GenderCombo.SelectedIndex = 0;
            BirthDateDP.SelectedDate = new DateTime(1990, 1, 1);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // --- 1. ФАМИЛИЯ ---
            if (!IsValidName(SurnameTB.Text, "Фамилия", 100)) return;

            // --- 2. ИМЯ ---
            if (!IsValidName(NameTB.Text, "Имя", 100)) return;

            // --- 3. ОТЧЕСТВО (необязательное, но если введено — проверяем) ---
            if (!string.IsNullOrWhiteSpace(PatronymicTB.Text))
            {
                if (!IsValidName(PatronymicTB.Text, "Отчество", 100)) return;
            }

            // --- 4. ДАТА РОЖДЕНИЯ ---
            if (BirthDateDP.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату рождения.");
                return;
            }
            if (BirthDateDP.SelectedDate.Value > DateTime.Today)
            {
                MessageBox.Show("Дата рождения не может быть в будущем.");
                return;
            }

            // --- 5. ПОЛ ---
            if (GenderCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите пол.");
                return;
            }

            // --- 6. ГОРОД ---
            if (!IsValidName(CityTB.Text, "Город", 100)) return;

            // --- 7. УЛИЦА (до 200 символов, можно цифры и точки) ---
            string street = StreetTB.Text.Trim();
            if (string.IsNullOrWhiteSpace(street))
            {
                MessageBox.Show("Укажите улицу.");
                return;
            }
            if (street.Length > 200)
            {
                MessageBox.Show("Название улицы не должно превышать 200 символов.");
                return;
            }
            if (!Regex.IsMatch(street, @"^[А-Яа-яЁёA-Za-z0-9\s\.\-]+$"))
            {
                MessageBox.Show("Улица может содержать только буквы, цифры, пробелы, точки и дефисы.");
                return;
            }

            // --- 8. ДОМ (до 20 символов) ---
            string house = HouseTB.Text.Trim();
            if (string.IsNullOrWhiteSpace(house))
            {
                MessageBox.Show("Укажите дом.");
                return;
            }
            if (house.Length > 20)
            {
                MessageBox.Show("Номер дома не должен превышать 20 символов.");
                return;
            }
            if (!Regex.IsMatch(house, @"^[А-Яа-яЁёA-Za-z0-9\-]+$"))
            {
                MessageBox.Show("Дом может содержать только буквы, цифры и дефисы.");
                return;
            }

            // ==========================================
            // ВСЕ ПРОВЕРКИ ПРОЙДЕНЫ - СОХРАНЯЕМ В БД
            // ==========================================
            var context = ПоликлиникаEntities.GetContext();
            string genderChar = GenderCombo.SelectedIndex == 0 ? "М" : "Ж";

            var newPatient = new Пациенты
            {
                Фамилия = SurnameTB.Text.Trim(),
                Имя = NameTB.Text.Trim(),
                Отчество = string.IsNullOrWhiteSpace(PatronymicTB.Text) ? null : PatronymicTB.Text.Trim(),
                Дата_рождения = BirthDateDP.SelectedDate.Value,
                Пол = genderChar,
                Город = CityTB.Text.Trim(),
                Улица = street,
                Дом = house
            };

            context.Пациенты.Add(newPatient);
            context.SaveChanges();

            DialogResult = true;
            Close();
        }

        // Вспомогательный метод для проверки ФИО и Города (только буквы, пробел, дефис)
        private bool IsValidName(string text, string fieldName, int maxLength)
        {
            string trimmed = text.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                MessageBox.Show($"Поле '{fieldName}' обязательно для заполнения.");
                return false;
            }

            if (trimmed.Length > maxLength)
            {
                MessageBox.Show($"Поле '{fieldName}' не должно превышать {maxLength} символов.");
                return false;
            }

            // Разрешаем русские/английские буквы, пробел и дефис
            if (!Regex.IsMatch(trimmed, @"^[А-Яа-яЁёA-Za-z\s\-]+$"))
            {
                MessageBox.Show($"Поле '{fieldName}' может содержать только буквы, пробелы и дефисы.");
                return false;
            }

            return true;
        }
    }
}