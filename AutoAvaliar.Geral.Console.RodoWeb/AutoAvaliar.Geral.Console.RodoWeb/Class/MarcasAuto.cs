using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAvaliar.Geral.Console.RodoWeb.Class
{
    public class Modelo
    {
        public string descricao { get; set; }
    }

    public class RootObject
    {
        public string marca { get; set; }
        public List<Modelo> modelos { get; set; }
    }

}
