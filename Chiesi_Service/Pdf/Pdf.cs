using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Chiesi.Log;
using Chiesi.BasicInfos;
using NReco.PdfGenerator;
using System.Configuration;

namespace Chiesi
{
    class Pdf : IPdf
    {
        //HtmlToPdf Renderer = new HtmlToPdf();

        public string reportTxt { get; set; }

        public string Batch { get; set; }

        public LogClass logClass { get; set; }

        public Text text { get; set; }

        public BasicInfoClass binfos { get; set; }

        public Pdf(string txt)
        {
            reportTxt = txt;
            binfos = BasicInfoClass.GetBasicInfo();
            logClass = new LogClass();
            text = new Text();
            Batch = binfos.KeepBatch;
        }


        public string gerarPdf(string header, BasicInfoClass basicInfo)
        {

            string preBody = "<html>" +
                "               <head>";
            preBody += "<link rel='stylesheet' type='text/css' href='" + ConfigurationManager.AppSettings["STYLESHEET"] + "'/>" +
                "<meta charset='UTF - 8'/>" +
                "               </head>" +
                "               <body>";

            string posBody = "</body></html>";
            string curpath = Directory.GetCurrentDirectory();
            string path = ConfigurationManager.AppSettings["RELATORIOPATH"];
            string pathBckp = ConfigurationManager.AppSettings["RELATORIOPATHBCKP"];
            string logopath = ConfigurationManager.AppSettings["LOGOPATH"];
            var headermodified = header.Replace("/", "");
            var htmlContent = preBody + "<h1 id='produto'>" + reportTxt + "</h1>" + posBody;
            var htmlToPdf = new HtmlToPdfConverter();

          

            htmlToPdf.CustomWkHtmlPageArgs = "--encoding utf-8 --images ";
            htmlToPdf.PageHeaderHtml = "<img id = 'logo' src = '" + logopath + "' >";
            htmlToPdf.PageFooterHtml = "<img class='fot' src =''>";

            var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
            File.WriteAllBytes(path + headermodified + Batch + ".pdf", pdfBytes);
            File.WriteAllBytes(pathBckp + headermodified + Batch + ".pdf", pdfBytes);
            
            htmlToPdf.LogReceived += (sender, e) =>
            {
                Console.WriteLine("WkHtmlToPdf Log: {0}", e.Data);
            };

            logClass.writeLog(header, basicInfo);

            Status.SetModeToIdle();
            text.cleanTxt();

            return "PDF FINAL GERADO";
        }

        public string salvarPdf(string pdf, string path)
        {
            return "PDF FINAL SALVO";
        }
    }
}
