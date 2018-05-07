using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Glicerol;
using Chiesi.Log;
using Chiesi.Operation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Loading
{
    class SecondLoadingClass : OperationHandler, IOperation
    {

        public double Variation { get; set; }

        public string headerName { get; set; }

        public string FirstLoadingName { get; set; }

        public FlowmeterClass flux { get; set; }

        public LoadingCellClass cell { get; set; }

        public BasicInfoClass infos { get; set; }

        public GlicerolClass gli { get; set; }

        public string limitFlow { get; set; }

        public string limitCell { get; set; }

        public bool changeColumn { get; set; }

        public bool checkBreak { get; set; }

        public ErrorLog errorlog { get; set; }

        public Convertion convert { get; set; }


        public Dictionary<string, string> TagsValues { get; set; }


        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;

        //conts


        public SecondLoadingClass(EquipamentType typeEq, string headerName, string limitFlow, string limitCell, 
            bool changeColumn, bool checkBreak)
        {
            this.TagsValues = new Dictionary<string, string>();
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.headerName = headerName;
            this.flux = FlowmeterClass.GetFlowmeterClass();
            this.cell = LoadingCellClass.GetLoadingCellClass();
            this.infos = BasicInfoClass.GetBasicInfo();
            this.gli = GlicerolClass.GetGlicerolClass();
            this.limitFlow = limitFlow;
            this.limitCell = limitCell;
            this.checkBreak = checkBreak;
            this.changeColumn = changeColumn;
            this.errorlog = new ErrorLog();
            this.convert = new Convertion(typeEq);
        }


        public bool checkError()
        {
            var tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC,eq.Read(StaticValues.TAGERRORPLC));

            while (tagerror)
            {
                tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC,eq.Read(StaticValues.TAGERRORPLC));
                Thread.Sleep(1000);
            }
            return tagerror;
        }

        public bool WaitSign()
        {
            var tagerror = checkError();

            var sign = convert.convertToBoolean(StaticValues.TAGSIGN, eq.Read(StaticValues.TAGSIGN));

            //configuravel
            if (!tagerror)
            {
                while (!sign)
                {
                    sign = convert.convertToBoolean(StaticValues.TAGSIGN, eq.Read(StaticValues.TAGSIGN));
                }
            }
            else
            {
                while (tagerror)
                {
                    tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC,eq.Read(StaticValues.TAGERRORPLC));
                }
                return WaitSign();
            }

            return sign;
        }

        /// <summary>
        /// Método utilizado para ler os equipamentos necessários para a atual operação
        /// </summary>
        public override void Calculate(Text txt)
        {
            //var signal = WaitSign();
            checkError();
            bool gerarPdf = false;
            bool inidate;
            bool enddate;
            DateTime keepinidate = DateTime.Now;
            string cellVariation = "";
            string flowvariation = "";

            try
            {
                inidate = convert.convertToBoolean(StaticValues.INISPEEDTIME,this.eq.Read(StaticValues.INISPEEDTIME));
                enddate = convert.convertToBoolean(StaticValues.ENDSPEEDTIME,this.eq.Read(StaticValues.ENDSPEEDTIME));
                eq.Write(StaticValues.FLOWMETERLIMIT, limitFlow);
                eq.Write(StaticValues.CELLLIMIT, limitCell);
                Thread.Sleep(300);

                while (Status.getStatus() != StatusType.Fail && inidate == false)
                {
                    inidate = convert.convertToBoolean(StaticValues.INISPEEDTIME,this.eq.Read(StaticValues.INISPEEDTIME));
                }
                if (inidate == true)
                {
                    keepinidate = DateTime.Now;
                    this.eq.Write(StaticValues.INISPEEDTIME, "False");
                }
                while (Status.getStatus() != StatusType.Fail && enddate == false)
                {
                    enddate = convert.convertToBoolean(StaticValues.ENDSPEEDTIME,this.eq.Read(StaticValues.ENDSPEEDTIME));
                }
                if (enddate == true)
                {
                    cellVariation = eq.Read(StaticValues.TAGVARCELL).Replace(".", ",");
                    flowvariation = eq.Read(StaticValues.TAGVARFLOW).Replace(".", ",");
                    this.flux.ReadPlc(); // inicializa os valores do Flowmeter
                    this.cell.ReadPlc(); // inicializa os valores da LoadingCell
                    this.infos.ReadPlc(); // inicializa os valores da BasicInfo
                    this.gli.ReadPlc(); // inicializa os valores de Glicerol
                    gli.OutFlowStart = keepinidate;
                    gli.OutFlowEnd = DateTime.Now;
                    this.infos.Date = gli.OutFlowEnd;
                    this.eq.Write(StaticValues.ENDSPEEDTIME, "False");
                    Thread.Sleep(200);
                }
            }
            catch (Exception e)
            {
                errorlog.writeLog("SecondLoading", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

            CultureInfo changeDotToComma = CultureInfo.GetCultureInfo("pt-BR");

            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", gli.GliQty), String.Format(changeDotToComma, "{0:0.0}", flux.TheoricQty), String.Format(changeDotToComma, "{0:0.0}", flux.RealQty/100), flowvariation,
                String.Format(changeDotToComma, "{0:0.0}", flux.Limit), String.Format(changeDotToComma, "{0:0.0}", cell.RealQty/100), cellVariation, cell.Limit.ToString(), gli.OutFlowStart.ToString("HH:mm"), gli.OutFlowEnd.ToString("HH:mm"));

            try
            {
                gerarPdf = convert.convertToBoolean(StaticValues.TAGCANCELOP, eq.Read(StaticValues.TAGCANCELOP));
            }
            catch (Exception e)
            {
                errorlog.writeLog("HighSpeedMix", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
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
            string column;
            string escoamento;
            string breakline;

            if (checkBreak)
            {
                breakline = "<p class='fot'></p>";
            }else
            {
                breakline = "";
            }


            if (changeColumn)
            {
                escoamento = "";

                column = "";
            }
            else
            {
                escoamento = "<p>Escoamento de Glicerol:<br> Inicio: " + values[8] + "h - Final " + values[9] + "</p>";

                column = "";
            }

            string txtCreate =
                "<h3>" + headerName + "</h3>" +
                escoamento +
                "<table>" +
                    "<tr>" +
                        "<th>Instrumento/Matéria Prima</th>" +
                        "<th>Qtd. Teórica Kg</th>" +
                        "<th>Qtd. Real Kg</th>" +
                        "<th>Variação %</th>" +
                        "<th>Limite %</th>" +
                    "</tr>" +
                    column +
                    "<tr>" +
                        "<td>Fluxímetro</td>" +
                        "<td>" + values[1] + "</td>" +
                        "<td>" + values[2] + "</td>" +
                        "<td>" + values[3] + "</td>" +
                        "<td> < " + limitFlow + "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>Célula de Carga</td>" +
                        "<td></td>" +
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
