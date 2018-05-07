using Chiesi.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Clenil
{
    class ClenilClass
    {

        private static ClenilClass _instance;

        private static object syncLock = new object();

        public double RpmLimit { get; set; }

        public string RpmLimitTag { get; set; }

        public double ClenilSpeed { get; set; }

        public string ClenilSpeedTag { get; set; }

        public DateTime EndTime { get; set; }

        public string EndTimeTag { get; set; }

        public DateTime IniTime { get; set; }

        public string IniTimeTag { get; set; }

        public Convertion convert { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        protected ClenilClass(string ClenilSpeedTag, string EndTimeTag, string IniTimeTag,  EquipamentType typeEq)
        {
            this.ClenilSpeedTag = ClenilSpeedTag;
            this.EndTimeTag = EndTimeTag;
            this.IniTimeTag = IniTimeTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
        }


        public static ClenilClass GetClenilClass()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ClenilClass(StaticValues.TURBINESPEED, StaticValues.INISPEEDTIME, StaticValues.ENDSPEEDTIME,  StaticValues.EQUIPAMENTTYPE);
                    };
                }
            }

            return _instance;
        }

        public void ReadPlc()
        {
            ClenilSpeed = convert.convertToDouble(ClenilSpeedTag, eq.Read(ClenilSpeedTag));
            EndTime = DateTime.Now;
            IniTime = DateTime.Now;
            RpmLimit = 0.00;
        }
    }
}
