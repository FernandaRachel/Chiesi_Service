using Chiesi.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    public enum ReportType
    {

        ReportClenil,
        ReportPlacebos,
        ReportFostair,
        ReportClenilCompositum,

    }

    class ReportFactory
    {

        public ReportFactory()
        {

        }



        public IReport ConstructEquipament(int reportType, int subtype, EquipamentType eqType)
        {

            if (reportType == 1)
            {
                if (subtype == 1)
                {
                    return new ReportClenilSubType1(eqType);
                }
                else if (subtype == 2)
                {
                    return new ReportClenilSubType2(eqType);
                }
                else if (subtype == 3)
                {
                    return new ReportClenilSubType3(eqType);
                }
            }
            else if (reportType == 2)
            {
                if (subtype == 1)
                {
                    return new ReportPlacebosClenil(eqType);
                }
                else if (subtype == 2)
                {
                    return new ReportPlacebosCompositum(eqType);
                }
                else if (subtype == 3)
                {
                    // Este é o Placebos Fostair que possui o mesmo Layout do Placebos Compositum
                    return new ReportPlacebosCompositum(eqType);
                }
            }
            else if (reportType == 3)
            {
                return new ReportFostair(eqType);
            }
            else if (reportType == 4)
            {
                if (subtype == 1)
                {
                    return new ReportClenilCompositum(eqType);
                }
                else if (subtype == 2)
                {
                    return new ReportClenilCompositumForte(eqType);
                }
            }
            return null;
        }
    }
}
