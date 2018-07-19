using System;
using System.Threading;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi_Service.Log;

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
            StaticValues.TAGRECIPENAME = n.TAGRECIPENAME;
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
            StaticValues.PATHLOGACTION = n.PATHLOGACTION;
            
        }
        public static void MainLogic(string[] args)
        {
            EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();
            IEquipament eq;
            LogAction logAction = new LogAction();

            try
            {
                string JSONPATH = ConfigurationManager.AppSettings["EnderecoJson"];
                //var path = Directory.GetCurrentDirectory() + JSONPATH;
                //LerJson(JSONPATH);
                logAction.writeLog("Feita a leitura dos nomes das tags -> 'teste.json'");

                eq = eqFact.ConstructEquipament(EquipamentType.PLC);


                //int sign = Convert.ToInt32(eq.Read(StaticValues.TAGSIGN));
                int report;
                int subType;
                bool sign;
                bool signOkRead;
                Status.SetModeToIdle();

                Thread NewThread = new Thread(Process);
                NewThread.IsBackground = true;
                //NewThread.Start(); // inicia a Thread do LifeBit


                while (DontStopMeNOW)
                {
                    logAction.writeLog("Iniciando Loop referente ao relatório" + DateTime.Now.ToString("dd-MM-yyyy"));
                    // COLOCAR APÓS LOOP DE OKREAD
                    eq.ReadAllData(); 


                    if (Status.getStatus() != StatusType.Fail)
                    {
                        logAction.writeLog("Lendo tag de ler infos - verifica se esta 'true'");

                        sign = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["CANREAD"]));
                        signOkRead = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["OKREAD"]));

                        // Fica lendo o Sinal de "Pode ler" até que ele seja settado para True
                        // Se OkRead estiver 'True' ele espera até estar "False"
                        while (signOkRead)
                        {
                            signOkRead = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["OKREAD"]));
                            Thread.Sleep(500);
                        }

                        while (!sign)
                        {
                            sign = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["CANREAD"]));
                            Thread.Sleep(500);
                        }

                        // DESCOMENTAR ISSO NA HORA DE TESTAR
                        logAction.writeLog("Entrando no ReadAllData() -----------------------");
                        eq.ReadAllData();
                        logAction.writeLog("-------------------------------------------------");
                        logAction.writeLog("Lendo Tipo e Subtipo do relatório");

                        report = Convert.ToInt32(eq.Read(ConfigurationManager.AppSettings["TAGPRODUCTYPE"]));
                        subType = Convert.ToInt32(eq.Read(ConfigurationManager.AppSettings["TAGSUBTYPE"]));
                        // Setta tipo e subtipo do relatório
                        while (report == 0)
                        {
                            report = Convert.ToInt32(eq.Read(ConfigurationManager.AppSettings["TAGPRODUCTYPE"]));
                            Thread.Sleep(500);
                            Console.WriteLine("while");
                        }
                        while (subType == 0)
                        {
                            subType = Convert.ToInt32(eq.Read(ConfigurationManager.AppSettings["TAGSUBTYPE"]));
                            Thread.Sleep(500);
                        }

                        //Se o STATUS estiver OK(Idle) - Inicia o relatório
                        if (Status.getStatus() == StatusType.Idle)
                        {
                            eq.Write(ConfigurationManager.AppSettings["TAGPRODUCTYPE"], "0");
                            eq.Write(ConfigurationManager.AppSettings["TAGSUBTYPE"], "0");

                            // INICIA A MONTAGEM DO RELATÓRIO
                            ReportFactory rf = new ReportFactory();
                            logAction.writeLog("Iniciando Preenchimento do Relatório");
                            rf.ConstructEquipament(report, subType, EquipamentType.PLC);

                            // CONFIRM THAT ALL VALUES WERE READ
                            eq.Write(ConfigurationManager.AppSettings["OKREAD"], "True");

                            Thread.Sleep(2000);
                            Console.WriteLine("Press any key to exit.");
                            Console.Read();
                        }
                    }
                    else
                    {
                        while (eq.Read(ConfigurationManager.AppSettings["TAGLIFEBIT"]).Equals("False")) { Thread.Sleep(1000); }
                        Status.SetModeToIdle();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog errorlog = new ErrorLog();
                eq = eqFact.ConstructEquipament(EquipamentType.PLC);
                errorlog.writeLog("Logic", "Erro de início", e.ToString(), DateTime.Now);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");

            }
        }

        // SEGUNDA THREAD que fica lendo o LIFEBIT - PARA CHECAR COMUNICAÇÃO COM O PLC
        public static void Process()
        {
            EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();
            IEquipament eq;
            eq = eqFact.ConstructEquipament(EquipamentType.PLC);
            var readLife = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGLIFEBIT"]));
            var readLifeCheck = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGLIFEBITCHECK"]));
            int count = 0;

            while (DontStopMeNOW)
            {

                readLife = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGLIFEBIT"]));
                readLifeCheck = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGLIFEBITCHECK"]));

                if (readLife)
                {
                    count = 0;
                    eq.Write(ConfigurationManager.AppSettings["TAGLIFEBITCHECK"], "True");
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
                        //eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], "Falha de Conexão - LifeBit !");
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
