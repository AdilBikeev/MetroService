using ClientMetro.MetroService;
using ClientMetro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientMetro.HelperMethods;

namespace ClientMetro.Controller
{
    public class MainWindowController
    {
        /// <summary>
        /// Объект для обращения к веб-сервису Метро
        /// </summary>
        private MetroServiceSoapClient client;

        /// <summary>
        /// Список пользователей
        /// </summary>
        private List<User> users = null;

        /// <summary>
        /// Список документов
        /// </summary>
        private List<Document> documents = null;

        public MainWindowController()
        {
            Initialize();
        }

        private void Initialize()
        {
            ServicePointManager.Expect100Continue = false;
            client = new MetroServiceSoapClient(endpointConfigurationName: "MetroServiceSoap12");
        }

        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        /// <param name="secret_key">Секретный ключ</param>
        /// <param name="message">Сообщение полученное в качестве ответа от сервиса</param>
        /// <returns>Список пользователей</returns>
        public List<User> GetUsers(string secret_key, out string message)
        {
            if (!string.IsNullOrEmpty(secret_key))
            {
                var response = client.GetUsers(secret_key);
                var json = JObject.Parse(response);
                if (JsonHelper.GetValue(json, "error") == "0")
                {
                    var jArray = json.GetValue("users").Value<JArray>();
                    if (this.users == null) this.users = new List<User>();
                    foreach (var item in jArray)
                    {
                        users.Add(new User(item.ToObject<JObject>()));
                    }
                    message = string.Empty;
                    return this.users;
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
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                var response = client.GetDocuments(login, password);
                var json = JObject.Parse(response);
                if (JsonHelper.GetValue(json, "error") == "0")
                {
                    var jArray = json.GetValue("documents").Value<JArray>();
                    if (this.documents == null) this.documents = new List<Document>();
                    foreach (var item in jArray)
                    {
                        this.documents.Add(new Document(item.ToObject<JObject>()));
                    }
                    message = string.Empty;
                    return this.documents;
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
            return null;
        }
    }
}
