using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Tanks;
using Chiesi_Service.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Operation
{
    class TankFinalWeight : OperationHandler, IOperation
    {
        public BasicInfoClass infos { get; set; }

        public TankClass tanks { get; set; }

        public Convertion convert { get; set; }

        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        public string OperationName { get; set; }

        public bool checkBreak { get; set; }

        public string operationID { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public TankFinalWeight(EquipamentType typeEq, string OperationName, bool checkBreak)
        {
            //ID da Operação - cada operação possui um ID exceto a incial(BeginOfMAnipulation)
            this.operationID = "11";
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.OperationName = OperationName;
            this.checkBreak = checkBreak;
            this.infos = BasicInfoClass.GetBasicInfo();
            this.tanks = TankClass.GetTankClass();
            this.errorlog = new ErrorLog();
            this.convert = new Convertion(typeEq);
            this.logAction = new LogAction();
        }

        public bool checkError()
        {
            var tagerror = convert.convertToBoolean(ConfigurationManager.AppSettings["TAGERRORPLC"],eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

            while (tagerror)
            {
                tagerror = convert.convertToBoolean(ConfigurationManager.AppSettings["TAGERRORPLC"],eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));
                Thread.Sleep(500);
            }
            return tagerror;
        }

       
        public override void Calculate(Text txt)
        {
            logAction.writeLog("------------------- ID: " + this.operationID + "----------------");

            logAction.writeLog("Entrando no método 'Calculate do Addition' para iniciar leituras das tags necessárias");

            checkError();
            // It will search the infos correponding to the specific operation
            var result = this.eq.recipe.WeightTank;

            bool gerarPdf = false;

            try
            {
                logAction.writeLog("Iniciando leituras das tags necessárias do TankFinalWeight");
                tanks.TankWeight = convert.convertToDouble("result", result);
            }
            catch (Exception e)
            {
                errorlog.writeLog("TankFinalWeight", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
            }

            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", tanks.TankWeight/100));


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
            logAction.writeLog("Iniciando CreateString");

            string breakline;

            if (checkBreak)
            {
                breakline = "<p class='fot'></p>";
            }
            else
            {
                breakline = "";
            }

            string txtCreate = "<br><label><strong>Peso Final do Produto no Tanque : <strong></label><span class='campo'>" 
                + values[0] + "</span> Kg"
                + breakline;

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }
    }
}
