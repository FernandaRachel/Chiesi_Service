using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi.Products;
using Chiesi.Shaker;
using Chiesi_Service.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Monitoring
{
    class TempMonitoringClass : OperationHandler, IMonitoring, IOperation

    {
        public int RpmLimit { get; set; }

        public int TempLimit { get; set; }

        public string OperationName { get; set; }

        public bool checkBreak { get; set; }

        public ShakerClass shaker { get; set; }

        public ProductClass prod { get; set; }

        public BasicInfoClass basicInfo { get; set; }

        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        public Convertion convert { get; set; }

        public string operationID { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public TempMonitoringClass(EquipamentType typeEq, bool checkBreak)
        {
            //ID da Operação - cada operação possui um ID exceto a incial
            this.operationID = "4";
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.shaker = ShakerClass.GetShakerClass();
            this.prod = ProductClass.GetProductClass();
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.errorlog = new ErrorLog();
            this.convert = new Convertion(typeEq);
            this.checkBreak = checkBreak;
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


        /// <summary>
        /// Método utilizado para calcular e/ou pegar valores que serão colocados dentro do txt 
        /// e passar o txt para a operação sucessora
        /// </summary>
        public override void Calculate(Text txt)
        {
            logAction.writeLog("------------------- ID: " + this.operationID + "----------------");

            logAction.writeLog("Entrando no método 'Calculate do TempMonitoring' para iniciar leituras das tags necessárias");

            checkError();
            // It will search the infos correponding to the specific operation
            var operationInfos = successor.SearchInfoInList(this.eq, this.operationID);
            var result = operationInfos.ElementAt(0);

            bool gerarPdf = false;
            double prodTemp = 0;

            try
            {

                logAction.writeLog("Iniciando leituras das tags necessárias de baixa velocidade");

                logAction.writeLog("Lendo temperatura e velocidade - baixa valocidade");
                this.prod.ProductTemp = convert.convertToDouble("result.Param_0", result.Param_0);
                this.shaker.ShakingSpeed= convert.convertToDouble("result.Param_1",result.Param_1);

                prodTemp = (this.prod.ProductTemp / 10);

               

                logAction.writeLog("Lendo basic info da mistura de baixa velocidade");
                // Define os novos valores do basic info = assinatura
                this.basicInfo.Hour = Convert.ToDateTime(result.Hora_0);
                this.basicInfo.Date = Convert.ToDateTime(result.Date);
                this.basicInfo.OperatorLogin = result.Asignature;

            }
            catch (Exception e)
            {
                errorlog.writeLog("TempMonitoring", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
            }

            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", prodTemp.ToString()), String.Format(changeDotToComma, "{0:0.0}", this.shaker.ShakingSpeed), this.shaker.RpmLimit.ToString());


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
                    pdf.gerarPdf(txt.Header, basicInfo);
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
                "<h3>Monitoramento de Temperatura e Agitação</h3>" +
                "<table>" +
                    "<tr>" +
                        "<th class='no-border'> </th>" +
                        "<th> Valor </th>" +
                        "<th > Limite </th>" +
                    "</tr>" +
                    "<tr >" +
                        "<td>Temperatura do Produto</td>" +
                        "<td>" + values[0] + "</td>" +
                        "<td> 20 a 24ºC </td>" +
                    "</tr>" +
                    "<tr><td>Velocidade de Agitação</td>" +
                        "<td>" + values[1] + "</td>" +
                        "<td>30 Rpm</td>" +
                    "</tr>" +
                "</table>" +
                basicInfo.CreateString()
                + breakline;

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }
    }
}
