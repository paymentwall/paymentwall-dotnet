using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Paymentwall.Pingbacks;

namespace Paymentwall.Pingbacks
{
    public abstract class PingbackBase : PaymentwallBase
    {
        /**
         * Pingback types
         */
        const int PINGBACK_TYPE_REGULAR = 0;
        const int PINGBACK_TYPE_GOODWILL = 1;
        const int PINGBACK_TYPE_NEGATIVE = 2;

        const int PINGBACK_TYPE_RISK_UNDER_REVIEW = 200;
        const int PINGBACK_TYPE_RISK_REVIEWED_ACCEPTED = 201;
        const int PINGBACK_TYPE_RISK_REVIEWED_DECLINED = 202;

        const int PINGBACK_TYPE_SUBSCRIPTION_CANCELLATION = 12;
        const int PINGBACK_TYPE_SUBSCRIPTION_EXPIRED = 13;
        const int PINGBACK_TYPE_SUBSCRIPTION_PAYMENT_FAILED = 14;


        protected abstract List<string> getSignatureParams();
        protected abstract IEnumerable<string> getRequiredParams();

        /**
         * Pingback parameters
         */
        protected Dictionary<string, string> parameters = new Dictionary<string, string>();


        /**
         * IP address
         */
        protected string ipAddress;

        /**
         * @param Dictionary<string, string> parameters associative array of parameters received by pingback processing script, e.g. Request.QueryString()
         * @param string ipAddress IP address from where the pingback request orginates, e.g. '127.0.0.1'
         */
        public static PingbackBase ParseByApiType(ApiTypes apiType, NameValueCollection parameters, string ipAddress)
        {
            PingbackBase pingback = null;
            if (apiType == ApiTypes.CART)
                pingback = new ApiCartPingback();
            else if (apiType == ApiTypes.GOODS)
                pingback = new ApiGoodsPingback();
            else if (apiType == ApiTypes.VC)
                pingback = new VirtualCurrencyPingback();
            else throw new Exception("Invalid Api Type passed");

            return initPingback(pingback, parameters, ipAddress);
        }

        public static PingbackBase ParseAuto(NameValueCollection parameters, string ipAddress)
        {
            PingbackBase pingback = null;
            if (parameters.AllKeys.Contains("currency"))
                pingback = new VirtualCurrencyPingback();
            else if (parameters.AllKeys.Contains("goodsid"))
                pingback = new ApiGoodsPingback();
            else if (parameters.AllKeys.Contains("goodsid[0]"))
                pingback = new ApiCartPingback();
            else
                throw new Exception("ApiType not set");

            return initPingback(pingback, parameters, ipAddress);
        }

        private static PingbackBase initPingback(PingbackBase pingback, NameValueCollection parameters, string ipAddress)
        {
            foreach (string p in parameters.AllKeys)
            {
                pingback.parameters.Add(p, parameters[p]);
            }
            pingback.ipAddress = ipAddress;

            return pingback;
        }


        /**
         * Check whether pingback is valid
         * 
         * @param bool skipIpWhiteListCheck if IP whitelist check should be skipped, e.g. if you have a load-balancer changing the IP
         * @return bool
         */
        public bool validate(bool skipIpWhiteListCheck = false)
        {
            bool validated = false;

            if (this.isParametersValid())
            {
                if (this.isIpAddressValid() || skipIpWhiteListCheck)
                {
                    if (this.isSignatureValid())
                    {
                        validated = true;
                    }
                    else
                    {
                        this.appendToErrors("Wrong signature");
                    }
                }
                else
                {
                    this.appendToErrors("IP address is not whitelisted");
                }
            }
            else
            {
                this.appendToErrors("Missing parameters");
            }

            return validated;
        }

        public bool isSignatureValid()
        {
            string signature = "";
            Dictionary<string, string> signatureParamsToSign = new Dictionary<string, string>();
            if (this.parameters.ContainsKey("sig"))
            {
                signature = this.parameters["sig"];
            }
            else
            {
                signature = null;
            }

            var signatureParams = getSignatureParams();
            if (!this.parameters.ContainsKey("sign_version")) //Check if signature version 1            
            {
                foreach (string field in signatureParams)
                {
                    if (this.parameters[field] != null)
                        signatureParamsToSign.Add(field, this.parameters[field]);
                    else
                        signatureParamsToSign.Add(field, null);
                }
                this.parameters["sign_version"] = SIGNATURE_VERSION_1.ToString();
            }
            else
            {
                signatureParamsToSign = this.parameters;
            }

            string signatureCalculated = this.calculateSignature(signatureParamsToSign, SecretKey, Convert.ToInt32(this.parameters["sign_version"]));

            return signatureCalculated == signature;
        }


        /**
         * @return bool
         */
        public bool isIpAddressValid()
        {
            List<string> ipWhitelist = new List<string>() { 
                "174.36.92.186",
			    "174.36.96.66",
			    "174.36.92.187",
			    "174.36.92.192",
			    "174.37.14.28" 
            };
            return ipWhitelist.Contains(this.ipAddress);
        }


        /**
         * @return bool 
         */
        public bool isParametersValid()
        {
            int errorsNumber = 0;
            var requiredParams = getRequiredParams();

            foreach (string field in requiredParams)
            {
                if (!this.parameters.ContainsKey(field) || String.IsNullOrWhiteSpace(this.parameters[field]))
                {
                    this.appendToErrors("Parameter " + field + " is missing");
                    errorsNumber++;
                }
            }

            return errorsNumber == 0;
        }

        


