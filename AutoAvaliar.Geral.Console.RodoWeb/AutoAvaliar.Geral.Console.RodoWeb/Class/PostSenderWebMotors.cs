using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using AutoAvaliar.Geral.Console.RodoWeb.Class;

namespace AutoAvaliar.Geral.Console.RodoWeb.Class
{
    public class PostSenderWebMotors 
    {
        public static List<RoboAnuncio> roboAnuncios;// = new List<RoboAnuncio>();

        private static bool IsLogging = false;

        #region [ Busca e Paginacao ]

        public static int PagingMotoCom(string pTipoAnuncio, string pMarca, string pPages, string pCondicao, ref CookieContainer motoCookies)
        {

            roboAnuncios = new List<RoboAnuncio>();

            ObjMoto moto = new ObjMoto();


            HttpWebRequest hwrRequestLogin = null;
            HttpWebResponse hwpResponseLogin = null;

            int returnPaging = 0;

            ServiceHelper.ClearCookies();

            try
            {
                ArrayList loginParametersList = LoadParameters(pTipoAnuncio, pMarca.ToLower(), pPages);
                String urlParameters = ServiceHelper.BuildFormVariables(loginParametersList);

                hwrRequestLogin = (HttpWebRequest)WebRequest.Create(String.Concat("http://www.webmotors.com.br/comprar/carros/novos-usados/veiculos-todos-estados", "?", urlParameters));
                hwrRequestLogin.CookieContainer = motoCookies;
                hwrRequestLogin.Accept = "*";
                hwrRequestLogin.AllowAutoRedirect = true;
                hwrRequestLogin.UserAgent = "http_requester/0.1";
                hwrRequestLogin.Method = "GET";
                hwrRequestLogin.Timeout = ServiceHelper.TimeOutRequest();
                hwpResponseLogin = (HttpWebResponse)hwrRequestLogin.GetResponse();


                StreamReader loResponseStream = null;
                string cartResponse = String.Empty;

                Encoding enc = System.Text.Encoding.GetEncoding(1252);
                loResponseStream = new StreamReader(hwpResponseLogin.GetResponseStream(), enc);

                cartResponse = loResponseStream.ReadToEnd();


                while (cartResponse != null)
                {

                    string paginas = Tools.ReplaceDate(cartResponse, "box-seo-count", "carros encontrados", pMarca.ToUpper()).Replace("< span class=\"size - xbigger\"></span><span class=\"size - xxxbig\">", "").Replace("boxResultCounter\">", "").Replace("\r\n\t\t\t\t\t", "").Replace("CITROÃ‹N", "");
                    string[] delpaginas = paginas.Split('/');
                    decimal counts = 0;
                    decimal divisao = 0;

                    if (Convert.ToInt32(delpaginas[0].Replace(".", "")) > 36)
                    {
                        divisao = Convert.ToDecimal(delpaginas[0]) / 36;
                        counts = Math.Ceiling(divisao);
                    }
                    else
                    {
                        counts = 1;
                    }
                    for (int i = 1; i <= counts; i++)
                    {
                        //if(i >= 474)
                        // {
                        System.Console.WriteLine("Marca: " + pMarca + " >>>>>>>>>>>>>>>>>>>>>>>>>>>>Pagina: " + i.ToString() + " De: " + counts.ToString());
                        TryMotoComBr(pTipoAnuncio, pMarca.Replace(" ", "-"), i.ToString(), pCondicao, ref motoCookies);
                        //d }


                    }

                    break;
                }

            }
            catch (Exception ex)
            {
                LogEntry(string.Format("Pagina {2} -> Metodo {1} -> Error -> {0}.", ex.ToString(), "Paging", pPages));
            }
            finally
            {
                hwpResponseLogin = null;
                hwrRequestLogin = null;
            }

            return returnPaging;
        }

        #endregion

        #region [ ListaMotos ]

