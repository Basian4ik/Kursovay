using System;
using System.Windows;
using System.Windows.Controls;

namespace Поликлиника
{
    public partial class AddHistoryWindow : Window
    {
        private Пациенты _patient;
        private Врачи _doctor;

        public AddHistoryWindow(Пациенты patient, Врачи doctor)
        {
            InitializeComponent();
            _patient = patient;
            _doctor = doctor;

            // Врач уже определён — только показываем, изменить нельзя
            DoctorTB.Text = $"{_doctor.Фамилия} {_doctor.Имя} {_doctor.Отчество}";

            TreatmentTypeCombo.SelectedIndex = 0;
            DateDP.SelectedDate = DateTime.Today;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DateDP.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату заболевания.");
                return;
            }

            if (RecoveryDateDP.SelectedDate != null &&
                RecoveryDateDP.SelectedDate.Value < DateDP.SelectedDate.Value)
            {
                MessageBox.Show("Дата выздоровления не может быть раньше даты заболевания.");
                return;
            }

            if (string.IsNullOrWhiteSpace(DiagnosisTB.Text))
            {
                MessageBox.Show("Укажите диагноз.");
                return;
            }

            if (TreatmentTypeCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите вид лечения.");
                return;
            }

            var context = ПоликлиникаEntities.GetContext();

            var newHistory = new История_болезней
            {
                ID_пациента = _patient.ID_пациента,
                Дата_заболевания = DateDP.SelectedDate.Value,
                Дата_выздоровления = RecoveryDateDP.SelectedDate,
                Диагноз = DiagnosisTB.Text.Trim(),
                Вид_лечения = ((ComboBoxItem)TreatmentTypeCombo.SelectedItem).Content.ToString(),
                ID_врача = _doctor.ID_врача
            };

            context.История_болезней.Add(newHistory);
            context.SaveChanges();

            DialogResult = true;
            Close();
        }
    }
}