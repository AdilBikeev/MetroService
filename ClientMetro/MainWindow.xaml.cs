using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClientMetro.MetroService;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientMetro.Models;
using Newtonsoft.Json.Linq;
using ClientMetro.HelperMethods;
using System.Net;
using ClientMetro.Controller;
using ClientMetro.Views;

namespace ClientMetro
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowController mainCotr;
        private AddData addData;

        public MainWindow()
        {
            InitializeComponent();
            mainCotr = new MainWindowController();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            MessageBox.Show("Пожалуйста подождите, операция выполняется", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            try
            {
                string selectNameTab = string.Empty;
                TabItem item = (tabControl.SelectedValue as TabItem);
                selectNameTab = item.Header.ToString();

                string message;


                switch (selectNameTab)
                {
                    case "Пользователи":
                    {
                        var list = mainCotr.GetUsers(secretKeyTb.Text, out message);
                        if(list != null) {  userDg.ItemsSource = list;  userDg.UpdateLayout(); }
                        break;
                    }
                    case "Документы":
                    {
                        var list = mainCotr.GetDocuments(loginTb.Text, passwordTb.Text, out message);
                        if (list != null) { documentDg.ItemsSource = list; documentDg.UpdateLayout(); }
                        break;
                    }
                    default:
                        message = string.Empty;
                        break;
                }

                if(message != string.Empty)
                {
                    MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }else
                {
                    MessageBox.Show("Операция прошла успешно !", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }finally
            {
                this.IsEnabled = true;
            }
        }

        private void CheckConnBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message;
                if( mainCotr.Ping(out message) )
                {
                    MessageBox.Show("Сервис доступен !", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                } else
                {
                    MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(addData == null)
                {
                    addData = new AddData();
                }
                this.IsEnabled = false;
                if( addData.ShowDialog() == true)
                {
                    this.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }finally
            {
                this.IsEnabled = true;
            }
        }
    }
}
