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
        private ChangeData changeData;

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
                        var list = mainCotr.GetUsers(out message);
                        if(list != null) {  
                                userDg.ItemsSource = list;  
                                userDg.UpdateLayout();
                                this.deleteBtn.Visibility = Visibility.Visible;
                        }else
                        {
                                this.deleteBtn.Visibility = Visibility.Hidden;
                        }
                        break;
                    }
                    case "Документы":
                    {
                        var list = mainCotr.GetDocuments(out message);
                        if (list != null) {
                                documentDg.ItemsSource = list; 
                                documentDg.UpdateLayout();
                                this.deleteBtn.Visibility = Visibility.Visible;
                        }else
                        {
                                this.deleteBtn.Visibility = Visibility.Hidden;
                        }
                        break;
                    }
                    case "Названия документов на ознакомление":
                    {
                        var list = mainCotr.GetNotFamiliarDocuments(out message, this.loginTb.Text, this.passwordTb.Text);
                        if (list != null) {
                                documentNotFamiliarDg.ItemsSource = list; 
                                documentNotFamiliarDg.UpdateLayout();
                                this.deleteBtn.Visibility = Visibility.Visible;
                        }else
                        {
                                this.deleteBtn.Visibility = Visibility.Hidden;
                        }
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
                    addData.Show();
                }

                string selectNameTab = string.Empty;
                TabItem item = (tabControl.SelectedValue as TabItem);
                selectNameTab = item.Header.ToString();

                this.IsEnabled = false;
                addData.view = selectNameTab;
                addData.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }finally
            {
                this.IsEnabled = true;
            }
        }

        private void UpdateConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mainCotr.UpdateConfig();
                MessageBox.Show("Данные успешно обновлены !", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void deleteDataDg_Click(object sender, RoutedEventArgs e)
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
                            var itemDel = userDg.SelectedItem as User;
                            var result = mainCotr.DeleteUser(out message, itemDel.LOGIN, itemDel.PASSWORD);
                            if (result) { 
                                var list = userDg.ItemsSource as List<User>;
                                list.Remove(itemDel);
                                userDg.ItemsSource = list;
                                userDg.Items.Refresh();
                            }
                            break;
                        }
                    case "Документы":
                        {
                            var itemDel = documentDg.SelectedItem as Document;
                            var result = mainCotr.DeleteDocument(out message, itemDel.NAME);
                            if (result)
                            {
                                var list = documentDg.ItemsSource as List<Document>;
                                list.Remove(itemDel);
                                documentDg.ItemsSource = list;
                                documentDg.Items.Refresh();
                            }
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
                    MessageBox.Show("Операция прошла успешно !", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectNameTab = string.Empty;
            TabItem item = (tabControl.SelectedValue as TabItem);
            selectNameTab = item.Header.ToString();

            switch (selectNameTab)
            {
                case "Названия документов на ознакомление":
                {
                    this.changeBtn.Visibility = Visibility.Hidden;
                    this.deleteBtn.Visibility = Visibility.Hidden;
                    break;
                }
                case "Документы":
                {
                    this.changeBtn.Visibility = Visibility.Visible;
                    if (this.documentDg.ItemsSource == null)
                    {
                        this.deleteBtn.Visibility = Visibility.Hidden;
                        this.changeBtn.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        this.deleteBtn.Visibility = Visibility.Visible;
                        this.changeBtn.Visibility = Visibility.Visible;
                    }
                    break;
                }
                default:
                    this.changeBtn.Visibility = Visibility.Hidden;
                    this.changeBtn.Visibility = Visibility.Hidden;
                    if (this.userDg.ItemsSource == null)
                        this.deleteBtn.Visibility = Visibility.Hidden;
                    else
                        this.deleteBtn.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            try
            {
                var itemDel = documentDg.SelectedItem as Document;
                if (itemDel != null)
                {
                    if (this.changeData == null)
                    {
                        changeData = new ChangeData(itemDel.NAME);
                        changeData.Show();
                    }
                    else
                    {
                        changeData.nameDoc = itemDel.NAME;
                        changeData.Visibility = Visibility.Visible;
                    }
                } else
                {
                    MessageBox.Show("Для начала выберите документ, который хотите изменить", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
