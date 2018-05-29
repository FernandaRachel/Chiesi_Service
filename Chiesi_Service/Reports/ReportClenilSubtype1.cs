using Chiesi.AverageSpeed;
using Chiesi.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiesi.Operation;
using Chiesi.Monitoring;
using Chiesi.Valve;
using Chiesi.Filling;


namespace Chiesi.Reports
{
    class ReportClenilSubType1 : IReport
    {

        OperationHandler op1;
        OperationHandler op2;
        OperationHandler op3;
        OperationHandler op4;
        OperationHandler op5;
        OperationHandler op7;
        OperationHandler op11;
        OperationHandler op12;
        OperationHandler op13;
        OperationHandler op14;
        OperationHandler op15;
        OperationHandler op16_1;
        OperationHandler op16_2;
        OperationHandler op16_3;
        OperationHandler op17;
        OperationHandler op18;
        public Text txt;

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        public ReportClenilSubType1(EquipamentType eqtype)
        {
            this.eqFact = EquipamentFactory.GetEquipamentFactory();

            op1 = new BeginOfManipulation(eqtype, "BeginOfManipulation", "Clenil");
            op2 = new ZeroLoadCell(eqtype);
            op3 = new FirstLoading(eqtype, "1º Carregamento de Álcool Etílico Anidro ", "1.2", "3",0);
            op4 = new TempMonitoringClass(eqtype, true);
            op5 = new SecondLoadingClass(eqtype, "2º Carregamento de Álcool Etílico Anidro + Glicerol", "1.2", "3", false, false,0);
            op7 = new ThirdLoadingClass(eqtype, "3º Carregamento de Álcool Etílico Anidro + Glicerol", "1.2", "3", true,1);
            op11 = new HighSpeedMix(eqtype, "30", "1500", "0", "0", false, false, false, "5",0);
            op12 = new AdditionClass(eqtype, "Adição de Dipropionato de Beclometasona", false, false,0);
            op13 = new HighSpeedMix(eqtype, "30", "1500", "0", "0", false, false, true, "10",1);
            op14 = new LowSpeedMix(eqtype, "10", "30", false);
            op15 = new TankFinalWeight(eqtype, "TankFinalWeight", false);
            op16_1 = new ValveClass(eqtype, "5", true, false, "200", "494", "V10","20");
            op16_2 = new ValveClass(eqtype, "5", false, true, "200", "494", "V9","19");
            op17 = new RecirculationHoseDrain(eqtype);
            op16_3 = new ValveClass(eqtype, "1", true, true, "200", "494", "V8","18");
            op18 = new EndOfManipulation(eqtype, false, true);
            this.txt = new Text();
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
            op7.SetSuccessor(op11);
            op11.SetSuccessor(op12);
            op12.SetSuccessor(op13);
            op13.SetSuccessor(op14);
            op14.SetSuccessor(op15);
            op15.SetSuccessor(op16_1);
            op16_1.SetSuccessor(op16_2);
            op16_2.SetSuccessor(op17);
            op17.SetSuccessor(op16_3);
            op16_3.SetSuccessor(op18);

            op1.Calculate(txt);

            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
    }
}
