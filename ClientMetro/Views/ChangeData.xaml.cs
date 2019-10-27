using ClientMetro.Controller;
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
    /// Логика взаимодействия для ChangeData.xaml
    /// </summary>
    public partial class ChangeData : Window
    {
        /// <summary>
        /// Название изменяемого документа
        /// </summary>
        public string nameDoc { get; set; }

        private ChangeDataController changeDataController;

        public ChangeData(string nameDoc)
        {
            InitializeComponent();
            this.nameDoc = nameDoc;
            changeDataController = new ChangeDataController(this.nameDoc);
        }

        private void ChangeDataBtn_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            MessageBox.Show("Пожалуйста подождите, операция выполняется", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            try
            {
                string message;

                var content = new TextRange(this.contentRtb.Document.ContentStart, this.contentRtb.Document.ContentEnd);
                
                if (! changeDataController.changeDataDocument(out message, this.headerTb.Text, content.Text))
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
    }
}
