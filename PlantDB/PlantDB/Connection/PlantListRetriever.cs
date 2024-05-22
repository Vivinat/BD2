using RestSharp;
using PlantDB.Types.Filtered;

namespace PlantDB.Connection;

using Newtonsoft.Json.Linq;

public static class PlantListRetriever
{
    public static void GetPlantList()
    {
        bool apiRunOut = false;
        int apiSavepoint = 0;
        string perenualApiKey = "sk-yUz2664cfc366009e4763";
        string trefleApiKey = "8W7mG7-djVtqTNKVd7m88q3MgJJai0w5W9Cy19CZ8Ig";
        var perenualClient = new RestClient("https://perenual.com/api/");
        var trefleClient = new RestClient("https://trefle.io/api/v1/");

        List<PlantSummary> plantSummaries = new List<PlantSummary>();
        List<PlantDetailsSummary> plantDetailsSummaries = new List<PlantDetailsSummary>();
        List<InformationsSummary> informationsSummaries = new List<InformationsSummary>();
        List<CultivationSummary> cultivationSummaries = new List<CultivationSummary>();

        for (int j = 0; j < 1; j++)
        {
            plantSummaries = new List<PlantSummary>();
            GetPerenualPlantPage(j + 1, perenualApiKey, perenualClient, plantSummaries);

            for (int i = apiSavepoint; i < plantSummaries.Count; i++) 
            {

                var plantDetailsRequest = new RestRequest($"species/details/{i + 1}?key={perenualApiKey}")
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
                        Flowers = (bool)jsonDetailsResponse["flowers"],
                        Cones = (bool)jsonDetailsResponse["cones"],
                        Fruits = (bool)jsonDetailsResponse["fruits"],
                        Edible_Fruit = (bool)jsonDetailsResponse["edible_fruit"],
                        Leaf = (bool)jsonDetailsResponse["leaf"],
                        Medicinal = (bool)jsonDetailsResponse["medicinal"],
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

                    string plantSciName = plantSummaries[i].ScientificName;
                    plantSciName = plantSciName.Trim().Replace(" ", "%20");
                    var plantTrefleRequest =
                        new RestRequest($"plants?token={trefleApiKey}&filter[scientific_name]={plantSciName}")
                        {
                            OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
                        };
                    try
                    {
                        var trefleQueryResult = trefleClient.ExecuteGet(plantTrefleRequest);
                        JObject jsonTrefleResponse = JObject.Parse(trefleQueryResult.Content);

                        if (jsonTrefleResponse.TryGetValue("data", out JToken dataToken) && dataToken.HasValues)
                        {
                            JObject dataObject = (JObject)dataToken.First;

                            InformationsSummary informationsSummary = new InformationsSummary
                            {
                                Id = plantSummaries[i].Id,
                                Slug = (string)dataObject["slug"] ?? string.Empty,
                                Year = dataObject["year"] != null ? (int)dataObject["year"] : 0,
                                Bibliography = (string)dataObject["bibliography"] ?? string.Empty,
                                Author = (string)dataObject["author"] ?? string.Empty,
                                Status = (string)dataObject["status"] ?? string.Empty,
                                Rank = (string)dataObject["rank"] ?? string.Empty,
                                Family_Common_Name = (string)dataObject["family_common_name"] ?? string.Empty,
                                Genus = (string)dataObject["genus"] ?? string.Empty,
                                Family = (string)dataObject["family"] ?? string.Empty,
                                ScientificName = plantSummaries[i].ScientificName,
                            };
                            informationsSummaries.Add(informationsSummary);
                        }
                        else
                        {
                            plantSciName = plantSciName.Replace("%20", " ");
                            Console.WriteLine("No data found for " + plantSciName +
                                              " in Trefle. Removing it from other lists");
                            PlantSummary plantToRemove = plantSummaries.Find(p => p.ScientificName == plantSciName);
                            PlantDetailsSummary plantDetailsToRemove =
                                plantDetailsSummaries.Find(p => p.Scientific_name == plantSciName);
                            CultivationSummary plantCultivationToRemove =
                                cultivationSummaries.Find(p => p.ScientificName == plantSciName);
                            plantSummaries.Remove(plantToRemove);
                            plantDetailsSummaries.Remove(plantDetailsToRemove);
                            cultivationSummaries.Remove(plantCultivationToRemove);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e + "Trefle FALHOU NO NOME " + plantSciName);
                        throw;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Perenual FALHOU NO I DE VALOR " + i);
                    Console.WriteLine(i);
                    throw;
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
        foreach (InformationsSummary pInfo in informationsSummaries)
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

