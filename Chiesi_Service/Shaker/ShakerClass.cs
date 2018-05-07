using Chiesi.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Shaker
{
    class ShakerClass
    {
        private static ShakerClass _instance;

        private static object syncLock = new object();

        public double ShakingSpeed { get; set; }

        public string ShakingSpeedTag { get; set; }

        public Convertion convert { get; set; }

        public double RpmLimit { get; set; }

        public string RpmLimitTag { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        protected ShakerClass(string ShakingSpeedTag, string RpmLimitTag, EquipamentType typeEq)
        {
            this.ShakingSpeedTag = ShakingSpeedTag;
            this.RpmLimitTag = RpmLimitTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
        }
        public static ShakerClass GetShakerClass()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ShakerClass(StaticValues.TAGSHAKESPEED, StaticValues.RPMLIMIT, StaticValues.EQUIPAMENTTYPE);
                    };
                }
            }


            return _instance;
        }

        public void ReadPlc()
        {
            ShakingSpeed = convert.convertToDouble(ShakingSpeedTag, eq.Read(ShakingSpeedTag));
            RpmLimit = 0.0;


        }
    }
}
