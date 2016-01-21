using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Paymentwall.Widgets
{
    public abstract class WidgetBase : Paymentwall_Base
    {

        /**
	     * Widget call URL
	     */
        const string BASE_URL = "https://api.paymentwall.com/api";


        protected string userId;
        protected string widgetCode;
        protected List<Paymentwall_Product> products = new List<Paymentwall_Product>();
        protected Dictionary<string, string> extraParams = new Dictionary<string, string>();


        ///*
        // * @param string userId identifier of the end-user who is viewing the widget
        // * @param string widgetCode e.g. p1 or p1_1, can be found inside of your Paymentwall Merchant account in the Widgets section
        // * @param List<Paymentwall_Product> products array that consists of Paymentwall_Product entities; for Flexible Widget Call use array of 1 product
        // * @param Dictionary<string, string> extraParams associative array of additional params that will be included into the widget URL, 
        // * e.g. 'sign_version' or 'email'. Full list of parameters for each API is available at http://paymentwall.com/documentation
        // */
        //public WidgetBase(string userId, string widgetCode, List<Paymentwall_Product> products, Dictionary<string, string> extraParams)
        //{
        //    this.userId = userId;
        //    this.widgetCode = widgetCode;
        //    this.extraParams = extraParams;

        //    this.products = products ?? new List<Paymentwall_Product>();
        //}

        //public WidgetBase(string userId, string widgetCode, Dictionary<string, string> extraParams) :
        //    this(userId, widgetCode, null, extraParams)
        //{ }


        public static WidgetBase Generate(int apiType, string userId, string widgetCode, List<Paymentwall_Product> products, Dictionary<string, string> extraParams)
        {
            WidgetBase widget = null;
            if (apiType == API_CART)
                widget = new ApiCartWidget();
            else if (apiType == API_GOODS)
                widget = new ApiGoodsWidget();
            else if (apiType == API_VC)
                widget = new ApiCartWidget();
            else throw new Exception("Invalid Api Type specified");

            widget.userId = userId;
            widget.widgetCode = widgetCode;
            widget.extraParams = extraParams;
            widget.products = products ?? new List<Paymentwall_Product>();

            return widget;
        }


        /*
         * Get default signature version for this API Type
         * 
         * @return int
         */
        public virtual int getDefaultSignatureVersion()
        {
            return WidgetBase.DEFAULT_SIGNATURE_VERSION;
        }


        /*
         * Return URL for the widget
         * 
         * @return string
         */
        public string getUrl()
        {
            var parameters = new Dictionary<string, string>();
            parameters["key"] = AppKey;
            parameters["uid"] = this.userId;
            parameters["widget"] = this.widgetCode;

            generateUrlParameters(ref parameters);

            int signatureVersion = this.getDefaultSignatureVersion();
            parameters.Add("sign_version", Convert.ToString(signatureVersion));

            if (this.extraParams.ContainsKey("sign_version"))
            {
                parameters["sign_version"] = this.extraParams["sign_version"];
                signatureVersion = Convert.ToInt32(this.extraParams["sign_version"]);
            }
            parameters = mergeDictionaries(parameters, extraParams);

            parameters["sign"] = WidgetBase.calculateSignature(parameters, SecretKey, signatureVersion);

            return WidgetBase.BASE_URL + "/" + this.buildController(this.widgetCode) + "?" + this.buildQueryString(parameters, "&");
        }

        protected abstract void generateUrlParameters(ref Dictionary<string, string> parameters);


        /**
	     * Return HTML code for the widget
	     *
	     * @param Dictionary<string, string> attributes associative array of additional HTML attributes, e.g. Dictionary.Add("width", "100%")
	     * @return string
	     */
        public string getHtmlCode(Dictionary<string, string> attributes = null)
        {
            Dictionary<string, string> defaultAttributes = new Dictionary<string, string>();
            defaultAttributes.Add("frameborder", "0");
            defaultAttributes.Add("width", "750");
            defaultAttributes.Add("height", "800");
            if (attributes != null)
            {
                attributes = mergeDictionaries(defaultAttributes, attributes);
            }
            else
            {
                attributes = defaultAttributes;
            }
            var attributesList = attributes.Select(
                    a => String.Format("{0}=\"{1}\"", a.Key, a.Value.Replace("\"", "'"))
                ).ToArray();
            return "<iframe src='" + this.getUrl() + "' " + String.Join(" ", attributesList) + "></iframe>";
        }


        /**
         * Build controller URL depending on API type
         *
         * @param string widget code of the widget
         * @param bool flexibleCall
         * @return string
         */
        protected abstract string buildController(string widget, bool flexibleCall = false);


        /**
	     * Build signature for the widget specified
	     *
	     * @param Dictionary<string, string> parameters
	     * @param string secret Paymentwall Secret Key
	     * @param int version Paymentwall Signature Version
	     * @return string
	     */
        public static string calculateSignature(Dictionary<string, string> parameters, string secret, int version)
        {
            string baseString = "";

            if (version == WidgetBase.SIGNATURE_VERSION_1)
            {   //TODO: throw exception if no uid parameter is present 

                if (parameters["uid"] != null)
                    baseString += parameters["uid"];
                else
                    baseString += secret;
                return WidgetBase.getHash(baseString, "md5");
            }
            else
            {
                parameters = parameters.OrderBy(d => d.Key, StringComparer.Ordinal).ToDictionary(d => d.Key, d => d.Value);

                foreach (KeyValuePair<string, string> param in parameters)
                {
                    baseString += param.Key + "=" + param.Value;
                }
                baseString += secret;

                if (version == WidgetBase.SIGNATURE_VERSION_2)
                    return WidgetBase.getHash(baseString, "md5");
                else
                    return WidgetBase.getHash(baseString, "sha256");
            }
        }


        /*
         * Build the query string
         * 
         * @param Dictionary<string, string> dict The input dictionary
         * @param string s The connector sign, e.g. &, =, or white space
         * @return string
         */
        private string buildQueryString(Dictionary<string, string> dict, string s)
        {
            var queryString = new StringBuilder();

            int count = 0;
            bool end = false;

            foreach (string key in dict.Keys)
            {
                if (count == dict.Count - 1) end = true;

                string escapedValue = Uri.EscapeDataString(dict[key] ?? string.Empty);
                if (end)
                    queryString.AppendFormat("{0}={1}", key, escapedValue);
                else
                    queryString.AppendFormat("{0}={1}{2}", key, escapedValue, s);

                count++;
            }
            return queryString.ToString();
        }


        /*
         * Generate a hased string
         * 
         * @param string inputString The string to be hased
         * @param string algorithm The hash algorithm, e.g. md5, sha256
         * @return string hashed string
         */
        private static string getHash(string inputString, string algorithm)
        {
            HashAlgorithm alg = null;

            if (algorithm == "md5")
                alg = MD5.Create();
            else if (algorithm == "sha256")
                alg = SHA256.Create();

            byte[] hash = alg.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }


        /**
         * Merging 2 dictionaries to 1 dictionary
         * 
         * @param dict1 Dictionary<string, string> The first dictionary
         * @param dict2 Dictionar<string, string> The second dictionary
         * @return Dictionary<string, string> The merged dictionary
         */
        private Dictionary<string, string> mergeDictionaries(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            foreach (KeyValuePair<string, string> kvp in dict2)
            {
                if (dict1.ContainsKey(kvp.Key))
                    dict1[kvp.Key] = kvp.Value;
                else
                    dict1.Add(kvp.Key, kvp.Value);
            }
            return dict1;
        }
    }
}