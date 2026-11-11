using System;
using System.Windows;

namespace Поликлиника
{
    public partial class AddOperationWindow : Window
    {
        private История_болезней _history;
        private Врачи _doctor;

        public AddOperationWindow(История_болезней history, Врачи doctor)
        {
            InitializeComponent();
            _history = history;
            _doctor = doctor;

            // Врач уже определён (авторизованный пользователь)
            DoctorTB.Text = $"{_doctor.Фамилия} {_doctor.Имя} {_doctor.Отчество}";

            DateDP.SelectedDate = DateTime.Today;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DateDP.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату операции.");
                return;
            }

            if (string.IsNullOrWhiteSpace(DescriptionTB.Text))
            {
                MessageBox.Show("Укажите описание операции.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ResultTB.Text))
            {
                MessageBox.Show("Укажите результат операции.");
                return;
            }

            var context = ПоликлиникаEntities.GetContext();

            var newOperation = new Операции
            {
                ID_истории = _history.ID_истории,
                ID_врача = _doctor.ID_врача,
                Дата_операции = DateDP.SelectedDate.Value,
                Описание_операции = DescriptionTB.Text.Trim(),
                Результат_операции = ResultTB.Text.Trim()
            };

            context.Операции.Add(newOperation);
            context.SaveChanges();

            DialogResult = true;
            Close();
        }
    }
}