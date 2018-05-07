using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Chiesi
{
    class NonStaticValues
    {
        //public string connectionString = @"Data Source=SPIN087\SQLEXPRESS;Initial Catalog = SPI_INTERLEVEL_DB; Integrated Security = True; Connect Timeout = 15; Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public string connectionString { get; set; }
        public string ADDRESS { get; set; }
        public string FLOWTHEORICTAG { get; set; }
        public string FLOWREALTAG { get; set; }
        public string FLOWMETERLIMIT { get; set; }
        public string CELREALTAG { get; set; }
        public string CELLLIMIT { get; set; }

        public string TAGDATA { get; set; }
        public string TAGHOUR { get; set; }
        //public string TAGOPERADOR { get; set; }
        public string TAGASSINATURA { get; set; }
        public string TAGVARFLOW { get; set; }
        public string TAGVARCELL { get; set; }
        public string TAGPRODUCTTEMP { get; set; }
        public string TAGSHAKESPEED { get; set; }

        public Array TEMPLIMIT = new Array[20, 24];
        public string RPMLIMIT { get; set; }
        public string TAGCODE { get; set; }

        public string RELATORIOPATH { get; set; }
        public string STYLESHEET { get; set; }
        public string LOGOPATH { get; set; }
        public string TAGBATCH { get; set; }
        public string TAGPRODUCT { get; set; }
        public string TAGSIGN { get; set; }
        public string TAGOUTFLOWSTART { get; set; }
        public string TAGOUTFLOWEND { get; set; }
        public string GLITHEORICTAG { get; set; }
        public string TANKWEITGHTTAG { get; set; }
        public double SHAKINGSPEEDFIVE = 5.0;
        public double SHAKINGSPEEDTEN = 10.0;
        public string ANCHORSPEED { get; set; }
        public string TURBINESPEED { get; set; }
        public string INISPEEDTIME { get; set; }
        public string ENDSPEEDTIME { get; set; }
        public string FILLINGTYPE { get; set; }
        public string TAGLIMITRPMRECIRCULTION { get; set; }
        public string TAGMIXTIME { get; set; }
        public string LOBULESSPEED { get; set; }
        public string TAGMINVALVE { get; set; }
        public string TAGCLENILFORTESPEED { get; set; }
        public string TAGCLENILFORTELIMIT { get; set; }
        public string PATHLOGCHIESI { get; set; }
        public string PATHERRORLOGCHIESI { get; set; }
        public string TAGPRODUCTYPE { get; set; }
        public string TAGSUBTYPE { get; set; }
        public string TAGINITIMEVALVE { get; set; }
        public string TAGINITIMECLENIL { get; set; }
        public string TAGERRORPLC { get; set; }

        public string TAGINITIMECLENILFORTE { get; set; }
        public string TAGINITIMETURBINE { get; set; }
        public string TAGLIFEBIT { get; set; }

        public string LOBULESINITIME { get; set; }
        public string GLIOUTFLOWSTART { get; set; }
        public string GLIOUTFLOWSEND { get; set; }
        public string ANCHORREALMIXTIME { get; set; }
        public string TAGHIGHLIMITRPMRECIRCULTION { get; set; }
        public string TAGLOWLIMITRPMRECIRCULTION { get; set; }
        public string CELLREALTAG { get; set; }
        public string ANCHORENDSPEEDTIME { get; set; }
        public string TAGTRIGGERSPEED { get; set; }

        public string TAGERRORMESSAGE { get; set; }

        public string PATHADCHIESI { get; set; }

        public string PATHDUMP { get; set; }

        public string TAGCANCELOP { get; set; }

        public string TAGRECIPETYPE { get; set; }

        public string TAGFILLINGNAME { get; set; }

        public string TAGLIFEBITCHECK { get; set; }

        public string INIHOURMIXTIME { get; set; }

        public string ENDHOURMIXTIME { get; set; }

        public string INIHOURVALVE10 { get; set; }

        public string ENDHOURVALVE10 { get; set; }

        public string INIHOURVALVE9 { get; set; }

        public string ENDHOURVALVE9 { get; set; }

        public string INIHOURVALVE8 { get; set; }

        public string ENDHOURVALVE8 { get; set; }

        public string INIHOURHIGHMIX { get; set; }

        public string ENDHOURHIGHMIX { get; set; }

        public string INIHOURDRAIN { get; set; }

        public string ENDHOURDRAIN { get; set; }


        public string ReadConfig(string path)
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

