using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using Listener;

namespace ExchangeStoreEmulator
{
    public partial class IPNTestHelper : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }


        protected void btnBuy_Click(object sender, EventArgs e)
        {
            string ipnNotificationTempate = @"transaction_subject=200703030249937&txn_type=web_accept&payment_date=23%3A36%3A36+Jan+11%2C+2014+PST&last_name=Balsom&residence_country=AU&item_name=RDBK_AutoNumber+Pro&payment_gross=5.50&mc_currency=USD&business=paypal@redbike.com.au&payment_type=instant&protection_eligibility=Ineligible&payer_status=verified&verify_sign=AFcWxV21C7fd0v3bYYYRCpSSRl31AsmAEVMnS38537K1tk5tZMnvtnW6&tax=0.50&payer_email=mbalsom@shoal.net.au&txn_id=0AG18756HD086633A&quantity=1&receiver_email=paypal@redbike.com.au&first_name=MARK&payer_id=NEH6BJPL9LBYG&receiver_id=GDGRD3PAZBMD8&item_number={0}&handling_amount=0.00&payment_status=Completed&payment_fee=0.43&mc_fee=0.43&shipping=0.00&mc_gross=5.50&custom=200703030249937&charset=windows-1252&notify_version=3.7&auth=A6P4OiUSwAL6901WUc3VK.fiUaYTR5AND5h.XpBaMqrI8gSmid.n0tFsfAMP6u3unDXUuiABwGtZWQlN.RFtDcA&form_charset=UTF-8&buyer_adsk_account={1}";
            string notification = string.Format(ipnNotificationTempate, 
                HttpUtility.UrlEncode(txtAppId.Text), 
                txtBuyerEmail.Text);

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
    }
}