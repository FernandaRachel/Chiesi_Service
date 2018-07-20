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


        public static void MainLogic(string[] args)
        {
            EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();
            IEquipament eq;
            LogAction logAction = new LogAction();

            try
            {
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

                        //while (!sign)
                        //{
                        //    sign = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["CANREAD"]));
                        //    Thread.Sleep(500);
                        //}

                        
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
                            logAction.writeLog("Entrando no ReadAllData() -----------------------");
                            eq.ReadAllData();

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
