using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PlantDB.Context;
using PlantDB.Controllers;
using RestSharp;
using PlantDB.Types.Filtered;

namespace PlantDB.Connection;

using Newtonsoft.Json.Linq;

public static class PlantListRetriever
{
    public static void GetPlantList()
    {
        int k = 0;
        List<string> apiKeys = new List<string>();
        //apiKeys.Add("sk-lSG36650d454dca195631"); 
        //apiKeys.Add("sk-yUz2664cfc366009e4763"); 
        //apiKeys.Add("sk-RH5s6650baec4ee3d5632"); 
        //apiKeys.Add("sk-xpZH66509c5155e255630"); 
        //apiKeys.Add("sk-bC9266436bfad30f45481"); 
        //apiKeys.Add("sk-9vpf66586661475685717"); 
        //apiKeys.Add("sk-hLrI665872198d9aa5720"); 
        //apiKeys.Add("sk-Ppo9665878f67ab255721");
        //apiKeys.Add("sk-YlgC665879bdd93345722"); 
        //apiKeys.Add("sk-qYla66587a2e10e6b5723");
        //apiKeys.Add("sk-yGwl665920b439d805731");
        //apiKeys.Add("sk-3Wnh66592196d237e5732"); 
        //apiKeys.Add("sk-oWiu6659221191ef85733");
        
        int currentPerenualId = 0;
        int pagecheckpoint = 0;
        string scientificName;
        List<string> scientificNameVerifier = new List<string>();
        var perenualClient = new RestClient("https://perenual.com/api/");
        List<PlantSummary> plantSummaries = new List<PlantSummary>();
        List<PlantDetailsSummary> plantDetailsSummaries = new List<PlantDetailsSummary>();
        List<DangerousPlantsSummary> dangerousPlantsSummaries = new List<DangerousPlantsSummary>();
        List<CultivationSummary> cultivationSummaries = new List<CultivationSummary>();
        
        string jsonString;
        string jsonPath =
            "C:\\Users\\Particular\\Documents\\GitHub\\BD2\\PlantDB\\PlantDB\\Connection\\pageManager.json";
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

        for (int j = 0; j < 3; j++)
        {
            pagecheckpoint += 1;
            Console.WriteLine("Página " + pagecheckpoint + " do Perenual Exigida");
            JArray pageResults = GetPerenualPlantPage(pagecheckpoint, apiKeys[k], perenualClient, plantSummaries);

            for (int i = 0; i < 30; i++) //IREI PERCORRER AS 30 PLANTAS
            {
                var plantDetailsRequest = new RestRequest($"species/details/{i+1}?key={apiKeys[k]}")
                {
                    OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
                };
                
                try
                {
                    
                    currentPerenualId += 1;
                    JObject plant = (JObject)pageResults[i];
                    scientificName = (string)plant["scientific_name"][0];
                    
                    if (scientificNameVerifier.Exists(p => p == scientificName))
                    {
                        Console.WriteLine($"Scientific name{scientificName} already on database");
                        continue;
                    }
                    else
                    {
                        scientificNameVerifier.Add(scientificName);
                    }
                    
                    PlantSummary plantSummary = new PlantSummary
                    {
                        id_plant = (int)plant["id"],
                        common_name = (string)plant["common_name"],
                        scientific_name = scientificName,
                    };
                    
                    plantSummaries.Add(plantSummary);
                    Console.WriteLine("Plant name: " + scientificName);
                    
                    var detailsQueryResult = perenualClient.ExecuteGet(plantDetailsRequest);
                    JObject jsonDetailsResponse = JObject.Parse(detailsQueryResult.Content);
                    
                    string growthRateString = (string)jsonDetailsResponse["growth_rate"];
                    Growthrate tempGrowthrate;
                    Growthrate growthrateToInsert;
                    if (growthRateString != null && Enum.TryParse(growthRateString.ToLower(), out tempGrowthrate))
                    {
                        growthrateToInsert = tempGrowthrate;
                    }
                    else
                    {
                        growthrateToInsert = Growthrate.nothing;
                    }
                    
                    PlantDetailsSummary plantDetailsSummary = new PlantDetailsSummary
                    {
                        edible_fruit = (bool)jsonDetailsResponse["edible_fruit"],
                        growth_rate = growthrateToInsert,
                        cuisine = (bool)jsonDetailsResponse["cuisine"],
                        invasive = (bool)jsonDetailsResponse["invasive"],
                        indoor = (bool)jsonDetailsResponse["indoor"],
                        medicinal = (bool)jsonDetailsResponse["medicinal"],
                        scientific_name = scientificName
                    };
                    plantDetailsSummaries.Add(plantDetailsSummary);
                    CultivationSummary cultivationSummary = new CultivationSummary
                    {
                        cycle = (string)jsonDetailsResponse["cycle"],
                        watering = (string)jsonDetailsResponse["watering"],
                        sunlight = (string)jsonDetailsResponse["sunlight"][0],
                        scientific_name = scientificName
                    };
                    cultivationSummaries.Add(cultivationSummary);
                    
                    string careLevelString = (string)jsonDetailsResponse["care_level"];
                    Carelevel temp;
                    Carelevel levelToInsert;
                    if (careLevelString != null && Enum.TryParse(careLevelString.ToLower(), out temp))
                    {
                        levelToInsert = temp;
                    }
                    else
                    {
                        levelToInsert = Carelevel.nothing;
                    }
                    
                    DangerousPlantsSummary dangerousPlantsSummary = new DangerousPlantsSummary
                    {
                        
                        care_level = levelToInsert,
                        thorny = (bool)jsonDetailsResponse["thorny"],
                        poisonous_to_humans = (bool)jsonDetailsResponse["poisonous_to_humans"],
                        poisonous_to_pets = (bool)jsonDetailsResponse["poisonous_to_pets"],
                        scientific_name = scientificName
                    };
                    dangerousPlantsSummaries.Add(dangerousPlantsSummary);
                    Console.WriteLine("Inserido planta de ID no perenual " + currentPerenualId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + " Perenual FALHOU NO ITEM DE VALOR " + i + "DA PAGINA " + j + " CICLO " + k);
                    throw;
                }
            }
            
            using (var dbContext = new DBContext())     //TENHO UMA BATCH DE 30 
            {
                DatabaseInsert databaseInsert = new DatabaseInsert(dbContext);
                databaseInsert.InsertIntoDatabase(plantSummaries,dangerousPlantsSummaries, plantDetailsSummaries, cultivationSummaries);
                Console.WriteLine("Inserted batch of page " + pagecheckpoint );
                plantSummaries.Clear();
                plantDetailsSummaries.Clear();
                dangerousPlantsSummaries.Clear();
                cultivationSummaries.Clear();
 
                var dict = new Dictionary<string, int> { { "page", pagecheckpoint } };
                string newPageNumber = JsonSerializer.Serialize(dict);
                File.WriteAllText(jsonPath, newPageNumber);
            }
            
            if (j == 2) //TROCA DE CHAVE
            {
                k += 1;
                if (k < apiKeys.Count) //PARA IMPEDIR LISTA DE ESTOURAR
                {
                    j = -1;  //REINICIA O CICLO
                    Console.WriteLine("Trocando para a chave " + apiKeys[k]);
                }
            }
        }
    }

    private static JArray GetPerenualPlantPage(int pageNumber, string perenualKey, RestClient perenualClient, List<PlantSummary> plantSummaries)
    {
        var plantRequest = new RestRequest($"species-list?key={perenualKey}&page={pageNumber}")
        {
            OnBeforeDeserialization = resp => { resp.ContentType = "application/json";}
        };
        var plantQueryResult = perenualClient.ExecuteGet(plantRequest);
        JObject jsonPlantResponse = JObject.Parse(plantQueryResult.Content);
        JArray dataArray = (JArray)jsonPlantResponse["data"];
        return dataArray;
    }
}

