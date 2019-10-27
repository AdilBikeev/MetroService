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
    public class MainWindowController: Config
    {
        public MainWindowController():base()
        {

        }

        public bool Ping(out string message)
        {
            if (!string.IsNullOrEmpty(this.Login) && !string.IsNullOrEmpty(this.Password))
            {
                var response = this.Client.Ping(this.Login, this.Password);
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
        /// <param name="Secret_key">Секретный ключ</param>
        /// <param name="message">Сообщение полученное в качестве ответа от сервиса</param>
        /// <returns>Список пользователей</returns>
        public HashSet<User> GetUsers(string Secret_key, out string message)
        {
            if(this.Ping(out message))
            {
                if (!string.IsNullOrEmpty(Secret_key))
                {
                    var response = this.Client.GetUsers(Secret_key);
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
        /// <param name="Login">Логин</param>
        /// <param name="Password">Пароль</param>
        /// <param name="message">Сообщение полученное в качестве ответа от сервиса</param>
        /// <returns>Список документов</returns>
        public List<Document> GetDocuments(string Login, string Password, out string message)
        {
            if(this.Ping(out message))
            {
                if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password))
                {
                    var response = this.Client.GetDocuments(Login, Password);
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
