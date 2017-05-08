using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAvaliar.Geral.Console.RodoWeb.Class
{
    public class RootObject2
    {
        public List<RoboAnuncio> Anuncios { get; set; }
    }

    public class RoboAnuncio
    {       
        public string IdAnuncio { get; set; }
        public string TipoVeiculo { get; set; }
        public string TipoAnuncio { get; set; }
        public string TipoAnunciante { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Versao { get; set; }
        public string Combustivel { get; set; }
        public string AnoModelo { get; set; }
        public string Porta { get; set; }
        public String DataAnuncio { get; set; }
        public string ValorVenda { get; set; }
        public string Opcionais { get; set; }
        public string KM { get; set; }
        public string Cambio { get; set; }
        public string Cor { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Observacoes { get; set; }
        public string FipeValor { get; set; }
        public string FipeValorData { get; set; }
        public string WebMotorsValorMinimoBrasil { get; set; }
        public string WebMotorsValorMedioBrasil { get; set; }
        public string WebMotorsValorMaximoBrasil { get; set; }
        public string WebMotorsValorDataBrasil { get; set; }


    }
}
