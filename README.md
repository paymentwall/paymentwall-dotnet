# About Paymentwall
[Paymentwall](https://www.paymentwall.com/?source=gh) is the leading global payments platform.

Merchants can plugin Paymentwall's API to accept payments from over 150 different methods including credit cards, debit cards, bank transfers, SMS/Mobile payments, prepaid cards, eWallets, landline payments and others. 

To sign up for a Paymentwall Merchant Account, [click here](https://www.paymentwall.com/signup/merchant?source=gh).

# Paymentwall C# Library
This library allows developers to use [Paymentwall APIs](https://docs.paymentwall.com/?source=gh) (Digital Goods API, Cart API, Brick API, Virtual Currency API).

To use Paymentwall, all you need to do is to sign up for a Paymentwall Merchant Account so you can setup a project designed for your site.
To open your merchant account and set up an project, you can [sign up here](https://www.paymentwall.com/signup/merchant?source=gh).

# Installation
We recommend to use NuGet for installing Paymentwall library. To install Paymentwall, please run the following command in the Package Manager Console
```
PM> Install-Package Paymentwall
```

# Code Samples

## Digital Goods API

#### Initializing Paymentwall
```
using Paymentwall;

Paymentwall_Base.setApiType(Paymentwall_Base.API_GOODS);
Paymentwall_Base.setAppKey("YOUR_PROJECT_KEY"); // available in your Paymentwall merchant area
Paymentwall_Base.setSecretKey("YOUR_SECRET_KEY"); // available in your Paymentwall merchant area
```

#### Widget Call
[Checkout API details](https://docs.paymentwall.com/integration/checkout-home)

The widget is a payment page hosted by Paymentwall that embeds the entire payment flow: selecting the payment method, completing the billing details, and providing customer support via the Help section. You can redirect the users to this page or embed it via iframe. Below is an example that renders an iframe with Paymentwall Widget.
```
List<Paymentwall_Product> productList = new List<Paymentwall_Product>();
Paymentwall_Product product = new Paymentwall_Product(
								"product301", // id of the product in your system
								9.99f, // price
								"USD", // currency code
								"Gold Membership", // product name
								Paymentwall_Product.TYPE_SUBSCRIPTION, // this is a time-based product; for one-time products, use Paymentwall_Product.TYPE_FIXED and omit the following 3 parameters
								1, // time duration
								Paymentwall_Product.PERIOD_TYPE_YEAR, // year
								true // recurring
							);
productList.Add(product);
Paymentwall_Widget widget = new Paymentwall_Widget(
	"user40012", // id of the end-user who's making the payment
	"pw", // widget code, e.g. pw; can be picked inside of your merchant account
	productList,
	new Dictionary<string, string>() {{"email", "user@hostname.com"}} // additional parameters
);
Response.Write(widget.getHtmlCode());
```

#### Pingback Processing

The Pingback is a webhook notifying about a payment being made. Pingbacks are sent via HTTP/HTTPS to your servers. To process pingbacks use the following code:
```
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
```

## Virtual Currency API

#### Initializing Paymentwall
```
using Paymentwall;

Paymentwall_Base.setApiType(Paymentwall_Base.API_VC);
Paymentwall_Base.setAppKey("YOUR_PROJECT_KEY"); // available in your Paymentwall merchant area
Paymentwall_Base.setSecretKey("YOUR_SECRET_KEY"); // available in your Paymentwall merchant area
```

#### Widget Call
```
Paymentwall_Widget widget = new Paymentwall_Widget(
	"user40012", // id of the end-user who's making the payment
	"p1_1", // widget code, e.g. p1; can be picked inside of your merchant account
	new Dictionary<string, string>() {{"email", "user@hostname.com"}} // additional parameters
);
Response.Write(widget.getHtmlCode());
```

#### Pingback Processing

```
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
```

## Cart API

#### Initializing Paymentwall
```
using Paymentwall;

Paymentwall_Base.setApiType(Paymentwall_Base.API_CART);
Paymentwall_Base.setAppKey("YOUR_PROJECT_KEY"); // available in your Paymentwall merchant area
Paymentwall_Base.setSecretKey("YOUR_SECRET_KEY"); // available in your Paymentwall merchant area
```

#### Widget Call
```
List<Paymentwall_Product> productList = new List<Paymentwall_Product>();
productList.AddRange(
	new List<Paymentwall_Product>() {
		new Paymentwall_Product("product301", 3.33f, "EUR", "Product name 1"), //first product on cart
		new Paymentwall_Product("product607", 7.77f, "EUR", "Product name 2") //second product on cart
	}
);
Paymentwall_Widget widget = new Paymentwall_Widget(
	"user40012", // id of the end-user who's making the payment
	"p1_1", // widget code, e.g. p1; can be picked inside of your merchant account
	productList,
	new Dictionary<string, string>() {{"email", "user@hostname.com"}} // additional parameters
);
Response.Write(widget.getHtmlCode());
```

#### Pingback Processing

```
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
```
