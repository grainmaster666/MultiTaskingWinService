using System;
using System.IO;

namespace C9ISM.Scheduler.Logger
{
    /// <summary>
    /// Cloud 9 infotech basic logger
    /// </summary>
    public class C9BasicLogger
    {        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logMessage"></param>
        public void LogWrite(string logMessage)
        {
            string filename = string.Format("{1}_{0}", @"log.txt", DateTime.Now.ToString("yyyy-MM-dd"));
            string fullpath = @"C:\logs\" + filename;

            if (File.Exists(fullpath))
            {
                using (StreamWriter sw = File.AppendText(fullpath))
                {
                    //This is where you will write your text to the new file if the other one was in use
                    Log(logMessage, sw);
                    sw.Flush();
                    sw.Close();
                }
            }
            else
            {
                using (StreamWriter sw = File.CreateText(fullpath))
                {
                    //This is where you will write your text to the new file if the other one was in use
                    Log(logMessage, sw);
                    sw.Flush();
                    sw.Close();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="txtWriter"></param>
        public void Log(string logMessage, TextWriter txtWriter)
        {
            txtWriter.Write("\r\nLog Entry : ");
            txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());          
            txtWriter.WriteLine("{0}", logMessage);            
        }
    }
}
