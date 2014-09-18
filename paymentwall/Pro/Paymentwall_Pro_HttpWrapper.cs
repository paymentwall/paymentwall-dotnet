using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Paymentwall.Pro
{
    public class Paymentwall_Pro_HttpWrapper : Paymentwall_Base
    {
        /**
         * @var Dictionary requestParams GET/POST parameters to send
         */
        private Dictionary<string, string> requestParams;


        /**
         * @var string responseString The response string read from HttpWebResponse stream
         */
        protected string responseString = "";


        /**
         * @var string status The HTTP response code
         */
        protected string status;


        /**
         * 
         */
        public Paymentwall_Pro_HttpWrapper(Dictionary<string, string> attributes)
        {
            string ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]; //Identifying if client is connecting through an HTTP Proxy
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (attributes != null)
            {
                this.requestParams = attributes;
                if (!this.requestParams.ContainsKey("browser_ip")) this.requestParams.Add("browser_ip", ipAddress);
            }
        }


        /**
         * Handle HTTP POST request
         * 
         * @return Dictionary
         */
        public Dictionary<string, string> post()
        {
            Dictionary<string, string> parameters = this.requestParams;
            string url = Paymentwall_Pro_HttpWrapper.CHARGE_URL;
            if (parameters.ContainsKey("period"))
            {
                if (parameters["period"] != null && parameters["period_duration"] != null)
                {

                    url = Paymentwall_Pro_HttpWrapper.SUBS_URL;
                }
            }
            Dictionary<string, string> response = this.handleRequest("POST", url);
            return (this.wrapResponseBody(response["body"]));
        }


        /**
         * DO HTTP request
         * 
         * @param string httpRequestType HTTP request type
         * @param string url The API URL
         * @return Dictionary
         */
        public Dictionary<string, string> handleRequest(string httpRequestType, string url)
        {
            NameValueCollection nvc = new NameValueCollection();
            foreach (var kvp in this.requestParams)
            {
                nvc.Add(kvp.Key.ToString(), kvp.Value.ToString());
            }

            WebClient wbClient = new WebClient();
            wbClient.Headers.Add("X-ApiKey", Paymentwall_Pro_HttpWrapper.getProApiKey());

            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                                delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                                                        System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

            try
            {
                var response = wbClient.UploadValues(url, nvc);
                this.responseString = wbClient.Encoding.GetString(response);
                this.status = "200";
            }
            catch (WebException we)
            {
                int statusNum = (int)((HttpWebResponse)we.Response).StatusCode;
                this.status = statusNum.ToString();
                this.responseString = new StreamReader(((HttpWebResponse)we.Response).GetResponseStream()).ReadToEnd();
            }

            return new Dictionary<string, string>() { { "status", this.status }, { "body", this.responseString } };
        }


        /**
         * @param string response The response string 
         * @return string
         */
        private Dictionary<string, string> wrapResponseBody(string responseString)
        {
            return this.parseResponse(responseString);
        }


        /**
         * @param string response The response string
         * @return Dictionary
         */
        private Dictionary<string, string> parseResponse(string responseString)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)(new JavaScriptSerializer()).DeserializeObject(responseString);
            Dictionary<string, string> response = null;

            if (dict != null)
            {
                response = new Dictionary<string, string>();
                foreach (var kvp in dict)
                {
                    response.Add(kvp.Key, (new JavaScriptSerializer()).Serialize(kvp.Value));
                }
            }

            if (response != null)
            {
                if (!Paymentwall_Pro_Error.isError(response))
                {
                    return response;
                }
                else
                {
                    return Paymentwall_Pro_Error.wrapError(response);
                }
            }
            else
            {
                return Paymentwall_Pro_Error.wrapInternalError(response);
            }
        }


    }
}