using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Operation;
using Chiesi.Products;
using Chiesi.Tanks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Filling
{
    class FillingStart : OperationHandler, IOperation
    {
        public string Product { get; set; }

        public BasicInfoClass infos { get; set; }

        public ProductClass prod { get; set; }

        public TankClass tank { get; set; }

        public Convertion convert { get; set; }

        public ErrorLog errorlog { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;

        //conts


        public FillingStart(EquipamentType typeEq, string Product)
        {
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.infos = BasicInfoClass.GetBasicInfo();
            this.prod = ProductClass.GetProductClass();
            this.tank = TankClass.GetTankClass();
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
            var signal = WaitSign();
            var fillingType = "";
            var gerarPdf = false;

            try
            {
                prod.ReadPlc();
                infos.ReadPlc();
                tank.ReadPlc();
                Product = this.eq.Read(StaticValues.TAGRECIPETYPE);
                fillingType = this.eq.Read(StaticValues.TAGFILLINGNAME);
                //this.eq.Write(StaticValues.TAGSIGN, "False");
                //Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                errorlog.writeLog("FillingStart", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

            var x = CreateString(prod.Code.ToString(), Product, prod.Batch.ToString(), fillingType);


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
                            "<h3>Inicio do Envase</h3>" +
                            "<label>Codigo : </label><span class='campo'>" + values[0] + "</span>" +
                            "<label> Produto : </label><span class='campo'>" + values[1] + "</span>" +
                            "<label> Lote : </label><span class='campo'>" + values[2] + "</span><br>" +
                            "<label> Tipo de Envase : </label><span class='campo'>" + values[3] + "</span><br>" +
                            infos.CreateString();



            return txtCreate;
        }
    }

}