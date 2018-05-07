using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Operation
{
    abstract class OperationHandler
    {
        protected OperationHandler successor { get; set; }


        public void SetSuccessor(OperationHandler successor)
        {
            this.successor = successor;

        }


        public abstract void Calculate(Text txt);
    }
}

