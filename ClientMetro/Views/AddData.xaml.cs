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

        public string view;

        public AddData()
        {
            InitializeComponent();
            dataController = new AddDataController();
            view = "Пользователи";
        }

        private void AddDataBtn_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            MessageBox.Show("Пожалуйста подождите, операция выполняется", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            try
            {
                string message;

                switch (this.view)
                {
                    case "Пользователи":
                        {
                            this.documentGrid.Visibility = Visibility.Hidden;
                            this.userGrid.Visibility = Visibility.Visible;
                            dataController.addUser(out message, this.loginTb.Text, this.passwordTb.Password);
                            break;
                        }
                    case "Документы":
                        {
                            this.documentGrid.Visibility = Visibility.Visible;
                            this.userGrid.Visibility = Visibility.Hidden;
                            var content = new TextRange(this.contentRtb.Document.ContentStart, this.contentRtb.Document.ContentEnd);
                            dataController.addDocument(out message, this.nameTb.Text, this.headerTb.Text, content.Text);
                            break;
                        }
                    default:
                        message = string.Empty;
                        break;
                }


                if (message != string.Empty)
                {
                    MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (MessageBoxResult.OK == MessageBox.Show("Операция прошла успешно !", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information))
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
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            switch (this.view)
            {
                case "Пользователи":
                    {
                        this.documentGrid.Visibility = Visibility.Hidden;
                        this.userGrid.Visibility = Visibility.Visible;
                        break;
                    }
                case "Документы":
                    {
                        this.documentGrid.Visibility = Visibility.Visible;
                        this.userGrid.Visibility = Visibility.Hidden;
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
