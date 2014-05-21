using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;

namespace Listener
{
    public class NetLog
    {
        /// <summary>
        /// </summary>
        /// <param name="strMessage"></param>
        public static void WriteTextLog(string strMessage)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"System\Log\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileFullPath = path + "log.txt";
            StringBuilder str = new StringBuilder();
            str.Append("Time:    " + DateTime.UtcNow.ToString() + "\r\n");
            str.Append("Message: " + strMessage + "\r\n");
            str.Append("-----------------------------------------------------------\r\n\r\n");
            StreamWriter sw;
            if (!File.Exists(fileFullPath))
            {
                sw = File.CreateText(fileFullPath);
            }
            else
            {
                sw = File.AppendText(fileFullPath);
            }
            sw.WriteLine(str.ToString());
            sw.Close();
        }  
    }
}