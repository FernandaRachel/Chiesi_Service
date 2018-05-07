using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Chiesi.Log;
using System.Threading;

namespace Chiesi
{
    class EquipamentPLC : IEquipament
    {
        public string address { get; set; }

        private static EquipamentPLC _instance;
        private static object syncLock = new object();
        public ErrorLog errorLog { get; set; }

        public static EquipamentPLC GetEquipamentPLC()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new EquipamentPLC();

                        _instance.Connect();
                    }
                }
            }

            return _instance;
        }

        private EquipamentPLC()
        {
            address = StaticValues.connectionString;
            //address = "";
            errorLog = new ErrorLog();
        }

        public void Connect()
        {
            Console.WriteLine("Connected");
        }

        public string Read(string Tag)
        {
            //Tag = "xuxu";
            bool checkRead = false; // check if try returned "ok"
            bool checkError = false; // check if a tag or tag value is null
            string r = "";
            string queryString = "SELECT TagValue FROM SPI_TB_IL_ADDRESS" + " WHERE TagAddress = @TagAddress";
            int count = 0;

            while (checkRead == false && checkError == false)
            {
                count++;

                using (SqlConnection connection =
            new SqlConnection(address))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@TagAddress", Tag);


                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        // se a TAG não existe no BD - não existirá "rows" - gerando erro de tag não mapeada
                        if (!reader.HasRows)
                        {
                            if (count <= 2)
                            {
                                errorLog.writeLog("Read() ", Tag, "Tag não mapeada", DateTime.Now);

                                Write(StaticValues.TAGERRORMESSAGE, "Nao Mapeada");
                                Write(StaticValues.TAGERRORPLC, "True");
                                checkError = true;
                                Thread.Sleep(5000);
                            }
                            return Read(Tag);
                        }// fim erro tag não mapeada
                        else
                        {
                            while (reader.Read())
                            {
                                //verifica se a tag é nula e seta ela como string vazia para não parar o relatório com erro de DB!!!
                                if (reader.IsDBNull(0))
                                {
                                    r = "";
                                    errorLog.writeLog("EquipamentPLC ", "Read ", Tag + " tag nula", DateTime.Now);
                                    checkRead = true;
                                }
                                else
                                {
                                    r = reader.GetString(0);
                                    checkRead = true;
                                }

                            }
                        }

                        reader.Close();
                    }
                    catch (SqlException ex)
                    {
                        if (count <= 2)
                        {
                            errorLog.writeLog("EquipamentPLC", "Read", ex.Message + " SQLException", DateTime.Now);
                            checkRead = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (count <= 2)
                        {
                            errorLog.writeLog("EquipamentPLC", "Read", Tag + ex.Message, DateTime.Now);
                            Write(StaticValues.TAGERRORPLC, "True");
                            Write(StaticValues.TAGERRORMESSAGE, Tag + ex.Message);
                            checkError = true;
                        }
                    }
                }

                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);

            return r;
        }




        public void Write(string Tag, string value)
        {
            using (SqlConnection connection =
            new SqlConnection(address))
            {
                using (SqlCommand cmd = new SqlCommand("SPI_SELECT_TAGID", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@TagAddress", SqlDbType.VarChar).Value = Tag;
                    cmd.Parameters.Add("@Value", SqlDbType.VarChar).Value = value;

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        Write(StaticValues.TAGERRORMESSAGE, Tag + ex.Message);
                        Write(StaticValues.TAGERRORPLC, "True");
                        errorLog.writeLog("EquipamentPLC", "Write", Tag + ex.Message, DateTime.Now);
                    }
                }
            }

            Thread.Sleep(1000);
        }


    }
}
