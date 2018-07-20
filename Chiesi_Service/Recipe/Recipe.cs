using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi_Service.Recipe
{
    class Recipe
    {
        public string Code;
        public string Batch;
        public string Date;
        public string Hour;
        public string Asignature;
        public string RecipeType;
        public string RecipeSubType;
        public string RecipeName;
        public string WeightTank;
        public List<RecipeData> Data;

        public Recipe()
        {
            Data = new List<RecipeData>();
        }
    }
}
