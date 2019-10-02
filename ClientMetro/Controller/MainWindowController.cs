using ClientMetro.MetroService;
using ClientMetro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientMetro.HelperMethods;

namespace ClientMetro.Controller
{
    public class MainWindowController
    {
        /// <summary>
        /// Логин для доступа к сервису
        /// </summary>
        private string login;

        /// <summary>
        /// Пароль для доступа к сервису
        /// </summary>
        private string password;

        /// <summary>
        /// Объект для обращения к веб-сервису Метро
        /// </summary>
        private MetroServiceSoapClient client;

        public MainWindowController()
        {
            Initialize();
        }

        /// <summary>
        /// Инициализирует клиента для взаимодейсвтия с сервисом
        /// </summary>
        private void Initialize()
        {
            ServicePointManager.Expect100Continue = false;
            client = new MetroServiceSoapClient(endpointConfigurationName: "MetroServiceSoap12");

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

            if (!string.IsNullOrEmpty(login))
            {
                this.login = login;
            }

            if (!string.IsNullOrEmpty(password))
            {
                this.password = password;
            }
        }

        public bool Ping(out string message)
        {
            this.initConfi();

            if (!string.IsNullOrEmpty(this.login) && !string.IsNullOrEmpty(this.password))
            {
                var response = client.Ping(login, password);
                var json = JObject.Parse(response);
                message = JsonHelper.GetValue(json, "message");
                if (JsonHelper.GetValue(json, "error") == "0")
                {
                    return true;
                }
            }else
            {
                message = "Неверно указан логин/пароль";
            }
            return false;
        }

        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        /// <param name="secret_key">Секретный ключ</param>
        /// <param name="message">Сообщение полученное в качестве ответа от сервиса</param>
        /// <returns>Список пользователей</returns>
        public HashSet<User> GetUsers(string secret_key, out string message)
        {
            if(this.Ping(out message))
            {
                if (!string.IsNullOrEmpty(secret_key))
                {
                    var response = client.GetUsers(secret_key);
                    var json = JObject.Parse(response);
                    if (JsonHelper.GetValue(json, "error") == "0")
                    {
                        var jArray = json.GetValue("users").Value<JArray>();
                        var users = new HashSet<User>();
                        foreach (var item in jArray)
                        {
                            users.Add(new User(item.ToObject<JObject>()));
                        }
                        message = string.Empty;
                        return users;
                    }
                    else
                    {
                        message = JsonHelper.GetValue(json, "message");
                   
                    }
                }
                else
                {
                    message = "Поле \'Секретный ключ\' обязательно для заполнения";
                }
            }

            return null;
        }

        /// <summary>
        /// Получает список документов
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="message">Сообщение полученное в качестве ответа от сервиса</param>
        /// <returns>Список документов</returns>
        public List<Document> GetDocuments(string login, string password, out string message)
        {
            if(this.Ping(out message))
            {
                if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
                {
                    var response = client.GetDocuments(login, password);
                    var json = JObject.Parse(response);
                    if (JsonHelper.GetValue(json, "error") == "0")
                    {
                        var jArray = json.GetValue("documents").Value<JArray>();
                        var documents = new List<Document>();
                        foreach (var item in jArray)
                        {
                            documents.Add(new Document(item.ToObject<JObject>()));
                        }
                        message = string.Empty;
                        return documents;
                    }
                    else
                    {
                        message = JsonHelper.GetValue(json, "message");
                    }
                }
                else
                {
                    message = "Вы не ввели логин/пароль";
                }
            }

            return null;
        }
    }
}
