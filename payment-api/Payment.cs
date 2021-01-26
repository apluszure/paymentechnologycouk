using System;
namespace payment_api
{
    public class Payment
    {
        public string authenticate_id { get; set; }
        public string authenticate_pw { get; set; }
        public string secret_key { get; set; }
        public string orderid { get; set; }
        public string transaction_type { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string card_info { get; set; }
        public string email { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string dob { get; set; }
        public string success_url { get; set; }
        public string fail_url { get; set; }
        public string notify_url { get; set; }
        public string customerip { get; set; }
        public string transaction_hash { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string card_number { get; set; }
        public string expiration_month { get; set; }
        public string expiration_year { get; set; }
        public string cvc { get; set; }
        public string signature { get; set; }
        public string type { get; set; }
    }
}
