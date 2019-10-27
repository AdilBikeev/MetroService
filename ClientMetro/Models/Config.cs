using ClientMetro.MetroService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClientMetro.Models
{
    public class Config
    {
        /// <summary>
        /// Логин для доступа к сервису
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль для доступа к сервису
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Секретный ключ для доступа к некоторым методам сервиса
        /// </summary>
        public string Secret_key { get; set; }

        /// <summary>
        /// Объект для обращения к веб-сервису Метро
        /// </summary>
        public MetroServiceSoapClient Client { get; set; }

        public Config()
        {
            Initialize();
            ServicePointManager.Expect100Continue = false;
            Client = new MetroServiceSoapClient(endpointConfigurationName: "MetroServiceSoap12");
        }

        /// <summary>
        /// Обновляет данные с конфиг файла
        /// </summary>
        public void UpdateConfig()
        {
            initConfi();
        }

        /// <summary>
        /// Инициализирует клиента для взаимодейсвтия с сервисом
        /// </summary>
        private void Initialize()
        {
            this.initConfi();
        }

        /// <summary>
        /// Парсит необходимые конфигурационные настройки из xml файла
        /// </summary>
        private void initConfi()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(@"../../ConfigureFiles/ClientConfig.xml");

            XmlElement element = xml.DocumentElement;

            var login = element["login"].InnerText;
            var password = element["password"].InnerText;
            var secret_key = element["secret_key"].InnerText;

            if (!string.IsNullOrEmpty(login))
            {
                this.Login = login;
            }

            if (!string.IsNullOrEmpty(password))
            {
                this.Password = password;
            }

            if (!string.IsNullOrEmpty(secret_key))
            {
                this.Secret_key = secret_key;
            }
        }

    }
}
