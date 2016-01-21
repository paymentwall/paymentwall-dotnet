using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paymentwall.Widgets
{
    public class ApiCartWidget : WidgetBase
    {
        public override int getDefaultSignatureVersion()
        {
            return WidgetBase.SIGNATURE_VERSION_2;
        }

        protected override void generateUrlParameters(ref Dictionary<string, string> parameters)
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

        protected override string buildController(string widget, bool flexibleCall = false)
        {
            return WidgetBase.CONTROLLER_PAYMENT_CART;
        }
    }
}
