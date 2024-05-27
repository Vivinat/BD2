using Microsoft.EntityFrameworkCore;
using PlantDB.Controllers;
using RestSharp;
using PlantDB.Types.Filtered;

namespace PlantDB.Connection;

using Newtonsoft.Json.Linq;

public static class PlantListRetriever
{
    private static DbContext _context;
    public static void GetPlantList()
    {
        int k = 0;
        List<string> apiKeys = new List<string>();
        apiKeys.Add("sk-lSG36650d454dca195631");
        apiKeys.Add("sk-yUz2664cfc366009e4763");
        apiKeys.Add("sk-RH5s6650baec4ee3d5632");
        apiKeys.Add("sk-xpZH66509c5155e255630");
        apiKeys.Add("sk-bC9266436bfad30f45481");
        
        var perenualClient = new RestClient("https://perenual.com/api/");

        List<PlantSummary> plantSummaries = new List<PlantSummary>();
        List<PlantDetailsSummary> plantDetailsSummaries = new List<PlantDetailsSummary>();
        List<DangerousPlantsSummary> dangerousPlantsSummaries = new List<DangerousPlantsSummary>();
        List<CultivationSummary> cultivationSummaries = new List<CultivationSummary>();

        DatabaseInsert dbInsert = new DatabaseInsert(_context);

        for (int j = 0; j < 3; j++)
        {
            plantSummaries = new List<PlantSummary>();  //UMA PÁGINA TEM 30 PLANTAS
            GetPerenualPlantPage(j + 1, apiKeys[k], perenualClient, plantSummaries);

            for (int i = 0; i < plantSummaries.Count; i++) //IREI PERCORRER AS 30 PLANTAS
            {

                var plantDetailsRequest = new RestRequest($"species/details/{i + 1}?key={apiKeys[k]}")
                {
                    OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
                };

                try
                {
                    var detailsQueryResult = perenualClient.ExecuteGet(plantDetailsRequest);
                    JObject jsonDetailsResponse = JObject.Parse(detailsQueryResult.Content);

                    PlantDetailsSummary plantDetailsSummary = new PlantDetailsSummary
                    {
                        IdDetail = (int)jsonDetailsResponse["id"],
                        EdibleFruit = (bool)jsonDetailsResponse["edible_fruit"],
                        Growth_Rate = (string)jsonDetailsResponse["growth_rate"],
                        Cuisine = (bool)jsonDetailsResponse["cuisine"],
                        Invasive = (bool)jsonDetailsResponse["invasive"],
                        Indoor = (bool)jsonDetailsResponse["indoor"],
                        Scientific_name = (string)jsonDetailsResponse["scientific_name"][0]
                    };
                    plantDetailsSummaries.Add(plantDetailsSummary);
                    CultivationSummary cultivationSummary = new CultivationSummary
                    {
                        Id = (int)jsonDetailsResponse["id"],
                        cycle = (string)jsonDetailsResponse["cycle"],
                        watering = (string)jsonDetailsResponse["watering"],
                        sunlight = (string)jsonDetailsResponse["sunlight"][0],
                        ScientificName = (string)jsonDetailsResponse["scientific_name"][0]
                    };
                    cultivationSummaries.Add(cultivationSummary);
                    DangerousPlantsSummary dangerousPlantsSummary = new DangerousPlantsSummary
                    {
                        Id = (int)jsonDetailsResponse["id"],
                        Care_Level = (string)jsonDetailsResponse["care_level"],
                        Thorny = (bool)jsonDetailsResponse["thorny"],
                        Poisonous_To_Humans = (bool)jsonDetailsResponse["poisonous_to_humans"],
                        Poisonous_To_Pets = (bool)jsonDetailsResponse["poisonous_to_pets"],
                        ScientificName = (string)jsonDetailsResponse["scientific_name"][0]
                    };
                    dangerousPlantsSummaries.Add(dangerousPlantsSummary);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + " Perenual FALHOU NO ITEM DE VALOR " + i + "DA PAGINA " + j + " CICLO " + k);
                    throw;
                }
            }
            
            if (j == 3) //TROCA DE CHAVE
            {
                k += 1;
                if (k < apiKeys.Count) //PARA IMPEDIR LISTA DE ESTOURAR
                {
                    j = 0;  //REINICIA O CICLO    
                }
            }
        }

        Console.WriteLine("-PLANTS-");
        foreach (PlantSummary pSum in plantSummaries)
        {
            Console.WriteLine(pSum.ScientificName + " - " + pSum.CommonName);
        }
        Console.WriteLine("-DETAILS-");
        foreach (PlantDetailsSummary pDet in plantDetailsSummaries)
        {
            Console.WriteLine(pDet.Scientific_name + " - " + pDet.IdDetail);
        }
        Console.WriteLine("-PLANTS-");
        foreach (DangerousPlantsSummary pInfo in dangerousPlantsSummaries)
        {
            Console.WriteLine(pInfo.ScientificName + " - " + pInfo.Id);
        }
        Console.WriteLine("-CULTIVATIONS-");
        foreach (CultivationSummary pCul in cultivationSummaries)
        {
            Console.WriteLine(pCul.ScientificName + " - " + pCul.Id);
        }
        
        
        
    }

    private static void GetPerenualPlantPage(int pageNumber, string perenualKey, RestClient perenualClient, List<PlantSummary> plantSummaries)
    {
        var plantRequest = new RestRequest($"species-list?key={perenualKey}&page={pageNumber}")
        {
            OnBeforeDeserialization = resp => { resp.ContentType = "application/json";}
        };
        var plantQueryResult = perenualClient.ExecuteGet(plantRequest);
        JObject jsonPlantResponse = JObject.Parse(plantQueryResult.Content);
        JArray dataArray = (JArray)jsonPlantResponse["data"];
        foreach (JObject plant in dataArray)
        {
            PlantSummary plantSummary = new PlantSummary
            {
                Id = (int)plant["id"],
                CommonName = (string)plant["common_name"],
                ScientificName = (string)plant["scientific_name"][0],
            };
            plantSummaries.Add(plantSummary);    
        }
    }
}

