using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Paymentwall;

namespace Paymentwall.Widgets
{
    public class VirtualCurrencyWidget : WidgetBase
    {
        protected override void generateUrlParameters(ref Dictionary<string, string> parameters)
        {
            // nothing to do for virtual currency widgets
        }

        protected override string buildController(string widget, bool flexibleCall = false)
        {
            if (!Regex.IsMatch(widget, @"^w|s|mw"))
            {
                return WidgetBase.CONTROLLER_PAYMENT_VIRTUAL_CURRENCY;
            }
            else
            {
                return "";
            }
        }
    }
}
