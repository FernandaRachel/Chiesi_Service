using Chiesi.Anchor;
using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Lobules;
using Chiesi.Log;
using Chiesi.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Valve
{
    class ValveClass : OperationHandler, IOperation
    {

        public DateTime EndTime { get; set; }

        public string EndTimeTag { get; set; }

        public DateTime IniTime { get; set; }

        public string IniTimeTag { get; set; }

        public string OperationName { get; set; }

        public string valveTime { get; set; }

        public bool checkIniValve { get; set; }

        public bool finalValve { get; set; }

        public string highLimit { get; set; }

        public string lowLimit { get; set; }

        public string valvName { get; set; }

        public AnchorClass anchor { get; set; }

        public LobulesClass lobules { get; set; }

        public BasicInfoClass basicInfo { get; set; }

        public Convertion convert { get; set; }

        public ErrorLog errorlog { get; set; }

        public Dictionary<string, string> TagsValues { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;

        public ValveClass(EquipamentType typeEq, string valveTime, bool checkIniValve, bool finalValve,
            string lowLimit, string highLimit, string valvName)
        {

            this.valvName = valvName;
            this.checkIniValve = checkIniValve;
            this.finalValve = finalValve;
            this.lobules = LobulesClass.GetLobulesClass();
            this.anchor = AnchorClass.GetAnchorClass();
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.valveTime = valveTime;
            this.highLimit = highLimit;
            this.lowLimit = lowLimit;
            this.errorlog = new ErrorLog();
            this.convert = new Convertion(typeEq);
            this.eq = this.eqFact.ConstructEquipament(typeEq);
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
            //var signal = WaitSign();
            checkError();
            bool gerarPdf = false;

            try
            {
                var inidate = Convert.ToBoolean(this.eq.Read(StaticValues.INISPEEDTIME));
                var enddate = Convert.ToBoolean(this.eq.Read(StaticValues.ENDSPEEDTIME));
                var readSpeed = Convert.ToBoolean(this.eq.Read(StaticValues.TAGTRIGGERSPEED));


                while (Status.getStatus() != StatusType.Fail && inidate == false)
                {
                    inidate = Convert.ToBoolean(this.eq.Read(StaticValues.INISPEEDTIME));

                }
                if (inidate == true)
                {
                    if (valvName.ToLower() == "v10")
                    {
                        IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURVALVE10));
                        this.eq.Write(StaticValues.INISPEEDTIME, "False");
                    }
                    else if (valvName.ToLower() == "v9")
                    {
                        IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURVALVE9));
                        this.eq.Write(StaticValues.INISPEEDTIME, "False");
                    }
                    else if (valvName.ToLower() == "v8")
                    {
                        IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURVALVE8));
                        this.eq.Write(StaticValues.INISPEEDTIME, "False");
                    }

                }
                while (Status.getStatus() != StatusType.Fail && enddate == false)
                {
                    enddate = Convert.ToBoolean(this.eq.Read(StaticValues.ENDSPEEDTIME));

                }
                if (enddate == true)
                {
                    if (valvName.ToLower() == "v10")
                    {
                        EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURVALVE10));
                        this.eq.Write(StaticValues.ENDSPEEDTIME, "False");
                    }
                    else if (valvName.ToLower() == "v9")
                    {
                        EndTime = IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURVALVE9));
                        this.eq.Write(StaticValues.ENDSPEEDTIME, "False");
                    }
                    else if (valvName.ToLower() == "v8")
                    {
                        EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURVALVE8));
                        this.eq.Write(StaticValues.ENDSPEEDTIME, "False");
                    }
                }
                while (Status.getStatus() != StatusType.Fail && readSpeed == false)
                {
                    readSpeed = Convert.ToBoolean(this.eq.Read(StaticValues.TAGTRIGGERSPEED));
                }
                if (readSpeed == true)
                {
                    anchor.AnchorSpeed = Convert.ToDouble(this.eq.Read(StaticValues.ANCHORSPEED));
                    lobules.lobulesSpeed = Convert.ToDouble(this.eq.Read(StaticValues.LOBULESSPEED));
                    basicInfo.ReadPlc(); // inicializa os valores da BasicInfo
                    basicInfo.Date = EndTime;
                    this.eq.Write(StaticValues.TAGTRIGGERSPEED, "False");
                    Thread.Sleep(250);
                }
            }
            catch (Exception e)
            {
                errorlog.writeLog("ValveClass", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }


            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
            var x = CreateString(IniTime.ToString("HH:mm"), EndTime.ToString("HH:mm"), String.Format(changeDotToComma, "{0:0.0}", anchor.AnchorSpeed), String.Format(changeDotToComma, "{0:0.0}", lobules.lobulesSpeed), lowLimit, highLimit, valveTime);


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

            string txtCreate = "";

            if (checkIniValve)
            {
                txtCreate =
                    "<h4>Recirculação do Produto</h4>" +
                    "<table>" +
                        "<tr>" +
                            "<th></th>" +
                            "<th>Inicio: </th>" +
                            "<th>Termino: </th>" +
                            "<th>Velocidade Agitador Âncora: </th>" +
                            "<th>Velocidade Bomba de Lobulos: </th>" +
                            "<th>Limite RPM: </th>" +
                            "<th>Tempo: </th>" +
                        "</tr>"
                    ;
            }
            txtCreate += "<tr>" +
                              "<td>" + valvName + "</td>" +
                              "<td>" + values[0] + "</td>" +
                              "<td>" + values[1] + "</td>" +
                              "<td>" + values[2] + "Rpm" + "</td>" +
                              "<td>" + values[3] + "Rpm" + "</td>" +
                              "<td>" + lowLimit + "-" + highLimit + "</td>" +
                              "<td>" + valveTime + "min" + "</td>" +
                           "</tr>";
            if (finalValve)
                txtCreate += "</table>" + basicInfo.CreateString();
            return txtCreate;
        }


    }
}
