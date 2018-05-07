using Chiesi.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Tanks
{
    class TankClass
    {
        private static TankClass _instance;

        private static object syncLock = new object();

        public double TankWeight { get; set; }

        public string TankWeightTag { get; set; }

        public Convertion convert { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;


        public TankClass(string TankWeightTag, EquipamentType typeEq)
        {
            this.TankWeightTag = TankWeightTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);

        }

        public static TankClass GetTankClass()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new TankClass(StaticValues.TANKWEITGHTTAG, StaticValues.EQUIPAMENTTYPE);

                    };
                }
            }

            return _instance;
        }

        public void ReadPlc()
        {
            TankWeight = convert.convertToDouble(TankWeightTag, eq.Read(TankWeightTag));
        }
    }
}
