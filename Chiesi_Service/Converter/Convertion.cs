using Chiesi.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chiesi.Converter
{
    class Convertion
    {
        EquipamentFactory eqFact = EquipamentFactory.GetEquipamentFactory();

        IEquipament eq;

        public ErrorLog errorLog { get; set; }

        public Convertion(EquipamentType typeEq)
        {

            this.eq = this.eqFact.ConstructEquipament(typeEq);
            this.errorLog = new ErrorLog();
        }
        public bool convertToBoolean(string tag,string value)
        {
            bool converted = true;
            bool tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

            try
            {
                converted = Convert.ToBoolean(value);
                return converted;
            }
            catch (Exception e)
            {
                errorLog.writeLog("ConvertionToBoolean", tag, e.ToString(), DateTime.Now);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

                while (tagerror)
                {
                    tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));
                    Thread.Sleep(1000);
                }


                return convertToBoolean(tag, value);

            }


        }
        public int convertToInt(string tag, string value)
        {
            
            int converted;
            bool tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

            try
            {
                converted = Convert.ToInt32(value);
                return converted;
            }
            catch (Exception e)
            {
                errorLog.writeLog("ConvertionToInt", tag, e.ToString(), DateTime.Now);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

                while (tagerror)
                {
                    tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));
                    Thread.Sleep(1000);
                }


                return convertToInt(tag, value);

            }


        }
        public double convertToDouble(string tag, string value)
        {
            double converted;
            bool tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

            try
            {
                converted = Convert.ToDouble(value);
                return converted;
            }
            catch (Exception e)
            {
                errorLog.writeLog("ConvertionToDouble", tag, e.ToString(), DateTime.Now);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

                while (tagerror)
                {
                    tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));
                    Thread.Sleep(1000);
                }


                return convertToDouble(tag, value);

            }


        }

        public sbyte convertToSbyte(string tag, string value)
        {
            sbyte converted;
            bool tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

            try
            {
                converted = Convert.ToSByte(value);
                return converted;
            }
            catch (Exception e)
            {
                errorLog.writeLog("ConvertionToSByte", tag, e.ToString(), DateTime.Now);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], e.Message);
                eq.Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));

                while (tagerror)
                {
                    tagerror = Convert.ToBoolean(eq.Read(ConfigurationManager.AppSettings["TAGERRORPLC"]));
                    Thread.Sleep(1000);
                }


                return convertToSbyte(tag, value);

            }


        }
    }
}
