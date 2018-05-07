using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Chiesi.Log;
using Chiesi.BasicInfos;
using NReco.PdfGenerator;

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
            preBody += "<link rel='stylesheet' type='text/css' href='" + StaticValues.STYLESHEET + "'/>" +
                "<meta charset='UTF - 8'/>" +
                "               </head>" +
                "               <body>";

            string posBody = "</body></html>";
            string curpath = Directory.GetCurrentDirectory();
            string path = StaticValues.RELATORIOPATH; //mockpdf 
            string logopath = StaticValues.LOGOPATH;
            var headermodified = header.Replace("/", "");
            var htmlContent = preBody + "<h1 id='produto'>" + reportTxt + "</h1>" + posBody;
            //var margins = new PageMargins();
            var htmlToPdf = new HtmlToPdfConverter();

            //margins.Top = 200;
            //margins.Bottom = 200;
            //margins.Left = 200;
            //margins.Right = 200;

            htmlToPdf.CustomWkHtmlPageArgs = "--encoding utf-8 --images ";
            htmlToPdf.PageHeaderHtml = "<img id = 'logo' src = '" + logopath + "' >";
            htmlToPdf.PageFooterHtml = "<img class='fot' src =''>";
            //htmlToPdf.Margins = margins;

            var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
            File.WriteAllBytes(StaticValues.RELATORIOPATH + headermodified + Batch + ".pdf", pdfBytes);

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




// IRON PDF
//PdfDocument pdf = PdfGenerator.GeneratePdf(preBody + "<h1 id='produto'>" + reportTxt + "</h1>" + posBody);
//pdf.Save(path + headermodified + DateTime.Now.Ticks + ".pdf");


//Renderer.PrintOptions.Header = new HtmlHeaderFooter()
//{
//    Height = 50,
//    HtmlFragment = "<img id='logo' src='" + logopath + "'>",
//    BaseUrl = new Uri(@"c:\Images\").AbsoluteUri
//};
//Renderer.RenderHtmlAsPdf(preBody + "<h1 id='produto'>" + reportTxt + "</h1>" + posBody).
//    SaveAs(path + headermodified +  DateTime.Now.Ticks + ".pdf");

//PdfDocument pdf = PdfGenerator.GeneratePdf(preBody + "<h1 id='produto'>" + reportTxt + "</h1>" + posBody, PageSize.A4);
//pdf.Save(path + headermodified + DateTime.Now.Ticks + ".pdf");