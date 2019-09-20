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

        public MainWindowController()
        {
            Initialize();
        }

        private void Initialize()
        {
            ServicePointManager.Expect100Continue = false;
            client = new MetroServiceSoapClient(endpointConfigurationName: "MetroServiceSoap12");
        }

        public (List<User>, string) GetUsers(string secret_key)
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
                    return (users, string.Empty);
                }
                else
                {
                    return  (null, JsonHelper.GetValue(json, "message"));
                }
            }
            else
            {
                return (null, "Поле \'Секретный ключ\' обязательно для заполнения");
            }
        }
    }
}
