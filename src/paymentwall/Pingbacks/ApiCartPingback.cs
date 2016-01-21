using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paymentwall.Pingbacks;

namespace Paymentwall.Pingbacks
{
    public class ApiCartPingback : PingbackBase
    {
        protected override List<string> getSignatureParams()
        {
            base.parameters["sign_version"] = SIGNATURE_VERSION_2.ToString();
            var signatureParams = new List<string> {"uid", "goodsid", "type", "ref"};

            return signatureParams;
        }

        protected override IEnumerable<string> getRequiredParams()
        {
            var requiredParams = new List<string> {"uid", "goodsid[0]", "type", "ref", "sig"};
            return requiredParams;
        }
    }
}
