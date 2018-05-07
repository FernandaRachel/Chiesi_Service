using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Loading
{
    class LoadingCellClass
    {
        private static LoadingCellClass _instance;
        private static object syncLock = new object();

        public double RealQty { get; set; }

        public string RealQtyTag { get; set; }

        public double CellVariation { get; set; }

        public string CellVariationTag { get; set; }

        public double Limit { get; set; }

        public string LimitTag { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;

        protected LoadingCellClass(string RealQtyTag, string LimitTag, string CellVariationTag, EquipamentType typeEq)
        {

            this.RealQtyTag = RealQtyTag;
            this.LimitTag = LimitTag;
            this.CellVariationTag = CellVariationTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
        }

        public static LoadingCellClass GetLoadingCellClass()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new LoadingCellClass(StaticValues.CELLREALTAG, StaticValues.CELLLIMIT, StaticValues.TAGVARCELL, StaticValues.EQUIPAMENTTYPE);

                    };
                }
            }

            return _instance;
        }

        public void ReadPlc()
        {
            RealQty = Convert.ToDouble(eq.Read(RealQtyTag));
            CellVariation = Convert.ToDouble(eq.Read(CellVariationTag));
            Limit = 0.0;
        }
    }

}