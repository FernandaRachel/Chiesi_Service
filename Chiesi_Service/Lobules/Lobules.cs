using Chiesi.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Lobules
{
    class LobulesClass
    {
        private static LobulesClass _instance;

        private static object syncLock = new object();

        public double lobulesSpeed { get; set; }

        public string lobulesSpeedTag { get; set; }

        public DateTime EndTime { get; set; }

        public string EndTimeTag { get; set; }

        public DateTime IniTime { get; set; }

        public string IniTimeTag { get; set; }

        public string RpmLimit { get; set; }

        public Convertion convert { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        protected LobulesClass(string EndTimeTag, string IniTimeTag, string RpmLimit, string lobulesSpeedTag, EquipamentType typeEq)
        {
            this.EndTimeTag = EndTimeTag;
            this.IniTimeTag = IniTimeTag;
            this.RpmLimit = RpmLimit;
            this.lobulesSpeedTag = lobulesSpeedTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
        }


        public static LobulesClass GetLobulesClass()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new LobulesClass(StaticValues.INISPEEDTIME, StaticValues.ENDSPEEDTIME, StaticValues.TAGLIMITRPMRECIRCULTION, StaticValues.LOBULESSPEED, StaticValues.EQUIPAMENTTYPE);
                    };
                }
            }


            return _instance;
        }

        public void ReadPlc()
        {
            lobulesSpeed = convert.convertToDouble(lobulesSpeedTag, eq.Read(lobulesSpeedTag));
            EndTime = DateTime.Now;
            IniTime = DateTime.Now;
        }
    }
}
