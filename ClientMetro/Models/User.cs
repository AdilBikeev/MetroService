using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ClientMetro.HelperMethods;

namespace ClientMetro.Models
{
    public class User
    {
        private string id;
        private string login;
        private string password;

        public string ID { get { return this.id; } }
        public string LOGIN { get { return this.login; } }
        public string PASSWORD { get { return this.password; } }


        public User()
        {
            this.id = string.Empty;
            this.login = string.Empty;
            this.password = string.Empty;
        }

        public User(JObject json)
        {
            this.id = JsonHelper.GetValue(json, "Id");
            this.login = JsonHelper.GetValue(json, "login");
            this.password = JsonHelper.GetValue(json, "password");
        }
    }
}
