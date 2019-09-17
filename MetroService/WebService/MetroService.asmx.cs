using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json.Linq;

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

        [WebMethod]
        public string Ping(string login, string password)
        {
            
            JObject response = new JObject();
            
            response.Add("error", "0");
            response.Add("message", "Ok");

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
    }
}
