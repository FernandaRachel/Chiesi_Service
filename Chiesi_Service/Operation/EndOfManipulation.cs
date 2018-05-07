using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using System;
using System.Collections.Generic;
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

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;

        public ErrorLog errorlog { get; set; }


        public EndOfManipulation(EquipamentType typeEq, bool checkBreak, bool gerarPdf)
        {
            this.infos = BasicInfoClass.GetBasicInfo();
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
            this.errorlog = new ErrorLog();
            this.checkBreak = checkBreak;
            this.gerarPdf = gerarPdf;
        }


        public bool checkError()
        {
            var tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC, eq.Read(StaticValues.TAGERRORPLC));

            while (tagerror)
            {
                tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC, eq.Read(StaticValues.TAGERRORPLC));
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
                    tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC, eq.Read(StaticValues.TAGERRORPLC));
                }
                return WaitSign();
            }

            return sign;
        }

        public override void Calculate(Text txt)
        {
            var signal = WaitSign();
            EndTime = DateTime.Now;
            string endTimeString = EndTime.ToString("HH:mm");
            string endData = EndTime.ToString("dd/MM/yyyy");
            var x = CreateString(endData, endTimeString);


            try
            {
                this.eq.Write(StaticValues.TAGSIGN, "False");
                Thread.Sleep(1000);
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
            else{
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
            return txtCreate;
        }


    }
}
