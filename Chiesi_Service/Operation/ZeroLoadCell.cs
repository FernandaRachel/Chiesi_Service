using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiesi.Products;
using Chiesi.Tanks;
using Chiesi.BasicInfos;
using Chiesi.Log;
using System.Threading;
using Chiesi.Converter;
using Chiesi_Service.Log;

namespace Chiesi.Operation
{
    class ZeroLoadCell : OperationHandler, IOperation
    {
        public TankClass tank { get; set; }

        public Convertion convert { get; set; }

        public BasicInfoClass basicInfo { get; set; }

        public string OperationName { get; set; }

        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public ZeroLoadCell(EquipamentType typeEq)
        {
            this.tank = TankClass.GetTankClass();
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            errorlog = new ErrorLog();
            this.convert = new Convertion(typeEq);
            this.logAction = new LogAction();
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


        /// <summary>
        /// Método utilizado para calcular e/ou pegar valores que serão colocados dentro do txt 
        /// e passar o txt para a operação sucessora
        /// </summary>
        public override void Calculate(Text txt)
        {

            logAction.writeLog("Entrando no método 'Calculate do ZeroLoadCell' para iniciar leituras das tags necessárias");


            checkError();
            bool gerarPdf = false;

            try
            {
                this.tank.ReadPlc();
                this.eq.Write(StaticValues.TAGSIGN, "False");
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                errorlog.writeLog("ZeroLoadCell", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

            // TESTANDO FORMAT DOT TO COMMA
            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", tank.TankWeight/100));

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

            string txtCreate =
                "<h3>Zerar Célula de Carga</h3>" +
                "<label>Peso do Tanque : </label><span class='campo'>" + values[0] + "kg</span>" +
                "<br>" + basicInfo.CreateString();

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }

    }
}
