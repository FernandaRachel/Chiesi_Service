using Chiesi.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiesi.BasicInfos;
using System.IO;
using Chiesi;

namespace Chiesi_Service.Log
{
    class LogAction
    {

        public void writeLog(string info)
        {
            string path = StaticValues.PATHLOGACTION + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(info);
                    sw.Close();
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(info);
                sw.Close();
            }
        }
    }
}
