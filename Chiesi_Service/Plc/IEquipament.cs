using Chiesi_Service.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    interface IEquipament
    {
        string address { get; set; }
        Recipe recipe { get; set; }
        RecipeData recipeData { get; set; }

        void Connect();
        Recipe ReadAllData();
        string Read(string Tag);
        Dictionary<string, string> ReadAll();
        void Write(string Tag, string value);
    }
}
