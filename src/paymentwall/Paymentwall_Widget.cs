using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Paymentwall
{
    public class Paymentwall_Widget : Paymentwall_Base
    {

        /**
	     * Widget call URL
	     */
        const string BASE_URL = "https://api.paymentwall.com/api";


        protected string userId;
        protected string widgetCode;
        protected List<Paymentwall_Product> products = new List<Paymentwall_Product>();
        protected Dictionary<string, string> extraParams = new Dictionary<string, string>();


        /*
         * @param string userId identifier of the end-user who is viewing the widget
         * @param string widgetCode e.g. p1 or p1_1, can be found inside of your Paymentwall Merchant account in the Widgets section
         * @param List<Paymentwall_Product> products array that consists of Paymentwall_Product entities; for Flexible Widget Call use array of 1 product
         * @param Dictionary<string, string> extraParams associative array of additional params that will be included into the widget URL, 
         * e.g. 'sign_version' or 'email'. Full list of parameters for each API is available at http://paymentwall.com/documentation
         */
        public Paymentwall_Widget(string userId, string widgetCode, List<Paymentwall_Product> products, Dictionary<string, string> extraParams)
        {
            this.userId = userId;
            this.widgetCode = widgetCode;
            this.products = products;
            this.extraParams = extraParams;
        }


        /*
         * Widget constructor for Virtual Currency API
         * 
         * @param string userId identifier of the end-user who is viewing the widget
         * @param string widgetCode e.g. p1 or p1_1, can be found inside of your Paymentwall Merchant account in the Widgets section
         * @param Dictionary<string, string> extraParams associative array of additional params that will be included into the widget URL, 
         * e.g. 'sign_version' or 'email'. Full list of parameters for each API is available at http://paymentwall.com/documentation
         */
        public Paymentwall_Widget(string userId, string widgetCode, Dictionary<string, string> extraParams)
        {
            this.userId = userId;
            this.widgetCode = widgetCode;
            this.extraParams = extraParams;
            this.products = new List<Paymentwall_Product>();
        }


        /*
         * Get default signature version for this API Type
         * 
         * @return int
         */
        public int getDefaultSignatureVersion()
        {
            if (Paymentwall_Widget.getApiType() != Paymentwall_Widget.API_CART)
            {
                return Paymentwall_Widget.DEFAULT_SIGNATURE_VERSION;
            }
            else
            {
                return Paymentwall_Widget.SIGNATURE_VERSION_2;
            }
        }


        /*
         * Return URL for the widget
         * 
         * @return string
         */
        public string getUrl()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["key"] = Paymentwall_Widget.getAppKey();
            parameters["uid"] = this.userId;
            parameters["widget"] = this.widgetCode;

            int productsNumber = this.products.Count;

            if (Paymentwall_Widget.getApiType() == Paymentwall_Widget.API_GOODS)
            {
                if (productsNumber > 0)
                {
                    if (productsNumber == 1)
                    {
                        Paymentwall_Product product = this.products[0];
                        Paymentwall_Product postTrialProduct = null;
                        if (product.getTrialProduct() is Paymentwall_Product)
                        {
                            postTrialProduct = product;
                            product = product.getTrialProduct();
                        }
                        parameters.Add("amount", product.getAmount());
                        parameters.Add("currencyCode", product.getCurrencyCode());
                        parameters.Add("ag_name", product.getName());
                        parameters.Add("ag_external_id", product.getId());
                        parameters.Add("ag_type", product.getType());

                        if (product.getType() == Paymentwall_Product.TYPE_SUBSCRIPTION)
                        {
                            parameters.Add("ag_period_length", product.getPeriodLength().ToString());
                            parameters.Add("ag_period_type", product.getPeriodType());

                            if (product.isRecurring())
                            {
                                parameters.Add("ag_recurring", (Convert.ToInt32(product.isRecurring())).ToString());

                                if (postTrialProduct != null)
                                {
                                    parameters.Add("ag_trial", "1");
                                    parameters.Add("ag_post_trial_external_id", postTrialProduct.getId());
                                    parameters.Add("ag_post_trial_period_length", postTrialProduct.getPeriodLength().ToString());
                                    parameters.Add("ag_post_trial_period_type", postTrialProduct.getPeriodType());
                                    parameters.Add("ag_post_trial_name", postTrialProduct.getName());
                                    parameters.Add("post_trial_amount", postTrialProduct.getAmount().ToString());
                                    parameters.Add("post_trial_currencyCode", postTrialProduct.getCurrencyCode().ToString());
                                }

                            }

                        }

                    } //end if (productNumber == 1)

                    else
                    {
                        //TODO: Paymentwall_Widget.appendToErrors('Only 1 product is allowed in flexible widget call');
                    }

                } //end if (productNumber > 0) 

            }
            else if (Paymentwall_Widget.getApiType() == Paymentwall_Widget.API_CART)
            {
                int index = 0;

                foreach (Paymentwall_Product product in this.products)
                {
                    parameters.Add("external_ids[" + index.ToString() + "]", product.getId());

                    if (product.getAmount() != -1f)
                    {
                        parameters.Add("prices[" + index.ToString() + "]", product.getAmount().ToString());
                    }

                    if (product.getCurrencyCode() != null)
                    {
                        parameters.Add("currencies[" + index.ToString() + "]", product.getCurrencyCode());
                    }

                    index++;
                }

                index = 0;
            }

            int signatureVersion = this.getDefaultSignatureVersion();
            parameters.Add("sign_version", Convert.ToString(signatureVersion));

            if (this.extraParams.ContainsKey("sign_version"))
            {
                parameters["sign_version"] = this.extraParams["sign_version"];
                signatureVersion = Convert.ToInt32(this.extraParams["sign_version"]);
            }
            parameters = mergeDictionaries(parameters, extraParams);

            parameters["sign"] = Paymentwall_Widget.calculateSignature(parameters, Paymentwall_Widget.getSecretKey(), signatureVersion);

            return Paymentwall_Widget.BASE_URL + "/" + this.buildController(this.widgetCode) + "?" + this.buildQueryString(parameters, "&");
        }


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
            var attributesQuery = this.buildQueryString(attributes, " ");
            return "<iframe src='" + this.getUrl() + "' " + attributesQuery + "></iframe>";
        }


        /**
         * Build controller URL depending on API type
         *
         * @param string widget code of the widget
         * @param bool flexibleCall
         * @return string
         */
        protected string buildController(string widget, bool flexibleCall = false)
        {
            if (Paymentwall_Widget.getApiType() == Paymentwall_Widget.API_VC)
            {
                if (!Regex.IsMatch(widget, @"^w|s|mw"))
                {
                    return Paymentwall_Widget.CONTROLLER_PAYMENT_VIRTUAL_CURRENCY;
                }
                else
                {
                    return "";
                }
            }
            else if (Paymentwall_Widget.getApiType() == Paymentwall_Widget.API_GOODS)
            {
                if (!flexibleCall)
                {
                    if (!Regex.IsMatch(widget, @"^w|s|mw"))
                    {
                        return Paymentwall_Widget.CONTROLELR_PAYMENT_DIGITAL_GOODS;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return Paymentwall_Widget.CONTROLELR_PAYMENT_DIGITAL_GOODS;
                }
            }
            else
            {
                return Paymentwall_Widget.CONTROLLER_PAYMENT_CART;
            }
        }


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

            if (version == Paymentwall_Widget.SIGNATURE_VERSION_1)
            {   //TODO: throw exception if no uid parameter is present 

                if (parameters["uid"] != null)
                    baseString += parameters["uid"];
                else
                    baseString += secret;
                return Paymentwall_Widget.getHash(baseString, "md5");
            }
            else
            {
				parameters = parameters.OrderBy(d => d.Key, StringComparer.Ordinal).ToDictionary(d => d.Key, d => d.Value);

                foreach (KeyValuePair<string, string> param in parameters)
                {
                    baseString += param.Key + "=" + param.Value;
                }
                baseString += secret;

                if (version == Paymentwall_Widget.SIGNATURE_VERSION_2)
                    return Paymentwall_Widget.getHash(baseString, "md5");
                else
                    return Paymentwall_Widget.getHash(baseString, "sha256");
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

			foreach(string key in dict.Keys)
			{
				if(count == dict.Count - 1) end = true;

				string escapedValue = Uri.EscapeDataString(dict[key]??string.Empty);
				if(end)
					queryString.AppendFormat("{0}={1}",key,escapedValue);
				else
					queryString.AppendFormat("{0}={1}{2}",key,escapedValue,s);

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
