using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Поликлиника
{
    public partial class ClientHistoryPage : Page
    {
        private Пациенты _patient;
        private Врачи _doctor;

        public ClientHistoryPage(Пациенты patient, Врачи doctor)
        {
            InitializeComponent();
            _patient = patient;
            _doctor = doctor;

            PatientFIOTB.Text = $"{patient.Фамилия} {patient.Имя} {patient.Отчество}";
            PatientInfoTB.Text = $"ID пациента: {patient.ID_пациента}";

            ReloadHistories();
        }

        private void ReloadHistories()
        {
            var histories = ПоликлиникаEntities.GetContext().История_болезней
                .Where(h => h.ID_пациента == _patient.ID_пациента)
                .OrderByDescending(h => h.Дата_заболевания)
                .ToList();

            HistoryListView.ItemsSource = histories;
            ClearLowerBlocks();
        }

        private void ClearLowerBlocks()
        {
            TreatmentListView.ItemsSource = null;
            OperationListView.ItemsSource = null;
            AddTreatmentBtn.IsEnabled = false;
            AddOperationBtn.IsEnabled = false;
        }

        private void HistoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var history = HistoryListView.SelectedItem as История_болезней;
            if (history == null)
            {
                ClearLowerBlocks();
                return;
            }

            AddTreatmentBtn.IsEnabled = true;
            AddOperationBtn.IsEnabled = true;

            ReloadTreatments(history.ID_истории);
            ReloadOperations(history.ID_истории);
        }

        private void ReloadTreatments(int historyId)
        {
            var treatments = ПоликлиникаEntities.GetContext().Лист_лечения
                .Where(l => l.ID_истории == historyId)
                .OrderByDescending(l => l.Дата_лечения)
                .ToList();

            TreatmentListView.ItemsSource = treatments;
        }

        private void ReloadOperations(int historyId)
        {
            var operations = ПоликлиникаEntities.GetContext().Операции
                .Where(o => o.ID_истории == historyId)
                .OrderByDescending(o => o.Дата_операции)
                .ToList();

            OperationListView.ItemsSource = operations;
        }

        private void AddHistory_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddHistoryWindow(_patient, _doctor) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true)
            {
                ReloadHistories();
            }
        }

        private void AddTreatment_Click(object sender, RoutedEventArgs e)
        {
            var history = HistoryListView.SelectedItem as История_болезней;
            if (history == null)
            {
                MessageBox.Show("Сначала выберите историю болезни.");
                return;
            }

            var win = new AddTreatmentWindow(history) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true)
            {
                ReloadTreatments(history.ID_истории);
            }
        }

        private void AddOperation_Click(object sender, RoutedEventArgs e)
        {
            var history = HistoryListView.SelectedItem as История_болезней;
            if (history == null)
            {
                MessageBox.Show("Сначала выберите историю болезни.");
                return;
            }

            var win = new AddOperationWindow(history, _doctor) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true)
            {
                ReloadOperations(history.ID_истории);
            }
        }
    }
}