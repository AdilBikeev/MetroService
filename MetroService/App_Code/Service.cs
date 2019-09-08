using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json.Linq;

// ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Реструктуризация" можно использовать для одновременного изменения имени класса "Service" в коде, SVC-файле и файле конфигурации.
public class Service : IService
{
    public string Ping(string login, string password)
    {
        JObject response = new JObject()
        {
            {"statusCode", "0"},
            {"message", "Ok" }
        };

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            response["statusCode"] = "400";
            response["message"] = "Вы ввели неверный логин или пароль";
        }

        return response.ToString();
    }
}
