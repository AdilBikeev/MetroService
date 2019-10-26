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

        /// <summary>
        /// Объект для взаимодействия с БД
        /// </summary>
        private MetroDbEntities1 MetroDbEntities1;

        public MetroService()
        {
            this.MetroDbEntities1 = new MetroDbEntities1();
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

        /// <summary>
        /// Проверяет существует ли в БД документ с указанным name
        /// </summary>
        /// <param name="name">Название документа</param>
        /// <returns>Возвращает true - документ с указанным названием существует и false в ином случаи</returns>
        private bool isDocumentExist(string name)
        {
            MetroDbEntities1.Document.Load();
            var lstUsers = MetroDbEntities1.Document.Local;
            var doc = lstUsers.FirstOrDefault(x => x.Name == name);
            if(doc != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Проверяет существует ли в БД пользователь с указанным login и password
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Возвращает true - пользователь с указанными данными существует и false в ином случаи</returns>
        private bool isUserExist(string login, string password)
        {
            MetroDbEntities1.User.Load();
            var lstUsers = MetroDbEntities1.User.Local;

            if (lstUsers.Count > 0)
            {
                foreach (var user in lstUsers)
                {
                    if (user.login == login && user.password == password)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Проверяет существует ли в БД пользователь с указанным login
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns>Возвращает true - пользователь с указанными логином существует и false в ином случаи</returns>
        private bool isLoginExist(string login)
        {
            MetroDbEntities1.User.Load();
            var lstUsers = MetroDbEntities1.User.Local;

            if (lstUsers.FirstOrDefault(x => x.login == login) != null)
                return true;
            else 
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
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    response["error"] = "1";
                    response["message"] = "Вы не указали логин/пароль";
                } else
                {
                    if (login != "1234admin1234metro1234service" || password != "43211234")
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

            if (!string.IsNullOrEmpty(sekret_key) && sekret_key == "rye3firbvlvjsne3n25123m2n1")
            {
                try
                {
                    MetroDbEntities1.User.Load();

                    var lstUsers = MetroDbEntities1.User.Local;
                    if (lstUsers.Count > 0)
                    {
                        JArray users = new JArray();
                        foreach (var item in lstUsers)
                        {
                            JObject user = new JObject();
                            user.Add("login", item.login);
                            user.Add("password", item.password);

                            users.Add(user);
                        }
                        response.Add("users", users);
                    } else
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
                    MetroDbEntities1.Document.Load();
                    if (isUserExist(login, password))
                    {
                        var lstDocuments = MetroDbEntities1.Document.Local;
                        if (lstDocuments.Count > 0)
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
                    } else
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

        /// <summary>
        /// Добавляет пользователя в БД с указанным login и password
        /// </summary>
        /// <param name="secret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>JSON объект с результатом выполнения операции</returns>
        [WebMethod(Description = "Добавляет пользователя в БД с указанным login и password")]
        public string AddUser(string secret_key, string login, string password)
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
                    if (!isLoginExist(login))
                    {
                        MetroDbEntities1.User.Add(new User
                        {
                            login = login,
                            password = password
                        });

                        MetroDbEntities1.SaveChanges();
                        response["message"] = "Пользователь успешно добавлен в БД";
                    }
                    else
                    {
                        response["error"] = "5";
                        response["message"] = "Пользователь с указанным логином уже существует";
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
        /// Добавляет пользователя в БД с указанным названием, заголовком и контентом
        /// </summary>
        /// <param name="secret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Название документа</param>
        /// <param name="header">Заголовок документа</param>
        /// <param name="content">Содержимое документа</param>
        /// <returns>JSON объект с результатом выполнения операции</returns>
        [WebMethod(Description = "Добавляет пользователя в БД с указанным названием, заголовком и контентом")]
        public string AddDocuments(string secret_key, string login, string password, string name, string header, string content)
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
                    if (isUserExist(login, password))
                    {
                        if(!isDocumentExist(name))
                        {
                            MetroDbEntities1.Document.Add(new Document
                            {
                                Name = name,
                                content = content,
                                header = header
                            });
                            MetroDbEntities1.SaveChanges();
                            response["message"] = "Документ успешно добавлен в БД";
                        }
                        else
                        {
                            response["error"] = "39";
                            response["message"] = "Документ с указаным названием уже существует";
                        }
                    }
                    else
                    {
                        response["error"] = "5";
                        response["message"] = "Неверный логин или пароль";
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
        /// Удаляет документ с указанным названием
        /// </summary>
        /// <param name="secret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Название документа</param>
        /// <returns>JSON объект с результатом выполнения операции</returns>
        [WebMethod(Description = "Удаляет документ с указанным названием.")]
        public string RemoveDocument(string secret_key, string login, string password, string name)
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
                    if(isUserExist(login, password))
                    {
                        if(isDocumentExist(name))
                        {
                            var doc = MetroDbEntities1.Document.First(x => x.Name == name);
                            MetroDbEntities1.Document.Remove(doc);
                            MetroDbEntities1.SaveChanges();
                        }
                        else
                        {
                            response["error"] = "41";
                            response["message"] = "Документа с указанным названием не существует";
                        }
                    }
                    else
                    {
                        response["error"] = "5";
                        response["message"] = "Неверный логин или пароль";
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
        /// Удаляет пользователя из БД
        /// </summary>
        /// <param name="secret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>JSON объект с результатом операции</returns>
        [WebMethod(Description = "Предоставляет список документов для пользователя с указанным логином и паролем.")]
        public string RemoveUser(string secret_key, string login, string password)
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
                    MetroDbEntities1.User.Load();

                    var lstUsers = MetroDbEntities1.User.Local;
                    var userDel = lstUsers.FirstOrDefault(x => x.login == login && x.password == password);
                    if (userDel != null)
                    {
                        MetroDbEntities1.User.Remove(userDel);
                        MetroDbEntities1.SaveChanges();
                    }
                    else
                    {
                        response["error"] = "40";
                        response["message"] = "Пользователь с указанным логином и паролем не найден";
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
        /// Изменяет данные дкоумента
        /// </summary>
        /// <param name="secret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Новый заголовок документа</param>
        /// <param name="header">Новый заголовок документа</param>
        /// <param name="content">Новый контент документа</param>
        /// <returns>JSON объект с результатом выполнения операции</returns>
        [WebMethod(Description = "Изменяет данные дкоумента.")]
        public string ChangeDataDocument(string secret_key, string login, string password, string name, string header, string content)
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
                    if (isUserExist(login, password))
                    {
                        if (isDocumentExist(name))
                        {
                            var changeItem = MetroDbEntities1.Document.First(x => x.Name == name);
                            var i = MetroDbEntities1.Document.Local.IndexOf(changeItem);
                            MetroDbEntities1.Document.Local[i].header = header;
                            MetroDbEntities1.Document.Local[i].content = content;
                            MetroDbEntities1.SaveChanges();
                        }
                        else
                        {
                            response["error"] = "41";
                            response["message"] = "Документа с указанным названием не существует";
                        }
                    }
                    else
                    {
                        response["error"] = "5";
                        response["message"] = "Неверный логин или пароль";
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
