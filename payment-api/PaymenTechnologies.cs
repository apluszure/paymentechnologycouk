using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace payment_api
{
    public class PaymenTechnologies
    {
        public Payment paymentParam { get; set; }

        public string api_url { get; set; }

        public string api_url_3DSv { get; set; }

        public string api_type { get; set; }

        public const string FormDataTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n";

        public PaymenTechnologies(Payment paymentParam)
        {
            this.paymentParam = paymentParam;
            this.api_url = "https://pay.paymentechnologies.co.uk/authorize_payment";
            this.api_url_3DSv = "https://pay.paymentechnologies.co.uk/authorize3dsv_payment";
            this.api_type = paymentParam.type;

            this.validatePayload();
        }

        public string Pay()
        {
            var url = "";
            if (this.paymentParam.type == "API")
            {
                url = this.api_url;
            } else if(this.paymentParam.type == "3DSV")
            {
                url = this.api_url_3DSv;
            }
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            string boundary = CreateFormDataBoundary();
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            Stream requestStream = webRequest.GetRequestStream();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("authenticate_id", this.paymentParam.authenticate_id);
            dic.Add("authenticate_pw", this.paymentParam.authenticate_pw);
            dic.Add("orderid", this.paymentParam.orderid);
            dic.Add("transaction_type", this.paymentParam.transaction_type);
            dic.Add("signature", this.paymentParam.signature);
            dic.Add("amount", this.paymentParam.amount);
            dic.Add("currency", this.paymentParam.currency);
            dic.Add("card_info", this.paymentParam.card_info);
            dic.Add("email", this.paymentParam.email);
            dic.Add("street", this.paymentParam.street);
            dic.Add("city", this.paymentParam.city);
            dic.Add("zip", this.paymentParam.zip);
            dic.Add("state", this.paymentParam.state);
            dic.Add("country", this.paymentParam.country);
            dic.Add("phone", this.paymentParam.phone);
            dic.Add("transaction_hash", this.paymentParam.transaction_hash);
            dic.Add("customerip", this.paymentParam.customerip);

            if (this.paymentParam.type == "3DSV")
            {
                dic.Add("dob", this.paymentParam.dob);
                dic.Add("success_url", this.paymentParam.success_url);
                dic.Add("fail_url", this.paymentParam.fail_url);
                dic.Add("notify_url", this.paymentParam.notify_url);
            }

            foreach (string key in dic.Keys)
            {
                string item = String.Format(FormDataTemplate, boundary, key, dic[key]);
                byte[] itemBytes = System.Text.Encoding.UTF8.GetBytes(item);
                requestStream.Write(itemBytes, 0, itemBytes.Length);
            }

            byte[] endBytes = System.Text.Encoding.UTF8.GetBytes("--" + boundary + "--");
            requestStream.Write(endBytes, 0, endBytes.Length);
            requestStream.Close();

            string result = "";
            using (WebResponse response = webRequest.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                 result = reader.ReadToEnd();
            };

            return result;
        }

        private string CreateFormDataBoundary()
        {
            return "---------------------------" + DateTime.Now.Ticks.ToString("x");
        }

        bool validatePayload()
        {
            return true;
        }
    }
}
