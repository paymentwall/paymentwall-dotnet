using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Paymentwall;
using Paymentwall.Widgets;

namespace Paymentwall.Widgets
{
    public class ApiGoodsWidget : WidgetBase
    {
        protected override void generateUrlParameters(ref Dictionary<string, string> parameters)
        {
            var productsNumber = this.products.Count;
            if (productsNumber > 0)
            {
                if (productsNumber == 1)
                {
                    PaymentwallProduct product = this.products[0];
                    PaymentwallProduct postTrialProduct = null;
                    if (product.getTrialProduct() != null)
                    {
                        postTrialProduct = product;
                        product = product.getTrialProduct();
                    }
                    parameters.Add("amount", product.getAmount().ToString());
                    parameters.Add("currencyCode", product.getCurrencyCode());
                    parameters.Add("ag_name", product.getName());
                    parameters.Add("ag_external_id", product.getId());
                    parameters.Add("ag_type", product.getType());

                    if (product.getType() == PaymentwallProduct.TYPE_SUBSCRIPTION)
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

        protected override string buildController(string widget, bool flexibleCall = false)
        {
            if (!flexibleCall)
            {
                if (!Regex.IsMatch(widget, @"^w|s|mw"))
                {
                    return WidgetBase.CONTROLELR_PAYMENT_DIGITAL_GOODS;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return WidgetBase.CONTROLELR_PAYMENT_DIGITAL_GOODS;
            }
        }
    }
}
