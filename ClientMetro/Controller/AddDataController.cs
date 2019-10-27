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
            if (!string.IsNullOrEmpty(this.Login) && !string.IsNullOrEmpty(this.Password))
            {
                var response = this.Client.AddUser(this.Secret_key, login, password);
                var json = JObject.Parse(response);
                message = JsonHelper.GetValue(json, "message");
                if (JsonHelper.GetValue(json, "error") == "0")
                {
                    return true;
                }
            }
            else
            {
                message = "Поля логин и пароль - обяхательны для заполнения";
            }
            return false;
        }
    }
}
