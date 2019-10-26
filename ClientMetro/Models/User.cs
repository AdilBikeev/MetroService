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
        private string login;
        private string password;

        public string LOGIN { get { return this.login; } }
        public string PASSWORD { get { return this.password; } }


        public User()
        {
            this.login = string.Empty;
            this.password = string.Empty;
        }

        public User(JObject json)
        {
            this.login = JsonHelper.GetValue(json, "login");
            this.password = JsonHelper.GetValue(json, "password");
        }
    }
}
