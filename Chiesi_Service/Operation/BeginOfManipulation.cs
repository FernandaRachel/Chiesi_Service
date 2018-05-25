using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiesi.Products;
using Chiesi.BasicInfos;
using Chiesi.Log;
using Chiesi.Converter;
using System.Threading;
using Chiesi_Service.Log;
using System.Configuration;

namespace Chiesi.Operation
{
    class BeginOfManipulation : OperationHandler, IOperation
    {
        public ErrorLog errorlog { get; set; }

        public LogAction logAction { get; set; }

        public ProductClass prod { get; set; }

        public BasicInfoClass basicInfo { get; set; }

        public DateTime Date { get; set; }

        public DateTime Hour { get; set; }

        public string OperatorLogin { get; set; }

        public string OperatorName { get; set; }

        public string OperationName { get; set; }

        public string Product { get; set; }

        public Convertion convert { get; set; }


        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;

        public BeginOfManipulation(EquipamentType typeEq, string OperationName, string Product)
        {
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.OperationName = OperationName;
            this.prod = ProductClass.GetProductClass();
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.errorlog = new ErrorLog();
            this.convert = new Convertion(typeEq);
            this.logAction = new LogAction();

        }

        public bool checkError()
        {
            logAction.writeLog("Entrando no método 'checkError'");

            var tagerror = convert.convertToBoolean(ConfigurationManager.AppSettings["TAGERRORPLC"], eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

            while (tagerror)
            {
                tagerror = convert.convertToBoolean(ConfigurationManager.AppSettings["TAGERRORPLC"], eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));
                Thread.Sleep(500);
            }
            return tagerror;
        }



        /// <summary>
        /// Método utilizado para calcular e/ou pegar valores que serão colocados dentro do txt
        /// </summary>
        public override void Calculate(Text txt)
        {
            logAction.writeLog("Entrando no método 'Calculate do BeginOfManipulation' para iniciar leituras das tags necessárias");

            checkError();

            bool gerarPdf = false;

            try
            {
                logAction.writeLog("Lendo hora inicial do BeginOfManipulation");
                this.basicInfo.ReadPlc(); // inicializa valores das prop da BasicInfo
                logAction.writeLog("Iniciando leituras das tags necessárias do BeginOfManipulation");
                this.prod.ReadPlc(); // inicializa valores das prop do Product
                this.basicInfo.KeepBatch = this.prod.Batch;
                Product = this.eq.Read(ConfigurationManager.AppSettings["TAGRECIPETYPE"]);
            }
            catch (Exception e)
            {
                errorlog.writeLog("BeginOfManipulation", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
            }

            try
            {
                gerarPdf = convert.convertToBoolean(StaticValues.TAGCANCELOP, eq.Read(StaticValues.TAGCANCELOP));
            }
            catch (Exception e)
            {
                errorlog.writeLog("HighSpeedMix", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
            }

            var x = CreateString();

            if (!gerarPdf)
            {
                txt.addItem(x);
                txt.saveTxt(x, true);

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
                            "<h3>Inicio da Manipulação - Seleção da Receita</h3>" +
                            "<label>Codigo : </label><span class='campo'>" + prod.Code + "</span>" +
                            "<label class='lab'>Produto : </label><span class='campo'>" + Product + "</span>" +
                            "<label class='lab'>Lote : </label><span class='campo'>" + prod.Batch + "</span><br>" +
                            basicInfo.CreateString();

            logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }

    }
}
