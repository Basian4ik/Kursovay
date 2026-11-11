using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Поликлиника
{
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;

        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text.Trim();

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин");
                return;
            }

            Врачи user = ПоликлиникаEntities.GetContext().Врачи.ToList()
                .Find(p => p.Логин == login);

            if (user != null)
            {
                failedAttempts = 0;
                NavigationService.Navigate(new ClientsPage(user));
                LoginTB.Text = "";
            }
            else
            {
                failedAttempts++;
                MessageBox.Show("Неверный логин. Повторите попытку.");
            }
        }
    }
}