using Chiesi_Service.Recipe;
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

        public List<RecipeData> SearchInfoInList(IEquipament eq, string id)
        {
            var recipesInfos = eq.recipe;

            var result = recipesInfos.Data.FindAll(d => d.Id == id);

            return result;
        }


        public abstract void Calculate(Text txt);
    }
}