        public static ObjMoto TryMotoComBr(string pTipoAnuncio, string pMarca, string pPages, string pCondicao, ref CookieContainer motoCookies)
        {

            ObjMoto moto = new ObjMoto();

            HttpWebRequest hwrRequestLogin = null;
            HttpWebResponse hwpResponseLogin = null;

            bool returnLogin = false;

            ServiceHelper.ClearCookies();

            try
            {//pMarca.Replace("-", "%20")
                ArrayList loginParametersList = LoadParameters(pTipoAnuncio, pMarca, pPages);
                String urlParameters = ServiceHelper.BuildFormVariables(loginParametersList);

                hwrRequestLogin = (HttpWebRequest)WebRequest.Create(String.Concat("http://www.webmotors.com.br/comprar/carros/novos-usados/veiculos-todos-estados", "?", urlParameters));
                hwrRequestLogin.CookieContainer = motoCookies;
                hwrRequestLogin.Accept = "*";
                hwrRequestLogin.AllowAutoRedirect = true;
                hwrRequestLogin.UserAgent = "http_requester/0.1";
                hwrRequestLogin.Method = "GET";
                hwrRequestLogin.Timeout = ServiceHelper.TimeOutRequest();
                hwpResponseLogin = (HttpWebResponse)hwrRequestLogin.GetResponse();

                StreamReader loResponseStream = null;
                string cartResponse = String.Empty;

                Encoding enc = System.Text.Encoding.GetEncoding(1252);
                loResponseStream = new StreamReader(hwpResponseLogin.GetResponseStream(), enc);

                cartResponse = loResponseStream.ReadToEnd();


                while (cartResponse != null)
                {
                    //string paginas = ReplaceDate(cartResponse, "boxResultadoA", "boxResultadoA", " ");
                    //paginas = ReplaceDate(cartResponse, "paginacao", "</html>", " ");

                    //cartResponse = cartResponse.Replace("title=\"Ver foto\">", ">").Replace("\" >", "\">").Replace("  >", ">");

                    //http://www.webmotors.com.br/comprar/fiat/palio/1-0-mpi-fire-young-8v-gasolina-2p-manual/2-portas/2001-2002/17556385
                    //<a href="http://www.webmotors.com.br/original-de-fabrica
                    //<a href="http://www.webmotors.com.br/original-de-fabrica/peugeot
                    //"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*"
                    //<a href="http://www.webmotors.com.br/original-de-fabrica

                    //string sPattern = @"<a[^>]*?href\s*=\s*[""']?http://www.webmotors.com.br/original-de-fabrica/" + @"([^'"" >]+?)[ '""]? >";
                    //string sPattern = @"<A[^>]*?HREF\s*=\s*[""']?http://www.webmotors.com.br/original-de-fabrica/" + @"([^'"" >]+?)[ '""]? >";

                    //string regex2 = "href\\s*=\\s*(?:\'"(?<1>[^\"]*)\"|(?<1>\\S+))";

                    //Regex regex = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
                    //Match match;
                    //for (match = regex.Match(cartResponse); match.Success; match = match.NextMatch())
                    //{
                    //    //System.Console.WriteLine("Found a href. Groups: ");
                    //    //foreach (Group group in match.Groups)
                    //    //{
                    //    //System.Console.WriteLine("Group value: {0}", group);

                    //    string str = "http://www.webmotors.com.br/original-de-fabrica/";
                    //    MatchCollection mc = Regex.Matches(match.Value, str);
                    //    foreach (Match m in mc)
                    //    {
                    //        System.Console.WriteLine(match.Value.Replace("href=", ""));
                    //    }
                    //    //}
                    //}
                   
                    MatchCollection matches = Regex.Matches(cartResponse, "href[^=]{0,}=[^\"]{0,}\"(.+?webmotors.+?comprar.+?)\"", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                    //MatchCollection matches = Regex.Matches(cartResponse, "href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                    if (matches.Count > 0)
                    {
                        string[] GetLink = null;
                        string last = null;

                        foreach (Match m in matches)
                        {
                            string str = "https://www.webmotors.com.br/comprar/" + pMarca.Replace("Ë", "E").ToLower();
                            string aux = (m.Value.Replace("href=\"", ""));
                            MatchCollection mc = Regex.Matches(aux, str);
                            foreach (Match m2 in mc)
                            {
                                //System.Console.WriteLine(m.Value.Replace("href=\"", ""));
                                GetDetailsMotoCom(m.Value.Replace("href=\"", "").Replace("\"", ""), pMarca, pCondicao);
                            }

                            //if (last != m.ToString().Split(';')[0])
                            //{
                            //    GetLink = m.ToString().Split(';');
                            //    System.Console.WriteLine(">>>" + GetLink[0].Replace("<a href=\"", "").Replace("\">", ""));
                            //    GetDetailsMotoCom(GetLink[0].Replace("<a href=\"", "").Replace("\">", ""), pMarca, pCondicao);
                            //    last = m.ToString().Split(';')[0];
                            //}
                        }
                    }

                    break;
                }

            }
            catch (Exception ex)
            {
                LogEntry(string.Format("Pagina {2} -> Fabricante {1} -> Error -> {0}.", ex.ToString(), pMarca, pPages));
            }
            finally
            {
                hwpResponseLogin = null;
                hwrRequestLogin = null;
            }

            return moto;
        }

        /// <summary>
        /// Build QueryString used to do a login in Retailer website
        /// </summary>
        /// <returns></returns>
        private static ArrayList LoadParameters(string pTipoAnuncio, string pMarca, string pPages)
        {

            FormParameters Parameters = null;
            ArrayList FormParametersList = new ArrayList();

            Parameters = new FormParameters();
            Parameters.ParameterName = "tipoveiculo";
            Parameters.ParameterValue = "carros";
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "tipoanuncio";
            Parameters.ParameterValue = pTipoAnuncio;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "marca1";
            Parameters.ParameterValue = pMarca;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "estado1";
            Parameters.ParameterValue = "veiculos-todos-estados";
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "qt";
            Parameters.ParameterValue = "36";
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "o";
            Parameters.ParameterValue = "1";
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "p";
            Parameters.ParameterValue = pPages;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);
            
            return FormParametersList;
        }

