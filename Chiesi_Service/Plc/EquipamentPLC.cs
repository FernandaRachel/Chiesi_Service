using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Chiesi.Log;
using System.Threading;
using System.Configuration;
using Chiesi_Service.Recipe;
using Chiesi_Service.Log;
using System.Collections.Specialized;

namespace Chiesi
{
    class EquipamentPLC : IEquipament
    {
        public string address { get; set; }

        public Recipe recipe { get; set; }

        public RecipeData recipeData { get; set; }

        /* USED TO MAKE THIS CLASS A SINGLETON CLASS*/
        private static EquipamentPLC _instance;
        private static object syncLock = new object();
        /* -------- */
        public ErrorLog errorLog { get; set; }

        public LogAction logAction { get; set; }


        public static EquipamentPLC GetEquipamentPLC()
        {

            if (_instance == null)
            {
                lock (syncLock)
                {
                    //if (_instance == null)
                    //{
                        _instance = new EquipamentPLC();

                        _instance.Connect();
                    //}
                }
            }

            return _instance;
        }

        private EquipamentPLC()
        {
            recipe = new Recipe();
            recipeData = new RecipeData();
            address = ConfigurationManager.AppSettings["connectionString"];
            errorLog = new ErrorLog();
            logAction = new LogAction();
            //logAction.writeLog("connectionString: " + address);
        }

