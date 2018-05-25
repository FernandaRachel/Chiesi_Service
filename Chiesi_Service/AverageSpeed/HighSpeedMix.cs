using Chiesi.Anchor;
using Chiesi.BasicInfos;
using Chiesi.Clenil;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi.Turbine;
using Chiesi_Service.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.AverageSpeed
{
    class HighSpeedMix : OperationHandler, IOperation
    {
        public AnchorClass anchor { get; set; }

        public TurbineClass turbine { get; set; }

        public BasicInfoClass basicInfo { get; set; }

        public ClenilClass clenil { get; set; }

        public ClenilStrong clenilStrong { get; set; }

        public string OperationName { get; set; }

        public string mixTime { get; set; }

        public string anchorLimit { get; set; }

        public string turbineLimit { get; set; }

        public string clenilLimit { get; set; }

        public string clenilStrongLimit { get; set; }

        public bool changeTable { get; set; }

        public bool clenilForte { get; set; }

        public bool checkBreak { get; set; }

        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        public Convertion convert { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;


        public HighSpeedMix(EquipamentType typeEq, string anchorLimit,
            string turbineLimit, string clenilLimit, string clenilStrongLimit, bool changeTable, bool clenilForte, bool checkBreak, string mixTime)
        {
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.anchor = AnchorClass.GetAnchorClass();
            this.turbine = TurbineClass.GetTurbineClass();
            this.clenil = ClenilClass.GetClenilClass();
            this.clenilStrong = ClenilStrong.GetClenilStrong();
            this.anchorLimit = anchorLimit;
            this.turbineLimit = turbineLimit;
            this.clenilLimit = clenilLimit;
            this.clenilForte = clenilForte;
            this.checkBreak = checkBreak;
            this.mixTime = mixTime;
            this.clenilStrongLimit = clenilStrongLimit;
            this.changeTable = changeTable;
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.convert = new Convertion(typeEq);
            this.errorlog = new ErrorLog();
            this.logAction = new LogAction();

        }

        public bool checkError()
        {
            logAction.writeLog("Entrando no método 'checkError'");

            var tagerror = convert.convertToBoolean(ConfigurationManager.AppSettings["TAGERRORPLC"], eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

            while (tagerror)
            {
                tagerror = convert.convertToBoolean(ConfigurationManager.AppSettings["TAGERRORPLC"], eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));
                Thread.Sleep(250);
            }
            return tagerror;
        }

     
        public override void Calculate(Text txt)
        {
            logAction.writeLog("Entrando no método 'Calculate do HighSpeedMix' para iniciar leituras das tags necessárias");

            double toConvertTime;
            checkError();
            bool gerarPdf = false;
            var x = "";

            try
            {
                // AQUI SERÁ NECESSÁRIO ADICIONAR AS NOVAS TAGS E PEGAR A DATA E HORA DAS ASSINATURAS DAS TAGS
                // TODOS DADOS SERÃO RECEBIDOS DO PLC


                this.anchor.ReadPlc();
                this.turbine.ReadPlc();
                this.clenil.ReadPlc();
                this.clenilStrong.ReadPlc();

                if (!changeTable)
                {
                    logAction.writeLog("Iniciando leituras das tags necessárias");

                    if (Status.getStatus() != StatusType.Fail)
                    {
                        logAction.writeLog("Lendo hora inicial da mistura de alta velocidade");

                        anchor.IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURHIGHMIX));
                        turbine.IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURHIGHMIX));
                        toConvertTime = convert.convertToDouble(StaticValues.TAGMIXTIME, this.eq.Read(StaticValues.TAGMIXTIME));
                    }

                    if (Status.getStatus() != StatusType.Fail)
                    {
                        logAction.writeLog("Lendo hora final da mistura de alta velocidade");

                        anchor.EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURHIGHMIX));
                        turbine.EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURHIGHMIX));
                        basicInfo.ReadPlc(); // inicializa os valores da BasicInfo
                        this.basicInfo.Date = anchor.EndTime;
                    }

                    if (Status.getStatus() != StatusType.Fail)
                    {
                        logAction.writeLog("Lendo velocidades da mistura de alta velocidade");

                        anchor.AnchorSpeed = convert.convertToDouble(StaticValues.ANCHORSPEED, this.eq.Read(StaticValues.ANCHORSPEED));
                        turbine.TurbineSpeed = convert.convertToDouble(StaticValues.TURBINESPEED, this.eq.Read(StaticValues.TURBINESPEED));
                    }
                }
                else
                {
                    logAction.writeLog("Iniciando leituras das tags necessárias");

                    if (Status.getStatus() != StatusType.Fail)
                    {
                        logAction.writeLog("Lendo hora inicial da mistura de alta velocidade");

                        anchor.IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURHIGHMIX));
                        clenil.IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURHIGHMIX));
                        clenilStrong.IniTime = Convert.ToDateTime(this.eq.Read(StaticValues.INIHOURHIGHMIX));
                        toConvertTime = convert.convertToDouble(StaticValues.TAGMIXTIME, this.eq.Read(StaticValues.TAGMIXTIME));
                    }

                    if (Status.getStatus() != StatusType.Fail)
                    {
                        logAction.writeLog("Lendo hora final da mistura de alta velocidade");

                        anchor.EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURHIGHMIX));
                        clenil.EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURHIGHMIX));
                        clenilStrong.EndTime = Convert.ToDateTime(this.eq.Read(StaticValues.ENDHOURHIGHMIX));
                        basicInfo.ReadPlc(); // inicializa os valores da BasicInfo
                        this.basicInfo.Date = anchor.EndTime;
                    }

                    if (Status.getStatus() != StatusType.Fail)
                    {
                        logAction.writeLog("Lendo velocidades da mistura de alta velocidade");

                        anchor.AnchorSpeed = convert.convertToDouble(StaticValues.ANCHORSPEED, this.eq.Read(StaticValues.ANCHORSPEED));
                        turbine.TurbineSpeed = convert.convertToDouble(StaticValues.TURBINESPEED, this.eq.Read(StaticValues.TURBINESPEED));
                    }
                }

            }
            catch (Exception e)
            {
                errorlog.writeLog("HighSpeedMix", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
            }


            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");

            if (changeTable)
            {
                x = CreateString(mixTime.ToString(), String.Format(changeDotToComma, "{0:0.0}", anchor.AnchorSpeed), anchor.IniTime.ToString("HH:mm"), anchor.EndTime.ToString("HH:mm"), anchor.RpmLimit.ToString(), String.Format(changeDotToComma, "{0:0.0}", turbine.TurbineSpeed),
                    clenil.IniTime.ToString("HH:mm"), clenil.EndTime.ToString("HH:mm"), clenil.RpmLimit.ToString(),
                    String.Format(changeDotToComma, "{0:0.0}", turbine.TurbineSpeed), clenilStrong.IniTime.ToString("HH:mm"), clenilStrong.EndTime.ToString("HH:mm"), clenilStrong.RpmLimit.ToString());
            }
            else
            {
                x = CreateString(mixTime.ToString(), String.Format(changeDotToComma, "{0:0.0}", anchor.AnchorSpeed), anchor.IniTime.ToString("HH:mm"), anchor.EndTime.ToString("HH:mm"), anchor.RpmLimit.ToString(),
                String.Format(changeDotToComma, "{0:0.0}", turbine.TurbineSpeed), turbine.IniTime.ToString("HH:mm"), turbine.EndTime.ToString("HH:mm"), turbine.RpmLimit.ToString());
            }


            try
            {
                gerarPdf = convert.convertToBoolean(StaticValues.TAGCANCELOP, eq.Read(StaticValues.TAGCANCELOP));
            }
            catch (Exception e)
            {
                errorlog.writeLog("HighSpeedMix", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
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
                    pdf.gerarPdf(txt.Header, basicInfo);
                    txt.cleanTxt();
                }
                else
                {
                    successor.Calculate(txt);
                }
            }
        }

        // Este método forma o layout do html e adiciona os valores necessários dentros dos devidos campos
        public string CreateString(params string[] values)
        {
            logAction.writeLog("Iniciando CreateString");

            string column;
            string td;
            string qtd;
            string breakline;

            if (checkBreak)
            {
                breakline = "<p class='fot'></p>";
            }
            else
            {
                breakline = "";
            }

            if (changeTable)
            {
                td = "<td></td>";
                qtd = "2";
                if (clenilForte)
                {
                    column =
                       "<tr>" +
                           "<td rowspan='2'>Agitador Turbina</td>" +
                           "<td>Clenil Compositium Forte</td>" +
                           "<td>" + values[9] + "Rpm</td>" +
                           "<td>" + values[10] + "</td>" +
                           "<td>" + values[11] + "</td>" +
                           "<td>" + clenilStrongLimit + "</td>" +
                       "</tr>" +
                       "</table>"
                       + breakline;
                }
                else
                {
                    column =
                      "<tr>" +
                          "<td rowspan='2'>Agitador Turbina</td>" +
                          "<td>Clenil Compositium</td>" +
                          "<td>" + values[5] + "Rpm</td>" +
                          "<td>" + values[6] + "</td>" +
                          "<td>" + values[7] + "</td>" +
                          "<td>" + clenilLimit + "</td>" +
                      "</tr>" +
                      "</table>"
                      + breakline;
                }

            }
            else
            {
                td = "";
                qtd = "1";
                column =
                "<tr>" +
                    "<td>Agitador Turbina</td>" + td +
                    "<td>" + values[5] + "Rpm </td>" +
                    "<td>" + values[6] + "</td>" +
                    "<td>" + values[7] + "</td>" +
                    "<td>" + turbineLimit + "</td>" +
                    "<td></td>" +
                "</tr>" +
                "</table>"
                + breakline;
            }

            string txtCreate =
              "<h3>Mistura Alta Velocidade</h3>" +
              "<table>" +
                "<tr>" +
                    "<th colspan='" + qtd + "'></th>" +
                    "<th> Velocidade RPM </th>" +
                    "<th> Inicio </th>" +
                    "<th> Término </th>" +
                    "<th> Limite RPM </th>" +
                    "<th> Tempo </th>" +
                "</tr>" +
                "<tr>" +
                    "<td>Agitador Âncora</td>" + td +
                    "<td>" + values[1] + "Rpm </td>" +
                    "<td>" + values[2] + "</td>" +
                    "<td>" + values[3] + "</td>" +
                    "<td>" + anchorLimit + "</td>" +
                    "<td rowspan='4'>" + mixTime + " min </td>" +
                "<tr>" +
                column + basicInfo.CreateString();

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }
    }
}
