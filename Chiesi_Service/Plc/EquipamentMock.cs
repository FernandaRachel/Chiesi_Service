using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiesi
{
    class EquipamentMock 
    {

        public Dictionary<string, string> dictMemory = new Dictionary<string, string>();



        public string address { get; set; }

        private static EquipamentMock _instance;
        private static object syncLock = new object();

        protected EquipamentMock(string address)
        {
            this.address = address;
        }

        public static EquipamentMock GetEquipamentMock()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new EquipamentMock(StaticValues.ADDRESS);

                        _instance.Connect();
                    }
                }
            }

            return _instance;
        }
        public void Connect()
        {

            dictMemory.Add("tag_code", "1");
            dictMemory.Add("tag_product", "Clenil");
            dictMemory.Add("tag_operador", "Cleiton Stroncio");
            dictMemory.Add("tag_assinatura", "cstroncio@gmail.com");
            dictMemory.Add("tag_date", "25/12/2017");
            dictMemory.Add("tag_time", "22:30");
            dictMemory.Add("tag_batch", "1000");
            dictMemory.Add("tag_theoric_qty", "12");
            dictMemory.Add("tag_gli_theoric_qty", "20");
            dictMemory.Add("tag_flow_real_qty", "10");
            dictMemory.Add("tag_cell_real_qty", "5");
            dictMemory.Add("tag_product_temp", "22");
            dictMemory.Add("tag_shaking_speed", "25");
            dictMemory.Add("tag_rpm_limit", "24");
            dictMemory.Add("tag_mix_time", "5");
            dictMemory.Add("tag_anchor_speed", "1400");
            dictMemory.Add("tag_turbine_speed", "14");
            dictMemory.Add("tag_turbine_limit", "30");
            dictMemory.Add("tag_lobules_speed", "");
            dictMemory.Add("tag_ini_speed", "16:22");
            dictMemory.Add("tag_end_speed", "16:27");
            dictMemory.Add("tag_tank_weight", "50");
            dictMemory.Add("ProjetoChiesi.plcchiesi.Operation.tag_signal", "1");
            dictMemory.Add("tag_variation_flux", "1");
            dictMemory.Add("tag_variation_loading_cell", "1");
            dictMemory.Add("tag_shake_speed", "25");
            dictMemory.Add("tag_outflow_start", "15:30");
            dictMemory.Add("tag_outflow_end", "16:00");
            dictMemory.Add("tag_filling_type", "tipo envase");
            dictMemory.Add("tag_var_flux", "15");
            dictMemory.Add("tag_var_cell", "13");
            dictMemory.Add("tag_rpm_limit_recirculation", "200-494");
            dictMemory.Add("tag_rpm_lobules_speed", "0");
            dictMemory.Add("tag_min_valve", "0");
            dictMemory.Add("tag_speed_clenil", "1400");
            dictMemory.Add("tag_speed_clenil_forte", "1800");
            dictMemory.Add("tag_clenil_limit", "1500");
            dictMemory.Add("tag_clenil_forte_limit", "2000");
            dictMemory.Add("tag_product_type", "Placebos");


            Console.WriteLine("Connected");

        }




        public string Read(string Tag)
        {

            return dictMemory[Tag];
        }



        public void Write(string Tag, string value)
        {

            //return dictMemory[Tag] = value;
        }
    }
}
