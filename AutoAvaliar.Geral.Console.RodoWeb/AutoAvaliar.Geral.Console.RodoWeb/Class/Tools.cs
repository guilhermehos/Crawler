using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AutoAvaliar.Geral.Console.RodoWeb.Class
{
    public class Tools
    {
        public static bool IsLogging = false;
        public static string GetWord(string pWord, string sPattern)
        {
            MatchCollection matches = Regex.Matches(pWord, sPattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            string newString = String.Empty;

            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    newString = m.ToString();
                }
            }

            return newString;
        }

        public static string Strip(string text)
        {
            return Regex.Replace(text, @"(,)|""|\>|\t|\r|\n|<(.|\n)*?>", string.Empty);
        }

        public static string ReplaceDate(string html, string inicio, string fim, string replace)
        {
            string pWord;
            pWord = html.Substring(html.IndexOf(inicio));
            pWord = pWord.Substring(0, pWord.IndexOf(fim));
            pWord = Strip(pWord).Replace(replace, "").Trim();
            pWord = Strip(pWord).Replace(inicio, string.Empty).Trim();
            return pWord;
        }

        public static string StripLetters(string text)
        {
            return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
        }

        public static string ReplaceValues(string html, string inicio, string fim, string replace)
        {
            string pWord;
            pWord = html.Substring(html.IndexOf(inicio));
            pWord = pWord.Substring(0, pWord.IndexOf(fim));
            pWord = StripLetters(pWord).Replace(replace, "").Trim();
            return pWord;
        }

        public static string MDYToDMY(string input)
        {
            return Regex.Replace(input, @"\b(?<month>\d{1,2})/(?<day>\d{1,2}/(?<year>\d{4})\b", "${day}-${month}-${year}");
        }
   
    }
}
