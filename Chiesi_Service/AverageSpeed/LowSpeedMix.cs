using Chiesi.Anchor;
using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi.Turbine;
using System;
using System.Collections.Generic;
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

        public AnchorClass anchor { get; set; }

        public BasicInfoClass basicInfo { get; set; }

        public ErrorLog errorlog { get; set; }

        public Convertion convert { get; set; }


        public Dictionary<string, string> TagsValues { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;


        public LowSpeedMix(EquipamentType typeEq, string mixTime, string rpmLimit, bool checkBreak)
        {
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.anchor = AnchorClass.GetAnchorClass();
            this.mixTime = mixTime;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.errorlog = new ErrorLog();
            this.rpmLimit = rpmLimit;
            this.checkBreak = checkBreak;
            this.convert = new Convertion(typeEq);
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
            checkError();
            bool gerarPdf = false;

            try
            {
                var inidate = convert.convertToBoolean(StaticValues.INISPEEDTIME, this.eq.Read(StaticValues.INISPEEDTIME));
                eq.Write(StaticValues.TAGMIXTIME, mixTime);
                Thread.Sleep(500);
                this.anchor.ReadPlc();

                while (Status.getStatus() != StatusType.Fail && inidate == false)
                {
                    inidate = convert.convertToBoolean(StaticValues.INISPEEDTIME, this.eq.Read(StaticValues.INISPEEDTIME));

                }
                if (inidate == true)
                {
                    anchor.IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURMIXTIME));
                    this.eq.Write(StaticValues.INISPEEDTIME, "False");
                }

                var enddate = convert.convertToBoolean(StaticValues.ENDSPEEDTIME, this.eq.Read(StaticValues.ENDSPEEDTIME));

                while (Status.getStatus() != StatusType.Fail && enddate == false)
                {
                    enddate = convert.convertToBoolean(StaticValues.ENDSPEEDTIME, this.eq.Read(StaticValues.ENDSPEEDTIME));

                }
                if (enddate == true)
                {
                    anchor.EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURMIXTIME));
                    this.eq.Write(StaticValues.ENDSPEEDTIME, "False");
                }

                var readSpeed = convert.convertToBoolean(StaticValues.TAGTRIGGERSPEED, this.eq.Read(StaticValues.TAGTRIGGERSPEED));

                while (Status.getStatus() != StatusType.Fail && readSpeed == false)
                {
                    readSpeed = convert.convertToBoolean(StaticValues.TAGTRIGGERSPEED, this.eq.Read(StaticValues.TAGTRIGGERSPEED));
                }
                if (readSpeed == true)
                {
                    anchor.AnchorSpeed = convert.convertToDouble(StaticValues.ANCHORSPEED, this.eq.Read(StaticValues.ANCHORSPEED));
                    anchor.mixTime = convert.convertToDouble(StaticValues.TAGMIXTIME, this.eq.Read(StaticValues.TAGMIXTIME));
                    basicInfo.ReadPlc();
                    basicInfo.Date = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURMIXTIME));
                    this.eq.Write(StaticValues.TAGTRIGGERSPEED, "False");
                    Thread.Sleep(200);
                }

                
            }
            catch (Exception e)
            {
                errorlog.writeLog("Low SpeedMix", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", anchor.AnchorSpeed), anchor.IniTime.ToString("HH:mm"), anchor.EndTime.ToString("HH:mm"), anchor.RpmLimit.ToString(), anchor.mixTime.ToString());

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
