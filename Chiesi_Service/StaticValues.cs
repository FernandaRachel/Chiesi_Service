using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Chiesi
{
    public static class StaticValues
    {


        public static string connectionString = "";

        public static string ADDRESS = "";
        public static string FLOWTHEORICTAG = "";
        public static string FLOWREALTAG = "";
        public static string FLOWMETERLIMIT = "";
        public static string CELREALTAG = "";
        public static string CELLLIMIT = "";
        public static EquipamentType EQUIPAMENTTYPE = EquipamentType.PLC;
        public static string TAGDATA = "";
        public static string TAGHOUR = "";
        public static string TAGOPERADOR = "Cleitão da Massa";
        public static string TAGASSINATURA = "";
        public static string TAGVARFLOW = "";
        public static string TAGVARCELL = "";
        public static string TAGPRODUCTTEMP = "";
        public static string TAGSHAKINGSPEED = "";
        public static Array TEMPLIMIT = new Array[20, 24];
        public static string RPMLIMIT = "";
        public static string TAGCODE = "";
        public static string TAGBATCH = "";
        public static string TAGPRODUCT = "";
        public static string TAGSIGN = "1";
        public static string TAGSHAKESPEED = "";
        public static string TAGOUTFLOWSTART = "";
        public static string TAGOUTFLOWEND = "";
        public static string GLITHEORICTAG = "";
        public static string TANKWEITGHTTAG = "";
        public static double SHAKINGSPEEDFIVE = 5.0;
        public static double SHAKINGSPEEDTEN = 10.0;
        public static string ANCHORSPEED = "";
        public static string TURBINESPEED = "";
        public static string INISPEEDTIME = "";
        public static string ENDSPEEDTIME = "";
        public static string FILLINGTYPE = "";
        public static string TAGLIMITRPMRECIRCULTION = "";
        public static string TAGMIXTIME = "";
        public static string LOBULESSPEED = "";
        public static string TAGMINVALVE = "";
        public static string TAGCLENILSPEED = "";
        public static string TAGCLENILFORTELIMIT = "";
        public static string PATHERRORLOGCHIESI = "";
        public static string PATHLOGCHIESI = "";
        public static string TAGPRODUCTYPE = "";
        public static string TAGSUBTYPE = "";
        public static string TAGINITIMEVALVE = "";
        public static string TAGINITIMECLENIL = "";
        public static string TAGINITIMECLENILFORTE = "";
        public static string TAGINITIMETURBINE = "";
        public static string TAGLIFEBIT = "";
        public static string TAGERRORPLC = "";
        public static string LOBULESINITIME = "";
        public static string GLIOUTFLOWSTART = "";
        public static string GLIOUTFLOWSEND = "";
        public static string ANCHORREALMIXTIME = "";
        public static string TAGHIGHLIMITRPMRECIRCULTION = "";
        public static string TAGLOWLIMITRPMRECIRCULTION = "";
        public static string CELLREALTAG = "";
        public static string TAGTRIGGERSPEED = "";
        public static string TAGERRORMESSAGE = "";
        public static string LOGOPATH = "";
        public static string STYLESHEET = "";
        public static string RELATORIOPATH = "";
        public static string PATHADCHIESI = "";
        public static string PATHDUMP = "";
        public static string TAGCANCELOP = "";
        public static string TAGRECIPETYPE = "";
        public static string TAGFILLINGNAME = "";
        public static string TAGLIFEBITCHECK = "";
        public static string INIHOURMIXTIME = "";
        public static string ENDHOURMIXTIME = "";
        public static string INIHOURVALVE10 = "";
        public static string ENDHOURVALVE10 = "";
        public static string INIHOURVALVE9 = "";
        public static string ENDHOURVALVE9 = "";
        public static string INIHOURVALVE8 = "";
        public static string ENDHOURVALVE8 = "";
        public static string INIHOURHIGHMIX = "";
        public static string ENDHOURHIGHMIX = "";
        public static string INIHOURDRAIN = "";
        public static string ENDHOURDRAIN = "";
        public static string PATHLOGCHIESITOSAVE = "";


        public static string ReadConfig(string path)
        {
            StreamReader x;
            string linha = "";
            x = File.OpenText(path);
            while (x.EndOfStream != true)
                linha += x.ReadLine();
            x.Close();
            return linha;
        }
    }
}
