using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AutoAvaliar.Geral.Console.RodoWeb.Class
{

    public class FormParameters
    {

        #region Private variables

        private string _ParameterName;
        private string _ParameterValue;
        private bool _UseUrlEncode;

        #endregion

        #region Constructors

        public FormParameters()
        {

        }

        public FormParameters(string ParameterName, string ParameterValue, bool UseUrlEncode)
        {
            _ParameterName = ParameterName;
            _ParameterValue = ParameterValue;
            _UseUrlEncode = UseUrlEncode;
        }

        #endregion

        #region override
        // Meaningful text representation
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.ParameterName);
            sb.Append(",");
            sb.Append(this.ParameterValue);
            return sb.ToString();
        }

        #endregion

        #region Public methods

        public string ParameterName
        {
            get { return _ParameterName; }
            set { _ParameterName = value; }
        }

        public string ParameterValue
        {
            get { return _ParameterValue; }
            set { _ParameterValue = value; }
        }

        public bool UseUrlEncode
        {
            get { return _UseUrlEncode; }
            set { _UseUrlEncode = value; }
        }

        #endregion
    }

}
