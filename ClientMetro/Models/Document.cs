using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ClientMetro.HelperMethods;

namespace ClientMetro.Models
{
    public class Document
    {
        private string header;
        private string Name;
        private string content;

        public string HEADER { get { return this.header; } }
        public string NAME { get { return this.Name; } }
        public string CONTENT { get { return this.content; } }


        public Document()
        {
            this.header = string.Empty;
            this.Name = string.Empty;
            this.content = string.Empty;
        }

        public Document(JObject json)
        {
            this.header = JsonHelper.GetValue(json, "header");
            this.Name = JsonHelper.GetValue(json, "Name");
            this.content = JsonHelper.GetValue(json, "content");
        }
    }
}
