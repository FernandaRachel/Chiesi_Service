using Chiesi.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiesi.BasicInfos;
using System.IO;
using Chiesi;
using System.Configuration;

namespace Chiesi_Service.Log
{
    class LogAction
    {

        public void writeLog(string info)
        {
            string path = ConfigurationManager.AppSettings["PATHLOGACTION"] +
                DateTime.Now.Date. ToString("dd-MM-yyyy") +
                DateTime.Now.ToString("HH") + ".txt";
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(DateTime.Now +": " + info);
                    sw.Close();
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(DateTime.Now + ": " + info);
                sw.Close();
            }
        }
    }
}
