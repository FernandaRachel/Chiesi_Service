using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiesi.BasicInfos;
using System.Configuration;

namespace Chiesi.Log
{
    class LogClass : InterfaceLog
    {
        public string log { get; set; }

        public LogClass()
        {

        }

        public void writeLog(string header, BasicInfoClass basicInfo)
        {
            // Pega do App.config o endereço para salvar o Log AD
            string path = ConfigurationManager.AppSettings["PATHADCHIESI"] + DateTime.Now.ToString("dd-MM-yyyy")+ ".txt";
            
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(header + ", gerado por: " + '\n' +
                    " Assinatura: " + basicInfo.OperatorLogin + '\n' +
                    " Data: " + basicInfo.Date.ToString("dd/MM/yyyy") + '\n' +
                    " Hora: " + basicInfo.Hour.ToString("HH:mm") + '\n');
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(header + ", gerado por: " + '\n' +
                   " Assinatura: " + basicInfo.OperatorLogin + '\n' +
                   " Data: " + basicInfo.Date.ToString("dd/MM/yyyy") + '\n' +
                   " Hora: " + basicInfo.Hour.ToString("HH:mm") + '\n');
            }

            // Open the file to read from.
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
        }
    }
}
