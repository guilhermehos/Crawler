using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoAvaliar.Geral.Console.RodoWeb.Class;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace AutoAvaliar.Geral.Console.RodoWeb
{
    class Program
    {

        private string url = "http://autoavaliar.com.br/api/dados_web.php";

        static void Main(string[] args)
        {


            //CookieContainer MotoComBrCookies = new CookieContainer();
            //PostSenderWebMotors.PagingMotoCom("novos|usados", "CITROËN", "1", "usadas", ref MotoComBrCookies);

            
            using (WebClient webClient = new System.Net.WebClient())
            {
                WebClient n = new WebClient();
                var json = n.DownloadString("file:///C:/Users/guilh/ListaMarcas.html");
                string valueOriginal = Convert.ToString(json);

                var RootObjects = JsonConvert.DeserializeObject<List<RootObject>>(json);

                //foreach (var rootObject in RootObjects.Where(c=>c.marca == "CITROËN"))
                foreach (var rootObject in RootObjects)
                {

                    CookieContainer MotoComBrCookies = new CookieContainer();
                    PostSenderWebMotors.PagingMotoCom("novos|usados", rootObject.marca.Replace("Ë", "E").ToLower(), "1", "usadas", ref MotoComBrCookies);

                }

                
            }

        }
        
    }
}
