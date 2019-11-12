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
        /// Возвращает список документов с которыми не ознакомился пользователь
        /// </summary>
        /// <param name="docLst">Список всех документов</param>
        /// <param name="docNotFamLst">Списко документов с которыми не ознакомился пользователь</param>
        /// <returns></returns>
        public static JArray ParceNotFamiliarDocument(List<Document> docLst, string[] docNotFamLst)
        {
            List<Document> docs = new List<Document>();
            if (docNotFamLst != null)
            {
                foreach (var item in docNotFamLst)
                {
                    var obj = docLst.FirstOrDefault(x => x.Name == item);
                    if (obj != null)
                        docs.Add(obj);
                }
            }


            var documents = new JArray();

            foreach (var doc in docs)
            {
                var document = new JObject();
                document.Add("header", doc.header);
                document.Add("Name", doc.Name);
                document.Add("content", doc.content);
                document.Add("dateGive", doc.dateGive.ToString("d"));
                document.Add("dateDeadLine", doc.dateDeadLine.ToString("d"));
                document.Add("finishDeadLine", DateTime.Now > doc.dateDeadLine ? "1" : "0");

                documents.Add(document);
            }

            return documents;
        }

        /// <summary>
        /// Возвращает список документов с которыми ознакомился пользователь
        /// </summary>
        /// <param name="docLst">Список всех документов</param>
        /// <param name="docNotFamLst">Списко документов с которыми не ознакомился пользователь</param>
        /// <returns></returns>
        public static JArray ParceFamiliarDocument(List<Document> docLst, string[] docNotFamLst)
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
                document.Add("dateGive", doc.dateGive.ToString("d"));
                document.Add("dateDeadLine", doc.dateDeadLine.ToString("d"));
                document.Add("finishDeadLine", DateTime.Now > doc.dateDeadLine ? "1" : "0");

                documents.Add(document);
            }

            return documents;
        }

        /// <summary>
        /// Удаляет те документы из docsAll, с которыми пользователь ознакомился
        /// </summary>
        /// <param name="docsNotFam">Документы, с которыми не ознакомился пользователь до текущего момента</param>
        /// <param name="docsFamNow">Документы с которыми пользователь только что ознакомился</param>
        /// <returns>Возвращает новый список документов, с которыми пользователю нужно ознакомиться</returns>
        public static string RemoveFamiliarDoc(string[] docsNotFam, string[] docsFamNow)
        {
            string docsNotFamNew = string.Empty;

            bool isFamiliarDoc = false;

            
            //Пробегаемся по неознакомл на данный момент документам
            for (int i = 0; i < docsNotFam.Length; i++, isFamiliarDoc = false)
            {
                //Пробегаемся по ознак. только что документам    
                for (int j = 0; j < docsFamNow.Length; j++)
                {
                    if(docsNotFam[i] == docsFamNow[j])
                    {
                        isFamiliarDoc = true;
                        break;
                    }
                }

                if (!isFamiliarDoc)
                {
                    if(docsNotFamNew != string.Empty)
                    {
                        docsNotFamNew += "," + docsNotFam[i];
                    }
                    else
                    {
                        docsNotFamNew = docsNotFam[i];
                    }
                }
            }

            return docsNotFamNew;
        }
    }
}