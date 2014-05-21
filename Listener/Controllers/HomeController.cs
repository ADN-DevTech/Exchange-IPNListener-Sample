using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Web.Script.Serialization;
using Listener.Models;

namespace Listener.Controllers
{
    public class HomeController : Controller
    {

        //This sample uses gmail to send email notification to customer, you may need to use your exchange server.
        //replace with your gmail address and password.
        #region change the const settings here
        const string FROM_EMAIL_ADDRESS = "yourname@gmail.com";
        const string FROM_PASSWORD = "your password";

        const string EXCHANGE_BASE_URL = "http://apps.exchange.autodesk.com/";
        //replace your Autodesk ID or email address here 
        const string PUBLISHER_AUTODESK_ID_OR_EMAILADDRESS = "daniel.du@autodesk.com";
        #endregion

        public ActionResult Index(string user="")
        {
            //TODO: Check whether user is logged in
            //TODO: Check whether current user is entitled to access your service.

            #region  Below is just a demo, you need to implement your own business logic

            if (string.IsNullOrWhiteSpace(user) == false)
            {
           
                //set cookie
                //Create a new cookie, passing the name into the constructor
                HttpCookie cookie = new HttpCookie("cloud_user_cookie");

                //Set the cookies value
                cookie.Value = user;

                //Set the cookie to expire in 1 minute
                DateTime dtNow = DateTime.Now;
                TimeSpan tsMinute = new TimeSpan(3, 0, 0, 0);
                cookie.Expires = dtNow + tsMinute;

                //Add the cookie
                Response.Cookies.Add(cookie);
       

                return RedirectToAction("index", "home");
            }

       
            //check the cookie user name
            string cookieUser = "";
            HttpCookie c = Request.Cookies["cloud_user_cookie"];
            if (c != null )
            {
                cookieUser = HttpUtility.UrlDecode(c.Value);
            }
          



            if (string.IsNullOrWhiteSpace(user) 
                && !string.IsNullOrWhiteSpace(cookieUser))
            {
                user = cookieUser;
            }
            
            if(string.IsNullOrWhiteSpace(user))
                ViewBag.Message = "Please login to start!";
            else
                ViewBag.Message = "Welcome " + user + "!";

            ViewBag.User = user;

            return View();

            #endregion
        }

        protected string GetIpnValidationUrl()
        {
            
                //Default to empty string
                string validationUrl = Convert.ToString(String.IsNullOrEmpty(ConfigurationManager.AppSettings["validationUrl"])
                                             ? "" : ConfigurationManager.AppSettings["validationUrl"]);
                if (string.IsNullOrWhiteSpace(validationUrl))
                {
                    //Autodesk Exchange Store IPN validation URL
                    return @"https://apps.exchange.autodesk.com/WebServices/ValidateIPN";
                }
                return validationUrl;
           
        }

        //get my published apps or web services
        protected List<string> GetMyPublishedApps(string publisherAutodeskIdOrEmail)
        {


            //Default to empty string
            string myAppIds = Convert.ToString(String.IsNullOrEmpty(ConfigurationManager.AppSettings["myappids"])
                                         ? "" : ConfigurationManager.AppSettings["myappids"]);
            if (string.IsNullOrWhiteSpace(myAppIds))
            {
                return null;
            }
            return myAppIds.Split(';').ToList();

            #region Get live Apps, not completed yet

            //List<string> appIds = new List<string>();

            //var postUrl = EXCHANGE_BASE_URL + "webservices/GetLiveApps?user="
            //    + publisherAutodeskIdOrEmail;

            //NetLog.WriteTextLog("HomeController[HandleIPNNotification - ]: posting to Autodesk Exchange: " + postUrl);

            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(postUrl);

            
            //req.Method = "GET";
            //req.ContentType = "application/json";
         

            ////Send the request to Autodesk and get the response
            ////StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            ////streamOut.Write(strRequest);
            ////streamOut.Close();

            //StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            //string strResponse = streamIn.ReadToEnd();
            //streamIn.Close();

            //NetLog.WriteTextLog("HomeController[HandleIPNNotification - ]: strResponse is: " + strResponse);


            ////convert response string to object
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //var liveAppsResult = serializer.Deserialize<GetLiveAppsResult>(strResponse);
            //foreach (var item in liveAppsResult.LiveApps)
            //{
            //    appIds.Add(item.ID);
            //}

            //return appIds;
            #endregion

        }

        public void IPNListener()
        {
            NetLog.WriteTextLog("====================Process Start =========================");

            string ipnNotification = Encoding.ASCII.GetString(Request.BinaryRead(Request.ContentLength));
            if (ipnNotification.Trim() == string.Empty)
            {
                NetLog.WriteTextLog("No IPN notification received. ");
                return;
            }
            NetLog.WriteTextLog(ipnNotification);

            string validationMessage = HandleIPNNotification(ipnNotification);
            NetLog.WriteTextLog(validationMessage);

            NetLog.WriteTextLog("====================Process End =========================");
        }

