using ClientMetro.HelperMethods;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientMetro.Models
{
    public class NotFamiliarDocuments
    {
        private string name;

        public string NAME { get { return this.name; } }

        public NotFamiliarDocuments()
        {
            this.name = string.Empty;
        }

        public NotFamiliarDocuments(string name)
        {
            this.name = name;
        }
    }
}
