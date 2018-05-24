using Chiesi.Anchor;
using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Lobules;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi_Service.Log;
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

        public LogAction logAction { get; set; }

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
            this.logAction = new LogAction();
        }


        public bool checkError()
        {
            logAction.writeLog("Entrando no método 'checkError'");

            var tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC, eq.Read(StaticValues.TAGERRORPLC));

            while (tagerror)
            {
                tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC, eq.Read(StaticValues.TAGERRORPLC));
                Thread.Sleep(400);
            }
            return tagerror;
        }


        public override void Calculate(Text txt)
        {

            logAction.writeLog("Entrando no método 'Calculate do ValveClass(Recirculação)' para iniciar leituras das tags necessárias");

            checkError();
            bool gerarPdf = false;

            try
            {


                logAction.writeLog("Lendo hora inicial da mistura de alta velocidade");

                if (valvName.ToLower() == "v10")
                {
                    IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURVALVE10));
                }
                else if (valvName.ToLower() == "v9")
                {
                    IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURVALVE9));
                }
                else if (valvName.ToLower() == "v8")
                {
                    IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURVALVE8));
                }



                logAction.writeLog("Lendo hora final da mistura de alta velocidade");
                if (valvName.ToLower() == "v10")
                {
                    EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURVALVE10));
                }
                else if (valvName.ToLower() == "v9")
                {
                    EndTime = IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURVALVE9));
                }
                else if (valvName.ToLower() == "v8")
                {
                    EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURVALVE8));
                }

                basicInfo.ReadPlc(); // inicializa os valores da BasicInfo
                basicInfo.Date = EndTime;

                logAction.writeLog("Iniciando leituras das tags necessárias");

                anchor.AnchorSpeed = Convert.ToDouble(this.eq.Read(StaticValues.ANCHORSPEED));
                lobules.lobulesSpeed = Convert.ToDouble(this.eq.Read(StaticValues.LOBULESSPEED));
                
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

                logAction.writeLog("Texto adicionado ao log.txt");
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

            logAction.writeLog("Iniciando CreateString");

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
                        "</tr>";
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

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }


    }
}
