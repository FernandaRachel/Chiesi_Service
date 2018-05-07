using Chiesi.AverageSpeed;
using Chiesi.Filling;
using Chiesi.Loading;
using Chiesi.Monitoring;
using Chiesi.Operation;
using Chiesi.Valve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.Reports
{
    class ReportPlacebosClenil : IReport, IOperationState
    {
        OperationHandler op1;
        OperationHandler op2;
        OperationHandler op3;
        OperationHandler op4;
        OperationHandler op5;
        OperationHandler op7;
        OperationHandler op10;
        OperationHandler op11;
        OperationHandler op12;
        //OperationHandler op13;
        //OperationHandler op13_1;
        //OperationHandler op14;
        Text txt;

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();



        public ReportPlacebosClenil(EquipamentType eqtype)
        {

            this.eqFact = EquipamentFactory.GetEquipamentFactory();


            txt = new Text();
            op1 = new BeginOfManipulation(eqtype, "BeginOfManipulation", "Placebos");
            op2 = new ZeroLoadCell(eqtype);
            op3 = new FirstLoading(eqtype, "1º Carregamento de Álcool Etílico Anidro", "1.2", "3");
            op4 = new TempMonitoringClass(eqtype, true);
            op5 = new SecondLoadingClass(eqtype, "2º Carregamento de Álcool Etílico Anidro + Glicerol", "1.2", "3", false, false);
            op7 = new HighSpeedMix(eqtype, "30", "1500", "0", "0", false, false, false, "5");
            op10 = new TankFinalWeight(eqtype, "Peso final do Produto no Tanque", true);
            op11 = new ValveClass(eqtype, "5", true, true, "200", "494", "V8");
            op12 = new EndOfManipulation(eqtype, false, true);
            //op13 = new FillingStart(eqtype, "Placebos");
            //op13_1 = new ValveClass(eqtype, "5", true, true, "200", "494", "V8");
            //op14 = new EndFilling(eqtype, false);
            StartReport();
        }

        public void StartReport()
        {
            Status.SetInProcessMode();
            txt.generateTxt("Relatório de Batch Record");
            op1.SetSuccessor(op2);
            op2.SetSuccessor(op3);
            op3.SetSuccessor(op4);
            op4.SetSuccessor(op5);
            op5.SetSuccessor(op7);
            op7.SetSuccessor(op10);
            op10.SetSuccessor(op11);
            op11.SetSuccessor(op12);
            //op12.SetSuccessor(op13);
            //op13.SetSuccessor(op13_1);
            //op13_1.SetSuccessor(op14);

            op1.Calculate(txt);
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }

        public void SetModeToIdle()
        {
            throw new NotImplementedException();
        }

        public void SetInProcessMode()
        {
            throw new NotImplementedException();
        }

        public void SetFailMode()
        {
            throw new NotImplementedException();
        }
    }
}
