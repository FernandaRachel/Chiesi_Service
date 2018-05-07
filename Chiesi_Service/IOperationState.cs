using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    interface IOperationState
    {
        //public  mode;

        //void Handle();

        void SetModeToIdle();

        void SetInProcessMode();

        void SetFailMode();
    }
}
