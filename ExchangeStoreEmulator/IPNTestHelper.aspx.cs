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
        static public string notification;

        protected void Page_Load(object sender, EventArgs e)
        {
            linkValidationIPN.Text = Request.Url.ToString().Replace("IPNTestHelper.aspx", "ValidateIPN.aspx");

        }


        protected void btnBuy_Click(object sender, EventArgs e)
        {
            //note following parameters
            //payer_status=verified
            //item_number=appstore.exchange.autodesk.com%3Ardbk_autonumberpro_windows32and64%3Aen
            //payment_status=Completed
            //buyer_adsk_account=buyername@buyercompany.com
            string onetimeIpnNotificationTempate = @"transaction_subject=200703030249937" +
                "&txn_type=web_accept&payment_date=23%3A36%3A36+Jan+11%2C+2014+PST" +
                "&last_name=Balsom&residence_country=AU&item_name=RDBK_AutoNumber+Pro" +
                "&payment_gross=5.50&mc_currency=USD&business=paypal@company.com.au" +
                "&payment_type=instant&protection_eligibility=Ineligible" +
                "&payer_status=verified&verify_sign=AFcWxV21C7fd0v3bYYYRCpSSRl31AsmAEVMnS38537K1tk5tZMnvtnW6" +
                "&tax=0.50&payer_email=name@company.net.au&txn_id=0AG18756HD086633A" +
                "&quantity=1&receiver_email=paypal@company.com.au&first_name=MARK" +
                "&payer_id=NEH6BJPL9LBYG&receiver_id=GDGRD3PAZBMD8" +
                "&item_number={0}" +
                "&handling_amount=0.00" +
                "&payment_status=Completed" +
                "&payment_fee=0.43&mc_fee=0.43&shipping=0.00&mc_gross=5.50" +
                "&custom=200703030249937&charset=windows-1252&notify_version=3.7" +
                "&auth=A6P4OiUSwAL6901WUc3VK.fiUaYTR5AND5h.XpBaMqrI8gSmid.n0tFsfAMP6u3unDXUuiABwGtZWQlN.RFtDcA" +
                "&form_charset=UTF-8" +
                "&buyer_adsk_account={1}";

            notification = string.Format(onetimeIpnNotificationTempate,
                 HttpUtility.UrlEncode(txtAppId.Text),
                 HttpUtility.UrlEncode(txtBuyerEmail.Text));

            SendNotification(notification);

        }




        protected void btnSubscribe_Click(object sender, EventArgs e)
        {
            // when subscribe an term-based app, exchange store sent 2 notificatons
            // one is for signup, another is for first payment
            // the subscr_id is the same, meaning they are for same subscription


            //this is the sign up notfication

            string subscriptonSignupNotificationTemplate =
               @"txn_type=subscr_signup" +    // this is subcription signup notification 
                "&subscr_id=I-M9UNWFABSCU0" + // subscription ID, 
                "&last_name=buyer&residence_country=US&mc_currency=USD" +
                "&item_name=Snoopy5&business=william_seller%40autodesk.com" +
                "&amount3=0.99&recurring=1" +
                "&verify_sign=As735jYkp7XIRB6.aiLUw3pQjX1zA8Hc.eUiJrGiQ-ix1S92lvtPhEw5" +
                "&payer_status=verified&test_ipn=1&payer_email=william_buyer%40autodesk.com" +
                "&first_name=williambuyer&receiver_email=william_seller%40autodesk.com" +
                "&payer_id=PYAX39C7LTU3E&reattempt=1" +
                "&item_number={0}" +          // app id
                "&subscr_date={1}" +          // the date time when sub signup
                "&custom=ZCGCKKGHNFN4&charset=windows-1252&notify_version=3.8" +
                "&period3=1+D" +              // one day, just for test. you probably see 1 month in production
                "&mc_amount3=0.99&ipn_track_id=651f78bba90fc" +
                "&buyer_adsk_account={2}";   // buyer's autodesk id


            string d = DateTime.Now.ToString("HH:mm:ss MMM dd, yyyy PST", CultureInfo.CreateSpecificCulture("en-US"));

            string subSignupIPN = string.Format(subscriptonSignupNotificationTemplate,

                    HttpUtility.UrlEncode(txtAppId.Text),
                    HttpUtility.UrlEncode(DateTime.Now.ToLongDateString()),
                    HttpUtility.UrlEncode(txtBuyerEmail.Text)
                    );


            //this is the first payment nortification

            string subscriptionPaymentNotificationTemplate = @"transaction_subject=Snoopy5" +
                "&payment_date={0}" +
                "&txn_type=subscr_payment" +           //this is a subscrip payment notification
                "&subscr_id=I-M9UNWFABSCU0" +          // subscription ID, 
                "&last_name=buyer&residence_country=US&item_name=Snoopy5" +
                "&payment_gross=0.99&mc_currency=USD&business=william_seller%40autodesk.com" +
                "&payment_type=instant&protection_eligibility=Ineligible" +
                "&verify_sign=Aw8jOlh0qjVJwUBom8vHwXnBIMQFAObdPVGABZ.j868wSztphYAUX1nX" +
                "&payer_status=verified" +
                "&test_ipn=1&payer_email=william_buyer%40autodesk.com" +
                "&txn_id=2KP144572R9206453" +
                "&receiver_email=william_seller%40autodesk.com" +
                "&first_name=williambuyer&payer_id=PYAX39C7LTU3E" +
                "&receiver_id=X7QMNEWVLHP4S" +
                "&item_number={1}" +
                "&payment_status=Completed" +
                "&payment_fee=0.33&mc_fee=0.33&mc_gross=0.99" + //payment amount
                "&custom=ZCGCKKGHNFN4&charset=windows-1252&notify_version=3.8" +
                "&ipn_track_id=651f78bba90fc" +
                "&buyer_adsk_account={2}";


            string paymentIPN = string.Format(subscriptionPaymentNotificationTemplate,
                HttpUtility.UrlEncode(DateTime.Now.ToLongDateString()),
                    HttpUtility.UrlEncode(txtAppId.Text),
                     HttpUtility.UrlEncode(txtBuyerEmail.Text)

                    );


            // do not assume the order you receive the notification is 
            // subscription sign up followed by payment when handling IPN. Actually the 
            // order of these two IPN notification may be different due
            // to internet latency.

            // so send at random order to mimic this.
            Random rand = new Random();
            if (rand.Next(10) > 5)
            {
                notification = subSignupIPN;
                SendNotification(subSignupIPN);

                notification = paymentIPN;
                SendNotification(paymentIPN);
                
            }
            else
            {
                notification = paymentIPN;
                SendNotification(paymentIPN);

                notification = subSignupIPN;
                SendNotification(subSignupIPN);
                
            }





        }

        protected void btnRenew_Click(object sender, EventArgs e)
        {
            //the renew process will be automatical, this is just for a test by a manually click

            string subscriptionRenewPaymentNotificationTemplate = @"transaction_subject=Snoopy5" +
                "&payment_date={0}" +
                "&txn_type=subscr_payment" +           //this is a subscrip payment notification
                "&subscr_id=I-M9UNWFABSCU0" +          // subscription ID, 
                "&last_name=buyer&residence_country=US&item_name=Snoopy5" +
                "&payment_gross=0.99&mc_currency=USD&business=william_seller%40autodesk.com" +
                "&payment_type=instant&protection_eligibility=Ineligible" +
                "&verify_sign=Aw8jOlh0qjVJwUBom8vHwXnBIMQFAObdPVGABZ.j868wSztphYAUX1nX" +
                "&payer_status=verified" +
                "&test_ipn=1&payer_email=buyer%40autodesk.com" +
                "&txn_id=4UTFDS522R9208564" +         // here is the difference with the first payment notification
                "&receiver_email=william_seller%40autodesk.com" +
                "&first_name=williambuyer&payer_id=PYAX39C7LTU3E" +
                "&receiver_id=X7QMNEWVLHP4S" +
                "&item_number={1}" +
                "&payment_status=Completed" +
                "&payment_fee=0.33&mc_fee=0.33&mc_gross=0.99" + //payment amount
                "&custom=ZCGCKKGHNFN4&charset=windows-1252&notify_version=3.8" +
                "&ipn_track_id=651f78bba90fc" +
                "&buyer_adsk_account={2}";

            string renewPaymentIPN = string.Format(subscriptionRenewPaymentNotificationTemplate,
                    HttpUtility.UrlEncode(DateTime.Now.ToLongDateString()),
                    HttpUtility.UrlEncode(txtAppId.Text),
                     HttpUtility.UrlEncode(txtBuyerEmail.Text)

                   );

            notification = renewPaymentIPN;
            SendNotification(renewPaymentIPN);
            
        }




        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string subscriptionCancelNotificationTemplate =
                @"txn_type=subscr_cancel" +          // subscription is canceled 
                "&subscr_id=I-M9UNWFABSCU0" +        // subscription ID
                "&last_name=buyer&residence_country=US&mc_currency=USD" +
                "&item_name=Snoopy5&business=william_seller%40autodesk.com" +
                "&amount3=0.99&recurring=1" +
                "&verify_sign=ABr7TF1VNJRHrFfJkVMfCcSa87ETAiCAKc.zaQF0c5T2TSCq6jf8QFRT" +
                "&payer_status=verified" +
                "&test_ipn=1&payer_email=buyer%40autodesk.com" +
                "&first_name=williambuyer&receiver_email=william_seller%40autodesk.com" +
                "&payer_id=PYAX39C7LTU3E&reattempt=1" +
                "&item_number={0}" +
                "&subscr_date=01%3A50%3A31+May+20%2C+2014+PDT" +
                "&custom=ZCGCKKGHNFN4&charset=windows-1252" +
                "&notify_version=3.8" +
                "&period3=1+D" +
                "&mc_amount3=0.99" +
                "&ipn_track_id=98a2bd77a6515" +
                "&buyer_adsk_account={1}";

            string subCancelIPN = string.Format(subscriptionCancelNotificationTemplate,

                HttpUtility.UrlEncode(txtAppId.Text),
                HttpUtility.UrlEncode(txtBuyerEmail.Text)
                );

            notification = subCancelIPN;
            SendNotification(subCancelIPN);
            
        }

        private void SendNotification(string notification)
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(txtIPNListener.Text);

            //send IPN notification to publishers IPN listener
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Timeout = 500000;
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

    }
}