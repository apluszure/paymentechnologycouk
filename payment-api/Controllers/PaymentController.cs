using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace payment_api.Controllers
{
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        // POST api/values
        [HttpPost]
        public JsonResult Post(Payment value)
        {
            //value.authenticate_id = "3616055c9aef906320afd0621cb309bb";
            //value.authenticate_pw = "0cf86254452d38e1513dcc7e36267fdd";
            //value.secret_key = "5e181e41ebb8d0.80799555";
            //value.firstname = "John";
            //value.lastname = "Smith";
            //value.card_number = "4111111111111111";
            //value.expiration_month = 12;
            //value.expiration_year = 24;
            //value.cvc = 465;
            //value.orderid = "62879";
            //value.transaction_type = "A";
            //value.amount = "1.00";
            //value.currency = "USD";
            //value.card_info = "";
            //value.email = "test44@sample.com";
            //value.street = "1600 Amphitheatre Parkway";
            //value.city = "Mountain View";
            //value.zip = "94043";
            //value.state = "CA";
            //value.country = "USA";
            //value.phone = "+12345345345";
            //value.transaction_hash = "zqT5fqc3V1yIrFeE0rPjkaF2wSmldT6p5AXD8Qho5ONTINGw6nUiji79HEq4iI70n4gczl8fbusXhz5r8rmTRg==";
            //value.customerip = "127.0.0.1";

            Card card = new Card(
                value.firstname,
                value.lastname,
                value.card_number,
                value.expiration_month,
                value.expiration_year,
                value.cvc,
                value.secret_key
             );

            value.card_info = card.EncryptCardInfo();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            // signature segment information
            dic.Add("authenticate_id", value.authenticate_id);
            dic.Add("authenticate_pw", value.authenticate_pw);
            dic.Add("orderid", value.orderid);
            dic.Add("transaction_type", value.transaction_type);
            dic.Add("amount", value.amount.ToString());
            dic.Add("currency", value.currency);
            dic.Add("card_info", value.card_info);
            dic.Add("email", value.email);
            dic.Add("street", value.street);
            dic.Add("city", value.city);
            dic.Add("zip", value.zip);
            dic.Add("state", value.state);
            dic.Add("country", value.country);
            dic.Add("phone", value.phone);
            //dic.Add("transaction_hash", value.transaction_hash);
            dic.Add("customerip", value.customerip);
            // end signature segment information

            // additional fields,  3dsv
            if (value.type == "3DSV")
            {
                dic.Add("dob", value.dob);
                //dic.Add("success_url", WebUtility.UrlEncode(value.success_url));
                //dic.Add("fail_url", WebUtility.UrlEncode(value.fail_url));
                //dic.Add("notify_url", WebUtility.UrlEncode(value.notify_url));
                dic.Add("success_url", HttpUtility.UrlEncode(HttpUtility.UrlEncode(value.success_url, Encoding.UTF8))); //.UrlEncode(this.paymentParam.success_url));
                dic.Add("fail_url", HttpUtility.UrlEncode(HttpUtility.UrlEncode(value.fail_url, Encoding.UTF8))); //.UrlEncode(this.paymentParam.success_url));
                dic.Add("notify_url", HttpUtility.UrlEncode(HttpUtility.UrlEncode(value.notify_url, Encoding.UTF8))); //.UrlEncode(this.paymentParam.success_url));
            }

            // only the above list need to calculate signature
            value.signature = card.Signature(dic);


            PaymenTechnologies pt = new PaymenTechnologies(value);
            var result = pt.Pay();

            return Json(result);

        }

    }
}
