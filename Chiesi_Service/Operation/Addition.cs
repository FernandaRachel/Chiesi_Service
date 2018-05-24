using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi_Service.Log;
using System;
using System.Collections.Generic;
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

        public LogAction logAction { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public AdditionClass(EquipamentType typeEq, string headerName, bool checkBreak, bool gerarPdf)
        {
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.headerName = headerName;
            this.checkBreak = checkBreak;
            this.gerarPdf = gerarPdf;
            this.infos = BasicInfoClass.GetBasicInfo();
            this.convert = new Convertion(typeEq);
            this.errorlog = new ErrorLog();
            this.logAction = new LogAction();
        }

        public bool checkError()
        {
            logAction.writeLog("Entrando no método 'checkError'");

            var tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC, eq.Read(StaticValues.TAGERRORPLC));

            while (tagerror)
            {
                tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC, eq.Read(StaticValues.TAGERRORPLC));
                Thread.Sleep(500);
            }
            return tagerror;
        }

        public override void Calculate(Text txt)
        {
            logAction.writeLog("Entrando no método 'Calculate do Addition' para iniciar leituras das tags necessárias");

            checkError();

            if (!gerarPdf)
            {
            }
            //var gerarPdf = false;

            try
            {
                logAction.writeLog("Iniciando leituras das tags necessárias de Addition - apenas classe basicInfo");
                this.infos.ReadPlc(); // inicializa valores das prop da infos
            }
            catch (Exception e)
            {
                errorlog.writeLog("EndFilling", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

            var x = CreateString();

            try
            {
                if (!gerarPdf)
                {
                    gerarPdf = convert.convertToBoolean(StaticValues.TAGCANCELOP, eq.Read(StaticValues.TAGCANCELOP));
                }
            }
            catch (Exception e)
            {
                errorlog.writeLog("HighSpeedMix", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

            // adiciona o texto na variavel global da classe Text
            txt.addItem(x);
            //salva o texto no log.txt
            txt.saveTxt(x, false);

            logAction.writeLog("Texto adicionado ao log.txt");

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
                    this.eq.Write(StaticValues.TAGSIGN, "False");
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
                "<h3>" + headerName + "</h3>" +
                infos.CreateString()
                + breakline;

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;

        }
    }
}
