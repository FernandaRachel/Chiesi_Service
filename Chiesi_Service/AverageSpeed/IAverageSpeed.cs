using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.AverageSpeed
{
    /// <summary>
    /// Interface Velocidade Média
    /// </summary>
    interface IAverageSpeed
    {
        int MixTime { get; set; }
        int RpmLimit { get; set; }
        double AnchorSpeed { get; set; }
        double TurbineSpeed { get; set; }
        double LobulesSpeed { get; set; }
        DateTime IniTime { get; set; }
        DateTime EndTime { get; set; }

        double CalculateSpeed();

    }
}