        #endregion

        #region [ DetalhesMotos ]

        public static void GetDetailsMotoCom(string links, string pFabricante, string pCondicao)
        {

            RoboAnuncio roboAnuncio = new RoboAnuncio();

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

                while (cartResponse != null)
                {

                    string Retorna = String.Empty;

                    if (cartResponse.IndexOf("<input type=\"hidden\" name=\"IdAnuncio\" id=\"IdAnuncio\"") != -1)
                    {
                        Retorna = Tools.ReplaceValues(cartResponse, "<input type=\"hidden\" name=\"IdAnuncio\" id=\"IdAnuncio\" value=\"", "\" />", "<input type=\"hidden\" name=\"IdAnuncio\" id=\"IdAnuncio\" value=\"");
                        roboAnuncio.IdAnuncio = Retorna;
                    }
                    if (cartResponse.IndexOf("<input type=\"hidden\" name=\"TipoVeiculo\" id=\"TipoVeiculo\"") != -1)
                    {
                        Retorna = Tools.ReplaceValues(cartResponse, "<input type=\"hidden\" name=\"TipoVeiculo\" id=\"TipoVeiculo\" value=\"", "\" />", "<input type=\"hidden\" name=\"TipoVeiculo\" id=\"TipoVeiculo\" value=\"");
                        roboAnuncio.TipoVeiculo = Retorna;
                    }
                    if (cartResponse.IndexOf("<input type=\"hidden\" name=\"TipoAnuncio\" id=\"TipoAnuncio\"") != -1)
                    {
                        Retorna = Tools.ReplaceValues(cartResponse, "<input type=\"hidden\" name=\"TipoAnuncio\" id=\"TipoAnuncio\" value=\"", "\" />", "<input type=\"hidden\" name=\"TipoAnuncio\" id=\"TipoAnuncio\" value=\"");
                        roboAnuncio.TipoAnuncio = Retorna;
                    }
                    if (cartResponse.IndexOf("<input type=\"hidden\" name=\"TipoAnunciante\" id=\"TipoAnunciante\"") != -1)
                    {
                        Retorna = Tools.ReplaceValues(cartResponse, "<input type=\"hidden\" name=\"TipoAnunciante\" id=\"TipoAnunciante\" value=\"", "\" />", "<input type=\"hidden\" name=\"TipoAnunciante\" id=\"TipoAnunciante\" value=\"");
                        roboAnuncio.TipoAnunciante = Retorna;
                    }
                    if (cartResponse.IndexOf("<input type=\"hidden\" name=\"Marca\" id=\"Marca\"") != -1)
                    {
                        Retorna = Tools.ReplaceValues(cartResponse, "<input type=\"hidden\" name=\"Marca\" id=\"Marca\" value=\"", "\" />", "<input type=\"hidden\" name=\"Marca\" id=\"Marca\" value=\"");
                        string b = WebUtility.HtmlDecode(Retorna);
                        roboAnuncio.Marca = b;
                    }
                    if (cartResponse.IndexOf("<input type=\"hidden\" name=\"Modelo\" id=\"Modelo\"") != -1)
                    {
                        Retorna = Tools.ReplaceValues(cartResponse, "<input type=\"hidden\" name=\"Modelo\" id=\"Modelo\" value=\"", "\" />", "<input type=\"hidden\" name=\"Modelo\" id=\"Modelo\" value=\"");
                        roboAnuncio.Modelo = Retorna;
                    }
                    if (cartResponse.IndexOf("<script>") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "'nmVersao':", "'nmCidade':", "'nmVersao':");
                        string tVersao = Retorna;
                        roboAnuncio.Versao = tVersao;
                    }
                    if (cartResponse.IndexOf("<script>") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "'combustivel':", "'preco':", "'combustivel:'");
                        roboAnuncio.Combustivel = Retorna;
                    }
                    if (cartResponse.IndexOf("<script>") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "'nrAnoModelo':", "'tipoAnuncio'", "'nrAnoModelo':");
                        roboAnuncio.AnoModelo = Retorna;
                    }
                    if (cartResponse.IndexOf("<span class=\"bold\">Portas:") != -1)
                    {
                        Retorna = Tools.ReplaceValues(cartResponse, "<span class=\"bold\">Portas:</span>", "</div>", "<span class=\"bold\">Portas:</span>").Replace("Portas:", "");
                        roboAnuncio.Porta = Retorna;
                    }
                    if (cartResponse.IndexOf("<span class=\"bold\">Data do anúncio:") != -1)
                    {
                        Retorna = Tools.ReplaceValues(cartResponse, "<span class=\"bold\">Data do anúncio:</span>", "</div>", "<span class=\"bold\">Data do anúncio:</span>").Replace("Data do anúncio:", "");
                        roboAnuncio.DataAnuncio = Retorna;
                    }
                    //if (cartResponse.IndexOf("<div class=\"novoBoxPreco\">") != -1)
                    //{

