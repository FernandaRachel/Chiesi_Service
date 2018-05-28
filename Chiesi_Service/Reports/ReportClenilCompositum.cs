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
    class ReportClenilCompositum : IReport, IOperationState
    {
        OperationHandler op1;
        OperationHandler op2;
        OperationHandler op3;
        OperationHandler op5;
        OperationHandler op6;
        OperationHandler op7;
        OperationHandler op8;
        OperationHandler op9;
        OperationHandler op10;
        OperationHandler op11;
        OperationHandler op12;
        OperationHandler op13;
        OperationHandler op14;
        public Text txt;

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();


        public ReportClenilCompositum(EquipamentType eqtype)


        {
            this.eqFact = EquipamentFactory.GetEquipamentFactory();


            this.txt = new Text();
            op1 = new BeginOfManipulation(eqtype, "BeginOfManipulation", "Clenil Compositum");
            op2 = new ZeroLoadCell(eqtype);
            op3 = new FirstLoading(eqtype, "1º Carregamento de Álcool Etílico Anidro", "1.2", "3",0);
            op5 = new TempMonitoringClass(eqtype, false);
            op6 = new AdditionClass(eqtype, "Adição de Excipiente pelo Bocal T2", false, false,0);
            op7 = new AdditionClass(eqtype, "Adição de Dipropionato de Baclometasoma", false, false,0);
            op8 = new HighSpeedMix(eqtype, "30", "0", "1500", "2000", true, false, false, "5", 1);
            op9 = new LowSpeedMix(eqtype, "15", "30", false);
            op10 = new AdditionClass(eqtype, "Adição de Sulfato de Salbutamol Micromizado", true, false,2);
            op11 = new HighSpeedMix(eqtype, "30", "0", "2000", "2500", true, false, false,"20",1);
            op12 = new TankFinalWeight(eqtype, "", false);
            op13 = new ValveClass(eqtype, "15", true, true, "200", "494", "V8",0);
            op14 = new EndOfManipulation(eqtype, false, true);

            StartReport();
        }


        public void StartReport()
        {
            Status.SetInProcessMode();
            txt.generateTxt("Relatório de Batch Record");
            op1.SetSuccessor(op2);
            op2.SetSuccessor(op3);
            op3.SetSuccessor(op5);
            op5.SetSuccessor(op6);
            op6.SetSuccessor(op7);
            op7.SetSuccessor(op8);
            op8.SetSuccessor(op9);
            op9.SetSuccessor(op10);
            op10.SetSuccessor(op11);
            op11.SetSuccessor(op12);
            op12.SetSuccessor(op13);
            op13.SetSuccessor(op14);

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
