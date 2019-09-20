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
                (var lstUsers, string message) = mainCotr.GetUsers(secretKeyTb.Text);
                if(lstUsers == null)
                {
                    MessageBox.Show(message, "Ошибка");
                }else
                {
                    userDg.ItemsSource = lstUsers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}
