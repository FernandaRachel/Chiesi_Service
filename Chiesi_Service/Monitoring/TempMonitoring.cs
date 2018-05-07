using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi.Products;
using Chiesi.Shaker;
using System;
using System.Collections.Generic;
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

        public Convertion convert { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;

        public TempMonitoringClass(EquipamentType typeEq, bool checkBreak)
        {

            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.shaker = ShakerClass.GetShakerClass();
            this.prod = ProductClass.GetProductClass();
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.errorlog = new ErrorLog();
            this.convert = new Convertion(typeEq);
            this.checkBreak = checkBreak;
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

        /// <summary>
        /// Método utilizado para calcular e/ou pegar valores que serão colocados dentro do txt 
        /// e passar o txt para a operação sucessora
        /// </summary>
        public override void Calculate(Text txt)
        {
            var signal = WaitSign();
            bool gerarPdf = false;
            double prodTemp = 0;

            try
            {
                var inidate = Convert.ToBoolean(this.eq.Read(StaticValues.INISPEEDTIME));
                var enddate = Convert.ToBoolean(this.eq.Read(StaticValues.ENDSPEEDTIME));
                var readSpeed = Convert.ToBoolean(this.eq.Read(StaticValues.TAGTRIGGERSPEED));


                while (Status.getStatus() != StatusType.Fail && inidate == false)
                {
                    inidate = Convert.ToBoolean(this.eq.Read(StaticValues.INISPEEDTIME));
                }

                this.eq.Write(StaticValues.INISPEEDTIME, "False");

                while (Status.getStatus() != StatusType.Fail && enddate == false)
                {
                    enddate = Convert.ToBoolean(this.eq.Read(StaticValues.ENDSPEEDTIME));
                }

                if (enddate == true)
                {
                    this.eq.Write(StaticValues.ENDSPEEDTIME, "False");
                    this.prod.ReadPlc(); // inicializa valores das prop da Product
                    this.basicInfo.ReadPlc(); //inicializa valores das prop da Basic Info
                    this.shaker.ReadPlc();//inicializa valores das prop da Shaker
                }

                while (Status.getStatus() != StatusType.Fail && readSpeed == false)
                {
                    readSpeed = Convert.ToBoolean(this.eq.Read(StaticValues.TAGTRIGGERSPEED));
                }

                if (readSpeed == true)
                {
                    this.prod.ReadPlc(); // inicializa valores das prop da Product
                    this.basicInfo.ReadPlc(); //inicializa valores das prop da Basic Info
                    this.shaker.ReadPlc();//inicializa valores das prop da Shaker
                    prodTemp = (this.prod.ProductTemp / 10);
                    this.eq.Write(StaticValues.TAGTRIGGERSPEED, "False");
                    Thread.Sleep(450);
                }

            }
            catch (Exception e)
            {
                errorlog.writeLog("TempMonitoring", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", prodTemp.ToString()), String.Format(changeDotToComma, "{0:0.0}", this.shaker.ShakingSpeed), this.shaker.RpmLimit.ToString());


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
            return txtCreate;
        }
    }
}
