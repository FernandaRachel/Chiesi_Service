using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiesi.BasicInfos;

namespace Chiesi
{
    class PdfMock : IPdf
    {
        //HtmlToPdf Renderer = new HtmlToPdf();

        public string reportTxt { get; set; }


        public PdfMock(string txt)
        {
            reportTxt = txt;
        }


        public string gerarPdf(string header, BasicInfoClass basicInfo)
        {
            System.IO.File.WriteAllText(@"C:/Users/fernandat/Documents/Visual Studio 2015/Projects/Chiesi/Chiesi/relatorioPdf.html", "<h1>" + reportTxt + "</h1>", Encoding.UTF8);


            return "PDF FINAL GERADO";
        }

        public string salvarPdf(string pdf, string path)
        {
            return "PDF FINAL SALVO";
        }
    }
}