        public Recipe ReadAllData()
        {
            //logAction.writeLog("Reading buffer header");
            Dictionary<string, string> Tags = new Dictionary<string, string>();
            recipe.Data = new List<RecipeData>();
            Tags = ReadAll();
            //POPULATE THE HEADER(BeginOfManipulation)
            recipe.Code = Tags["TAGCODE"];
            recipe.Batch = Tags["TAGBATCH"];
            recipe.Asignature = Tags["TAGASSINATURA_BEGINOFMANIPULATION"];
            recipe.Date = Tags["TAGDATA_BEGINOFMANIPULATION"];
            recipe.Hour = Tags["TAGHOUR_BEGINOFMANIPULATION"];
            recipe.RecipeType = Tags["TAGPRODUCTYPE"];
            recipe.RecipeSubType = Tags["TAGSUBTYPE"];
            recipe.RecipeName = Tags["TAGRECIPENAME"];
            recipe.WeightTank = Tags["TAGPESOTANK"];

            // POPULATE THE ARRAY INDEX 0 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 0");
            recipeData.Param_0 = Tags["RECEITA.DADOS[0].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[0].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[0].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[0].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[0].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[0].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[0].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[0].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[0].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[0].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 1 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 1");
            recipeData.Param_0 = Tags["RECEITA.DADOS[1].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[1].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[1].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[1].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[1].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[1].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[1].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[1].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[1].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[1].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 2 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 2");
            recipeData.Param_0 = Tags["RECEITA.DADOS[2].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[2].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[2].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[2].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[2].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[2].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[2].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[2].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[2].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[2].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 3 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 3");
            recipeData.Param_0 = Tags["RECEITA.DADOS[3].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[3].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[3].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[3].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[3].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[3].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[3].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[3].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[3].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[3].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 4 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 4");
            recipeData.Param_0 = Tags["RECEITA.DADOS[4].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[4].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[4].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[4].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[4].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[4].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[4].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[4].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[4].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[4].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 5 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 5");
            recipeData.Param_0 = Tags["RECEITA.DADOS[5].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[5].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[5].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[5].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[5].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[5].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[5].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[5].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[5].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[5].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 6 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 6");
            recipeData.Param_0 = Tags["RECEITA.DADOS[6].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[6].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[6].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[6].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[6].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[6].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[6].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[6].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[6].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[6].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 7 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 7");
            recipeData.Param_0 = Tags["RECEITA.DADOS[7].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[7].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[7].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[7].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[7].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[7].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[7].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[7].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[7].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[7].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 8 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 8");
            recipeData.Param_0 = Tags["RECEITA.DADOS[8].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[8].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[8].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[8].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[8].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[8].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[8].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[8].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[8].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[8].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 9 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 9");
            recipeData.Param_0 = Tags["RECEITA.DADOS[9].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[9].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[9].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[9].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[9].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[9].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[9].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[9].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[9].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[9].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 10 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 10");
            recipeData.Param_0 = Tags["RECEITA.DADOS[10].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[10].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[10].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[10].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[10].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[10].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[10].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[10].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[10].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[10].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 11 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 11");
            recipeData.Param_0 = Tags["RECEITA.DADOS[11].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[11].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[11].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[11].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[11].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[11].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[11].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[11].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[11].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[11].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 12 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 12");
            recipeData.Param_0 = Tags["RECEITA.DADOS[12].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[12].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[12].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[12].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[12].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[12].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[12].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[12].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[12].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[12].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 13 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 13");
            recipeData.Param_0 = Tags["RECEITA.DADOS[13].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[13].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[13].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[13].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[13].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[13].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[13].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[13].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[13].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[13].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 14 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 14");
            recipeData.Param_0 = Tags["RECEITA.DADOS[14].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[14].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[14].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[14].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[14].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[14].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[14].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[14].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[14].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[14].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 15 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 15");
            recipeData.Param_0 = Tags["RECEITA.DADOS[15].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[15].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[15].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[15].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[15].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[15].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[15].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[15].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[15].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[15].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 16 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 16");
            recipeData.Param_0 = Tags["RECEITA.DADOS[16].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[16].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[16].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[16].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[16].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[16].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[16].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[16].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[16].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[16].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 17 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 17");
            recipeData.Param_0 = Tags["RECEITA.DADOS[17].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[17].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[17].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[17].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[17].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[17].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[17].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[17].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[17].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[17].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 18 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 18");
            recipeData.Param_0 = Tags["RECEITA.DADOS[18].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[18].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[18].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[18].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[18].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[18].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[18].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[18].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[18].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[18].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 19 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 19");
            recipeData.Param_0 = Tags["RECEITA.DADOS[19].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[19].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[19].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[19].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[19].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[19].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[19].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[19].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[19].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[19].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 20 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 20");
            recipeData.Param_0 = Tags["RECEITA.DADOS[20].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[20].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[20].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[20].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[20].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[20].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[20].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[20].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[20].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[20].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 21 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 21");
            recipeData.Param_0 = Tags["RECEITA.DADOS[21].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[21].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[21].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[21].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[21].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[21].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[21].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[21].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[21].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[21].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 22 OF DATA
            recipeData = new RecipeData();
            //logAction.writeLog("Reading Buffer Index 22");
            recipeData.Param_0 = Tags["RECEITA.DADOS[22].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[22].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[22].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[22].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[22].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[22].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[22].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[22].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[22].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[22].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 23 OF DATA
            //logAction.writeLog("Reading Buffer Index 23");
            recipeData.Param_0 = Tags["RECEITA.DADOS[23].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[23].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[23].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[23].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[23].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[23].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[23].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[23].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[23].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[23].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 24 OF DATA
            //logAction.writeLog("Reading Buffer Index 24");
            recipeData.Param_0 = Tags["RECEITA.DADOS[24].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[24].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[24].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[24].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[24].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[24].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[24].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[24].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[24].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[24].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 25 OF DATA
            //logAction.writeLog("Reading Buffer Index 25");
            recipeData.Param_0 = Tags["RECEITA.DADOS[25].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[25].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[25].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[25].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[25].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[25].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[25].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[25].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[25].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[25].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 26 OF DATA
            //logAction.writeLog("Reading Buffer Index 26");
            recipeData.Param_0 = Tags["RECEITA.DADOS[26].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[26].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[26].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[26].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[26].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[26].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[26].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[26].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[26].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[26].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 27 OF DATA
            //logAction.writeLog("Reading Buffer Index 27");
            recipeData.Param_0 = Tags["RECEITA.DADOS[27].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[27].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[27].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[27].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[27].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[27].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[27].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[27].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[27].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[27].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 28 OF DATA
            //logAction.writeLog("Reading Buffer Index 28");
            recipeData.Param_0 = Tags["RECEITA.DADOS[28].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[28].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[28].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[28].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[28].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[28].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[28].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[28].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[28].ASSINATURA"];
            recipeData.Id = Tags["RECEITA.DADOS[28].ID"];
            recipe.Data.Add(recipeData);
            // POPULATE THE ARRAY INDEX 29 OF DATA
            //logAction.writeLog("Reading Buffer Index 29");
            recipeData.Param_0 = Tags["RECEITA.DADOS[29].PARAMETROS[0]"];
            recipeData.Param_1 = Tags["RECEITA.DADOS[29].PARAMETROS[1]"];
            recipeData.Param_2 = Tags["RECEITA.DADOS[29].PARAMETROS[2]"];
            recipeData.Param_3 = Tags["RECEITA.DADOS[29].PARAMETROS[3]"];
            recipeData.Param_4 = Tags["RECEITA.DADOS[29].PARAMETROS[4]"];
            recipeData.Hora_0 = Tags["RECEITA.DADOS[29].HORA[0]"];
            recipeData.Hora_1 = Tags["RECEITA.DADOS[29].HORA[1]"];
            recipeData.Date = Tags["RECEITA.DADOS[29].DATA"];
            recipeData.Asignature = Tags["RECEITA.DADOS[29].ASSINATURA"];
            //recipeData.Id = Tags["RECEITA.DADOS[29].ID"];
            recipeData.Id = "0";
            recipe.Data.Add(recipeData);


            return recipe;
        }


        public void Connect()
        {
            Console.WriteLine("Connected");
        }

        public Dictionary<string, string> ReadAll()
        {
            //logAction.writeLog("It is going to read all Tags");

            bool checkRead = false; // check if try returned "ok"
            bool checkError = false; // check if a tag or tag value is null
            Dictionary<string, string> Tags = new Dictionary<string, string>();
            string key = "";
            string value = "";
            string queryString = "SELECT TagAddress,TagValue FROM SPI_TB_IL_ADDRESS";
            var applicationSettings = ConfigurationManager.GetSection("appSettings") as NameValueCollection;
            //string queryString = "SELECT TagValue FROM SPI_TB_IL_ADDRESS" + " WHERE TagAddress = @TagAddress";
            int count = 0;

            while (checkRead == false && checkError == false)
            {
                count++;

                using (SqlConnection connection =
            new SqlConnection(address))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    //command.Parameters.AddWithValue("@TagAddress", Tag);


                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        // se a TAG não existe no BD - não existirá "rows" - gerando erro de tag não mapeada
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var teste = reader.IsDBNull(reader.GetOrdinal("TagValue"));
                                //verifica se a tag é nula e seta ela como string vazia para não parar o relatório com erro de DB!!!
                                if (teste)
                                {
                                    key = reader.GetString(0);
                                    //var key2 = reader.GetString(1);
                                    value = "";
                                    errorLog.writeLog("EquipamentPLC ", "Read ", " tag nula", DateTime.Now);
                                    checkRead = true;

                                    //var sl = applicationSettings.AllKeys.Where(k => k.StartsWith(key));
                                    //var tags = from tagss in applicationSettings.AllKeys
                                    //           where tagss == key
                                    //           select tagss;

                                    foreach (var keyyy in applicationSettings.AllKeys)
                                    {
                                        var valueApp = applicationSettings[keyyy];
                                        if (valueApp == key)
                                        {
                                            Tags.Add(keyyy, value);
                                            break;
                                        }
                                    }

                                }
                                else
                                {
                                    key = reader.GetString(0);
                                    value = reader.GetString(1);
                                    checkRead = true;
                                    foreach (var keyyy in applicationSettings.AllKeys)
                                    {
                                        var valueApp = applicationSettings[keyyy];
                                        if (valueApp == key)
                                        {
                                            Tags.Add(keyyy, value);
                                            break;
                                        }
                                    }

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
                            errorLog.writeLog("EquipamentPLC", "Read", ex.Message, DateTime.Now);
                            Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                            Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], ex.Message);
                            checkError = true;
                        }
                    }
                }

                Thread.Sleep(1000);
            }
            Thread.Sleep(100);

            //logAction.writeLog("All Tags have been read");

            return Tags;
        }


        public string Read(string Tag)
        {
            //logAction.writeLog("It is going to read tag named as: " + Tag);

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

                                Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], "Nao Mapeada");
                                Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
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
                            Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                            Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], Tag + ex.Message);
                            checkError = true;
                        }
                    }
                }

                Thread.Sleep(1000);
            }
            Thread.Sleep(100);

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
                        Write(ConfigurationManager.AppSettings["TAGERRORMESSAGE"], Tag + ex.Message);
                        Write(ConfigurationManager.AppSettings["TAGERRORPLC"], "True");
                        errorLog.writeLog("EquipamentPLC", "Write", Tag + ex.Message, DateTime.Now);
                    }
                }
            }

            Thread.Sleep(1000);
        }


    }
}
