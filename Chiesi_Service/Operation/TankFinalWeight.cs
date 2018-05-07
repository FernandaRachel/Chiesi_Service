﻿using Chiesi.BasicInfos;
using Chiesi.Converter;
using Chiesi.Log;
using Chiesi.Tanks;
using System;
using System.Collections.Generic;
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

        public string OperationName { get; set; }

        public bool checkBreak { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public TankFinalWeight(EquipamentType typeEq, string OperationName, bool checkBreak)
        {
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.OperationName = OperationName;
            this.checkBreak = checkBreak;
            this.infos = BasicInfoClass.GetBasicInfo();
            this.tanks = TankClass.GetTankClass();
            this.errorlog = new ErrorLog();
            this.convert = new Convertion(typeEq);
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

                while (tagerror)
                {
                    tagerror = convert.convertToBoolean(StaticValues.TAGERRORPLC,eq.Read(StaticValues.TAGERRORPLC));
                }

            return true;
        }

        public override void Calculate(Text txt)
        {
            // deve ser feita alteração - chamar o waitSign , pois ele precisa verificar se há erro antes de iniciar a OP
            //var signal = WaitSign();
            bool gerarPdf = false;

            try
            {
                tanks.ReadPlc();
                //this.eq.Write(StaticValues.TAGSIGN, "False");
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                errorlog.writeLog("TankFinalWeight", "tag não especificada", e.ToString(), DateTime.Now);
                this.eq.Write(StaticValues.TAGERRORMESSAGE, e.Message);
                this.eq.Write(StaticValues.TAGERRORPLC, "True");
            }

            var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
            var x = CreateString(String.Format(changeDotToComma, "{0:0.0}", tanks.TankWeight/100));

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
            return txtCreate;
        }
    }
}
