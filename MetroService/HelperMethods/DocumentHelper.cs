using MetroService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetroService.HelperMethods
{
    public static class DocumentHelper
    {
        public static string ParceDocument(List<Document> lst)
        {
            string listStr = string.Empty;

            for (int i = 0; i < lst.Count; i++)
            {
                listStr += lst[i].Name;
                if(i+1 < lst.Count)
                {
                    listStr += ",";
                }
            }

            return listStr;
        }
    }
}