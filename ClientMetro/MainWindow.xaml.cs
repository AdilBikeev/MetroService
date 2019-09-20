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

namespace ClientMetro
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Объект для обращения к веб-сервису Метро
        /// </summary>
        private MetroServiceSoapClient client;

        /// <summary>
        /// Список пользователей
        /// </summary>
        private List<User> users = null;
        public MainWindow()
        {
            InitializeComponent();
            ServicePointManager.Expect100Continue = false;
            client = new MetroServiceSoapClient(endpointConfigurationName: "MetroServiceSoap12");
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(secretKeyTb.Text))
                {
                    var response = client.GetUsers(secretKeyTb.Text);
                    var json = JObject.Parse(response);
                    if(JsonHelper.GetValue(json, "error") == "0")
                    {
                        var jArray = json.GetValue("users").Value<JArray>();
                        if (this.users == null) this.users = new List<User>();
                        foreach(var item in jArray)
                        {
                            users.Add(new User(item.ToObject<JObject>()));
                        }
                        userDg.ItemsSource = users;
                    } else
                    {
                        MessageBox.Show(JsonHelper.GetValue(json, "message"), "Ошибка");
                    }
                } else
                {
                    MessageBox.Show("Поле \'Секретный ключ\' обязательно для заполнения", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}
