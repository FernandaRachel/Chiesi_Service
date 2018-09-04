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
    class ThirdLoadingClass : OperationHandler, IOperation
    {

        public double Variation { get; set; }

        public string headerName { get; set; }

        public string limitFlow { get; set; }

        public string limitCell { get; set; }

        public bool checkBreak { get; set; }

        public FlowmeterClass flux { get; set; }

        public LoadingCellClass cell { get; set; }

        public BasicInfoClass infos { get; set; }

        public GlicerolClass gli { get; set; }

        public ErrorLog errorlog { get; set; }

        //public //logAction //logAction { get; set; }

        public Convertion convert { get; set; }

        public string operationID { get; set; }

        public int index { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;

        //conts


        public ThirdLoadingClass(EquipamentType typeEq, string headerName, string limitFlow, string limitCell, bool checkBreak, int index)
        {
            //ID da Operação - cada operação possui um ID exceto a incial
            this.operationID = "5";
            this.index = index;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.headerName = headerName;
            this.flux = FlowmeterClass.GetFlowmeterClass();
            this.cell = LoadingCellClass.GetLoadingCellClass();
            this.infos = BasicInfoClass.GetBasicInfo();
            this.gli = GlicerolClass.GetGlicerolClass();
            this.errorlog = new ErrorLog();
            this.limitFlow = limitFlow;
            this.limitCell = limitCell;
            this.checkBreak = checkBreak;
            this.convert = new Convertion(typeEq);
            //this.logAction = new //logAction();

        }


        public bool checkError()
        {
            ////logAction.writeLog("Entrando no método 'checkError'");

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
            ////logAction.writeLog("------------------- ID: " + this.operationID + "----------------");
            ////logAction.writeLog("Entrando no método 'Calculate do ThirdLoading' para iniciar leituras das tags necessárias");

            checkError();
            // It will search the infos correponding to the specific operation
            var operationInfos = SearchInfoInList(this.eq, this.operationID);

            bool gerarPdf = false;
            string cellVariation = "";
            string flowvariation = "";
            string x = "";

            // Verifica se retornou alguma info
            // Se não retornou então a receita foi cancelada
            if ((index) < operationInfos.Count())
            {
                var result = operationInfos.ElementAt(index);

                try
                {
                    ////logAction.writeLog("Iniciando leituras das tags necessárias do ThirdLoading");

                    //PEGAR HORA DO PLC
                    ////logAction.writeLog("Lendo hora inicial da mistura do ThirdLoading");
                    gli.OutFlowStart = Convert.ToDateTime(result.Hora_0);

                    //LENDO VARIAÇÕES e QTD
                    ////logAction.writeLog("Iniciando leituras variações e quantidades");
                    cell.RealQty = convert.convertToDouble("result.Param_0", result.Param_0);
                    flux.RealQty = convert.convertToDouble("result.Param_1", result.Param_1);
                    flux.TheoricQty = result.Param_2.Replace(".", ",");
                    flowvariation = result.Param_3.Replace(".", ",");
                    cellVariation = result.Param_4.Replace(".", ",");

                    var auxIniFlow = Convert.ToDateTime(result.Hora_0);
                    gli.OutFlowEnd = auxIniFlow.AddMinutes(3);

                    // Define os novos valores do basic info = assinatura
                    this.infos.Hour = Convert.ToDateTime(result.Hora_1);
                    this.infos.Date = Convert.ToDateTime(result.Date);
                    this.infos.OperatorLogin = result.Asignature;
                    // ------------------------------------
                }
                catch (Exception e)
                {
                    errorlog.writeLog("ThirdLoading ", "tag não especificada", e.ToString(), DateTime.Now);
                    this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                    this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                }

                CultureInfo changeDotToComma = CultureInfo.GetCultureInfo("pt-BR");

                x = CreateString(String.Format(changeDotToComma, "{0:0.0}", gli.GliQty), String.Format(changeDotToComma, "{0:0.0}", flux.TheoricQty), String.Format(changeDotToComma, "{0:0.0}", flux.RealQty / 100), flowvariation,
                    String.Format(changeDotToComma, "{0:0.0}", flux.Limit), String.Format(changeDotToComma, "{0:0.0}", cell.RealQty / 100), cellVariation, cell.Limit.ToString(), gli.OutFlowStart.ToString("HH:mm:ss"), gli.OutFlowEnd.ToString("HH:mm:ss"));
            }
            else
            {
                gerarPdf = true;
            }


            if (!gerarPdf)
            {
                txt.addItem(x);
                txt.saveTxt(x, false);
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
                    "<tr>" +
                        "<td>Fluxímetro</td>" +
                        "<td>" + values[1] + "</td>" +
                        "<td>" + values[2] + "</td>" +
                        "<td>" + values[3] + "</td>" +
                        "<td> < " + limitFlow + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>Célula de Carga</td>" +
                        "<td>" + "" + "</td>" +
                        "<td>" + values[5] + "</td>" +
                        "<td>" + values[6] + "</td>" +
                        "<td> < " + limitCell + "</td>" +
                    "</tr>" +
                "</table>" +
                this.infos.CreateString()
                + breakline;

            return txtCreate;
        }

    }
}
