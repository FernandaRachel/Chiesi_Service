using Chiesi.Anchor;
using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi.Turbine;
using Chiesi_Service.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.AverageSpeed
{
    class LowSpeedMix : OperationHandler, IOperation
    {

        public string rpmLimit { get; set; }

        public string OperationName { get; set; }

        public string mixTime { get; set; }

        public bool checkBreak { get; set; }

        public string operationID { get; set; }

        public AnchorClass anchor { get; set; }

        public BasicInfoClass basicInfo { get; set; }

        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        public Convertion convert { get; set; }

        public Dictionary<string, string> TagsValues { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;


        public LowSpeedMix(EquipamentType typeEq, string mixTime, string rpmLimit, bool checkBreak)
        {
            //ID da Operação - cada operação possui um ID exceto a incial
            this.operationID = "8";
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.anchor = AnchorClass.GetAnchorClass();
            this.mixTime = mixTime;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.errorlog = new ErrorLog();
            this.rpmLimit = rpmLimit;
            this.checkBreak = checkBreak;
            this.convert = new Convertion(typeEq);
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
            logAction.writeLog("------------------- ID: " + this.operationID + "----------------");
            logAction.writeLog("Entrando no método 'Calculate do LowSpeedMix' para iniciar leituras das tags necessárias");

            checkError();

            // It will search the infos correponding to the specific operation
            var operationInfos = SearchInfoInList(this.eq, this.operationID);
            var result = operationInfos.ElementAt(0);
            bool gerarPdf = false;

            if (mixTime == "30")
                result = operationInfos.ElementAt(1);


            try
            {
                logAction.writeLog("Iniciando leituras das tags necessárias de baixa velocidade");

                // LENDO HORA INCIAL
                logAction.writeLog("Lendo hora inicial da mistura de baixa velocidade");
                anchor.IniTime = Convert.ToDateTime(result.Hora_0);

                // LENDO HORA FINAL
                logAction.writeLog("Lendo hora final da mistura de baixa velocidade");
                anchor.EndTime = Convert.ToDateTime(result.Hora_1);

                // LENDO VELOCIDADES E TEMPO DE MISTURA
                logAction.writeLog("Lendo velocidades da mistura de baixa velocidade e basic infos");
                anchor.AnchorSpeed = convert.convertToDouble("result.Param_0", result.Param_0);
                anchor.mixTime = convert.convertToDouble("mixTime", mixTime);

                // Define os novos valores do basic info = assinatura
                this.basicInfo.Hour = Convert.ToDateTime(result.Hora_1);
                this.basicInfo.Date = Convert.ToDateTime(result.Date);
                this.basicInfo.OperatorLogin = result.Asignature;

            }
            catch (Exception e)
            {
                errorlog.writeLog("Low SpeedMix", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
            }

            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", anchor.AnchorSpeed), anchor.IniTime.ToString("HH:mm"), anchor.EndTime.ToString("HH:mm"), anchor.RpmLimit.ToString(), anchor.mixTime.ToString());


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
                "<h3>Mistura Baixa Velocidade</h3>" +
                "<table>" +
                    "<tr>" +
                        "<th></th>" +
                        "<th>Velocidade Agitador </th>" +
                        "<th>Inicio </th>" +
                        "<th>Termino </th>" +
                        "<th>Limite </th>" +
                        "<th>Tempo de Mistura </th>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>Agitador Âncora</td>" +
                        "<td>" + values[0] + "Rpm</td>" +
                        "<td>" + values[1] + "</td>" +
                        "<td>" + values[2] + "</td>" +
                        "<td>" + rpmLimit + "Rpm</td>" +
                        "<td>" + values[4] + "min</td>" +
                    "</tr>" +
                "</table>"
                + basicInfo.CreateString()
                + breakline;

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }

        public void TagValuesStored(params string[] values)
        {
            Dictionary<string, string> tagsValues = new Dictionary<string, string>();
            tagsValues.Add(StaticValues.TAGPRODUCTTEMP, values[0]);
            tagsValues.Add(StaticValues.TAGSHAKESPEED, values[1]);


            this.TagsValues = tagsValues;
        }


    }
}
