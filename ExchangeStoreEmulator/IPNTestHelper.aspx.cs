using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using Listener;
using System.Globalization;

namespace ExchangeStoreEmulator
{
    public partial class IPNTestHelper : System.Web.UI.Page
    {
        //These are just some test IPN notifications
        string subscriptonSignupNotificationTemplate = @"txn_type=subscr_signup&subscr_id=I-76SUA2Y7P5K5&last_name=buyer&residence_country=US&mc_currency=USD&item_name=Test+de+Gabriel+-64&business=exchange_seller%40autodesk.com&amount3=1.00&recurring=1&verify_sign=AeSBxYt3nvUbxfi3gYqXWPB11vZtAeOdrfZ4kPOgKx-.I0RDQDs85osN&payer_status=verified&test_ipn=1&payer_email={0}&first_name=williambuyer&receiver_email=exchange_seller%40autodesk.com&payer_id=PYAX39C7LTU3E&reattempt=1&item_number={1}&subscr_date={2}&custom=SDK1234&charset=windows-1252&notify_version=3.7&period3=1+D&mc_amount3=1.00&ipn_track_id=22f86b5f8d99f";
        string subscriptionPaymentNotificationTemplate = @"transaction_subject=Jay+Zhang+tech+solution&payment_date={0}&txn_type=subscr_payment&subscr_id=I-4N501KPSTAAR&last_name=buyer&residence_country=US&item_name=Jay+Zhang+tech+solution&payment_gross=1.00&mc_currency=USD&business=william_seller%40autodesk.com&payment_type=instant&protection_eligibility=Ineligible&verify_sign=Aiw6kvlhXvjgpgbG0jzxWckTd0RYATtHCvt0Kela22kmJ.7CcPbHpeS-&payer_status=verified&test_ipn=1&payer_email={1}&txn_id=4XB77198MM084732U&receiver_email=william_seller%40autodesk.com&first_name=williambuyer&payer_id=PYAX39C7LTU3E&receiver_id=X7QMNEWVLHP4S&item_number={2}&payment_status=Completed&payment_fee=0.33&mc_fee=0.33&mc_gross=1.00&custom=SDK1234&charset=windows-1252&notify_version=3.7&ipn_track_id=d94bf9a638d6b";
        string onetimeIpnNotificationTempate = @"transaction_subject=200703030249937&txn_type=web_accept&payment_date=23%3A36%3A36+Jan+11%2C+2014+PST&last_name=Balsom&residence_country=AU&item_name=RDBK_AutoNumber+Pro&payment_gross=5.50&mc_currency=USD&business=paypal@redbike.com.au&payment_type=instant&protection_eligibility=Ineligible&payer_status=verified&verify_sign=AFcWxV21C7fd0v3bYYYRCpSSRl31AsmAEVMnS38537K1tk5tZMnvtnW6&tax=0.50&payer_email=mbalsom@shoal.net.au&txn_id=0AG18756HD086633A&quantity=1&receiver_email=paypal@redbike.com.au&first_name=MARK&payer_id=NEH6BJPL9LBYG&receiver_id=GDGRD3PAZBMD8&item_number={0}&handling_amount=0.00&payment_status=Completed&payment_fee=0.43&mc_fee=0.43&shipping=0.00&mc_gross=5.50&custom=200703030249937&charset=windows-1252&notify_version=3.7&auth=A6P4OiUSwAL6901WUc3VK.fiUaYTR5AND5h.XpBaMqrI8gSmid.n0tFsfAMP6u3unDXUuiABwGtZWQlN.RFtDcA&form_charset=UTF-8&buyer_adsk_account={1}";

        static public string notification;

        protected void Page_Load(object sender, EventArgs e)
        {
            linkValidationIPN.Text = Request.Url.ToString().Replace("IPNTestHelper.aspx","ValidateIPN.aspx");
            
        }


        protected void btnBuy_Click(object sender, EventArgs e)
        {
           notification = string.Format(onetimeIpnNotificationTempate, 
                HttpUtility.UrlEncode(txtAppId.Text),
                HttpUtility.UrlEncode(txtBuyerEmail.Text));

            SendNotification(notification);

        }

        private void SendNotification(string notification)
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(txtIPNListener.Text);

            //send IPN notification to publishers IPN listener
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            string strRequest = notification;

            //Send the request to IPN listener and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();

            NetLog.WriteTextLog("IPNTestHelper: Sending notification IPN listener:" + strRequest);

            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();

            NetLog.WriteTextLog("IPNTestHelper: Get response from IPN listener:" + strResponse);
        }


        protected void btnSubscribe_Click(object sender, EventArgs e)
        {


            string d = DateTime.Now.ToString("HH:mm:ss MMM dd, yyyy PST", CultureInfo.CreateSpecificCulture("en-US"));

            string subSignupIPN = string.Format(subscriptonSignupNotificationTemplate,
                    HttpUtility.UrlEncode(txtBuyerEmail.Text),
                    HttpUtility.UrlEncode(txtAppId.Text),
                    HttpUtility.UrlEncode(DateTime.Now.ToLongDateString())
                    
                    );


            string paymentIPN = string.Format(subscriptionPaymentNotificationTemplate,
                    HttpUtility.UrlEncode(DateTime.Now.ToLongDateString()),
                     HttpUtility.UrlEncode(txtBuyerEmail.Text),
                    HttpUtility.UrlEncode(txtAppId.Text)
                    
                    );

       
            // do not assume the order you receive the notification is 
            // subscription sign up followed by payment when handling IPN. Actually the 
            // order of these two IPN notification may be different due
            // to internet latency.

            // so send at random order to mimic this.
            Random rand = new Random();
            if (rand.Next(10) > 5)
	        {
                SendNotification(subSignupIPN);
                notification = subSignupIPN;

                SendNotification(paymentIPN);
                notification = subSignupIPN;
	        }
            else
            {
                SendNotification(paymentIPN);
                notification = paymentIPN;

                SendNotification(subSignupIPN);
                notification = subSignupIPN;
            }


            


        }

        protected void btnRenew_Click(object sender, EventArgs e)
        {
            string paymentIPN = string.Format(subscriptionPaymentNotificationTemplate,
                   HttpUtility.UrlEncode(DateTime.Now.ToLongDateString()),
                    HttpUtility.UrlEncode(txtBuyerEmail.Text),
                   HttpUtility.UrlEncode(txtAppId.Text)

                   );

            SendNotification(paymentIPN);
        }
    }
}