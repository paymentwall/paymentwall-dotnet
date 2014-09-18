#About Paymentwall
[Paymentwall](http://paymentwall.com/?source=gh) is the leading digital payments platform for globally monetizing digital goods and services. Paymentwall assists game publishers, dating sites, rewards sites, SaaS companies and many other verticals to monetize their digital content and services. 
Merchants can plugin Paymentwall's API to accept payments from over 100 different methods including credit cards, debit cards, bank transfers, SMS/Mobile payments, prepaid cards, eWallets, landline payments and others. 

To sign up for a Paymentwall Merchant Account, [click here](http://paymentwall.com/signup/merchant?source=gh).

#Paymentwall C# Library
This library allows developers to use [Paymentwall APIs](http://paymentwall.com/en/documentation/API-Documentation/722?source=gh) (Virtual Currency, Digital Goods featuring recurring billing, and Virtual Cart).

To use Paymentwall, all you need to do is to sign up for a Paymentwall Merchant Account so you can setup a project designed for your site.
To open your merchant account and set up an project, you can [sign up here](http://paymentwall.com/signup/merchant?source=gh).

#Installation
The Paymentwall assembly provides integration access to the Paymentwall Gateway.
To download the library in your environment, you can download the [ZIP archive](https://github.com/paymentwall/paymentwall-csharp.git), build it to get dll package and place into your project, then make reference to it. 
In order to make reference, right click on you project and select Add Reference..., browsing Paymentwall dll package, then press OK button.

#Code Samples

##Digital Goods API

####Initializing Paymentwall
<pre><code>
using Paymentwall;

Paymentwall_Base.setApiType(Paymentwall_Base.API_GOODS);
Paymentwall_Base.setAppKey("YOUR_PROJECT_KEY"); // available in your Paymentwall merchant area
Paymentwall_Base.setSecretKey("YOUR_SECRET_KEY"); // available in your Paymentwall merchant area
</code></pre>

####Widget Call
[Web API details](http://www.paymentwall.com/en/documentation/Digital-Goods-API/710#paymentwall_widget_call_flexible_widget_call)

The widget is a payment page hosted by Paymentwall that embeds the entire payment flow: selecting the payment method, completing the billing details, and providing customer support via the Help section. You can redirect the users to this page or embed it via iframe. Below is an example that renders an iframe with Paymentwall Widget.

<pre><code>List<Paymentwall_Product> productList = new List<Paymentwall_Product>();
Paymentwall_Product product = new Paymentwall_Product("product301", 9.99f, "USD", "Gold", Paymentwall_Product.TYPE_FIXED, 1, Paymentwall_Product.PERIOD_TYPE_YEAR, true, null);
productList.Add(product);
Paymentwall_Widget widget = new Paymentwall_Widget(
	"user40012", // id of the end-user who's making the payment
	"p1_1", // widget code, e.g. p1; can be picked inside of your merchant account
	productList,
	new Dictionary<string, string>() {{"email", "user@hostname.com"}} // additional parameters
);
Response.Write(widget.getHtmlCode());
</code></pre>

####Pingback Processing

The Pingback is a webhook notifying about a payment being made. Pingbacks are sent via HTTP/HTTPS to your servers. To process pingbacks use the following code:
<pre><code>
NameValueCollection parameters = Request.QueryString;
Paymentwall_Pingback pingback = new Paymentwall_Pingback(parameters, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
if (pingback.validate())
{
	string productId = pingback.getProduct().getId();
	if (pingback.isDeliverable())
	{
		//deliver the product
	}
	else if (pingback.isCancelable())
	{
		//withdraw the product
	}
	Response.Write("OK"); // Paymentwall expects response to be OK, otherwise the pingback will be resent
}
else {                
	Response.Write(pingback.getErrorSummary());
}
</code></pre>

##Virtual Currency API

####Initializing Paymentwall
<pre><code>
using Paymentwall;

Paymentwall_Base.setApiType(Paymentwall_Base.API_VC);
Paymentwall_Base.setAppKey("YOUR_PROJECT_KEY"); // available in your Paymentwall merchant area
Paymentwall_Base.setSecretKey("YOUR_SECRET_KEY"); // available in your Paymentwall merchant area
</code></pre>

####Widget Call
<pre><code>
Paymentwall_Widget widget = new Paymentwall_Widget(
	"user40012", // id of the end-user who's making the payment
	"p1_1", // widget code, e.g. p1; can be picked inside of your merchant account	
	new List<Paymentwall_Product>(),	// leave here a blank list of product for Virtual Currency API
	new Dictionary<string, string>() {{"email", "user@hostname.com"}} // additional parameters
);
Response.Write(widget.getHtmlCode());
</code></pre>

####Pingback Processing

<pre><code>
NameValueCollection parameters = Request.QueryString;
Paymentwall_Pingback pingback = new Paymentwall_Pingback(parameters, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
if (pingback.validate())
{
	float virtualCurrency = float.Parse(pingback.getVirtualCurrencyAmount());
	if (pingback.isDeliverable())
	{
		// deliver the virtual currency
	} else 
	{
		// withdraw the virtual currency
	}
	Response.Write("OK"); // Paymentwall expects response to be OK, otherwise the pingback will be resent
}
else {
	Response.Write(pingback.getErrorSummary());
}
</code></pre>

##Cart API

####Initializing Paymentwall
<pre><code>
using Paymentwall;

Paymentwall_Base.setApiType(Paymentwall_Base.API_CART);
Paymentwall_Base.setAppKey("YOUR_PROJECT_KEY"); // available in your Paymentwall merchant area
Paymentwall_Base.setSecretKey("YOUR_SECRET_KEY"); // available in your Paymentwall merchant area
</code></pre>

####Widget Call
<pre><code>
List<Paymentwall_Product> productList = new List<Paymentwall_Product>();
Paymentwall_Product product1 = new Paymentwall_Product("product301", 3.33f, "EUR"); // first product in cart
Paymentwall_Product product2 = new Paymentwall_Product("product607", 7.77f, "EUR"); // second product in cart
productList.Add(product1);
productList.Add(product2);
Paymentwall_Widget widget = new Paymentwall_Widget(
	"user40012", // id of the end-user who's making the payment
	"p1_1", // widget code, e.g. p1; can be picked inside of your merchant account
	productList,
	new Dictionary<string, string>() {{"email", "user@hostname.com"}} // additional parameters
);
Response.Write(widget.getHtmlCode());
</code></pre>

####Pingback Processing

<pre><code>
NameValueCollection parameters = Request.QueryString;
Paymentwall_Pingback pingback = new Paymentwall_Pingback(parameters, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
if (pingback.validate())
{
	List<Paymentwall_Product> products = pingback.getProducts();
	if (pingback.isDeliverable())
	{
		// deliver products from the cart
	} else 
	{
		// withdraw products from the cart
	}
	Response.Write("OK"); // Paymentwall expects response to be OK, otherwise the pingback will be resent
}
else 
{
	Response.Write(pingback.getErrorSummary());
}
</code></pre>
