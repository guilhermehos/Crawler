using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAvaliar.Geral.Console.RodoWeb.Class
{
    public class ObjMoto
    {
        #region Private variables

        private int _QTD;

        #endregion

        public int QTD
        {
            get { return _QTD; }
            set { _QTD = value; }
        }
    }
}