        /**
         * Get pingback parameter
         * 
         * @param string param
         * @return string
         */
        public string getParameter(string param)
        {
            if (!String.IsNullOrWhiteSpace(this.parameters[param]))
                return this.parameters[param];
            else
                return null;
        }


        /**
         * Get pingback parameter "type"
         * 
         * @return int
         */
        public int getPingbackType()
        {   //changed to getPingbackType() to avoid duplicate name with C# method getType()
            if (this.parameters["type"] != null)
                return Convert.ToInt32(this.parameters["type"]);
            else
                return -1;
        }


        /**
         * Get verbal explanation of the informational pingback
         * 
         * @return string
         */
        public string getTypeVerbal()
        {
            Dictionary<string, string> pingbackTypes = new Dictionary<string, string>();
            pingbackTypes.Add(PINGBACK_TYPE_SUBSCRIPTION_CANCELLATION.ToString(), "user_subscription_cancellation");
            pingbackTypes.Add(PINGBACK_TYPE_SUBSCRIPTION_EXPIRED.ToString(), "user_subscription_expired");
            pingbackTypes.Add(PINGBACK_TYPE_SUBSCRIPTION_PAYMENT_FAILED.ToString(), "user_subscription_payment_failed");

            if (!String.IsNullOrWhiteSpace(this.parameters["type"]))
            {
                if (pingbackTypes.ContainsKey(this.parameters["type"]))
                    return pingbackTypes[this.parameters["type"]];
                else
                    return null;
            }
            else
            {
                return null;
            }
        }


        /**
         * Get pingback parameter "uid"
         * 
         * @return string
         */
        public string getUserId()
        {
            return this.getParameter("uid");
        }


        /**
         * Get pingback parameter "currency"
         * 
         * @return string
         */
        public string getVirtualCurrencyAmount()
        {
            return this.getParameter("currency");
        }


        /**
         * Get product id
         * 
         * @return string
         */
        public string getProductId()
        {
            return this.getParameter("goodsid");
        }


        /**
         * @return int
         */
        public int getProductPeriodLength()
        {
            return Convert.ToInt32(this.getParameter("slength"));
        }


        /*
         * @return string
         */
        public string getProductPeriodType()
        {
            return this.getParameter("speriod");
        }


        /*
         *  @return Paymentwall_Product 
         */
        public PaymentwallProduct getProduct()
        {
            string productType = (this.getProductPeriodLength() > 0 ? PaymentwallProduct.TYPE_SUBSCRIPTION : PaymentwallProduct.TYPE_FIXED);

            PaymentwallProduct product = new PaymentwallProduct(
                    this.getProductId(),
                    0,
                    null,
                    null,
                    productType,
                    this.getProductPeriodLength(),
                    this.getProductPeriodType()
            );

            return product;
        }


        /*
         * @return List<Paymentwall_Product>
         */
        public List<PaymentwallProduct> getProducts()
        {
            List<PaymentwallProduct> products = new List<PaymentwallProduct>();
            List<string> productIds = new List<string>();

            foreach (var productId in this.parameters["goodsid"])
            {
                productIds.Add(productId.ToString());
            }

            if (productIds.Any())
            {
                foreach (string id in productIds)
                {
                    products.Add(new PaymentwallProduct(id));
                }
            }

            return products;
        }


        /*
         * Get pingback parameter "ref"
         * 
         * @return string
         */
        public string getReferenceId()
        {
            return this.getParameter("ref");
        }


        /*
         * Returns unique identifier of the pingback that can be used for checking
         * If the same pingback was already processed by your servers
         * Two pingbacks with the same unique ID should not be processed more than once
         * 
         * @return string
         */
        public string getPingbackUniqueId()
        {
            return this.getReferenceId() + "_" + this.getPingbackType();
        }


        /*
         * Check wheter product is deliverable
         * 
         * @return bool
         */
        public bool isDeliverable()
        {
            return (
              this.getPingbackType() == PINGBACK_TYPE_REGULAR ||
              this.getPingbackType() == PINGBACK_TYPE_GOODWILL ||
              this.getPingbackType() == PINGBACK_TYPE_RISK_REVIEWED_ACCEPTED
            );
        }


        /*
         * Check wheter product is cancelable
         * 
         * @return bool
         */
        public bool isCancelable()
        {
            return (
                this.getPingbackType() == PINGBACK_TYPE_NEGATIVE ||
                this.getPingbackType() == PINGBACK_TYPE_RISK_REVIEWED_DECLINED
            );
        }


        /*
         * Check whether product is under review
         * 
         * @return bool
         */
        public bool isUnderReview()
        {
            return this.getPingbackType() == PINGBACK_TYPE_RISK_UNDER_REVIEW;
        }


        /*
         * Build signature for the pingback received
         * 
         * @param Dictionary<string, string> parameters
         * @param string secret Paymentwall Secret Key
         * @param int version Paymentwall Signature Version
         * @return string
         */
        public string calculateSignature(Dictionary<string, string> signatureParamsToSign, string secret, int version)
        {
            string baseString = "";
            signatureParamsToSign.Remove("sig");

            if (version == SIGNATURE_VERSION_2 || version == SIGNATURE_VERSION_3)
            {
                signatureParamsToSign = signatureParamsToSign.OrderBy(d => d.Key, StringComparer.Ordinal).ToDictionary(d => d.Key, d => d.Value);
            }

            foreach (KeyValuePair<string, string> kvp in signatureParamsToSign)
            {
                baseString += kvp.Key + "=" + kvp.Value;
            }
            baseString += secret;

            if (version == SIGNATURE_VERSION_3)
            {
                return getHash(baseString, "sha256");
            }
            else
            {
                return getHash(baseString, "md5");
            }
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





    }
}