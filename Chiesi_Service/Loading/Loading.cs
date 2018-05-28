using Chiesi.BasicInfos;
using Chiesi.Converter;
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
    class FirstLoading : OperationHandler, IOperation
    {
        public string headerName { get; set; }

        public double Variation { get; set; }

        public FlowmeterClass flux { get; set; }

        public LoadingCellClass cell { get; set; }

        public BasicInfoClass infos { get; set; }

        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        public string limitFlow { get; set; }

        public string limitCell { get; set; }

        public Convertion convert { get; set; }

        public string operationID { get; set; }

        public int index { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;

        //conts

        public FirstLoading(EquipamentType typeEq, string headerName, string limitFlow, string limitCell, int index)
        {
            this.operationID = "3";
            this.index = index;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.headerName = headerName;
            this.flux = FlowmeterClass.GetFlowmeterClass();
            this.cell = LoadingCellClass.GetLoadingCellClass();
            this.infos = BasicInfoClass.GetBasicInfo();
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
                Thread.Sleep(400);
            }
            return tagerror;
        }


        /// <summary>
        /// Método utilizado para ler os equipamentos necessários para a atual operação
        /// </summary>
        public override void Calculate(Text txt)
        {
            logAction.writeLog("Entrando no método 'Calculate do Loading' para iniciar leituras das tags necessárias");

            checkError();
            // It will search the infos correponding to the specific operation
            var operationInfos = successor.SearchInfoInList(this.eq, this.operationID);
            bool gerarPdf = false;
            string cellVariation = "";
            string flowvariation = "";

            // AQUI SERÁ NECESSÁRIO ADICIONAR AS NOVAS TAGS E PEGAR A DATA E HORA DAS ASSINATURAS DAS TAGS
            // TODOS DADOS SERÃO RECEBIDOS DO PLC

            try
            {

                logAction.writeLog("Iniciando leituras das tags necessárias de Loading");


                cellVariation = eq.Read(StaticValues.TAGVARCELL).Replace(".", ",");
                flowvariation = eq.Read(StaticValues.TAGVARFLOW).Replace(".", ",");
                this.flux.ReadPlc(); // inicializa os valores do Flowmeter
                this.cell.ReadPlc(); // inicializa os valores da LoadingCell
                this.infos.ReadPlc(); // inicializa os valores da BasicInfo

            }
            catch (Exception e)
            {
                errorlog.writeLog("Loading", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
            }

            CultureInfo changeDotToComma = CultureInfo.GetCultureInfo("pt-BR");

            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", flux.TheoricQty), String.Format(changeDotToComma, "{0:0.0}", flux.RealQty / 100), flowvariation, String.Format(changeDotToComma, "{0:0.0}", flux.Limit),
                String.Format(changeDotToComma, "{0:0.0}", cell.RealQty / 100), cellVariation, cell.Limit.ToString());


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
                "<table class='tabela'>" +
                    "<tr>" +
                        "<th>Instrumento</th>" +
                        "<th>Qtd. Teórica Kg</th>" +
                        "<th>Qtd. Real Kg</th>" +
                        "<th>Variação %</th>" +
                        "<th>Limite %</th>" +
                    "</tr>" +
                    "<tr >" +
                        "<td >Fluxímetro</td>" +
                        "<td>" + values[0] + "</td>" +
                        "<td>" + values[1] + "</td>" +
                        "<td>" + values[2] + "</td>" +
                        "<td> <" + limitFlow + "</td>" +
                    "</tr>" +
                    "<tr >" +
                        "<td>Célula de Carga</td>" +
                        "<td>" + "" + "</td>" +
                        "<td>" + values[4] + "</td>" +
                        "<td>" + values[5] + "</td>" +
                        "<td> <" + limitCell + "</td>" +
                    "</tr>" +
                "</table></html>" +
                this.infos.CreateString();

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }

    }
}

