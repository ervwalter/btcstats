using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Threading;

namespace BtcStats
{
    public class BtcStatsWebClient : System.Net.WebClient
    {
        public int? RequestTimeout { get; set; }
        public bool PreAuthenticate { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public bool FollowRedirects { get; set; }

        public BtcStatsWebClient()
        {
            FollowRedirects = true;
        }

        protected override System.Net.WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            HttpWebRequest httpRequest = request as HttpWebRequest;
            if (httpRequest != null)
            {
                if (RequestTimeout.HasValue)
                {
                    httpRequest.Timeout = RequestTimeout.Value;
                }
                httpRequest.PreAuthenticate = PreAuthenticate;
                httpRequest.CookieContainer = CookieContainer;
                httpRequest.AllowAutoRedirect = FollowRedirects;
            }
            return request;
        }
    }
}