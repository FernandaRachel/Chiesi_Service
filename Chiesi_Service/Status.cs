using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    public enum StatusType
    {
        Fail,
        InProcess,
        Idle
    }


    public static class Status
    {
        private static StatusType actualState;


        public static StatusType getStatus()
        {
            return actualState;
        }

        public static void SetFailMode()
        {
            actualState = StatusType.Fail;
        }

        public static void SetInProcessMode()
        {
            actualState = StatusType.InProcess;
        }

        public static void SetModeToIdle()
        {
            actualState = StatusType.Idle;
        }
    }
}
