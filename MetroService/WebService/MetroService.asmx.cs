﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using MetroService.Models;
using MetroService.HelperMethods;

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
        /// Обновляет в БД для пользователя список документов, с которыми он ознакомился
        /// </summary>
        /// <param name="sekret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="docFamiliarLst">Список всех названий документов, с которыми ознакомился пользователь
        /// перечисленные в виде строки через запятую или пустая строка, если таких документов нет
        /// </param>
        /// <returns>JSON объект с кодом ошибки и сопровождающим сообщением.</returns>
        [WebMethod(Description = "Обновляет в БД для пользователя список документов, с которыми он ознакомился.")]
        public string UpdateListNotFamiliarDoc(string sekret_key, string login, string password, string docFamiliarLst)
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
                        if(string.IsNullOrEmpty(docFamiliarLst))
                        {
                            docFamiliarLst = string.Empty;
                        }

                        MetroDbEntities1.NotFamiliarDocuments.Load();
                        var user = MetroDbEntities1.NotFamiliarDocuments.FirstOrDefault(x => x.user_Login == login);

                        //Парсим список документов с которыми ознакомился пользователь
                        var familiarLst = docFamiliarLst.Split(',');

                        //Если пользователь впервые зарегестрирвоался
                        if (user == null)
                        {
                            MetroDbEntities1.Document.Load();
                            var docAll = MetroDbEntities1.Document.Local;
                            MetroDbEntities1.NotFamiliarDocuments.Add(new NotFamiliarDocuments
                            {
                                user_Login = login,
                                names_DocumentsList = docAll.Count == 0 ? "": DocumentHelper.ParceDocument(docAll.ToList())
                            });
                        }
                        else
                        {
                            var i = MetroDbEntities1.NotFamiliarDocuments.Local.IndexOf(user);
                            MetroDbEntities1.NotFamiliarDocuments.Local[i].names_DocumentsList = DocumentHelper.RemoveFamiliarDoc(user.names_DocumentsList.Split(','), familiarLst);
                        }
                        MetroDbEntities1.SaveChanges();
                    }
                    else
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
                        response["message"] = "Неверно указан логин/пароль";
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

        /// <summary>
        /// Возвращает ФИО пользователя с указанным login и password.
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>JSON объект с данными ФИО пользователя</returns>
        [WebMethod(Description = "Возвращает ФИО пользователя с указанным login и password.")]
        public string GetDataUser(string login, string password)
        {
            JObject response = this.getObjResponse();

            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    MetroDbEntities1.User.Load();

                    var lstUsers = MetroDbEntities1.User.Local;
                    if (lstUsers.Count > 0)
                    {
                        var dataUser = lstUsers.FirstOrDefault(x => x.login == login && x.password == password);
                        if(dataUser != null)
                        {
                            JObject user = new JObject();
                            user.Add("name", dataUser.name);
                            user.Add("surname", dataUser.surname);
                            user.Add("lastname", dataUser.lastname);
                            response.Add("fio", user);
                        }else
                        {
                            response["error"] = "55";
                            response["message"] = "Вы указали неверный логин/пароль";
                        }
                    }
                    else
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
            }
            else
            {
                response["error"] = "1";
                response["message"] = "Отказано в доступе";
            }

            return response.ToString();
        }

        /// <summary>
        /// Предоставляет список документов для пользователя с указанным логином и паролем.
        /// </summary>
        /// <param name="sekret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>JSON объект со списком всех документов в БД</returns>
        [WebMethod(Description = "Предоставляет список документов для пользователя с указанным логином и паролем.")]
        public string GetDocuments(string sekret_key, string login, string password)
        {
            JObject response = this.getObjResponse();
            if (!string.IsNullOrEmpty(sekret_key) && sekret_key == "rye3firbvlvjsne3n25123m2n1")
            {
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
                                    document.Add("dateGive", doc.dateGive.ToString("d"));
                                    document.Add("dateDeadLine", doc.dateDeadLine.ToString("d"));
                                    document.Add("finishDeadLine", DateTime.Now > doc.dateDeadLine ? "1" : "0");

                                    documents.Add(document);
                                }

                                response.Add("documents", documents);
                            }
                            else
                            {
                                response["error"] = "505";
                                response["message"] = "На сервисе не нашелся ни один документ";
                            }
                        }
                        else
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
            }
            else
            {
                response["error"] = "1";
                response["message"] = "Отказано в доступе";
            }
            return response.ToString();
        }

        /// <summary>
        /// Добавляет пользователя в БД с указанным login и password
        /// </summary>
        /// <param name="sekret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="lastname">Отчество</param>
        /// <returns>JSON объект с результатом выполнения операции</returns>
        [WebMethod(Description = "Добавляет пользователя в БД с указанным login, password и личными данными")]
        public string AddUser(string sekret_key, string login, string password, string name, string surname, string lastname)
        {
            JObject response = this.getObjResponse();

            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    response["error"] = "1";
                    response["message"] = "Вы не указали логин/пароль";
                }
                else if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
                {
                    response["error"] = "12";
                    response["message"] = "Вы не указали имя/фамилию";
                }
                else if (!isLoginExist(login))
                {
                    MetroDbEntities1.User.Add(new User
                    {
                        login = login,
                        password = password,
                        name = name,
                        surname = surname,
                        lastname = string.IsNullOrEmpty(lastname) ? string.Empty : lastname
                    });

                    MetroDbEntities1.SaveChanges();
                    response["message"] = "Пользователь успешно добавлен в БД";
                    var result = UpdateListNotFamiliarDoc(sekret_key, login, password, string.Empty);
                    response.Add("inner_message", JObject.Parse(result));
                }
                else
                {
                    response["error"] = "5";
                    response["message"] = "Пользователь с указанным логином уже существует";
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
        /// <param name="sekret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Название документа</param>
        /// <param name="header">Заголовок документа</param>
        /// <param name="content">Содержимое документа</param>
        /// <param name="dateDeadLine">Дата окончания срока для ознакомления с документов в формате ДД.ММ.ГГГГ</param>
        /// <returns>JSON объект с результатом выполнения операции</returns>
        [WebMethod(Description = "Добавляет пользователя в БД с указанным названием, заголовком и контентом")]
        public string AddDocuments(string sekret_key, string login, string password, string name, string header, string content, string dateDeadLine)
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
                            if (string.IsNullOrEmpty(dateDeadLine))
                            {
                                response["error"] = "44";
                                response["message"] = "Поля dateDeadLine - обязательны для заполнения";
                            }
                            else
                            {
                                var dateDeadLineMass = dateDeadLine.Split('.');
                                MetroDbEntities1.Document.Add(new Document
                                {
                                    Name = name,
                                    content = content,
                                    header = header,
                                    dateGive = DateTime.Now,
                                    dateDeadLine = new DateTime(int.Parse(dateDeadLineMass[2]), int.Parse(dateDeadLineMass[1]), int.Parse(dateDeadLineMass[0]))
                                });
                                MetroDbEntities1.SaveChanges();
                                response["message"] = "Документ успешно добавлен в БД";

                                try
                                {
                                    MetroDbEntities1.NotFamiliarDocuments.Load();
                                    var size = MetroDbEntities1.NotFamiliarDocuments.Local.Count;
                                    for (int i = 0; i < size; i++)
                                    {
                                        MetroDbEntities1.NotFamiliarDocuments.Local[i].names_DocumentsList += $",{name}";
                                    }
                                    MetroDbEntities1.SaveChanges();
                                }
                                catch (Exception exc)
                                {
                                    response.Add("inner_message", exc.Message);
                                }
                            }
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
        /// <param name="sekret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Название документа</param>
        /// <returns>JSON объект с результатом выполнения операции</returns>
        [WebMethod(Description = "Удаляет документ с указанным названием.")]
        public string RemoveDocument(string sekret_key, string login, string password, string name)
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
                            this.UpdateListNotFamiliarDoc(sekret_key, login, password, this.GetFamiliarDocuments(sekret_key, login, password));
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
        /// <param name="sekret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>JSON объект с результатом операции</returns>
        [WebMethod(Description = "Удаляет пользователя из БД с указанным логином и паролем.")]
        public string RemoveUser(string sekret_key, string login, string password)
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
                    MetroDbEntities1.NotFamiliarDocuments.Load();
                    MetroDbEntities1.User.Load();

                    var lstDocuments = MetroDbEntities1.NotFamiliarDocuments.Local;
                    var lstUsers = MetroDbEntities1.User.Local;

                    var docDel = lstDocuments.FirstOrDefault(x => x.user_Login == login);
                    var userDel = lstUsers.FirstOrDefault(x => x.login == login && x.password == password);

                    if (userDel != null && docDel != null)
                    {
                        MetroDbEntities1.NotFamiliarDocuments.Remove(docDel);
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
        /// <param name="sekret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Новый заголовок документа</param>
        /// <param name="header">Новый заголовок документа</param>
        /// <param name="content">Новый контент документа</param>
        /// <returns>JSON объект с результатом выполнения операции</returns>
        [WebMethod(Description = "Изменяет данные дкоумента.")]
        public string ChangeDataDocument(string sekret_key, string login, string password, string name, string header, string content)
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

        /// <summary>
        /// Предоставляет список документов, с которыми не ознакомился пользователь с указанным логином и паролем.
        /// </summary>
        /// <param name="sekret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>JSON объект со списком всех документов в БД</returns>
        [WebMethod(Description = "Предоставляет список документов, с которыми не ознакомился пользователь с указанным логином и паролем.")]
        public string GetNotFamiliarDocuments(string sekret_key, string login, string password)
        {
            JObject response = this.getObjResponse();
            if (!string.IsNullOrEmpty(sekret_key) && sekret_key == "rye3firbvlvjsne3n25123m2n1")
            {
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
                            MetroDbEntities1.NotFamiliarDocuments.Load();
                            MetroDbEntities1.Document.Load();
                            var docs = MetroDbEntities1.Document.Local.ToList();
                            var lstDocuments = MetroDbEntities1.NotFamiliarDocuments.Local;
                            var user = lstDocuments.FirstOrDefault(x => x.user_Login == login);
                            if (user != null)
                            {
                                var docNotFamiliarLst = new JObject();
                                if (!string.IsNullOrEmpty(user.names_DocumentsList))
                                {
                                    if (user.names_DocumentsList[0] == ',')
                                        user.names_DocumentsList = user.names_DocumentsList.Remove(0, 1);
                                    if (user.names_DocumentsList[user.names_DocumentsList.Length - 1] == ',')
                                        user.names_DocumentsList = user.names_DocumentsList.Remove(user.names_DocumentsList.Length - 1, 1);
                                    response.Add("docNotFamiliarLst", DocumentHelper.ParceNotFamiliarDocument(docs, user.names_DocumentsList.Split(',')));
                                } else
                                {
                                    response.Add("docNotFamiliarLst", string.Empty);
                                }
                            }
                            else
                            {
                                response["error"] = "225";
                                response["message"] = "Пользователь ознакомился со всеми документами / документов в БД нет ни одного документа";
                            }
                        }
                        else
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
            }
            else
            {
                response["error"] = "1";
                response["message"] = "Отказано в доступе";
            }
            return response.ToString();
        }

        /// <summary>
        /// Предоставляет список документов, с которыми ознакомился пользователь с указанным логином и паролем.
        /// </summary>
        /// <param name="sekret_key">Секретный ключ</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>JSON объект со списком всех документов в БД</returns>
        [WebMethod(Description = "Предоставляет список документов, с которыми ознакомился пользователь с указанным логином и паролем.")]
        public string GetFamiliarDocuments(string sekret_key, string login, string password)
        {
            JObject response = this.getObjResponse();
            if (!string.IsNullOrEmpty(sekret_key) && sekret_key == "rye3firbvlvjsne3n25123m2n1")
            {
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
                            MetroDbEntities1.Document.Load();

                            var docLst = MetroDbEntities1.Document.Local.ToList();
                            if (docLst != null && docLst.Count > 0)
                            {
                                MetroDbEntities1.NotFamiliarDocuments.Load();
                                var docNotFamLst = MetroDbEntities1.NotFamiliarDocuments.Local.ToList();

                                if (docNotFamLst != null && docNotFamLst.Count > 0)
                                {
                                    var docsUser = docNotFamLst.FirstOrDefault(x => x.user_Login == login);
                                    if (docsUser != null)
                                    {
                                        response.Add(
                                             "docFamiliarLst",
                                             DocumentHelper.ParceFamiliarDocument(docLst, docsUser.names_DocumentsList.Split(','))
                                         );
                                    }
                                    else
                                    {
                                        throw new Exception("Не удалось найти у пользователя инфомрмацию о неознакомленных документов");
                                    }
                                }
                                else
                                {
                                    response.Add(
                                        "docFamiliarLst",
                                        DocumentHelper.ParceFamiliarDocument(docLst, null)
                                    );
                                }
                            }
                            else
                            {
                                response["error"] = "505";
                                response["message"] = "На сервисе не нашелся ни один документ";
                            }
                        }
                        else
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
            }
            else
            {
                response["error"] = "1";
                response["message"] = "Отказано в доступе";
            }
            return response.ToString();
        }

    }
}
