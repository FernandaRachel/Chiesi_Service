using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    interface IText
    {
        //string txtAtual { get; set; }
        string txtAtual { get; set; }
        Dictionary<string, Dictionary<string, string>> LastValues { get; set; }

        string generateTxt(string header);
        void saveTxt(string txt, bool firstOp);
        string addItem(string item);
        string cleanTxt();
        string ReadPreviewsTags(string Tag, string operationName);
    }
}
