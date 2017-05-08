using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Web;
using System.Configuration;
using System.IO;


namespace AutoAvaliar.Geral.Console.RodoWeb.Class
{
    public static class ServiceHelper
    {
        private static bool IsLogging = false;
        public static int RetailerId()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["RetailerID"]);
        }

        public static int DataSourceId()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["DataSourceID"]);
        }

        public static int TimeOutRequest()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);
        }

        public static string LogPath()
        {
            return ConfigurationManager.AppSettings["LogPath"].ToString();
        }


        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="logEntry."></param>
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

        public static string BuildFormVariables(ArrayList FormParametersList)
        {
            StringBuilder FormVariables = new StringBuilder();
            ASCIIEncoding encoding = new ASCIIEncoding();

            FormParameters Parameter;
            for (int x = 0; x < FormParametersList.Count; x++)
            {
                Parameter = (FormParameters)FormParametersList[x];
                FormVariables.Append(Parameter.ParameterName);
                FormVariables.Append("=");

                if (Parameter.UseUrlEncode)
                {
                    FormVariables.Append(HttpUtility.UrlEncode(Parameter.ParameterValue));
                    //FormVariables.Append(Parameter.ParameterValue);
                }
                else
                {
                    FormVariables.Append(Parameter.ParameterValue);
                }

                if (x < (FormParametersList.Count - 1))
                    FormVariables.Append("&");
            }

            return FormVariables.ToString();
        }

        public static string BuildFormVariablesParameter(ArrayList FormParametersList)
        {
            StringBuilder FormVariables = new StringBuilder();
            ASCIIEncoding encoding = new ASCIIEncoding();
            string _return = string.Empty;

            FormParameters Parameter;
            for (int x = 0; x < FormParametersList.Count; x++)
            {
                Parameter = (FormParameters)FormParametersList[x];
                //FormVariables.Append(Parameter.ParameterName);
                //FormVariables.Append("=");

                if (Parameter.UseUrlEncode)
                {
                    FormVariables.Append(HttpUtility.UrlEncode(Parameter.ParameterValue));
                    //FormVariables.Append(Parameter.ParameterValue);
                }
                else
                {
                    FormVariables.Append(Parameter.ParameterValue);
                }

                if (x < (FormParametersList.Count - 1))
                    FormVariables.Append(",");
            }

            /*if(FormVariables.ToString() !="")
                _return = FormVariables.ToString().Substring(1, FormVariables.ToString().Length - 1);
            */
            return FormVariables.ToString();
        }

        public static void ClearCookies()
        {
            try
            {
                ClearFolderCookies(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Cookies)));
                ClearFolderCookies(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearFolderCookies(System.IO.DirectoryInfo folder)
        {
            try
            {
                string cookieName = System.Configuration.ConfigurationManager.AppSettings["CookieRetailer"].ToString();

                foreach (FileInfo file in folder.GetFiles())
                {
                    if (file.Name.IndexOf(cookieName) != -1)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch
                        { }
                        finally
                        { }
                    }
                }
            }
            //For Each subfolder As IO.DirectoryInfo In folder.GetDirectories 
            //ClearFolderCookies(subfolder) 
            //Next 
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
