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
    public class AddDataController: Config
    {
        public AddDataController():base()
        {
            
        }

        public bool addUser(out string message, string login, string password)
        {
            message = string.Empty;
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                this.UpdateConfig();

                var response = this.Client.AddUser(this.Secret_key, login, password);
                var json = JObject.Parse(response);
                if (JsonHelper.GetValue(json, "error") == "0")
                {
                    return true;
                } else
                {
                    message = JsonHelper.GetValue(json, "message");
                    return false;
                }
            }
            else
            {
                message = "Поля логин и пароль - обязательны для заполнения";
            }
            return false;
        }

        public bool addDocument(out string message, string name, string header, string content)
        {
            this.UpdateConfig();

            if (!string.IsNullOrEmpty(this.Login) && !string.IsNullOrEmpty(this.Password) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(content))
            {
                message = string.Empty;
                var response = this.Client.AddDocuments(this.Secret_key, this.Login, this.Password, name, header, content);
                var json = JObject.Parse(response);
                if (JsonHelper.GetValue(json, "error") == "0")
                {
                    return true;
                }
                else
                {
                    message = JsonHelper.GetValue(json, "message");
                    return false;
                }
            }
            else
            {
                message = "Все поля - обязательны для заполнения";
            }
            return false;
        }
    }
}
