using Chiesi_Service.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    interface IOperation
    {

        string CreateString(params string[] values);

    }
}
