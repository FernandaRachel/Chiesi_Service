using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi.BasicInfos
{
    class BasicInfoClass
    {
        private static BasicInfoClass _instance;

        public string KeepBatch { get; set; }

        private static object syncLock = new object();

        public DateTime Date { get; set; }

        public DateTime Hour { get; set; }

        public string DateTag { get; set; }

        public string HourTag { get; set; }

        public string OperatorLogin { get; set; }

        public string OperatorLoginTag { get; set; }

        public string OperatorName { get; set; }

        public string OperatorNameTag { get; set; }

        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;



        protected BasicInfoClass(string DateTag, string HourTag, string OperatorLoginTag, string OperatorNameTag, EquipamentType typeEq)
        {
            this.DateTag = DateTag;
            this.HourTag = HourTag;
            this.OperatorLoginTag = OperatorLoginTag;
            this.OperatorNameTag = OperatorNameTag;
            this.eq = this.eqFact.ConstructEquipament(typeEq);

        }


        public static BasicInfoClass GetBasicInfo()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new BasicInfoClass(StaticValues.TAGDATA, StaticValues.TAGHOUR, ConfigurationManager.AppSettings["TAGASSINATURA"], StaticValues.TAGOPERADOR,  StaticValues.EQUIPAMENTTYPE);

                    };
                }
            }

            return _instance;
        }

        public void ReadPlc()
        {
            Date = DateTime.Now;
            Hour = DateTime.Now;
            //OperatorName = eq.Read(OperatorNameTag);
            OperatorLogin = eq.Read(OperatorLoginTag);
        }


        public string CreateString()
        {
            string txtCreate =
                "<div class='basic-info'>" +
                    "<label>Data : </label><span class='campo'>" + Date.ToString("dd/MM/yyyy") + "</span>" +
                    "<label class='lab'>Hora : </label><span class='campo'>" + Date.ToString("HH:mm") + "</span>" +
                    "<br><label>Assinatura : </label><span class='campo'>" + OperatorLogin + "</span>" +
                "</div>";
            return txtCreate;
        }
    }
}
