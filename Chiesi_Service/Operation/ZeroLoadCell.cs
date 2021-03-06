﻿using System;
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
using System.Configuration;

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

        public string operationID { get; set; }

        private EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        private IEquipament eq;


        public ZeroLoadCell(EquipamentType typeEq)
        {
            //ID da Operação - cada operação possui um ID exceto a incial(BeginOfMAnipulation)
            // e a ZeroLoadCell
            this.operationID = "2";
            this.tank = TankClass.GetTankClass();
            this.basicInfo = BasicInfoClass.GetBasicInfo();
            this.eq = this.eqFact.ConstructEquipament(typeEq);
            errorlog = new ErrorLog();
            this.convert = new Convertion(typeEq);
            this.logAction = new LogAction();
        }

        public bool checkError()
        {
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
        /// e passar o txt para a operação sucessora
        /// </summary>
        public override void Calculate(Text txt)
        {
            //logAction.writeLog("------------------- ID: " + this.operationID + "----------------");
            //logAction.writeLog("Entrando no método 'Calculate do ZeroLoadCell' para iniciar leituras das tags necessárias");

            checkError();
            // It will search the infos correponding to the specific operation
            var operationInfos = SearchInfoInList(this.eq, this.operationID);

            // Verifica se retornou alguma info
            // Se não retornou então a receita foi cancelada
            if (operationInfos.Count > 0)
            {
                var result = operationInfos.ElementAt(0);

                bool gerarPdf = false;
                double tankWeigth = 0.0;
                string tankWightModified = "";

                try
                {
                    tankWeigth = convert.convertToDouble("result.Param_0", result.Param_0);
                    this.basicInfo.Hour = Convert.ToDateTime(result.Hora_0);
                    this.basicInfo.Date = Convert.ToDateTime(result.Date);
                    this.basicInfo.OperatorLogin = result.Asignature;
                    tankWeigth = (tankWeigth / 100);
                    tankWightModified = tankWeigth.ToString().Replace(".", ",");
                }
                catch (Exception e)
                {
                    errorlog.writeLog("ZeroLoadCell", "tag não especificada", e.ToString(), DateTime.Now);
                    this.eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                    this.eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                }

                // TESTANDO FORMAT DOT TO COMMA
                var changeDotToComma = System.Globalization.CultureInfo.GetCultureInfo("de-De");
                var x = CreateString(tankWightModified);

             
                if (!gerarPdf)
                {
                    txt.addItem(x);
                    txt.saveTxt(x, false);

                    //logAction.writeLog("Texto adicionado ao log.txt");
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
            else
            {
                Pdf pdf = new Pdf(txt.txtAtual);
                pdf.gerarPdf(txt.Header, basicInfo);
                txt.cleanTxt();
            }
        }

        public string CreateString(params string[] values)
        {
            //logAction.writeLog("Iniciando CreateString");

            string txtCreate =
                "<h3>Zerar Célula de Carga</h3>" +
                "<label>Peso do Tanque : </label><span class='campo'>" + values[0] + "kg</span>" +
                "<br>" + basicInfo.CreateString();

            //logAction.writeLog("CreateString executado, string gerada: " + "\n" + txtCreate);

            return txtCreate;
        }

    }
}
