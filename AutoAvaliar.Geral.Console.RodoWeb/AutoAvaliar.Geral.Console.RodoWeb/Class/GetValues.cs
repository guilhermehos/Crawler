using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoAvaliar.Geral.Console.RodoWeb.Class;

namespace AutoAvaliar.Geral.Console.RodoWeb.Class
{
    public class GetValues : Tools
    {
        public static void GetValuesFipe(string links, string idAnuncio, RoboAnuncio roboAnuncio)
        {



            CookieContainer motos = new CookieContainer();

            HttpWebRequest hwrRequestLogin = null;
            HttpWebResponse hwpResponseLogin = null;

            try
            {
                hwrRequestLogin = (HttpWebRequest)WebRequest.Create(links);
                hwrRequestLogin.CookieContainer = motos;
                hwrRequestLogin.Accept = "*";
                hwrRequestLogin.AllowAutoRedirect = true;
                hwrRequestLogin.UserAgent = "http_requester/0.1";
                hwrRequestLogin.Method = "GET";
                hwrRequestLogin.Timeout = ServiceHelper.TimeOutRequest();
                hwpResponseLogin = (HttpWebResponse)hwrRequestLogin.GetResponse();



                StreamReader loResponseStream = null;
                string cartResponse = String.Empty;
                //Encoding enc = System.Text.Encoding.GetEncoding("ISO-8859-1");

                loResponseStream = new StreamReader(hwpResponseLogin.GetResponseStream(), Encoding.UTF8);
                cartResponse = loResponseStream.ReadToEnd();


                string Retorna = String.Empty;
                if (cartResponse.IndexOf("dis-tc col-5 last size-default") != -1)
                {
                    Retorna = ReplaceValues(cartResponse, "Mínimo:", "Médio:", "Mínimo:");
                    roboAnuncio.WebMotorsValorMinimoBrasil = Retorna;
                }
                if (cartResponse.IndexOf("dis-tc col-5 last size-default") != -1)
                {
                    Retorna = ReplaceValues(cartResponse, "Médio:", "Máximo:", "Médio:");
                    roboAnuncio.WebMotorsValorMedioBrasil = Retorna;
                }
                if (cartResponse.IndexOf("dis-tc col-5 last size-default") != -1)
                {
                    Retorna = ReplaceValues(cartResponse, "Máximo:", "</div", "Máximo:");
                    roboAnuncio.WebMotorsValorMaximoBrasil = Retorna;
                }
                if (cartResponse.IndexOf("dis-tc col-5 last valign-m size-default") != -1)
                {
                    Retorna = ReplaceValues(cartResponse, "Média FIPE:", "</div", "Média FIPE:");
                    roboAnuncio.FipeValor = Retorna;
                }
                if (cartResponse.IndexOf("size-small lh-gutter") != -1)
                {
                    Retorna = ReplaceValues(cartResponse, "Última atualização:", "</p", "Última atualização:");
                    roboAnuncio.WebMotorsValorDataBrasil = Retorna;
                }
                if (cartResponse.IndexOf("dis-tc col-5 valign-m lh-0") != -1)
                {
                    Retorna = ReplaceValues(cartResponse, "<span class=\"size-small lh-oh_gutter\">Última atualização:", "</span", "Última atualização:");
                    roboAnuncio.FipeValorData = Retorna;
                }

            }
            catch (Exception ex)
            {
                PostSenderWebMotors.LogEntry(string.Format("Pagina {2} -> Metodo {1} -> Error -> {0}.", ex.ToString(), "GetDetails", links));

            }

        }
    }
}

