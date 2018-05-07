using Chiesi.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Turbine
{
    class TurbineClass
    {
        private static TurbineClass _instance;

        private static object syncLock = new object();

        public double RpmLimit { get; set; }

        public string RpmLimitTag { get; set; }

        public double TurbineSpeed { get; set; }
        public Convertion convert { get; set; }

        public string TurbineSpeedTag { get; set; }

        public DateTime EndTime { get; set; }

        public string EndTimeTag { get; set; }

        public DateTime IniTime { get; set; }

        public string IniTimeTag { get; set; }

        public double mixTime { get; set; }

        public string mixTimeTag { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        protected TurbineClass(string TurbineSpeedTag, string IniTimeTag, string EndTimeTag, string mixTimeTag, EquipamentType typeEq)
        {
            this.TurbineSpeedTag = TurbineSpeedTag;
            this.IniTimeTag = IniTimeTag;
            this.EndTimeTag = EndTimeTag;
            this.RpmLimitTag = RpmLimitTag;
            this.mixTimeTag = mixTimeTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
        }


        public static TurbineClass GetTurbineClass()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new TurbineClass(StaticValues.TURBINESPEED,  StaticValues.INISPEEDTIME, StaticValues.ENDSPEEDTIME, StaticValues.TAGMIXTIME, StaticValues.EQUIPAMENTTYPE);
                    };
                }
            }


            return _instance;
        }

        public void ReadPlc()
        {
            TurbineSpeed = convert.convertToDouble(TurbineSpeedTag, eq.Read(TurbineSpeedTag));
            RpmLimit = 30;
            mixTime = convert.convertToDouble(mixTimeTag, eq.Read(mixTimeTag));
            IniTime = DateTime.Now;
            EndTime = DateTime.Now;

        }
    }
}
