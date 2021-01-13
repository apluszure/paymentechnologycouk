using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace payment_api
{

    public class Card
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string card_number { get; set; }
        public string expiration_month { get; set; }
        public string expiration_year { get; set; }
        public string cvc { get; set; }
        public string secret_key { get; set; }

        public Card(string firstname, string lastname, string card_number, string expiration_month, string expiration_year, string cvc, string secret_key)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.card_number = card_number;
            this.expiration_month = expiration_month;
            this.expiration_year = expiration_year;
            this.cvc = cvc;
            this.secret_key = secret_key;
        }

        private static string Encryptor(string TextToEncrypt, byte[] key, string strIV)
        {
            //Turn the plaintext into a byte array.
            byte[] PlainTextBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(TextToEncrypt);

            //Setup the AES providor for our purposes.
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();

            if (key.Length < 32)
            {
                var paddedkey = new byte[32];
                Buffer.BlockCopy(key, 0, paddedkey, 0, key.Length);
                key = paddedkey;
            }
            aesProvider.BlockSize = 128;
            aesProvider.KeySize = 256;
            //My key and iv that i have used in openssl
            aesProvider.Key = key;
            aesProvider.IV = System.Text.Encoding.ASCII.GetBytes(strIV);
            aesProvider.Padding = PaddingMode.PKCS7;
            aesProvider.Mode = CipherMode.CBC;

            ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor(aesProvider.Key, aesProvider.IV);
            byte[] EncryptedBytes = cryptoTransform.TransformFinalBlock(PlainTextBytes, 0, PlainTextBytes.Length);
            return Convert.ToBase64String(EncryptedBytes);
        }

        public string EncryptCardInfo()
        {
            string expiry = string.Format("{0}/{1}",this.expiration_month, this.expiration_year);
            string payload = string.Format("ccn||{0}__expire||{1}__cvc||{2}__firstname||{3}__lastname||{4}",
                this.card_number,
                expiry,
                this.cvc,
                this.firstname,
                this.lastname);

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");

            string newSecret = rgx.Replace(this.secret_key, "").Substring(0, 16);

            string ivString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            //byte[] iv = System.Text.Encoding.ASCII.GetBytes(ivString);
            byte[] key = System.Text.Encoding.ASCII.GetBytes(newSecret);
            string encrypted = Encryptor(payload, key, ivString);
            return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(encrypted + "::" + ivString));
        }

        public string Signature(Dictionary<string, string> dic)
        {
            StringBuilder sb = new StringBuilder();
            var list = dic.Keys.ToList();
            list.Sort();

            foreach (string key in list)
            {
                sb.Append(dic[key]);
            }

            sb.Append(this.secret_key);

            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}
