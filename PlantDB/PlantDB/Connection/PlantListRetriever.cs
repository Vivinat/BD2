using System.Text.Json;
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
        
        int k = 0;      //Identador da lista
        List<string> apiKeys = new List<string>();      //Lista de chaves de API
        apiKeys.Add("sk-lSG36650d454dca195631"); 
        apiKeys.Add("sk-yUz2664cfc366009e4763");
        apiKeys.Add("sk-RH5s6650baec4ee3d5632"); 
        apiKeys.Add("sk-xpZH66509c5155e255630"); 
        apiKeys.Add("sk-bC9266436bfad30f45481"); 
        apiKeys.Add("sk-9vpf66586661475685717"); 
        apiKeys.Add("sk-hLrI665872198d9aa5720"); 
        apiKeys.Add("sk-Ppo9665878f67ab255721");
        apiKeys.Add("sk-YlgC665879bdd93345722"); 
        apiKeys.Add("sk-qYla66587a2e10e6b5723");
        apiKeys.Add("sk-yGwl665920b439d805731");
        apiKeys.Add("sk-3Wnh66592196d237e5732"); 
        apiKeys.Add("sk-oWiu6659221191ef85733");
        apiKeys.Add("sk-cSDj666078f9b26105808");
        apiKeys.Add("sk-oFeY66607937773a05809");
        apiKeys.Add("sk-F9Yn6660796c692f35810");
        apiKeys.Add("sk-cqrL666079a71fc4c5811");
        apiKeys.Add("sk-axFP666079f91ee4b5812");
        apiKeys.Add("sk-QfQE6660acf95d0ed5815");
        apiKeys.Add("sk-b5oH6660ae6423af45818");
        apiKeys.Add("sk-3FiE6660aeb8c63765817");
        
        int currentPerenualId = 0;          //Id de iteração, apenas para saber em qual planta estamos
        int pagecheckpoint = 0;             //Em qual página parou
        string scientificName;              //Cria uma lista de nomes cientificos para impedir inserções duplicadas no banco
        List<string> scientificNameVerifier = new List<string>();
        var perenualClient = new RestClient("https://perenual.com/api/");     //Cria o cliente da API
        List<PlantSummary> plantSummaries = new List<PlantSummary>();               //Listas para as 4 tabelas
        List<PlantDetailsSummary> plantDetailsSummaries = new List<PlantDetailsSummary>();
        List<DangerousPlantsSummary> dangerousPlantsSummaries = new List<DangerousPlantsSummary>();
        List<CultivationSummary> cultivationSummaries = new List<CultivationSummary>();
        
        string jsonString;      //Define e resgata o JSON que contem a página onde parei
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
            Console.WriteLine("Página " + pagecheckpoint + " do Perenual Exigida");     //Pede uma página
            JArray pageResults = GetPerenualPlantPage(pagecheckpoint, apiKeys[k], perenualClient);

            for (int i = 0; i < 30; i++) //Cada página possui 30 plantas
            {
                var plantDetailsRequest = new RestRequest($"species/details/{i+1}?key={apiKeys[k]}")
                {
                    OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
                };  //Prepara uma requisição especifica para cada uma das 30 plantas
                
                try
                {
                    
                    currentPerenualId += 1;
                    JObject plant = (JObject)pageResults[i];        //A partir de uma planta na página
                    scientificName = (string)plant["scientific_name"][0];   //Coleta seu nome cientifico
                    
                    if (scientificNameVerifier.Exists(p => p == scientificName))    //Se está na lista, já foi adicionada
                    {
                        Console.WriteLine($"Scientific name{scientificName} already on database");  
                        continue;
                    }
                    else
                    {
                        scientificNameVerifier.Add(scientificName); //Se não está na lista, precisa ser adicionada.
                    }
                    
                    PlantSummary plantSummary = new PlantSummary    //Cria um objeto para a tabela plant
                    {
                        id_plant = (int)plant["id"],
                        common_name = (string)plant["common_name"],
                        scientific_name = scientificName,
                    };
                    
                    plantSummaries.Add(plantSummary);               //Adiciona na lista
                    Console.WriteLine("Plant name: " + scientificName);
                    
                    var detailsQueryResult = perenualClient.ExecuteGet(plantDetailsRequest);
                    JObject jsonDetailsResponse = JObject.Parse(detailsQueryResult.Content);
                    
                    string growthRateString = (string)jsonDetailsResponse["growth_rate"];   //Conversão de string para enum
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
                    
                    PlantDetailsSummary plantDetailsSummary = new PlantDetailsSummary   //Criação de objeto para tabela plant_details
                    {
                        edible_fruit = (bool)jsonDetailsResponse["edible_fruit"],
                        growth_rate = growthrateToInsert,
                        invasive = (bool)jsonDetailsResponse["invasive"],
                        indoor = (bool)jsonDetailsResponse["indoor"],
                        medicinal = (bool)jsonDetailsResponse["medicinal"],
                        scientific_name = scientificName
                    };
                    plantDetailsSummaries.Add(plantDetailsSummary);

                    CultivationSummary cultivationSummary = new CultivationSummary      //Criação de objeto para tabela cultivation
                    {
                        watering = (string)jsonDetailsResponse["watering"],
                        sunlight = (string)jsonDetailsResponse["sunlight"][0],
                        scientific_name = scientificName
                    };
                    cultivationSummaries.Add(cultivationSummary);
                    
                    string careLevelString = (string)jsonDetailsResponse["care_level"]; //Conversão de string para enum
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
                    
                    DangerousPlantsSummary dangerousPlantsSummary = new DangerousPlantsSummary  //Criação de objeto para tabela dangerous_plants
                    {
                        care_level = levelToInsert,
                        scientific_name = scientificName,
                        poisonous_to_pets = (bool)jsonDetailsResponse["poisonous_to_pets"]
                    };
                    
                    dangerousPlantsSummaries.Add(dangerousPlantsSummary);
                    Console.WriteLine("Inserido planta de ID no perenual " + currentPerenualId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + " FALHA NO ITEM DE VALOR " + i + "DA PAGINA " + j + " CICLO " + k);
                    throw;
                }
            }
            
            using (var dbContext = new DBContext())     //Tenho uma batch de 30 plantas ao final do loop
            {
                DatabaseInsert databaseInsert = new DatabaseInsert(dbContext);
                databaseInsert.InsertIntoDatabase(plantSummaries,dangerousPlantsSummaries, plantDetailsSummaries, cultivationSummaries);
                Console.WriteLine("Inserted batch of page " + pagecheckpoint );
                plantSummaries.Clear();                         //Limpa as listas
                plantDetailsSummaries.Clear();
                dangerousPlantsSummaries.Clear();
                cultivationSummaries.Clear();
 
                var dict = new Dictionary<string, int> { { "page", pagecheckpoint } };
                string newPageNumber = JsonSerializer.Serialize(dict);
                File.WriteAllText(jsonPath, newPageNumber);
            }
            
            if (j == 2)                 //Se j = 2, minha chave já fez 90 requisições. Preciso trocá-la
            {
                k += 1;
                if (k < apiKeys.Count) //Impede lista de estourar
                {
                    j = -1;             //Troca de chave e reinicia 
                    Console.WriteLine("Trocando para a chave " + apiKeys[k]);
                }
            }
        }
    }
    //Devolve uma página contendo 30 plantas
    private static JArray GetPerenualPlantPage(int pageNumber, string perenualKey, RestClient perenualClient)
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

