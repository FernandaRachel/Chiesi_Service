using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    interface IFilling
    {
        string Code { get; set; }
        string Product { get; set; }
        string Batch { get; set; }
        string FillingType { get; set; }
        double TankWeight { get; set; }

        void IniFilling();
        void EndFilling();

    }
}
