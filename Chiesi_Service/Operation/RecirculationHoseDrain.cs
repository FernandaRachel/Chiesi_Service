using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Operation
{
    class RecirculationHoseDrain : OperationHandler, IOperation
    {
        public ProductClass prod { get; set; }

        public DateTime IniTime { get; set; }

        public DateTime EndTime { get; set; }

        public string OperationName { get; set; }

        public BasicInfoClass infos { get; set; }

        public Convertion convert { get; set; }

        public ErrorLog errorlog { get; set; }

        public Dictionary<string, string> TagsValues { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public RecirculationHoseDrain(EquipamentType typeEq)
        {
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.OperationName = OperationName;
            this.prod = ProductClass.GetProductClass();
            this.infos = BasicInfoClass.GetBasicInfo();
            this.convert = new Convertion(typeEq);
            this.errorlog = new ErrorLog();
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

        public override void Calculate(Text txt)
        {
            //var signal = WaitSign();
            bool gerarPdf = false;
            checkError();
            bool inidate;
            bool enddate;
            string iniTimeString = "";
            string endTimeString = "";

            try
            {
                inidate = convert.convertToBoolean(StaticValues.INISPEEDTIME,this.eq.Read(StaticValues.INISPEEDTIME));
                enddate = convert.convertToBoolean(StaticValues.ENDSPEEDTIME,this.eq.Read(StaticValues.ENDSPEEDTIME));
                gerarPdf = convert.convertToBoolean(StaticValues.TAGCANCELOP, eq.Read(StaticValues.TAGCANCELOP));

                while (Status.getStatus() != StatusType.Fail && inidate == false)
                {
                    inidate = convert.convertToBoolean(StaticValues.INISPEEDTIME,this.eq.Read(StaticValues.INISPEEDTIME));
                }
                if (inidate == true)
                {
                    IniTime = DateTime.Now;
                    iniTimeString = IniTime.ToString("HH:mm");
                    this.eq.Write(StaticValues.INISPEEDTIME, "False");
                }
                while (Status.getStatus() != StatusType.Fail && enddate == false)
                {
                    enddate = convert.convertToBoolean(StaticValues.ENDSPEEDTIME,this.eq.Read(StaticValues.ENDSPEEDTIME));
                }
                if (enddate == true)
                {
                    EndTime = DateTime.Now;
                    endTimeString = EndTime.ToString("HH:mm");
                    this.eq.Write(StaticValues.ENDSPEEDTIME, "False");
                }
            }
            catch (Exception e)
            {
                errorlog.writeLog("RecirculationHoseDrain", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

            var x = CreateString(iniTimeString, endTimeString);


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
            string txtCreate =
                "<h3>Drenagem da Mangueira de Recirculação</h3>" +
                "<label> Escoamento do Produto: &nbsp</label>Inicio : <span class='campo'>" + values[0] + "</span> Final : <span class='campo'>" + values[1] + "</span>";

            return txtCreate;
        }

    }
}
