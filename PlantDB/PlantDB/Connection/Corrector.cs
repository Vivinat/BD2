using System.Text.Json;
using Newtonsoft.Json.Linq;
using PlantDB.Context;
using PlantDB.Controllers;
using PlantDB.Types.Filtered;
using RestSharp;

namespace PlantDB.Connection;

public static class Corrector
{
    public static void CorrectColumns()
    {
        var perenualClient = new RestClient("https://perenual.com/api/");
        List<string> apiKeys = new List<string>();
        //apiKeys.Add("sk-gtjB665eff981f0055796"); 
        //apiKeys.Add("sk-gKF2665f00044ef195797");
        //apiKeys.Add("sk-jCm6665e566e258225787"); 
        apiKeys.Add("sk-RH5s6650baec4ee3d5632"); //CORINGA
        int k = 0;
        int pagecheckpoint = 0;
        string scientificName;
        string jsonString;
        string jsonPath =
            "C:\\Users\\Particular\\Documents\\GitHub\\BD2\\PlantDB\\PlantDB\\Connection\\pageCorrector.json";
        using (StreamReader reader = new StreamReader(jsonPath))
        {
            jsonString = reader.ReadToEnd();
        }
        JsonDocument doc = JsonDocument.Parse(jsonString);
        if (doc.RootElement.TryGetProperty("page", out JsonElement element))
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                pagecheckpoint = element.GetInt32();
            }
            else
            {
                Console.WriteLine("Falha no JSON de pagina");
            }
        }
        else
        {
            Console.WriteLine("Falha no JSON de pagina");
        }

        for (int j = 0; j < 1; j++)
        {
            for (int i = 0; i < 100; i++)
            {
                JArray pageResults = GetPerenualEdiblePage(pagecheckpoint, apiKeys[k], perenualClient);
                foreach (JObject plant in pageResults)
                {
                    scientificName = (string)plant["scientific_name"][0];
                    using (var dbContext = new DBContext())     
                    {
                        var plantToChange = dbContext.plant_details.FirstOrDefault(p => p.scientific_name == scientificName);
                        if (plantToChange != null)
                        {
                            plantToChange.indoor = true;
                            Console.WriteLine("Corrected " + scientificName );
                            dbContext.SaveChanges();
                        }
                        var dict = new Dictionary<string, int> { { "page", pagecheckpoint } };
                        string newPageNumber = JsonSerializer.Serialize(dict);
                        File.WriteAllText(jsonPath, newPageNumber);
                    }    
                }
                pagecheckpoint++;
            }
            k += 1;
            if (k < apiKeys.Count) //PARA IMPEDIR LISTA DE ESTOURAR
            {
                j = -1; //REINICIA O CICLO
                Console.WriteLine("Trocando para a chave " + apiKeys[k]);
            }
        }


    }
    
    
    private static JArray GetPerenualEdiblePage(int pageNumber, string perenualKey, RestClient perenualClient)
    {
        var plantRequest = new RestRequest($"species-list?key={perenualKey}&indoor=1&page={pageNumber}")
        {
            OnBeforeDeserialization = resp => { resp.ContentType = "application/json";}
        };
        var plantQueryResult = perenualClient.ExecuteGet(plantRequest);
        JObject jsonPlantResponse = JObject.Parse(plantQueryResult.Content);
        JArray dataArray = (JArray)jsonPlantResponse["data"];
        return dataArray;
    }
    
}