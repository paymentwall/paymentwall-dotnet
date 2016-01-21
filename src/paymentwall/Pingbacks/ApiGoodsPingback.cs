using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paymentwall.Pingbacks;

namespace Paymentwall.Pingbacks
{
    public class ApiGoodsPingback : PingbackBase
    {
        protected override List<string> getSignatureParams()
        {
            var signatureParams = new List<string> { "uid", "goodsid", "slength", "speriod", "type", "ref" };
            return signatureParams;
        }

        protected override IEnumerable<string> getRequiredParams()
        {
            var requiredParams = new List<string> { "uid", "goodsid", "type", "ref", "sig" };
            return requiredParams;
        }
    }
}
