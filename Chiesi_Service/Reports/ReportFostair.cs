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
    class ReportFostair : IReport, IOperationState
    {

        OperationHandler op1;
        OperationHandler op2;
        OperationHandler op3;
        OperationHandler op4;
        OperationHandler op5;
        OperationHandler op7;
        OperationHandler op8;
        OperationHandler op9;
        OperationHandler op10;
        OperationHandler op11;
        OperationHandler op12;
        OperationHandler op13_1;
        OperationHandler op13_2;
        OperationHandler op13_3;
        OperationHandler op14;
        OperationHandler op15;
        OperationHandler op16;
       
        Text txt;

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        public ReportFostair(EquipamentType eqtype)
        {
            this.eqFact = EquipamentFactory.GetEquipamentFactory();

            txt = new Text();
            op1 = new BeginOfManipulation(eqtype, "BeginOfManipulation", "Fostair");
            op2 = new ZeroLoadCell(eqtype);
            op3 = new FirstLoading(eqtype, "1º Carregamento de Álcool Etílico Anidro", "1.2", "3");
            op4 = new TempMonitoringClass(eqtype, true);
            op5 = new SecondLoadingClass(eqtype, "2º Carregamento de Álcool Etílico Anidro + Formoterol", "1.2", "3", true, false);
            op7 = new AdditionClass(eqtype, "Adição de Dipropionato de Beclometasona", false, false,0);
            op8 = new HighSpeedMix(eqtype, "30", "1500", "0", "0", false, false, true, "10",0);
            op9 = new LowSpeedMix(eqtype, "30", "10", false);
            op10 = new AdditionClass(eqtype, "Adição de Excipiente pelo Bocal T2", false, false,1);
            op11 = new LowSpeedMix(eqtype, "10", "30", false);
            op12 = new TankFinalWeight(eqtype, "0", true);
            op13_1 = new ValveClass(eqtype, "5", true, false, "200", "494", "V10");
            op13_2 = new ValveClass(eqtype, "5", false, true, "200", "494", "V9");
            op14 = new RecirculationHoseDrain(eqtype);
            op13_3 = new ValveClass(eqtype, "1", true, true, "200", "494", "V8");
            op15 = new EndOfManipulation(eqtype, true, false);
            op16 = new AdditionClass(eqtype, "Retirada de Amostra para Umidade e Titulação de HCL", false, true,2);

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
            op7.SetSuccessor(op8);
            op8.SetSuccessor(op9);
            op9.SetSuccessor(op10);
            op10.SetSuccessor(op11);
            op11.SetSuccessor(op12);
            op12.SetSuccessor(op13_1);
            op13_1.SetSuccessor(op13_2);
            op13_2.SetSuccessor(op14);
            op14.SetSuccessor(op13_3);
            op13_3.SetSuccessor(op15);
            op15.SetSuccessor(op16); 

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
