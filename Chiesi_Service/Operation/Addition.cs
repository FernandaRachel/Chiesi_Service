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
    class AdditionClass : OperationHandler, IOperation
    {
        public string headerName { get; set; }

        public bool checkBreak { get; set; }

        public bool gerarPdf { get; set; }

        public BasicInfoClass infos { get; set; }

        public Convertion convert { get; set; }

        public ErrorLog errorlog { get; set; }

        //public //logAction //logAction { get; set; }

        public string operationID { get; set; }

        public int index { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public AdditionClass(EquipamentType typeEq, string headerName, bool checkBreak, bool gerarPdf, int index)
        {
            //ID da Operação - cada operação possui um ID exceto a incial(BeginOfMAnipulation)
            this.operationID = "7";
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.index = index;
            this.headerName = headerName;
            this.checkBreak = checkBreak;
            this.gerarPdf = gerarPdf;
            this.infos = BasicInfoClass.GetBasicInfo();
            this.convert = new Convertion(typeEq);
            this.errorlog = new ErrorLog();
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

        public override void Calculate(Text txt)
        {
            ////logAction.writeLog("------------------- ID: " + this.operationID + "----------------");

            ////logAction.writeLog("Entrando no método 'Calculate do Addition' para iniciar leituras das tags necessárias");

            checkError();
            // It will search the infos correponding to the specific operation
            ////logAction.writeLog("Iniciando leituras das tags necessárias de Addition - apenas classe basicInfo");
            var operationInfos = SearchInfoInList(this.eq, this.operationID);
            string x = "";

            // Verifica se retornou alguma info
            // Se não retornou então a receita foi cancelada
            if ((index) < operationInfos.Count())
            {
                var result = operationInfos.ElementAt(index);

                try
                {
                    // Gera o HTML com as informações
                    x = CreateString(String.Format(result.Date, "dd/MM/yyyy"), String.Format(result.Hora_0, "HH:mm:ss"), String.Format(result.Hora_1, "HH:mm:ss"), result.Asignature);
                    // adiciona o texto na variavel global da classe Text
                    txt.addItem(x);
                    //salva o texto no log.txt
                    txt.saveTxt(x, false);

                    ////logAction.writeLog("Texto adicionado ao log.txt");
                }
                catch (Exception e)
                {
                    errorlog.writeLog("Addition", "tag não especificada", e.ToString(), DateTime.Now);
                    this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                    this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                }
            }
            else
            {
                gerarPdf = true;
            }


            if (successor != null)
            {
                if (gerarPdf)
                {
                    Pdf pdf = new Pdf(x);
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
            ////logAction.writeLog("Iniciando CreateString");

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
                "<div class='basic-info'>" +
                    "<label>Data : </label><span class='campo'>" + values[0] + "</span>" +
                    "<label class='lab'>Hora : </label><span class='campo'>" + values[1] + "</span>" +
                    "<br><label>Assinatura : </label><span class='campo'>" + values[3] + "</span>" +
                "</div>"
            + breakline;

            ////logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;

        }
    }
}
