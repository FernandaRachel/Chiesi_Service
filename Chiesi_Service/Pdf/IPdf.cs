using Chiesi.BasicInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    interface IPdf
    {
        string reportTxt { get; set; }

        string gerarPdf(string header, BasicInfoClass basicInfo);
        string salvarPdf(string pdf, string path);
    }
}