        public void SendAccountInfoToBuyer(string email="", string username="", string password="")
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username))
                return;
            try
            {
                NetLog.WriteTextLog("start to send email to " + email + "with name:" + username);
               
                string webserviceUrl = Convert.ToString(String.IsNullOrEmpty(Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("webserviceUrl"))
                                         ? "" : Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("webserviceUrl"));

                string bodyTemplate = @"Hi {0},<br/><br/>Thank you for selecting Cloud Rendering! Please access the Web Service with below information.<br/>User name: {1} <br/>Password: {2}<br/>The link is: {3}. <br/><br/>Thanks,<br/>Cloud Rendering Team.";

                string body = string.Format(bodyTemplate, username, username,password,webserviceUrl);

                string subject = "You account for Cloud Rendering";
                SendEmail(email, subject, body);

  
                NetLog.WriteTextLog("Send email to " + email + "with name:" + username);
            }
            catch (Exception ex)
            {
                NetLog.WriteTextLog("fail to send email, the error is :" + ex.Message + ex.InnerException != null ? ex.InnerException.ToString() : "" + ex.StackTrace);
            }
        }

        private static void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress(FROM_EMAIL_ADDRESS);
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = FROM_PASSWORD;
           

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        private string HandleIPNNotification(string notification)
        {
            // refer to following link for the complete parameters
            // here is IPN API link: https://developer.paypal.com/webapps/developer/docs/classic/ipn/integration-guide/IPNandPDTVariables/
            /*

            A sample priced app/web service IPN sent to publisher:
             
            transaction_subject=200703030249937
            &txn_type=web_accept
            &payment_date=23%3A36%3A36+Jan+11%2C+2014+PST
            &last_name=Balsom
            &residence_country=AU
            &item_name=RDBK_AutoNumber+Pro
            &payment_gross=5.50
            &mc_currency=USD
            &business=paypal@redbike.com.au
            &payment_type=instant
            &protection_eligibility=Ineligible
            &payer_status=verified
            &verify_sign=AFcWxV21C7fd0v3bYYYRCpSSRl31AsmAEVMnS38537K1tk5tZMnvtnW6
            &tax=0.50
            &payer_email=mbalsom@shoal.net.au
            &txn_id=0AG18756HD086633A
            &quantity=1
            &receiver_email=paypal@redbike.com.au
            &first_name=MARK
            &payer_id=NEH6BJPL9LBYG
            &receiver_id=GDGRD3PAZBMD8
            &item_number=appstore.exchange.autodesk.com%3Ardbk_autonumberpro_windows32and64%3Aen
            &handling_amount=0.00
            &payment_status=Completed
            &payment_fee=0.43
            &mc_fee=0.43
            &shipping=0.00
            &mc_gross=5.50
            &custom=200703030249937
            &charset=windows-1252
            &notify_version=3.7
            &auth=A6P4OiUSwAL6901WUc3VK.fiUaYTR5AND5h.XpBaMqrI8gSmid.n0tFsfAMP6u3unDXUuiABwGtZWQlN.RFtDcA
            &form_charset=UTF-8
            &buyer_adsk_account=xxx@redbike.com.au   //this one is added by exchange store

            free app/web service IPN sent to publisher:
             
            txn_id={0}&custom={1}&payment_status={2}&item_number={3}&buyer_adsk_account={4}&mc_gross=0.00

            */
            string strResponse = "Unknown";

            //TODO: Check whether this message has been handled already,
            //If hanlded, return.

            try
            {
                //Pass-through variable for you to track purchases. It will get passed back to you at the completion of the payment.
                //The value is the appid of app/webservice which is bought by customer
                string thisapp = HttpUtility.UrlDecode(Request["item_number"]);

                bool isMyApp = Preprocess_CheckAppId(thisapp);
                if (!isMyApp)
                {
                    return "Unknow App, not notification for me.";
                }


                // POST IPN notification back to Autodesk Exchange to validate
                // whether it is a validate ipn notification from Autodesk Exchange
                bool verified = VerifyIpnValidation(notification, thisapp);

                
                if (verified)
                {
                    //If the IPN notification is valid one, then it was sent from Autodesk Exchange Store, 
                    
                  
                    string txn_type = Request["txn_type"];
                    if (txn_type == "subscr_signup")  // subscription signup
                    {
                        string subject = "Thank you for subscription ";
                        string body = "You have subscribed this app/web service:" + thisapp
                            + " for " + Request["period3"]  //TODO: tranlate the period3 value into more user friendly msg
                            +" from " + Request["subscr_date"];
                        string buyerEmail = Request["buyer_adsk_account"];
                        SendEmail(buyerEmail, subject, body);


                    }
                    else if (txn_type == "subscr_payment") //subscription payment
                    {
                        string subject = "Thank you for subscription payment";
                        string body = "You have paied your subscription of app/web service:" + thisapp
                            + " on  " + Request["payment_date"];
                        string buyerEmail = Request["buyer_adsk_account"];
                        SendEmail(buyerEmail, subject, body);
                        
                    }
                    else if (txn_type == "subscr_cancel") // sbuscrition cacel
                    {
                        string subject = "Your subscription is canceled";
                        string body = "We are sorry to lose you from subscription of app/web service:" + thisapp
                            + " on  " + Request["payment_date"];
                        string buyerEmail = Request["buyer_adsk_account"];
                        SendEmail(buyerEmail, subject, body);
                    }
                    else if (txn_type == "web_accept")    //one time payment
                    {
                        // check the payment status
                        string payment_status = Request["payment_status"];
                        if (String.Compare(payment_status, "Completed", true) != 0)
                        {
                            NetLog.WriteTextLog("HomeController[HandleIPNNotification - " + thisapp + "]: payment not completed yet.");
                            return "Not paied";
                        }

                        ////customer has already paied for my web service, now I can go ahead to create an account for him
                        //and enable him to access my web service

                        //For priced app
                        string quantity = Request["quantity"];
                        //TODO: handle batch purchure

                        //buyer's email address which is used to buy the service
                        string email = Request["buyer_adsk_account"];

                        //As an example solution, create an account with initial password
                        //create an accout for the user with this email address and initial password
                        string pwd = CreateAccountWithInitialPassword(email);
                        GrantAccessToUser(email);

                        //inform the customer how he can access your service with his intial account
                        SendAccountInfoToBuyer(email, email, pwd);
                    }
                    else
                    {
                        NetLog.WriteTextLog("HomeController[HandleIPNNotification - " + thisapp + "]: unknown tranaction type.");
                        return "unknown tranaction type";

                    }

                    
                
                }
                else
                {
                    NetLog.WriteTextLog("HomeController[HandleIPNNotification - " + thisapp + "]: IPN notifiacation is not verified. Someone is hacking me? Response is: " + strResponse);
                }
            }
            catch (System.Exception ex)
            {
                NetLog.WriteTextLog("PaymentProvider[HandleIPNNotification]: Error to get IPN validation info from Autodesk. strResponse is: " + strResponse);
                NetLog.WriteTextLog(ex.Message + ex.InnerException != null ? ex.InnerException.ToString() : "" + ex.StackTrace);
            }

            return strResponse;
        }

        private bool VerifyIpnValidation(string notification,  string thisapp)
        {
            string strResponse;

            // POST IPN notification back to Autodesk Exchange to validate
            //https://developer.paypal.com/webapps/developer/docs/classic/ipn/ht_ipn/
            //For Autodesk Exchange Store, you do not need to contact with Paypal directly, valide with Autodesk Exchange
            var postUrl = GetIpnValidationUrl();
            NetLog.WriteTextLog("HomeController[HandleIPNNotification - " + thisapp + "]: posting to Autodesk Exchange: " + postUrl);


            strResponse = GetResponseString(postUrl, notification);

            if (strResponse == "Verified")
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        private string GetResponseString(string url, string poststring)
        {
            HttpWebRequest httpRequest =
            (HttpWebRequest)WebRequest.Create(url);

            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            byte[] bytedata = Encoding.UTF8.GetBytes(poststring);
            httpRequest.ContentLength = bytedata.Length;

            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();


            HttpWebResponse httpWebResponse =
            (HttpWebResponse)httpRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();

            StringBuilder sb = new StringBuilder();

            using (StreamReader reader =
            new StreamReader(responseStream, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.Append(line);
                }
            }

            return sb.ToString();

        }

        private bool Preprocess_CheckAppId(string thisapp)
        {
            List<string> apps = GetMyPublishedApps(PUBLISHER_AUTODESK_ID_OR_EMAILADDRESS);

            if (apps == null || string.IsNullOrWhiteSpace(thisapp) || apps.Contains(thisapp) == false)
            {
                NetLog.WriteTextLog("HomeController[HandleIPNNotification]: Not my App, just return.");
                return false;
            }
            return true;
          
        }

        private void GrantAccessToUser(string email)
        {
            //you business logic here
        }

        private string CreateAccountWithInitialPassword(string email)
        {

            //TODO: your business logic here, 
            //create account, and generate an initila password

            //This is just a sample
            //return the password 
            return GetRandString(6);
        }

        public ActionResult About()
        {
            return View();
        }

        private string GetRandString(int length)
        {
            string str = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~!@#$%^&*()_+";
            Random r = new Random();
            string result = string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                int m = r.Next(0, str.Length);
                string s = str.Substring(m, 1);
                sb.Append(s);
            }

            return sb.ToString();
        }

    }
}
