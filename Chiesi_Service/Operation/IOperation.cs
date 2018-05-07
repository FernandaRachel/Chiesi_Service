using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    interface IOperation
    {


        bool WaitSign();

        string CreateString(params string[] values);





    }
}
