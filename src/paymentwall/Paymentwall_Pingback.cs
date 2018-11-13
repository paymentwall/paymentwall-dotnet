using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Paymentwall
{
    public class Paymentwall_Pingback : Paymentwall_Base
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

        const int PINGBACK_TYPE_RISK_AUTHORIZATION_VOIDED = 203;

        const int PINGBACK_TYPE_SUBSCRIPTION_CANCELLATION = 12;
        const int PINGBACK_TYPE_SUBSCRIPTION_EXPIRED = 13;
        const int PINGBACK_TYPE_SUBSCRIPTION_PAYMENT_FAILED = 14;


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
        public Paymentwall_Pingback(NameValueCollection parameters, string ipAddress)
        {
            foreach (string p in parameters.AllKeys)
            {
                this.parameters.Add(p, parameters[p]);
            }
            this.ipAddress = ipAddress;
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


        /**
         * @return bool
         */
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

            List<string> signatureParams = new List<string>();

            if (Paymentwall_Pingback.getApiType() == Paymentwall_Pingback.API_VC)
            {
                signatureParams.AddRange(new string[] { "uid", "currency", "type", "ref" });
            }
            else if (Paymentwall_Pingback.getApiType() == Paymentwall_Pingback.API_GOODS)
            {
                signatureParams.AddRange(new string[] { "uid", "goodsid", "slength", "speriod", "type", "ref" });
            }
            else
            { //API_CART
                signatureParams.AddRange(new string[] { "uid", "goodsid", "type", "ref" });
                this.parameters["sign_version"] = Paymentwall_Pingback.SIGNATURE_VERSION_2.ToString();
            }

            if (!this.parameters.ContainsKey("sign_version")) //Check if signature version 1            
            {
                foreach (string field in signatureParams)
                {
                    if (this.parameters[field] != null)
                        signatureParamsToSign.Add(field, this.parameters[field]);
                    else
                        signatureParamsToSign.Add(field, null);
                }
                this.parameters["sign_version"] = Paymentwall_Pingback.SIGNATURE_VERSION_1.ToString();
            }
            else
            {
                signatureParamsToSign = this.parameters;
            }

            string signatureCalculated = this.calculateSignature(signatureParamsToSign, Paymentwall_Pingback.getSecretKey(), Convert.ToInt32(this.parameters["sign_version"]));

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
                "174.37.14.28",
                "216.127.71.0",
                "216.127.71.1",
                "216.127.71.2",
                "216.127.71.3",
                "216.127.71.4",
                "216.127.71.5",
                "216.127.71.6",
                "216.127.71.7",
                "216.127.71.8",
                "216.127.71.9",
                "216.127.71.10",
                "216.127.71.11",
                "216.127.71.12",
                "216.127.71.13",
                "216.127.71.14",
                "216.127.71.15",
                "216.127.71.16",
                "216.127.71.17",
                "216.127.71.18",
                "216.127.71.19",
                "216.127.71.20",
                "216.127.71.21",
                "216.127.71.22",
                "216.127.71.23",
                "216.127.71.24",
                "216.127.71.25",
                "216.127.71.26",
                "216.127.71.27",
                "216.127.71.28",
                "216.127.71.29",
                "216.127.71.30",
                "216.127.71.31",
                "216.127.71.32",
                "216.127.71.33",
                "216.127.71.34",
                "216.127.71.35",
                "216.127.71.36",
                "216.127.71.37",
                "216.127.71.38",
                "216.127.71.39",
                "216.127.71.40",
                "216.127.71.41",
                "216.127.71.42",
                "216.127.71.43",
                "216.127.71.44",
                "216.127.71.45",
                "216.127.71.46",
                "216.127.71.47",
                "216.127.71.48",
                "216.127.71.49",
                "216.127.71.50",
                "216.127.71.51",
                "216.127.71.52",
                "216.127.71.53",
                "216.127.71.54",
                "216.127.71.55",
                "216.127.71.56",
                "216.127.71.57",
                "216.127.71.58",
                "216.127.71.59",
                "216.127.71.60",
                "216.127.71.61",
                "216.127.71.62",
                "216.127.71.63",
                "216.127.71.64",
                "216.127.71.65",
                "216.127.71.66",
                "216.127.71.67",
                "216.127.71.68",
                "216.127.71.69",
                "216.127.71.70",
                "216.127.71.71",
                "216.127.71.72",
                "216.127.71.73",
                "216.127.71.74",
                "216.127.71.75",
                "216.127.71.76",
                "216.127.71.77",
                "216.127.71.78",
                "216.127.71.79",
                "216.127.71.80",
                "216.127.71.81",
                "216.127.71.82",
                "216.127.71.83",
                "216.127.71.84",
                "216.127.71.85",
                "216.127.71.86",
                "216.127.71.87",
                "216.127.71.88",
                "216.127.71.89",
                "216.127.71.90",
                "216.127.71.91",
                "216.127.71.92",
                "216.127.71.93",
                "216.127.71.94",
                "216.127.71.95",
                "216.127.71.96",
                "216.127.71.97",
                "216.127.71.98",
                "216.127.71.99",
                "216.127.71.100",
                "216.127.71.101",
                "216.127.71.102",
                "216.127.71.103",
                "216.127.71.104",
                "216.127.71.105",
                "216.127.71.106",
                "216.127.71.107",
                "216.127.71.108",
                "216.127.71.109",
                "216.127.71.110",
                "216.127.71.111",
                "216.127.71.112",
                "216.127.71.113",
                "216.127.71.114",
                "216.127.71.115",
                "216.127.71.116",
                "216.127.71.117",
                "216.127.71.118",
                "216.127.71.119",
                "216.127.71.120",
                "216.127.71.121",
                "216.127.71.122",
                "216.127.71.123",
                "216.127.71.124",
                "216.127.71.125",
                "216.127.71.126",
                "216.127.71.127",
                "216.127.71.128",
                "216.127.71.129",
                "216.127.71.130",
                "216.127.71.131",
                "216.127.71.132",
                "216.127.71.133",
                "216.127.71.134",
                "216.127.71.135",
                "216.127.71.136",
                "216.127.71.137",
                "216.127.71.138",
                "216.127.71.139",
                "216.127.71.140",
                "216.127.71.141",
                "216.127.71.142",
                "216.127.71.143",
                "216.127.71.144",
                "216.127.71.145",
                "216.127.71.146",
                "216.127.71.147",
                "216.127.71.148",
                "216.127.71.149",
                "216.127.71.150",
                "216.127.71.151",
                "216.127.71.152",
                "216.127.71.153",
                "216.127.71.154",
                "216.127.71.155",
                "216.127.71.156",
                "216.127.71.157",
                "216.127.71.158",
                "216.127.71.159",
                "216.127.71.160",
                "216.127.71.161",
                "216.127.71.162",
                "216.127.71.163",
                "216.127.71.164",
                "216.127.71.165",
                "216.127.71.166",
                "216.127.71.167",
                "216.127.71.168",
                "216.127.71.169",
                "216.127.71.170",
                "216.127.71.171",
                "216.127.71.172",
                "216.127.71.173",
                "216.127.71.174",
                "216.127.71.175",
                "216.127.71.176",
                "216.127.71.177",
                "216.127.71.178",
                "216.127.71.179",
                "216.127.71.180",
                "216.127.71.181",
                "216.127.71.182",
                "216.127.71.183",
                "216.127.71.184",
                "216.127.71.185",
                "216.127.71.186",
                "216.127.71.187",
                "216.127.71.188",
                "216.127.71.189",
                "216.127.71.190",
                "216.127.71.191",
                "216.127.71.192",
                "216.127.71.193",
                "216.127.71.194",
                "216.127.71.195",
                "216.127.71.196",
                "216.127.71.197",
                "216.127.71.198",
                "216.127.71.199",
                "216.127.71.200",
                "216.127.71.201",
                "216.127.71.202",
                "216.127.71.203",
                "216.127.71.204",
                "216.127.71.205",
                "216.127.71.206",
                "216.127.71.207",
                "216.127.71.208",
                "216.127.71.209",
                "216.127.71.210",
                "216.127.71.211",
                "216.127.71.212",
                "216.127.71.213",
                "216.127.71.214",
                "216.127.71.215",
                "216.127.71.216",
                "216.127.71.217",
                "216.127.71.218",
                "216.127.71.219",
                "216.127.71.220",
                "216.127.71.221",
                "216.127.71.222",
                "216.127.71.223",
                "216.127.71.224",
                "216.127.71.225",
                "216.127.71.226",
                "216.127.71.227",
                "216.127.71.228",
                "216.127.71.229",
                "216.127.71.230",
                "216.127.71.231",
                "216.127.71.232",
                "216.127.71.233",
                "216.127.71.234",
                "216.127.71.235",
                "216.127.71.236",
                "216.127.71.237",
                "216.127.71.238",
                "216.127.71.239",
                "216.127.71.240",
                "216.127.71.241",
                "216.127.71.242",
                "216.127.71.243",
                "216.127.71.244",
                "216.127.71.245",
                "216.127.71.246",
                "216.127.71.247",
                "216.127.71.248",
                "216.127.71.249",
                "216.127.71.250",
                "216.127.71.251",
                "216.127.71.252",
                "216.127.71.253",
                "216.127.71.254",
                "216.127.71.255"
            };
            return ipWhitelist.Contains(this.ipAddress);
        }


        /**
         * @return bool 
         */
        public bool isParametersValid()
        {
            int errorsNumber = 0;
            List<string> requiredParams = new List<string>();

            if (Paymentwall_Pingback.getApiType() == Paymentwall_Pingback.API_VC)
            {
                requiredParams.AddRange(new string[] { "uid", "currency", "type", "ref", "sig" });
            }
            else if (Paymentwall_Pingback.getApiType() == Paymentwall_Pingback.API_GOODS)
            {
                requiredParams.AddRange(new string[] { "uid", "goodsid", "type", "ref", "sig" });
            }
            else
            { //API_CART
                requiredParams.AddRange(new string[] { "uid", "goodsid[0]", "type", "ref", "sig" });
            }

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
            pingbackTypes.Add(Paymentwall_Pingback.PINGBACK_TYPE_SUBSCRIPTION_CANCELLATION.ToString(), "user_subscription_cancellation");
            pingbackTypes.Add(Paymentwall_Pingback.PINGBACK_TYPE_SUBSCRIPTION_EXPIRED.ToString(), "user_subscription_expired");
            pingbackTypes.Add(Paymentwall_Pingback.PINGBACK_TYPE_SUBSCRIPTION_PAYMENT_FAILED.ToString(), "user_subscription_payment_failed");

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
        public Paymentwall_Product getProduct()
        {
            string productType = (this.getProductPeriodLength() > 0 ? Paymentwall_Product.TYPE_SUBSCRIPTION : Paymentwall_Product.TYPE_FIXED);

            Paymentwall_Product product = new Paymentwall_Product(
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
        public List<Paymentwall_Product> getProducts()
        {
            List<Paymentwall_Product> products = new List<Paymentwall_Product>();
            List<string> productIds = new List<string>();

            foreach (var productId in this.parameters["goodsid"])
            {
                productIds.Add(productId.ToString());
            }

            if (productIds.Any())
            {
                foreach (string id in productIds)
                {
                    products.Add(new Paymentwall_Product(id));
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
              this.getPingbackType() == Paymentwall_Pingback.PINGBACK_TYPE_REGULAR ||
              this.getPingbackType() == Paymentwall_Pingback.PINGBACK_TYPE_GOODWILL ||
              this.getPingbackType() == Paymentwall_Pingback.PINGBACK_TYPE_RISK_REVIEWED_ACCEPTED
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
                this.getPingbackType() == Paymentwall_Pingback.PINGBACK_TYPE_NEGATIVE ||
                this.getPingbackType() == Paymentwall_Pingback.PINGBACK_TYPE_RISK_REVIEWED_DECLINED
            );
        }


        /*
         * Check whether product is under review
         * 
         * @return bool
         */
        public bool isUnderReview()
        {
            return this.getPingbackType() == Paymentwall_Pingback.PINGBACK_TYPE_RISK_UNDER_REVIEW;
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

            if (version == Paymentwall_Pingback.SIGNATURE_VERSION_2 || version == Paymentwall_Pingback.SIGNATURE_VERSION_3)
            {
                signatureParamsToSign = signatureParamsToSign.OrderBy(d => d.Key, StringComparer.Ordinal).ToDictionary(d => d.Key, d => d.Value);
            }

            foreach (KeyValuePair<string, string> kvp in signatureParamsToSign)
            {
                baseString += kvp.Key + "=" + kvp.Value;
            }
            baseString += secret;

            if (version == Paymentwall_Pingback.SIGNATURE_VERSION_3)
            {
                return Paymentwall_Pingback.getHash(baseString, "sha256");
            }
            else
            {
                return Paymentwall_Pingback.getHash(baseString, "md5");
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
