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

namespace ClientMetro
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowController mainCotr;

        public MainWindow()
        {
            InitializeComponent();
            mainCotr = new MainWindowController();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
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
                        if(list != null) {  userDg.ItemsSource = list; }
                        break;
                    }
                    case "Документы":
                    {
                        var list = mainCotr.GetDocuments(loginTb.Text, passwordTb.Text, out message);
                        if (list != null) { documentDg.ItemsSource = list; }
                        break;
                    }
                    default:
                        message = string.Empty;
                        break;
                }

                if(message != string.Empty)
                {
                    MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
