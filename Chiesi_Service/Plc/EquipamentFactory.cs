using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{



    public enum EquipamentType
    {
        Mock,
        PLC
    }

    class EquipamentFactory
    {
        private static EquipamentFactory _instance;
        private static object syncLock = new object();

        protected EquipamentFactory()
        {

        }

        public static EquipamentFactory GetEquipamentFactory()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new EquipamentFactory();
                    }
                }
            }

            return _instance;
        }

        public IEquipament ConstructEquipament(EquipamentType equipamentType)
        {

            switch (equipamentType)
            {
                //case EquipamentType.Mock:
                //    return EquipamentMock.GetEquipamentMock();

                case EquipamentType.PLC:
                    return EquipamentPLC.GetEquipamentPLC();

                default:
                    return null;

            }

        }
    }
}
