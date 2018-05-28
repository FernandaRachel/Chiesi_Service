using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi_Service.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Operation
{
    class EndOfManipulation : OperationHandler, IOperation
    {
        public bool checkBreak { get; set; }

        public bool gerarPdf { get; set; }

        public BasicInfoClass infos { get; set; }

        public Convertion convert { get; set; }

        public DateTime EndTime { get; set; }

        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        public string operationID { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public EndOfManipulation(EquipamentType typeEq, bool checkBreak, bool gerarPdf)
        {
            //ID da Operação - cada operação possui um ID exceto a incial(BeginOfMAnipulation)
            this.operationID = "12";
            this.infos = BasicInfoClass.GetBasicInfo();
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
            this.errorlog = new ErrorLog();
            this.checkBreak = checkBreak;
            this.gerarPdf = gerarPdf;
            this.logAction = new LogAction();
        }


        public bool checkError()
        {
            logAction.writeLog("Entrando no método 'checkError'");

            var tagerror = convert.convertToBoolean(ConfigurationManager.AppSettings["TAGERRORPLC"], eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

            while (tagerror)
            {
                tagerror = convert.convertToBoolean(ConfigurationManager.AppSettings["TAGERRORPLC"], eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));
                Thread.Sleep(500);
            }
            return tagerror;
        }



        public override void Calculate(Text txt)
        {
            logAction.writeLog("Entrando no método 'Calculate do EndOfManipulation' para iniciar leituras das tags necessárias");

            checkError();
            // It will search the infos correponding to the specific operation
            var operationInfos = successor.SearchInfoInList(this.eq, this.operationID);


            try
            {
                // PEGAR DO PLC HORA E DATA
                logAction.writeLog("Lendo hora do EndOfManipulation");
                EndTime = DateTime.Now;
                string endTimeString = EndTime.ToString("HH:mm");
                string endData = EndTime.ToString("dd/MM/yyyy");

                var x = CreateString(endData, endTimeString);

                // adiciona o texto na variavel global da classe Text
                txt.addItem(x);
                //salva o texto no log.txt
                txt.saveTxt(x, false);

            }
            catch (Exception e)
            {
                errorlog.writeLog("HighSpeedMix", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
            }

          

            if (successor != null)
            {
                if (gerarPdf)
                {
                    Pdf pdf = new Pdf(txt.txtAtual);
                    pdf.gerarPdf(txt.Header, infos);
                    txt.cleanTxt();
                }
                else
                {
                    successor.Calculate(txt);
                }
            }
            else
            {
                if (gerarPdf)
                {
                    Pdf pdf = new Pdf(txt.txtAtual);
                    pdf.gerarPdf(txt.Header, infos);
                    txt.cleanTxt();
                }
            }
        }

        public string CreateString(params string[] values)
        {
            logAction.writeLog("Iniciando CreateString");

            string breakline;

            if (checkBreak)
            {
                breakline = "<p class='fot'></p>";
            }
            else
            {
                breakline = "";
            }

            string txtCreate =
                            "<h3>Final da Manipulação</h3>" +
                            "<label>Data: </label><span class='campo'>" + values[0] + "</span>" +
                            "<label> Hora: </label><span class='campo'>" + values[1] + "</span>"
                            + breakline;
            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }


    }
}
