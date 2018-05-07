using Chiesi.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Clenil
{
    class ClenilStrong
    {

        private static ClenilStrong _instance;

        private static object syncLock = new object();

        public double RpmLimit { get; set; }

        public string RpmLimitTag { get; set; }

        public double ClenilSpeedStrong { get; set; }

        public string ClenilSpeedStrongTag { get; set; }

        public DateTime EndTime { get; set; }

        public string EndTimeTag { get; set; }

        public DateTime IniTime { get; set; }

        public string IniTimeTag { get; set; }

        public Convertion convert { get; set; }


        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        protected ClenilStrong(string ClenilSpeedStrongTag, string EndTimeTag, string IniTimeTag, string RpmLimitTag, EquipamentType typeEq)
        {
            this.ClenilSpeedStrongTag = ClenilSpeedStrongTag;
            this.EndTimeTag = EndTimeTag;
            this.IniTimeTag = IniTimeTag;
            this.RpmLimitTag = RpmLimitTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
        }


        public static ClenilStrong GetClenilStrong()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ClenilStrong(StaticValues.TURBINESPEED, StaticValues.INISPEEDTIME, StaticValues.ENDSPEEDTIME, StaticValues.TAGCLENILFORTELIMIT, StaticValues.EQUIPAMENTTYPE);
                    };
                }
            }

            return _instance;
        }

        public void ReadPlc()
        {
            ClenilSpeedStrong = convert.convertToDouble(ClenilSpeedStrongTag,eq.Read(ClenilSpeedStrongTag));
            EndTime = DateTime.Now;
            IniTime = DateTime.Now;
            RpmLimit = 0.0;
        }
    }
}
