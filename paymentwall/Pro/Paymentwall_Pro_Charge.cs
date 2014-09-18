using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Paymentwall.Pro
{
    public class Paymentwall_Pro_Charge
    {
        private Dictionary<string, string> properties = new Dictionary<string, string>();

        /**
         * Constructor
         * 
         * @param Dictionary<string, string> attr
         */
        public Paymentwall_Pro_Charge(Dictionary<string, string> attr)
        {
            Paymentwall_Pro_HttpWrapper result = new Paymentwall_Pro_HttpWrapper(attr);
            this.properties = result.post();
        }


        public Dictionary<string, string> getPublicData()
        {
            return Paymentwall_Pro_Error.getPublicData(this.properties);
        }


        public bool isCaptured()
        {
            bool b;
            if (this.properties.ContainsKey("captured") && !String.IsNullOrEmpty(this.properties["captured"]))
                if (bool.TryParse(this.properties["captured"], out b)) return b;
            return false;
        }


        public bool isRiskPending()
        {
            if (!String.IsNullOrEmpty(this.properties["risk"]))
                if (this.properties["risk"] == "pending") return true;
            return false;
        }


        public string getProperty(string propertyName)
        {
            if (this.properties[propertyName] != null)
                return this.properties[propertyName];
            return null;
        }


    }
}