using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class RequestInfo
    {
        public Dictionary<string, string> RequestParameters
        {
            get; private set;
        }
        public RequestInfo(string url, Dictionary<string, string> pars)
        {
            Url = url;
            RequestParameters = pars;
        }
        public string Url { get; }

        public FormUrlEncodedContent Content
        {
            get
            {
                return new FormUrlEncodedContent(RequestParameters);
            }
        }

        private string _PostContent = "";
        public string PostContent
        {
            get
            {
                _PostContent = new FormUrlEncodedContent(RequestParameters).ReadAsStringAsync().Result;

                return _PostContent;
            }
        }
    }
}
