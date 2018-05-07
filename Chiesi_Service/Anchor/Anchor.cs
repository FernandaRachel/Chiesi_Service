using Chiesi.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Anchor
{
    class AnchorClass
    {
        private static AnchorClass _instance;

        private static object syncLock = new object();

        public double mixTime { get; set; }

        public double AnchorSpeed { get; set; }

        public string AnchorSpeedTag { get; set; }

        public double RpmLimit { get; set; }

        public string RpmLimitTag { get; set; }

        public DateTime EndTime { get; set; }

        public string EndTimeTag { get; set; }

        public DateTime IniTime { get; set; }

        public Convertion convert { get; set; }

        public string IniTimeTag { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;

        protected AnchorClass(string AnchorSpeedTag, string IniTimeTag, string EndTimeTag, EquipamentType typeEq)
        {
            this.AnchorSpeedTag = AnchorSpeedTag;
            this.IniTimeTag = IniTimeTag;
            this.RpmLimitTag = RpmLimitTag;
            this.EndTimeTag = EndTimeTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
        }


        public static AnchorClass GetAnchorClass()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new AnchorClass(StaticValues.ANCHORSPEED,  StaticValues.INISPEEDTIME, StaticValues.ENDSPEEDTIME, StaticValues.EQUIPAMENTTYPE);
                    };
                }
            }


            return _instance;
        }

        public void ReadPlc()
        {
            AnchorSpeed = convert.convertToDouble(AnchorSpeedTag, eq.Read(AnchorSpeedTag));
            RpmLimit = 1500;
            IniTime = DateTime.Now;
            EndTime = DateTime.Now;
        }
    }
}
