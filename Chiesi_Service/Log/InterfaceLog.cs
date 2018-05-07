using Chiesi.BasicInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Log
{
    interface InterfaceLog
    {
        string log { get; set; }


        void writeLog(string header, BasicInfoClass basicInfo);
    }
}
