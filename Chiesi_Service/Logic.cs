using System;
using System.Threading;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Chiesi.Converter;
using Chiesi.Log;

namespace Chiesi
{

    public static class Logic
    {



        public static bool DontStopMeNOW { get; set; }
        public static void LerJson(string path)
        {
            string json = StaticValues.ReadConfig(path);
            NonStaticValues n = JsonConvert.DeserializeObject<NonStaticValues>(json);
            StaticValues.FLOWTHEORICTAG = n.FLOWTHEORICTAG;
            StaticValues.FLOWREALTAG = n.FLOWREALTAG;
            StaticValues.FLOWMETERLIMIT = n.FLOWMETERLIMIT;
            StaticValues.FLOWREALTAG = n.FLOWREALTAG;
            StaticValues.CELREALTAG = n.CELREALTAG;
            StaticValues.CELLLIMIT = n.CELLLIMIT;
            StaticValues.TAGASSINATURA = n.TAGASSINATURA;
            StaticValues.TAGVARFLOW = n.TAGVARFLOW;
            StaticValues.TAGVARCELL = n.TAGVARCELL;
            StaticValues.TAGPRODUCTTEMP = n.TAGPRODUCTTEMP;
            StaticValues.RPMLIMIT = n.RPMLIMIT;
            StaticValues.TAGCODE = n.TAGCODE;
            StaticValues.TAGBATCH = n.TAGBATCH;
            StaticValues.TAGPRODUCT = n.TAGPRODUCT;
            StaticValues.TAGSIGN = n.TAGSIGN;
            StaticValues.TAGSHAKESPEED = n.TAGSHAKESPEED;
            StaticValues.TAGOUTFLOWSTART = n.TAGOUTFLOWSTART;
            StaticValues.TAGOUTFLOWEND = n.TAGOUTFLOWEND;
            StaticValues.GLITHEORICTAG = n.GLITHEORICTAG;
            StaticValues.TANKWEITGHTTAG = n.TANKWEITGHTTAG;
            StaticValues.ANCHORSPEED = n.ANCHORSPEED;
            StaticValues.INISPEEDTIME = n.INISPEEDTIME;
            StaticValues.ENDSPEEDTIME = n.ENDSPEEDTIME;
            StaticValues.FILLINGTYPE = n.FILLINGTYPE;
            StaticValues.TAGLIMITRPMRECIRCULTION = n.TAGLIMITRPMRECIRCULTION;
            StaticValues.TAGMIXTIME = n.TAGMIXTIME;
            StaticValues.LOBULESSPEED = n.LOBULESSPEED;
            StaticValues.TAGMINVALVE = n.TAGMINVALVE;
            StaticValues.PATHLOGCHIESI = n.PATHLOGCHIESI;
            StaticValues.PATHERRORLOGCHIESI = n.PATHERRORLOGCHIESI;
            StaticValues.connectionString = n.connectionString;
            StaticValues.TAGPRODUCTYPE = n.TAGPRODUCTYPE;//Mudar para int
            StaticValues.TAGINITIMEVALVE = n.TAGINITIMEVALVE;
            StaticValues.TAGINITIMECLENIL = n.TAGINITIMECLENIL;
            StaticValues.TAGINITIMECLENILFORTE = n.TAGINITIMECLENILFORTE;
            StaticValues.TAGINITIMETURBINE = n.TAGINITIMETURBINE;
            StaticValues.TAGLIFEBIT = n.TAGLIFEBIT;
            StaticValues.TAGERRORPLC = n.TAGERRORPLC;
            StaticValues.LOBULESINITIME = n.LOBULESINITIME;
            StaticValues.GLIOUTFLOWSTART = n.GLIOUTFLOWSTART;
            StaticValues.GLIOUTFLOWSEND = n.GLIOUTFLOWSEND;
            StaticValues.ANCHORREALMIXTIME = n.ANCHORREALMIXTIME;
            StaticValues.TAGHIGHLIMITRPMRECIRCULTION = n.TAGHIGHLIMITRPMRECIRCULTION;
            StaticValues.TAGLOWLIMITRPMRECIRCULTION = n.TAGLOWLIMITRPMRECIRCULTION;
            StaticValues.CELLREALTAG = n.CELLREALTAG;
            StaticValues.TAGERRORMESSAGE = n.TAGERRORMESSAGE;
            StaticValues.TAGTRIGGERSPEED = n.TAGTRIGGERSPEED;
            StaticValues.LOGOPATH = n.LOGOPATH;
            StaticValues.RELATORIOPATH = n.RELATORIOPATH;
            StaticValues.STYLESHEET = n.STYLESHEET;
            StaticValues.TURBINESPEED = n.TURBINESPEED;
            StaticValues.TAGCLENILFORTELIMIT = n.TAGCLENILFORTELIMIT;
            StaticValues.PATHADCHIESI = n.PATHADCHIESI;
            StaticValues.PATHDUMP = n.PATHDUMP;
            StaticValues.TAGCANCELOP = n.TAGCANCELOP;
            StaticValues.TAGSUBTYPE = n.TAGSUBTYPE;
            StaticValues.TAGRECIPETYPE = n.TAGRECIPETYPE;
            StaticValues.TAGFILLINGNAME = n.TAGFILLINGNAME;
            StaticValues.TAGLIFEBITCHECK = n.TAGLIFEBITCHECK;
            StaticValues.INIHOURMIXTIME = n.INIHOURMIXTIME;
            StaticValues.ENDHOURMIXTIME = n.ENDHOURMIXTIME;
            StaticValues.INIHOURVALVE10 = n.INIHOURVALVE10;
            StaticValues.INIHOURVALVE10 = n.INIHOURVALVE10;
            StaticValues.ENDHOURVALVE10 = n.ENDHOURVALVE10;
            StaticValues.INIHOURVALVE9 = n.INIHOURVALVE9;
            StaticValues.ENDHOURVALVE9 = n.ENDHOURVALVE9;
            StaticValues.INIHOURVALVE8 = n.INIHOURVALVE8;
            StaticValues.ENDHOURVALVE8 = n.ENDHOURVALVE8;
            StaticValues.INIHOURHIGHMIX = n.INIHOURHIGHMIX;
            StaticValues.ENDHOURHIGHMIX = n.ENDHOURHIGHMIX;
            StaticValues.INIHOURDRAIN = n.INIHOURDRAIN;
            StaticValues.ENDHOURDRAIN = n.ENDHOURDRAIN;
            StaticValues.PATHLOGCHIESITOSAVE = n.PATHLOGCHIESITOSAVE;
        }
        public static void MainLogic(string[] args)
        {
            EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();
            IEquipament eq;

            try
            {
                
                string JSONPATH = ConfigurationManager.AppSettings["EnderecoJson"];
                //var path = Directory.GetCurrentDirectory() + JSONPATH;
                LerJson(JSONPATH);

                eq = eqFact.ConstructEquipament(EquipamentType.PLC);

                var a1 = eq.Read(StaticValues.INIHOURVALVE10);
                var a2 = eq.Read(StaticValues.ENDHOURVALVE10);
                var a3 = eq.Read(StaticValues.INIHOURVALVE9);
                var a4 = eq.Read(StaticValues.ENDHOURVALVE9);
                var a5 = eq.Read(StaticValues.INIHOURVALVE8);
                var a6 = eq.Read(StaticValues.ENDHOURVALVE8);
                var a7 = eq.Read(StaticValues.INIHOURHIGHMIX);
                var a8 = eq.Read(StaticValues.ENDHOURHIGHMIX);
                var a9 = eq.Read(StaticValues.INIHOURDRAIN);
                var a10 = eq.Read(StaticValues.ENDHOURDRAIN);

                //int sign = Convert.ToInt32(eq.Read(StaticValues.TAGSIGN));
                int report;
                int subType;
                bool sign;
                Status.SetModeToIdle();

                Thread NewThread = new Thread(Process);
                NewThread.IsBackground = true;
                NewThread.Start(); // inicia a Thread do LifeBit

                while (DontStopMeNOW)
                {
                    if (Status.getStatus() != StatusType.Fail)
                    {
                        sign = Convert.ToBoolean(eq.Read(StaticValues.TAGSIGN));
                        var cancelOp = Convert.ToBoolean(eq.Read(StaticValues.TAGCANCELOP));
                        //verifica se a operação foi cancelada e derruba as TAGS - EVITA ERRO DE ACHAR QUE A OP INICIOU
                        if (cancelOp)
                        {
                            eq.Write(StaticValues.INISPEEDTIME, "False");
                            eq.Write(StaticValues.ENDSPEEDTIME, "False");
                            eq.Write(StaticValues.TAGTRIGGERSPEED, "False");
                            eq.Write(StaticValues.TAGCANCELOP, "False");
                        }

                        // só setta os tipos e subtipos dos relatórios caso a op se inicie
                        while (!sign)
                        {
                            if (cancelOp)
                            {
                                eq.Write(StaticValues.ENDSPEEDTIME, "False");
                                eq.Write(StaticValues.TAGTRIGGERSPEED, "False");
                                eq.Write(StaticValues.TAGCANCELOP, "False");
                            }
                            sign = Convert.ToBoolean(eq.Read(StaticValues.TAGSIGN));
                            Thread.Sleep(500);
                        }

                        report = Convert.ToInt32(eq.Read(StaticValues.TAGPRODUCTYPE));
                        subType = Convert.ToInt32(eq.Read(StaticValues.TAGSUBTYPE));

                        while (report == 0)
                        {
                            report = Convert.ToInt32(eq.Read(StaticValues.TAGPRODUCTYPE));
                            Thread.Sleep(500);
                            Console.WriteLine("while");
                        }
                        while (subType == 0)
                        {
                            subType = Convert.ToInt32(eq.Read(StaticValues.TAGSUBTYPE));
                            Thread.Sleep(500);
                        }

                        // preciso ficar lendo o sinal no MAIN, ou apenas entre as op ?????????????
                        if (Status.getStatus() == StatusType.Idle)
                        {
                            eq.Write(StaticValues.TAGCANCELOP, "False");
                            eq.Write(StaticValues.TAGPRODUCTYPE, "0");
                            eq.Write(StaticValues.TAGSUBTYPE, "0");
                            eq.Write(StaticValues.ENDSPEEDTIME, "False");
                            //eq.Write(StaticValues.INISPEEDTIME, "False");
                            eq.Write(StaticValues.TAGTRIGGERSPEED, "False");
                            //eq.Write(StaticValues.TAGSIGN, "False");

                            ReportFactory rf = new ReportFactory();
                            rf.ConstructEquipament(report, subType, EquipamentType.PLC);// trocar o zero pela Tag do tipo de produto
                                                                                        // executa tal relatório
                            Thread.Sleep(2000);
                            Console.WriteLine("Press any key to exit.");
                            Console.Read();
                        }
                    }
                    else
                    {
                        while (eq.Read(StaticValues.TAGLIFEBIT).Equals("False")) { Thread.Sleep(1000); }
                        Status.SetModeToIdle();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog errorlog = new ErrorLog();
                eq = eqFact.ConstructEquipament(EquipamentType.PLC);
                errorlog.writeLog("Logic", "Erro de início", e.ToString(), DateTime.Now);
                eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                eq.Write(StaticValues.TAGERRORPLC, "True");

            }
        }

        public static void Process()
        {
            EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();
            IEquipament eq;
            eq = eqFact.ConstructEquipament(EquipamentType.PLC);
            var readLife = Convert.ToBoolean(eq.Read(StaticValues.TAGLIFEBIT));
            var readLifeCheck = Convert.ToBoolean(eq.Read(StaticValues.TAGLIFEBITCHECK));
            int count = 0;

            while (DontStopMeNOW)
            {

                readLife = Convert.ToBoolean(eq.Read(StaticValues.TAGLIFEBIT));
                readLifeCheck = Convert.ToBoolean(eq.Read(StaticValues.TAGLIFEBITCHECK));

                if (readLife)
                {
                    count = 0;
                    eq.Write(StaticValues.TAGLIFEBITCHECK, "True");
                    Thread.Sleep(20000);
                }
                else if (!readLife)
                {
                    if (count > 10)
                    {
                        ErrorLog errorlog = new ErrorLog();
                        Status.SetFailMode();
                        errorlog.writeLog("Second Thread", "Falha de Conexão", "Falha de Conexão - LifeBit !", DateTime.Now);
                        //eq.Write(StaticValues.TAGERRORPLC, "True");
                        //eq.Write(StaticValues.TAGERRORMESSAGE, "Falha de Conexão - LifeBit !");
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        count++;
                    }

                }

            }
        }


    }

}
