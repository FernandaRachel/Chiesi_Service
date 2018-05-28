using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Products
{
    class ProductClass
    {
        private static ProductClass _instance;

        private static object syncLock = new object();

        public string Code { get; set; }

        public string CodeTag { get; set; }

        public string Product { get; private set; }

        public string ProductTag { get; set; }

        public string Batch { get; private set; }

        public string BatchTag { get; set; }

        public double ProductTemp { get; set; }

        public string ProductTempTag { get; set; }


        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;

        protected ProductClass(string CodeTag, string ProductTag, string BatchTag, string ProductTempTag, EquipamentType typeEq)
        {
            this.CodeTag = CodeTag;
            this.ProductTag = ProductTag;
            this.BatchTag = BatchTag;
            this.ProductTempTag = ProductTempTag;

            this.eq = this.eqFact.ConstructEquipament(typeEq);

        }

        public static ProductClass GetProductClass()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ProductClass(StaticValues.TAGCODE, StaticValues.TAGPRODUCT, StaticValues.TAGBATCH,
                            StaticValues.TAGPRODUCTTEMP, StaticValues.EQUIPAMENTTYPE);
                    };
                }
            }


            return _instance;
        }


        public void ReadPlc()
        {
            Code = eq.Read(CodeTag);
            Product = eq.Read(ProductTag);
            Batch = eq.Read(BatchTag);
            ProductTemp = Convert.ToDouble(eq.Read(ProductTempTag));
        }


    }
}
