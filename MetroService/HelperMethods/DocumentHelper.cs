using MetroService.Models;
using Newtonsoft.Json.Linq;
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

        /// <summary>
        /// Возвращает список документов с которыми ознакомился пользователь
        /// </summary>
        /// <param name="docLst">Список всех документов</param>
        /// <param name="docNotFamLst">Списко документов с которыми не ознакомился пользователь</param>
        /// <returns></returns>
        public static string ParceFamiliarDocument(List<Document> docLst, string[] docNotFamLst)
        {
            if(docNotFamLst != null)
            {
                foreach (var item in docNotFamLst)
                {
                    var doc = docLst.FirstOrDefault(x => x.Name == item);
                    if(doc != null)
                        docLst.Remove(doc);
                }
            }


            var documents = new JArray();

            foreach (var doc in docLst)
            {
                var document = new JObject();
                document.Add("header", doc.header);
                document.Add("Name", doc.Name);
                document.Add("content", doc.content);
                document.Add("dateGive", doc.dateGive);
                document.Add("dateDeadLine", doc.dateDeadLine);
                document.Add("finishDeadLine", DateTime.Now > doc.dateDeadLine ? "1" : "0");

                documents.Add(document);
            }

            return documents.ToString();
        }
    }
}