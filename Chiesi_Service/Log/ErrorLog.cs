using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Log
{
    class ErrorLog
    {
        public void writeLog(string operacao, string tag, string mensagem, DateTime date)
        {
            string path = ConfigurationManager.AppSettings["PATHERRORLOGCHIESI"] + 
                DateTime.Now.Date.ToString("dd-MM-yyyy") + DateTime.Now.Date.ToString("HH")+  ".txt";
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Operação:" + operacao +"Tag:"+ tag + "Data:" + date  + "Mensagem :" + mensagem);
                    sw.Close();
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(date + ":" + "Mensagem :" + mensagem);
                sw.Close();
            }
        }


    }
}
