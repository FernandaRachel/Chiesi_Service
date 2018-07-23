using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Products;
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
    class RecirculationHoseDrain : OperationHandler, IOperation
    {
        public ProductClass prod { get; set; }

        public DateTime IniTime { get; set; }

        public DateTime EndTime { get; set; }

        public string OperationName { get; set; }

        public BasicInfoClass infos { get; set; }

        public Convertion convert { get; set; }

        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        public string operationID { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public RecirculationHoseDrain(EquipamentType typeEq)
        {
            //ID da Operação - cada operação possui um ID exceto a incial(BeginOfMAnipulation)
            this.operationID = "11";
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.OperationName = OperationName;
            this.prod = ProductClass.GetProductClass();
            this.infos = BasicInfoClass.GetBasicInfo();
            this.convert = new Convertion(typeEq);
            this.errorlog = new ErrorLog();
            this.logAction = new LogAction();
        }

        public bool checkError()
        {
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
            logAction.writeLog("------------------- ID: " + this.operationID + "----------------");
            logAction.writeLog("Entrando no método 'Calculate do RecirculationHoseDrain' para iniciar leituras das tags necessárias");

            checkError();
            // It will search the infos correponding to the specific operation
            var operationInfos = SearchInfoInList(this.eq, this.operationID);

            bool gerarPdf = false;
            string iniTimeString = "";
            string endTimeString = "";
            string x = "";

            if (operationInfos.Count() > 0)
            {
                var result = operationInfos.ElementAt(0);

                try
                {

                    // PEGAR HORA E DATA DO PLC !!!!!!
                    iniTimeString = String.Format(result.Hora_0, "HH:mm");
                    endTimeString = String.Format(result.Hora_1, "HH:mm");
                    // ---------------------------------
                }
                catch (Exception e)
                {
                    errorlog.writeLog("RecirculationHoseDrain", "tag não especificada", e.ToString(), DateTime.Now);
                    this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                    this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                }

                x = CreateString(iniTimeString, endTimeString);

            }
            else
            {
                gerarPdf = true;
            }

            if (!gerarPdf)
            {
                txt.addItem(x);
                txt.saveTxt(x, false);

                logAction.writeLog("Texto adicionado ao log.txt");
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

        }

        public string CreateString(params string[] values)
        {
            logAction.writeLog("Iniciando CreateString");

            string txtCreate =
                "<h3>Drenagem da Mangueira de Recirculação</h3>" +
                "<label> Escoamento do Produto: &nbsp</label>Inicio : <span class='campo'>" + values[0] + "</span> Final : <span class='campo'>" + values[1] + "</span>";

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }

    }
}
