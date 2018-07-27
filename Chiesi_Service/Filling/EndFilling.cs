using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi.Tanks;
using Chiesi_Service.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        //public //logAction //logAction { get; set; }

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

            ////logAction.writeLog("Entrando no método 'EndFilling' para iniciar leituras das tags necessárias");

            var gerarPdf = true;

            try
            {
                infos.ReadPlc();
                tank.ReadPlc();
                Thread.Sleep(250);
            }
            catch (Exception e)
            {
                errorlog.writeLog("EndFilling", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
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