                    //    //Retorna = ReplaceValues(cartResponse, "<div class=\"novoBoxPreco\">", "</div>", "<div class=\"novoBoxPreco\">").Replace("R$","").Replace(" ","");
                    //    roboAnuncio.ValorVenda = Retorna;
                    //}
                    if (cartResponse.IndexOf("<script>") != -1)
                    {
                        Retorna = Tools.ReplaceValues(cartResponse, "'preco':", "}]", "'preco':");
                        roboAnuncio.ValorVenda = Retorna;
                    }
                    if (cartResponse.IndexOf("size-default pad-h_gutter-t pad-gutter-lr lh-oh_gutter") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "Opcionais", "<span class", "Opcionais");
                        roboAnuncio.Opcionais = Retorna;
                    }
                    if (cartResponse.IndexOf("<span class=\"bold\">KM:") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "<span class=\"bold\">KM:</span>", "</div>", "<span class=\"bold\">KM:</span>").Replace("KM:", "");
                        roboAnuncio.KM = Retorna;
                    }
                    if (cartResponse.IndexOf("<script>") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "'cambio':", "'quilometragem':", "'cambio':");
                        roboAnuncio.Cambio = Retorna;
                    }
                    if (cartResponse.IndexOf("<script>") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "'cor':", "'carroceria':", "'cor':");
                        roboAnuncio.Cor = Retorna;
                    }
                    if (cartResponse.IndexOf("<script>") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "'nmCidade':", "'nmEstado':", "'nmCidade':");
                        roboAnuncio.Cidade = Retorna;
                    }
                    if (cartResponse.IndexOf("<script>") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "'nmEstado':", "'tipoCarroceria':", "'nmEstado':");
                        roboAnuncio.Estado = Retorna;
                    }
                    if (cartResponse.IndexOf("size-default pad-h_gutter-t pad-gutter-lr lh-oh_gutter") != -1)
                    {
                        Retorna = Tools.ReplaceDate(cartResponse, "Observações do vendedor", "<img", "Observações do vendedor");
                        roboAnuncio.Observacoes = Retorna;
                    }

                    string link = "https://www.webmotors.com.br/comprar/avaliacao/?idAnuncio=" + roboAnuncio.IdAnuncio + "&tipo=" + roboAnuncio.TipoVeiculo;
                    GetValues.GetValuesFipe(link, roboAnuncio.IdAnuncio, roboAnuncio);

                    //string s = String.Format("IdAnuncio: {0}, TipoVeiculo: {1}, TipoAnuncio: {2}, TipoAnunciante: {3}, Marca: {4}, Modelo: {5}, Versao: {9}, Valor: {6}, Estado: {7}, Cidade: {8}",
                    string s = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {9}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}",
                         roboAnuncio.IdAnuncio,
                         roboAnuncio.TipoVeiculo,
                         roboAnuncio.TipoAnuncio,
                         roboAnuncio.TipoAnunciante,
                         roboAnuncio.Marca,
                         roboAnuncio.Modelo,
                         roboAnuncio.Versao,
                         roboAnuncio.Combustivel,
                         roboAnuncio.AnoModelo,
                         roboAnuncio.Porta,
                         roboAnuncio.DataAnuncio,
                         roboAnuncio.ValorVenda,
                         roboAnuncio.Opcionais,
                         roboAnuncio.KM,
                         roboAnuncio.Cambio,
                         roboAnuncio.Cor,
                         roboAnuncio.Cidade,
                         roboAnuncio.Estado,
                         roboAnuncio.Observacoes,
                         roboAnuncio.FipeValor,
                         roboAnuncio.WebMotorsValorMinimoBrasil,
                         roboAnuncio.WebMotorsValorMedioBrasil,
                         roboAnuncio.WebMotorsValorMaximoBrasil);

                    //System.Console.WriteLine(s);



                    //List<RoboAnuncio> data = new List<RoboAnuncio>() { new RoboAnuncio() };

                    roboAnuncios.Add(roboAnuncio);
                    var json = JsonConvert.SerializeObject(new
                    {
                        tAnuncios = roboAnuncios
                    });



                    //var json = new JavaScriptSerializer().Serialize(roboAnuncio);
                    //System.Console.WriteLine(json);
                    File.WriteAllText(@"C:\TEMP\log\WebMotors_" + pFabricante + ".json", json);



                    break;
                }

            }
            catch (Exception ex)
            {
                LogEntry(string.Format("Pagina {2} -> Metodo {1} -> Error -> {0}.", ex.ToString(), "GetDetails", links));
            }
            finally
            {
                hwpResponseLogin.Close();
                hwpResponseLogin = null;
                hwrRequestLogin = null;
            }
        }

        /// <summary>
        /// Build QueryString used to do a login in Retailer website
        /// </summary>
        /// <returns></returns>
        private static ArrayList LoadParameters(string contentText)
        {
            FormParameters Parameters = null;
            ArrayList FormParametersList = new ArrayList();

            Parameters = new FormParameters();
            Parameters.ParameterName = "Preeenviarncheu";
            Parameters.ParameterValue = "true";
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "returnPage";
            Parameters.ParameterValue = "this";
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "contentText";
            Parameters.ParameterValue = contentText;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "contentType";
            Parameters.ParameterValue = "Anuncio Moto";
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "contentId";
            Parameters.ParameterValue = contentText;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            return FormParametersList;
        }

 
        #endregion


        #region [ Telefone ]

        public static string GetPhoneMotoCom(string[] pParamentros)
        {
            CookieContainer motos = new CookieContainer();

            HttpWebRequest hwrRequestLogin = null;
            HttpWebResponse hwpResponseLogin = null;

            string returnPhone = String.Empty;

            try
            {
                ArrayList loginParametersList = LoadParameters(pParamentros[2].Trim().Replace("'", ""), pParamentros[3].Trim().Replace("'", ""), pParamentros[4].Trim().Replace("'", ""), pParamentros[5].Trim().Replace("'", ""), pParamentros[6].Trim().Replace("'", ""));
                String urlParameters = ServiceHelper.BuildFormVariables(loginParametersList);

                hwrRequestLogin = (HttpWebRequest)WebRequest.Create(String.Concat("http://www.webmotors.com.br/WebMotors/ssComum/ssPopup/pgVerTelefone/pgVerTelefone.aspx", "?", urlParameters));
                hwrRequestLogin.CookieContainer = motos;
                hwrRequestLogin.Accept = "*";
                hwrRequestLogin.AllowAutoRedirect = true;
                hwrRequestLogin.UserAgent = "http_requester/0.1";
                hwrRequestLogin.Method = "GET";
                hwrRequestLogin.Timeout = ServiceHelper.TimeOutRequest();
                hwpResponseLogin = (HttpWebResponse)hwrRequestLogin.GetResponse();



                StreamReader loResponseStream = null;
                string cartResponse = String.Empty;
                Encoding enc = System.Text.Encoding.GetEncoding(1252);
                loResponseStream = new StreamReader(hwpResponseLogin.GetResponseStream(), enc);
                cartResponse = loResponseStream.ReadToEnd();

                while (cartResponse != null)
                {
                    ///<!-- Telefone --> 
                    string RetornaFone = String.Empty;

                    if (cartResponse.IndexOf("<li id=\"liTelefone1\"><span>") != -1)
                    {
                        RetornaFone += Tools.ReplaceDate(cartResponse, "<li id=\"liTelefone1\"><span>", "</span></li>", "<font class=\"titulo\">").Replace("Telefone:", "") + ";";
                    }
                    if (cartResponse.IndexOf("<li id=\"liTelefone2\"><span>") != -1)
                    {
                        RetornaFone += Tools.ReplaceDate(cartResponse, "<li id=\"liTelefone2\"><span>", "</span></li>", "<font class=\"titulo\">").Replace("Telefone:", "") + ";";
                    }
                    if (cartResponse.IndexOf("<li id=\"liTelefone3\"><span>") != -1)
                    {
                        RetornaFone += Tools.ReplaceDate(cartResponse, "<li id=\"liTelefone3\"><span>", "</span></li>", "<font class=\"titulo\">").Replace("Telefone:", "");
                    }

                    ///Juridico               <li id="liTelefonePJ1"><span>
                    if (cartResponse.IndexOf("<li id=\"liTelefonePJ1\"><span>") != -1)
                    {
                        RetornaFone += Tools.ReplaceDate(cartResponse, "<li id=\"liTelefonePJ1\"><span>", "</span></li>", "<font class=\"titulo\">").Replace("Telefone:", "") + ";";
                    }
                    if (cartResponse.IndexOf("<li id=\"liTelefonePJ2\"><span>") != -1)
                    {
                        RetornaFone += Tools.ReplaceDate(cartResponse, "<li id=\"liTelefonePJ2\"><span>", "</span></li>", "<font class=\"titulo\">").Replace("Telefone:", "") + ";";
                    }
                    if (cartResponse.IndexOf("<li id=\"liTelefoneIdNextel\"><span>") != -1)
                    {
                        RetornaFone += Tools.ReplaceDate(cartResponse, "<li id=\"liTelefoneIdNextel\"><span>", "</span></li>", "<font class=\"titulo\">").Replace("Telefone:", "");
                    }


                    returnPhone = RetornaFone;

                    break;
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                hwpResponseLogin.Close();
                hwpResponseLogin = null;
                hwrRequestLogin = null;
            }

            return returnPhone;
        }

        /// <summary>
        /// Build QueryString used to do a login in Retailer website
        /// </summary>
        /// <returns></returns>
        private static ArrayList LoadParameters(string TipoVeiculo, string CodigoAnuncio, string Local, string novoUsado, string Canal)
        {
            FormParameters Parameters = null;
            ArrayList FormParametersList = new ArrayList();

            Parameters = new FormParameters();
            Parameters.ParameterName = "TipoVeiculo";
            Parameters.ParameterValue = TipoVeiculo;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "CodigoAnuncio";
            Parameters.ParameterValue = CodigoAnuncio;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "Local";
            Parameters.ParameterValue = Local;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "novoUsado";
            Parameters.ParameterValue = novoUsado;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            Parameters = new FormParameters();
            Parameters.ParameterName = "Canal";
            Parameters.ParameterValue = Canal;
            Parameters.UseUrlEncode = true;
            FormParametersList.Add(Parameters);

            return FormParametersList;
        }
        #endregion

        /*
        private static Object SaveRoboUsuario(RoboUsuario pRoboUsuarios)
        {
            using (RoboMotoDBDataContext repositorio = new RoboMotoDBDataContext())
            {
                repositorio.RoboUsuarios.InsertOnSubmit(pRoboUsuarios);
                repositorio.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return pRoboUsuarios.GetType().GetProperty("RoboUsuarioID").GetValue(pRoboUsuarios, null);
            }
        }

        private static Object SaveRoboMoto(RoboMoto pRoboMotos)
        {
            using (RoboMotoDBDataContext repositorio = new RoboMotoDBDataContext())
            {
                repositorio.RoboMotos.InsertOnSubmit(pRoboMotos);
                repositorio.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return pRoboMotos.GetType().GetProperty("RoboMotoID").GetValue(pRoboMotos, null);
            }
        }

        private static Object SaveRoboMotoPhoto(RoboMotoPhoto pRoboMotoPhoto)
        {
            using (RoboMotoDBDataContext repositorio = new RoboMotoDBDataContext())
            {
                repositorio.RoboMotoPhotos.InsertOnSubmit(pRoboMotoPhoto);
                repositorio.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return pRoboMotoPhoto.GetType().GetProperty("RoboMotoPhotoID").GetValue(pRoboMotoPhoto, null);
            }
        }
        */
        public static void LogEntry(string logEntry)
        {
            //Aguarda a gravação do log anterior para começar a gravar o log atual.
            // Foi colocado este controle pois o agente executa processos em threads 
            //e pode acontecer de duas threads tentarem gravar simultaneamente.
            while (IsLogging)
            {
                Thread.Sleep(300);
            }
            IsLogging = true;
            string logEntryLine = String.Empty;
            string fileName = String.Empty;
            StreamWriter stwLog = null;

            try
            {

                fileName = String.Concat(LogPath(), DateTime.Now.ToString("yyyyMMdd"), ".txt");
                logEntryLine = String.Concat(DateTime.Now.ToString("HHmm"), ">", logEntry);
                stwLog = new StreamWriter(fileName, true);
                stwLog.WriteLine(logEntryLine);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (stwLog != null)
                {
                    stwLog.Close();
                    stwLog.Dispose();

                }
                IsLogging = false;
            }
        }
        public static string LogPath()
        {
            return ConfigurationManager.AppSettings["LogPath"].ToString();
        }

        public static string PhotoPath()
        {
            return ConfigurationManager.AppSettings["PhotoPath"].ToString();
        }

        public static string LogoPath()
        {
            return ConfigurationManager.AppSettings["LogoPath"].ToString();
        }

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="logEntry."></param>



    }
}
