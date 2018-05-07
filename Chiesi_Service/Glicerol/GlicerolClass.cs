using Chiesi.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Glicerol
{
    class GlicerolClass
    {
        private static GlicerolClass _instance;

        private static object syncLock = new object();

        public double GliQty { get; set; }

        public string GliQtyTag { get; set; }

        public DateTime OutFlowStart { get; set; }

        public string OutFlowStartTag { get; set; }

        public DateTime OutFlowEnd { get; set; }

        public string OutFlowEndTag { get; set; }

        public Convertion convert { get; set; }


        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        protected GlicerolClass(string GliQtyTag, string OutFlowStartTag, string OutFlowEndTag, EquipamentType typeEq)
        {
            this.GliQtyTag = GliQtyTag;
            this.OutFlowStartTag = OutFlowStartTag;
            this.OutFlowEndTag = OutFlowEndTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
        }


        public static GlicerolClass GetGlicerolClass()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new GlicerolClass(StaticValues.GLITHEORICTAG, StaticValues.TAGOUTFLOWSTART, StaticValues.TAGOUTFLOWEND, StaticValues.EQUIPAMENTTYPE);
                    };
                }
            }

            return _instance;
        }

        public void ReadPlc()
        {
            GliQty = convert.convertToDouble(GliQtyTag, eq.Read(GliQtyTag));
            OutFlowStart = DateTime.Now;
            OutFlowEnd = DateTime.Now;
        }

    }
}
