using ClientMetro.Controller;
using ClientMetro.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ClientMetro.Views
{
    /// <summary>
    /// Логика взаимодействия для AddData.xaml
    /// </summary>
    public partial class AddData : Window
    {
        private AddDataController dataController;

        public AddData()
        {
            InitializeComponent();
            dataController = new AddDataController();
        }

        private void AddDataBtn_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false; 
            MessageBox.Show("Пожалуйста подождите, операция выполняется", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            try
            {
                string message;

                dataController.addUser(out message, this.loginTb.Text, this.passwordTb.Password);

                if (message != string.Empty)
                {
                    MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if( MessageBoxResult.OK == MessageBox.Show("Операция прошла успешно !", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information) )
                    {
                        this.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}
