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

namespace Chiesi.Operation
{
    class BeginOfManipulation : OperationHandler, IOperation
    {
        public ErrorLog errorlog { get; set; }

        public ProductClass prod { get; set; }

        public BasicInfoClass basicInfo { get; set; }

        public DateTime Date { get; set; }

        public DateTime Hour { get; set; }

        public string OperatorLogin { get; set; }

        public string OperatorName { get; set; }

        public string OperationName { get; set; }

        public string Product { get; set; }

        public Convertion convert { get; set; }

        public Dictionary<string, string> TagsValues { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;

        public BeginOfManipulation(EquipamentType typeEq, string OperationName, string Product)
        {
            this.TagsValues = new Dictionary<string, string>();
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.OperationName = OperationName;
            //this.Product = Product;
            this.prod = ProductClass.GetProductClass();
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.errorlog = new ErrorLog();
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


        /// <summary>
        /// Método utilizado para calcular e/ou pegar valores que serão colocados dentro do txt
        /// </summary>
        public override void Calculate(Text txt)
        {
            var signal = WaitSign();
            bool gerarPdf = false;

            try
            {
                this.eq.Write(StaticValues.TAGSIGN, "False");
                this.basicInfo.ReadPlc(); // inicializa valores das prop da BasicInfo
                this.prod.ReadPlc(); // inicializa valores das prop do Product
                this.basicInfo.KeepBatch = this.prod.Batch;
                Product =  this.eq.Read(StaticValues.TAGRECIPETYPE);
                Thread.Sleep(500);
            }
            catch (Exception e)
            {
                errorlog.writeLog("BeginOfManipulation", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

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

            var x = CreateString();

            if (!gerarPdf)
            {
                txt.addItem(x);
                txt.saveTxt(x, true);
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

            string txtCreate =
                            "<h3>Inicio da Manipulação - Seleção da Receita</h3>" +
                            "<label>Codigo : </label><span class='campo'>" + prod.Code + "</span>" +
                            "<label class='lab'>Produto : </label><span class='campo'>" + Product + "</span>" +
                            "<label class='lab'>Lote : </label><span class='campo'>" + prod.Batch + "</span><br>" +
                            basicInfo.CreateString();

            return txtCreate;
        }

    }
}
