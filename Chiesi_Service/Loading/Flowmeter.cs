using Chiesi.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Chiesi.Loading
{
    class FlowmeterClass
    {
        private static FlowmeterClass _instance;

        private static object syncLock = new object();

        public string TheoricQty { get; set; }

        public string TheoricQtyTag { get; set; }

        public double RealQty { get; set; }

        public string RealQtyTag { get; set; }

        public double FlowVariation { get; set; }

        public string FlowVariationTag { get; set; }

        public double Limit { get; set; }

        public Convertion convert { get; set; }

        public string LimitTag { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        protected FlowmeterClass(string TheoricQtyTag, string RealQtyTag, string FlowVariationTag, string LimitTag, EquipamentType typeEq)
        {

            this.TheoricQtyTag = TheoricQtyTag;
            this.RealQtyTag = RealQtyTag;
            this.LimitTag = LimitTag;
            this.FlowVariationTag = FlowVariationTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
        }


        public static FlowmeterClass GetFlowmeterClass()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new FlowmeterClass(StaticValues.FLOWTHEORICTAG, StaticValues.FLOWREALTAG, StaticValues.TAGVARFLOW, StaticValues.FLOWMETERLIMIT, StaticValues.EQUIPAMENTTYPE);
                    };
                }
            }


            return _instance;
        }

        public void ReadPlc()
        {
            TheoricQty = eq.Read(TheoricQtyTag).Replace(".", ",");
            RealQty = convert.convertToDouble(RealQtyTag, eq.Read(RealQtyTag));
            FlowVariation = convert.convertToDouble(FlowVariationTag, eq.Read(FlowVariationTag));
            Limit = convert.convertToDouble(LimitTag, eq.Read(LimitTag));

        }
    }
}
