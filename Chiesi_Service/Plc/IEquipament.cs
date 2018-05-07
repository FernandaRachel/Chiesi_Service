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

        void Connect();
        string Read(string Tag);
        void Write(string Tag, string value);
    }
}
