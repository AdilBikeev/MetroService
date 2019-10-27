using ClientMetro.HelperMethods;
using ClientMetro.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientMetro.Controller
{
    class ChangeDataController: Config
    {
        /// <summary>
        /// Название изменяемого документа
        /// </summary>
        private string nameDoc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameDoc">Название изменяемого документа</param>
        public ChangeDataController(string nameDoc) :base()
        {
            this.nameDoc = nameDoc;
        }

        /// <summary>
        /// Изменяет данные документа с указанным name в БД
        /// </summary>
        /// <param name="message">Сообщение от сервера</param>
        /// <param name="header">Новый заголовок документа</param>
        /// <param name="content">Новое содержание документа</param>
        /// <returns>Возвращает true - если изменения в БД вступили в силу, false - в ином случаи</returns>
        public bool changeDataDocument(out string message, string header, string content)
        {
            if (!string.IsNullOrEmpty(this.Login) && !string.IsNullOrEmpty(this.Password))
            {
                this.UpdateConfig();

                var response = this.Client.ChangeDataDocument(this.Secret_key, this.Login, this.Password, this.nameDoc, header, content);
                var json = JObject.Parse(response);
                message = JsonHelper.GetValue(json, "message");
                if (JsonHelper.GetValue(json, "error") == "0")
                {
                    return true;
                }
            }
            else
            {
                message = "Поля логин и пароль - обязательны для заполнения";
            }
            return false;
        }
    }
}
