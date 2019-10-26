using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using MetroService.Models;

namespace MetroService.WebService
{
    /// <summary>
    /// Сводное описание для MetroService
    /// </summary>
    [WebService(Namespace = "http://MetroService/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку. 
    [System.Web.Script.Services.ScriptService]
    public class MetroService : System.Web.Services.WebService
    {

        public MetroService ()
        {
            MetroDbEntities = new MetroDbEntities();
        }

        /// <summary>
        /// Объект для взаимодействия с БД
        /// </summary>
        private MetroDbEntities MetroDbEntities;

        public MetroService ()
        {
            this.MetroDbEntities = new MetroDbEntities();
        }

        /// <summary>
        /// Возвращает стандартный объект с положительным ответом
        /// </summary>
        /// <returns>JSON объект</returns>
        private JObject getObjResponse()
        {
            JObject response = new JObject();

            response.Add("error", "0");
            response.Add("message", "Ok");

            return response;
        }


        private bool isUserExist(string login, string password)
        {
            MetroDbEntities.User.Load();
            var lstUsers = MetroDbEntities.User.Local;
            
            if(lstUsers.Count > 0)
            {
                foreach(var user in lstUsers)
                {
                    if(user.login == login && user.password == password)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Проверяет соединение с сервером.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>JSON объект с кодом ошибки и сопровождающим сообщением.</returns>
        [WebMethod(Description = "Проверяет соединение с сервером.")]
        public string Ping(string login, string password)
        {

            JObject response = this.getObjResponse();

            try
            {
                if( string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    response["error"] = "1";
                    response["message"] = "Вы не указали логин/пароль";
                } else
                {
                    if(login!= "1234admin1234metro1234service" || password != "43211234")
                    {
                        response["error"] = "5";
                        response["message"] = "Неверно указан логин или пароль логин/пароль";
                    }
                }
            }
            catch (Exception ex)
            {
                response["error"] = "30";
                response["message"] = ex.Message;
            }

            return response.ToString();
        }

        /// <summary>
        /// Предоставляет список пользователей сервиса.
        /// </summary>
        /// <param name="sekre_key">Секретный ключ для доступа к списку пользователей.</param>
        /// <returns>JSON объект со списком пользователей</returns>
        [WebMethod(Description = "Предоставляет список пользователей сервиса.")]
        public string GetUsers(string sekret_key)
        {
            JObject response = this.getObjResponse();

            if(!string.IsNullOrEmpty(sekret_key) && sekret_key == "rye3firbvlvjsne3n25123m2n1")
            {
                try
                {
                    MetroDbEntities.User.Load();

                    var lstUsers = MetroDbEntities.User.Local;
                    if(lstUsers.Count > 0)
                    {
                        JArray users = new JArray();
                        foreach(var item in lstUsers)
                        {
                            JObject user = new JObject();
                            user.Add("Id", item.Id);
                            user.Add("login", item.login);
                            user.Add("password", item.password);

                            users.Add(user);
                        }
                        response.Add("users", users);
                    }else
                    {
                        response["error"] = "505";
                        response["message"] = "На сервисе нет зарегестрированных пользователей";
                    }

                }
                catch (Exception ex)
                {
                    response["error"] = "30";
                    response["message"] = ex.Message;
                }
            } else
            {
                response["error"] = "1";
                response["message"] = "Отказано в доступе";
            }

            return response.ToString();
        }

        [WebMethod(Description = "Предоставляет список документов для пользователя с указанным логином и паролем.")]
        public string GetDocuments(string login, string password)
        {
            JObject response = this.getObjResponse();

            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    response["error"] = "1";
                    response["message"] = "Вы не указали логин/пароль";
                }
                else
                {
                    MetroDbEntities.Document.Load();
                    if(isUserExist(login, password))
                    { 
                        var lstDocuments = MetroDbEntities.Document.Local;
                        if(lstDocuments.Count > 0)
                        {
                            var documents = new JArray();
                        
                            foreach (var doc in lstDocuments)
                            {
                                var document = new JObject();
                                document.Add("header", doc.header);
                                document.Add("Name", doc.Name);
                                document.Add("content", doc.content);

                                documents.Add(document);
                            }

                            response.Add("documents", documents);
                        } else
                        {
                            response["error"] = "505";
                            response["message"] = "На сервисе не нашелся ни один документ";
                        }
                    }else
                    {
                        response["error"] = "5";
                        response["message"] = "Указан неверный логин/пароль";
                    }
                }
            }
            catch (Exception ex)
            {
                response["error"] = "30";
                response["message"] = ex.Message;
            }
            return response.ToString();
        }
    }
}
