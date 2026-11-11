using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Поликлиника
{
    public partial class AddTreatmentWindow : Window
    {
        private История_болезней _history;

        public AddTreatmentWindow(История_болезней history)
        {
            InitializeComponent();
            _history = history;

            DateDP.SelectedDate = DateTime.Today;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка даты
            if (DateDP.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату лечения.");
                return;
            }

            // 2. Проверка температуры
            if (string.IsNullOrWhiteSpace(TemperatureTB.Text))
            {
                MessageBox.Show("Укажите температуру.");
                return;
            }

            decimal temperature;
            string tempText = TemperatureTB.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(tempText, NumberStyles.Number, CultureInfo.InvariantCulture, out temperature))
            {
                MessageBox.Show("Температура должна быть числом (например, 36.6).");
                return;
            }

            // Ограничение БД: Температура от 32.0 до 42.0
            if (temperature < 32.0m || temperature > 42.0m)
            {
                MessageBox.Show("Температура должна быть в пределах от 32.0 до 42.0.");
                return;
            }

            // 3. Проверка давления
            if (string.IsNullOrWhiteSpace(PressureTB.Text))
            {
                MessageBox.Show("Укажите давление.");
                return;
            }

            string pressure = PressureTB.Text.Trim();

            // Ограничение БД: Давление должно содержать '/' (формат 120/80)
            if (!pressure.Contains("/"))
            {
                MessageBox.Show("Давление должно быть указано в формате 'верхнее/нижнее' (например, 120/80).");
                return;
            }

            // Дополнительная защита: проверяем, что до и после дроби есть числа
            if (!Regex.IsMatch(pressure, @"^\d+/\d+$"))
            {
                MessageBox.Show("Давление должно состоять только из цифр и символа '/' (например, 120/80).");
                return;
            }

            // 4. Проверка лекарств
            if (string.IsNullOrWhiteSpace(MedicinesTB.Text))
            {
                MessageBox.Show("Укажите лекарства.");
                return;
            }

            // 5. Проверка состояния больного
            if (string.IsNullOrWhiteSpace(StateTB.Text))
            {
                MessageBox.Show("Укажите состояние больного.");
                return;
            }

            // Все проверки пройдены — сохраняем
            var context = ПоликлиникаEntities.GetContext();

            var newTreatment = new Лист_лечения
            {
                ID_истории = _history.ID_истории,
                Дата_лечения = DateDP.SelectedDate.Value,
                Температура = temperature,
                Давление = pressure,
                Лекарства = MedicinesTB.Text.Trim(),
                Состояние_больного = StateTB.Text.Trim()
            };

            context.Лист_лечения.Add(newTreatment);
            context.SaveChanges();

            DialogResult = true;
            Close();
        }
    }
}