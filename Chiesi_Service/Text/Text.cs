﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Chiesi.BasicInfos;
using System.Configuration;

namespace Chiesi
{
    class Text : IText
    {

        public string Header { get; set; }

        public string txtAtual { get; set; }

        public Dictionary<string, Dictionary<string, string>> LastValues { get; set; }

        public BasicInfoClass binfos { get; set; }

        //public Dictionary<string, string> tagValues = new Dictionary<string, string>();


        public Text()
        {
            this.LastValues = new Dictionary<string, Dictionary<string, string>>();
            binfos = BasicInfoClass.GetBasicInfo();
        }


        public string generateTxt(string header)
        {
            Header = header;
            txtAtual = header;

            return txtAtual;
        }



        public void saveTxt(string txt, bool firstOp)
        {
            string path = ConfigurationManager.AppSettings["PATHLOGCHIESI"];
            if (firstOp && File.Exists(path))
            {
                File.Copy(ConfigurationManager.AppSettings["PATHLOGCHIESI"], ConfigurationManager.AppSettings["PATHDUMP"] +
                    DateTime.Now.Date.ToString("dd-MM-yyyy") + DateTime.Now.ToString("HH") +
                DateTime.Now.ToString("mm") + ".txt");

                File.Delete(path);
            }
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(txt);
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(txt);
            }
        }

        public string addItem(string item)
        {
            txtAtual += item;

            Console.WriteLine(item);

            return txtAtual;

        }

        public string cleanTxt()
        {
            string path = ConfigurationManager.AppSettings["PATHLOGCHIESI"];
            string pathtosave = ConfigurationManager.AppSettings["PATHLOGCHIESITOSAVE"];
            File.Delete(path);

            txtAtual = "";
            return txtAtual;
        }


        public string ReadPreviewsTags(string Tag, string operationName)
        {
            return LastValues[operationName][Tag];
        }




    }
}
