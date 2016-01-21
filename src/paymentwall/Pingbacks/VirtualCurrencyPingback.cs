using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paymentwall.Pingbacks;

namespace paymentwall.Pingbacks
{
    public class VirtualCurrencyPingback : PingbackBase
    {
        protected override List<string> getSignatureParams()
        {
            var signatureParams = new List<string> {"uid", "currency", "type", "ref"};
            return signatureParams;
        }

        protected override IEnumerable<string> getRequiredParams()
        {
            var requiredParams = new List<string> { "uid", "currency", "type", "ref", "sig" };
            return requiredParams;
        }
    }
}
