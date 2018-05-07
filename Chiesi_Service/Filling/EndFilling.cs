using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi.Tanks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Filling
{
    class EndFilling : OperationHandler, IOperation
    {
        public bool checkBreak { get; set; }

        public TankClass tank { get; set; }

        public BasicInfoClass infos { get; set; }

        public Convertion convert { get; set; }

        public ErrorLog errorlog { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;


        public EndFilling(EquipamentType typeEq, bool checkBreak)
        {

            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.infos = BasicInfoClass.GetBasicInfo();
            this.tank = TankClass.GetTankClass();
            this.convert = new Convertion(typeEq);
            this.errorlog = new ErrorLog();
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

        public override void Calculate(Text txt)
        {
            var signal = WaitSign();
            var gerarPdf = true;

            try
            {
                infos.ReadPlc();
                tank.ReadPlc();
                this.eq.Write(StaticValues.TAGSIGN, "False");
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                errorlog.writeLog("EndFilling", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }



            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", tank.TankWeight/100));

            txt.addItem(x);
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
            else
            {
                if (gerarPdf)
                {
                    Pdf pdf = new Pdf(txt.txtAtual);
                    pdf.gerarPdf(txt.Header, infos);
                    txt.cleanTxt();
                    this.eq.Write(StaticValues.TAGSIGN, "False");
                    Thread.Sleep(1000);
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
                            "<h3>Término do Envase</h3>" +
                            "<label>Peso do Tanque : </label><span class='campo'>" + values[0] + " Kg</span><br>" +
                            infos.CreateString()
                            + breakline;


            return txtCreate;
        }
    }
}
