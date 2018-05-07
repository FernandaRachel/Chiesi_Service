using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Loading
{
    interface ILoading
    {

        double Limit { get; set; }
        double RealQty { get; set; }
        double TheoricQty { get; set; }
        double Variation { get; set; }
        FlowmeterClass flux { get; set; }

        double CalculateVariation();

    }
}
