using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Glicerol;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi_Service.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Loading
{
    class FourthLoadingClass : OperationHandler, IOperation
    {

        public double Variation { get; set; }

        public string headerName { get; set; }

        public FlowmeterClass flux { get; set; }

        public LoadingCellClass cell { get; set; }

        public BasicInfoClass infos { get; set; }

        public GlicerolClass gli { get; set; }

        public string limitFlow { get; set; }

        public Convertion convert { get; set; }

        public string limitCell { get; set; }

        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        public string operationID { get; set; }

        public int index { get; set; }


        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;

        //conts


        public FourthLoadingClass(EquipamentType typeEq, string headerName, string limitFlow, string limitCell, int index)
        {
            //ID da Operação - cada operação possui um ID exceto a incial
            this.index = index;
            this.operationID = "5";
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.headerName = headerName;
            this.flux = FlowmeterClass.GetFlowmeterClass();
            this.cell = LoadingCellClass.GetLoadingCellClass();
            this.infos = BasicInfoClass.GetBasicInfo();
            this.gli = GlicerolClass.GetGlicerolClass();
            this.errorlog = new ErrorLog();
            this.limitCell = limitCell;
            this.limitFlow = limitFlow;
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


        /// <summary>
        /// Método utilizado para ler os equipamentos necessários para a atual operação
        /// </summary>
        public override void Calculate(Text txt)
        {
            logAction.writeLog("Entrando no método 'Calculate do FourthLoading' para iniciar leituras das tags necessárias");

            checkError();
            // It will search the infos correponding to the specific operation
            var operationInfos = successor.SearchInfoInList(this.eq, this.operationID);

            bool gerarPdf = false;
            DateTime keepinidate = DateTime.Now;
            string cellVariation = "";
            string flowvariation = "";

            try
            {
                logAction.writeLog("Iniciando leituras/escrita das tags necessárias");

                eq.Write(StaticValues.FLOWMETERLIMIT, limitFlow);
                eq.Write(StaticValues.CELLLIMIT, limitCell);
                Thread.Sleep(250);


                // DATA SERÁ PEGA DO PLC

                logAction.writeLog("Lendo hora inicial FourthLoading");
                if (Status.getStatus() != StatusType.Fail)
                {
                    keepinidate = DateTime.Now;
                }

                if (Status.getStatus() != StatusType.Fail)
                {
                    cellVariation = eq.Read(StaticValues.TAGVARCELL).Replace(".", ",");
                    flowvariation = eq.Read(StaticValues.TAGVARFLOW).Replace(".", ",");
                    this.flux.ReadPlc(); // inicializa os valores do Flowmeter
                    this.cell.ReadPlc(); // inicializa os valores da LoadingCell
                    this.infos.ReadPlc(); // inicializa os valores da BasicInfo
                    this.gli.ReadPlc(); // inicializa os valores de Glicerol
                    // DATA SERÁ PEGA DO PLC
                    gli.OutFlowStart = keepinidate;
                    gli.OutFlowEnd = DateTime.Now;
                    this.infos.Date = gli.OutFlowEnd;
                    // ------------------------------
                    Thread.Sleep(300);
                }

            }
            catch (Exception e)
            {
                errorlog.writeLog("FourthLoading", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
            }

            CultureInfo changeDotToComma = CultureInfo.GetCultureInfo("pt-BR");

            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", gli.GliQty), String.Format(changeDotToComma, "{0:0.0}", flux.TheoricQty), String.Format(changeDotToComma, "{0:0.0}", flux.RealQty / 100), flowvariation,
                String.Format(changeDotToComma, "{0:0.0}", flux.Limit), String.Format(changeDotToComma, "{0:0.0}", cell.RealQty / 100), cellVariation, cell.Limit.ToString(), gli.OutFlowStart.ToString("HH:mm"), gli.OutFlowEnd.ToString("HH:mm"));

            try
            {
                gerarPdf = convert.convertToBoolean(StaticValues.TAGCANCELOP, eq.Read(StaticValues.TAGCANCELOP));
            }
            catch (Exception e)
            {
                errorlog.writeLog("HighSpeedMix", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
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
            "<h3>" + headerName + "</h3>" +
            "<p>Escoamento de Glicerol:<br> Inicio: " + values[8] + "h - Final " + values[9] + "</p>" +
            "<table>" +
                "<tr>" +
                    "<th>Instrumento/Matéria Prima </th>" +
                    "<th>Qtd. Teórica Kg </th>" +
                    "<th>Qtd. Real Kg </th>" +
                    "<th>Variação % </th>" +
                    "<th>Limite % </th>" +
                "</tr>" +
                //"<tr>" +
                //    "<td>Glicerol</td>" +
                //    "<td>" + values[0] + "</td>" +
                //    "<td></td>" +
                //    "<td></td>" +
                //    "<td></td>" +
                //"</tr>" +
                "<tr>" +
                    "<td>Fluxímetro</td>" +
                    "<td>" + values[1] + "</td>" +
                    "<td>" + values[2] + "</td>" +
                    "<td>" + values[3] + "</td>" +
                    "<td> <" + limitFlow + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<td>Célula de Carga</td>" +
                    "<td>" + "" + "</td>" +
                    "<td>" + values[5] + "</td>" +
                    "<td>" + values[6] + "</td>" +
                    "<td> <" + limitCell + "</td>" +
                "</tr>" +
            "</table>" +
            this.infos.CreateString();

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }

    }
}
