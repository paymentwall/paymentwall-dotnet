using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Paymentwall.Pro
{
    public class Paymentwall_Pro_Error
    {
        public const string ERROR = "error";
        public const string ERROR_MESSAGE = "error";
        public const string ERROR_CODE = "code";

        const string RISK = "risk";
        const string RISK_PENDING = "pending";
        const string CLICK_ID = "id";
        const string SUPPORT_LINK = "support_link";


        /**
         * Error codes
         */
        public const string GENERAL_INTERNAL = "1000";
        public const string APPLICATION_NOT_LOADED = "1001";
        public const string CHARGE_NOT_FOUND = "3000";
        public const string CHARGE_PERMISSION_DENIED = "3001";
        public const string CHARGE_WRONG_AMOUNT = "3002";
        public const string CHARGE_WRONG_CARD_NUMBER = "3003";
        public const string CHARGE_WRONG_EXP_MONTH = "3004";
        public const string CHARGE_WRONG_EXP_YEAR = "3005";
        public const string CHARGE_WRONG_EXP_DATE = "3006";
        public const string CHARGE_WRONG_CURRENCY = "3007";
        public const string CHARGE_EMPTY_FIELDS = "3008";
        public const string CHARGE_WRONG_TOKEN = "3111";
        public const string CHARGE_CARD_NUMBER_ERROR = "3101";
        public const string CHARGE_CARD_NUMBER_EXPIRED = "3102";
        public const string CHARGE_UNSUPPORTED_CARD = "3103";
        public const string CHARGE_UNSUPPORTED_COUNTRY = "3104";
        public const string CHARGE_BILLING_ADDRESS_ERROR = "3009";
        public const string CHARGE_BANK_DECLINE = "3010";
        public const string CHARGE_INSUFFICIENT_FUNDS = "3011";
        public const string CHARGE_GATEWAY_DECLINE = "3012";
        public const string CHARGE_FRAUD_SUSPECTED = "3013";
        public const string CHARGE_FAILED = "3200";
        public const string CHARGE_ALREADY_REFUNDED = "3201";
        public const string CHARGE_CANCEL_FAILED = "3202";
        public const string CHARGE_ALREADY_CAPTURED = "3203";
        public const string CHARGE_REFUND_FAILED = "3204";
        public const string CHARGE_DUPLICATE = "3205";
        public const string CHARGE_AUTH_EXPIRED = "3206";
        public const string FIELD_FIRSTNAME = "3301";
        public const string FIELD_LASTNAME = "3302";
        public const string FIELD_ADDRESS = "3303";
        public const string FIELD_CITY = "3304";
        public const string FIELD_STATE = "3305";
        public const string FIELD_ZIP = "3306";
        public const string SUBSCRIPTION_WRONG_PERIOD = "3401";
        public const string SUBSCRIPTION_NOT_FOUND = "3402";
        public const string SUBSCRIPTION_WRONG_PERIOD_DURATION = "3403";
        public const string SUBSCRIPTION_MISSING_TRIAL_PARAMS = "3404";
        public const string SUBSCRIPTION_WRONG_TRIAL_PERIOD = "3405";
        public const string SUBSCRIPTION_WRONG_TRIAL_PERIOD_DURATION = "3406";
        public const string SUBSCRIPTION_WRONG_TRIAL_AMOUNT = "3407";
        public const string SUBSCRIPTION_WRONG_PAYMENTS_LIMIT = "3408";
        public const string API_UNDEFINED_METHOD = "4004";
        public const string API_EMPTY_REQUEST = "4005";
        public const string API_KEY_MISSED = "4006";
        public const string API_KEY_INVALID = "4007";
        public const string API_DECRYPTION_FAILED = "4008";
        public const string API_WRONG_SIGNATURE = "4009";
        public const string API_NOT_ACTIVATED = "4010";
        public const string USER_BANNED = "5000";
        public const string PARAMETER_WRONG_COUNTRY_CODE = "6001";


        /**
	     * Messages with fields to highlight in JavaScript library corresponding to error codes
	     */
        public static Dictionary<string, Dictionary<string, string>> messages = new Dictionary<string, Dictionary<string, string>>() { 
                {Paymentwall_Pro_Error.GENERAL_INTERNAL, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.APPLICATION_NOT_LOADED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_NOT_FOUND, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_PERMISSION_DENIED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_WRONG_AMOUNT, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_WRONG_CARD_NUMBER, new Dictionary<string, string>() {{"field", "cc-number"}}},
                {Paymentwall_Pro_Error.CHARGE_WRONG_EXP_DATE, new Dictionary<string, string>() {{"field", "cc-expiry"}}},
                {Paymentwall_Pro_Error.CHARGE_WRONG_EXP_MONTH, new Dictionary<string, string>() {{"field", "cc-expiry"}}},
                {Paymentwall_Pro_Error.CHARGE_WRONG_EXP_YEAR, new Dictionary<string, string>() {{"field", "cc-expiry"}}},
                {Paymentwall_Pro_Error.CHARGE_WRONG_CURRENCY, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_EMPTY_FIELDS, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_WRONG_TOKEN, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_CARD_NUMBER_ERROR, new Dictionary<string, string>() {{"field", "cc-number"}}},
                {Paymentwall_Pro_Error.CHARGE_CARD_NUMBER_EXPIRED, new Dictionary<string, string>() {{"field", "cc-number"}}},
                {Paymentwall_Pro_Error.CHARGE_UNSUPPORTED_CARD, new Dictionary<string, string>() {{"field", "cc-number"}}},
                {Paymentwall_Pro_Error.CHARGE_UNSUPPORTED_COUNTRY, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_BILLING_ADDRESS_ERROR, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_BANK_DECLINE, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_INSUFFICIENT_FUNDS, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_GATEWAY_DECLINE, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_FRAUD_SUSPECTED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_FAILED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_ALREADY_REFUNDED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_CANCEL_FAILED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_ALREADY_CAPTURED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_REFUND_FAILED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_DUPLICATE, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.CHARGE_AUTH_EXPIRED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.FIELD_FIRSTNAME, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.FIELD_LASTNAME, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.FIELD_ADDRESS, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.FIELD_CITY, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.FIELD_STATE, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.FIELD_ZIP, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.SUBSCRIPTION_WRONG_PERIOD, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.SUBSCRIPTION_NOT_FOUND, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.SUBSCRIPTION_WRONG_PERIOD_DURATION, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.SUBSCRIPTION_MISSING_TRIAL_PARAMS, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.SUBSCRIPTION_WRONG_TRIAL_PERIOD, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.SUBSCRIPTION_WRONG_TRIAL_PERIOD_DURATION, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.SUBSCRIPTION_WRONG_TRIAL_AMOUNT, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.SUBSCRIPTION_WRONG_PAYMENTS_LIMIT, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.API_UNDEFINED_METHOD, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.API_EMPTY_REQUEST, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.API_KEY_MISSED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.API_KEY_INVALID, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.API_DECRYPTION_FAILED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.API_WRONG_SIGNATURE, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.API_NOT_ACTIVATED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.USER_BANNED, new Dictionary<string, string>() {{"field", ""}}},
                {Paymentwall_Pro_Error.PARAMETER_WRONG_COUNTRY_CODE, new Dictionary<string, string>() {{"field", ""}}}
            };



        /**
         *
         */
        public static string getFieldFromMessages(string errorCode)
        {
            if (Paymentwall_Pro_Error.messages.ContainsKey(errorCode))
                return ((Dictionary<string, string>)Paymentwall_Pro_Error.messages[errorCode])["field"].ToString();
            return null;
        }


        /*
         * return bool
         */
        public static bool isError(Dictionary<string, string> response)
        {
            if (response.ContainsKey("type"))
            {
                if (response["type"] == "\"Error\"")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        /**
         * @param Dictionary response
         * @return Dictionary
         */
        public static Dictionary<string, string> wrapError(Dictionary<string, string> response)
        {
            return new Dictionary<string, string>() { { "error", (new JavaScriptSerializer()).Serialize(response) } };
        }


        /**
         * @param Dictionary response
         * @return Dictionary
         */
        public static Dictionary<string, string> wrapInternalError(Dictionary<string, string> response)
        {
            Dictionary<string, string> errorMsg = new Dictionary<string, string>() { { "message", "Sorry, internal error occuried" } };
            Dictionary<string, string> result = new Dictionary<string, string>() { 
                                    { "success", "0" }, 
                                    { "error", (new JavaScriptSerializer()).Serialize(errorMsg) } 
                                };
            return result;
        }


        /** 
         * @return Dictionary
         */
        public static Dictionary<string, string> getPublicData(Dictionary<string, string> properties)
        {
            if (properties.ContainsKey(Paymentwall_Pro_Error.ERROR) && !String.IsNullOrEmpty(properties[Paymentwall_Pro_Error.ERROR]))
            {
                var errDict = (Dictionary<string, object>)(new JavaScriptSerializer()).DeserializeObject(properties[Paymentwall_Pro_Error.ERROR]);

                if (errDict.ContainsKey(Paymentwall_Pro_Error.ERROR_CODE))
                {
                    Dictionary<string, string> error = new Dictionary<string, string>() 
                                            { 
                                                {"message", (errDict[Paymentwall_Pro_Error.ERROR_MESSAGE]).ToString()},
                                                {"field", Paymentwall_Pro_Error.getFieldFromMessages( (errDict[Paymentwall_Pro_Error.ERROR_CODE]).ToString()  )}
                                            };

                    return new Dictionary<string, string>()
                    {
                        {"success", "0"},
                        {"error", (new JavaScriptSerializer()).Serialize(error)}
                    };
                }
                else
                {
                    return new Dictionary<string, string>();
                }
            }
            else if (!String.IsNullOrEmpty(properties[Paymentwall_Pro_Error.RISK]) && properties[Paymentwall_Pro_Error.RISK] == Paymentwall_Pro_Error.RISK_PENDING)
            {
                return new Dictionary<string, string>()
                {
                    {"risk", "1"},
                    {"support_link", properties[Paymentwall_Pro_Error.SUPPORT_LINK]},
                    {"click_id", properties[Paymentwall_Pro_Error.CLICK_ID]}
                };
            }
            else
            {
                return new Dictionary<string, string>() { { "success", "1" } };
            }


        }


    }
}