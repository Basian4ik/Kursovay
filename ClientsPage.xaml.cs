using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Поликлиника
{
    public partial class ClientsPage : Page
    {
        private Врачи _currentUser;

        public ClientsPage(Врачи user)
        {
            InitializeComponent();
            _currentUser = user;
            RoleTB.Text = user.Должность;
            FIOTB.Text = user.Фамилия + " " + user.Имя + " " + user.Отчество;

            // Подписываемся на события поиска/сортировки/фильтрации
            TBoxSearch.TextChanged += (s, e) => UpdatePatients();
            RButtonUp.Checked += (s, e) => UpdatePatients();
            RButtonDown.Checked += (s, e) => UpdatePatients();
            ComboType.SelectionChanged += (s, e) => UpdatePatients();

            ComboType.SelectedIndex = 0; // «Все виды» по умолчанию
            UpdatePatients();
        }

        private void UpdatePatients()
        {
            if (TBoxSearch == null || ComboType == null) return;

            var currentPatients = ПоликлиникаEntities.GetContext().Пациенты.ToList();

            // ---------- ФИЛЬТРАЦИЯ по типу лечения ----------
            if (ComboType.SelectedIndex == 1)
            {
                currentPatients = currentPatients
                    .Where(p => p.История_болезней.Any(l => l.Вид_лечения == "Амбулаторное"))
                    .ToList();
            }
            else if (ComboType.SelectedIndex == 2)
            {
                currentPatients = currentPatients
                    .Where(p => p.История_болезней.Any(l => l.Вид_лечения == "Стационарное"))
                    .ToList();
            }

            // ---------- ПОИСК по фамилии / имени / отчеству ----------
            string search = (TBoxSearch.Text ?? "").Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                currentPatients = currentPatients
                    .Where(p =>
                        (p.Фамилия ?? "").ToLower().Contains(search) ||
                        (p.Имя ?? "").ToLower().Contains(search) ||
                        (p.Отчество ?? "").ToLower().Contains(search))
                    .ToList();
            }

            // ---------- СОРТИРОВКА по дате последнего обращения ----------
            if (RButtonUp.IsChecked == true)
            {
                currentPatients = currentPatients
                    .OrderByDescending(p => p.История_болезней
                        .Select(h => (DateTime?)h.Дата_заболевания)
                        .Max() ?? DateTime.MinValue)
                    .ToList();
            }
            else if (RButtonDown.IsChecked == true)
            {
                currentPatients = currentPatients
                    .OrderBy(p => p.История_болезней
                        .Select(h => (DateTime?)h.Дата_заболевания)
                        .Max() ?? DateTime.MaxValue)
                    .ToList();
            }

            ClientListView.ItemsSource = currentPatients;
        }

        // Открытие истории болезни пациента
        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            var patient = ClientListView.SelectedItem as Пациенты;
            if (patient == null)
            {
                MessageBox.Show("Выберите пациента.");
                return;
            }

            NavigationService.Navigate(new ClientHistoryPage(patient, _currentUser));
        }

        // Добавление нового клиента
        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddPatientWindow() { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true)
            {
                UpdatePatients(); // обновляем список после добавления
            }
        }
    }
}