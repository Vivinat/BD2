using System.Text.Json;
using Newtonsoft.Json.Linq;
using PlantDB.Context;
using RestSharp;

namespace PlantDB.Connection;

public static class Corrector  //Corrector foi criado para corrigir dados faltantes/nulos a partir de pesquisas em endpoints especificos da API
{
    public static void CorrectColumns()
    {
        var perenualClient = new RestClient("https://perenual.com/api/");       //Criação do cliente
        List<string> apiKeys = new List<string>();      //Lista de chaves
        apiKeys.Add("sk-gtjB665eff981f0055796"); 
        apiKeys.Add("sk-gKF2665f00044ef195797");
        apiKeys.Add("sk-jCm6665e566e258225787"); 
        apiKeys.Add("sk-WQN16660e436953f65821");

        int k = 0;              //Identador para lista de chaves
        int pagecheckpoint = 0; //Em qual página parou
        string scientificName;  
        string jsonString;      //JSON que guarda a página
        string jsonPath =
            "C:\\Users\\Particular\\Documents\\GitHub\\BD2\\PlantDB\\PlantDB\\Connection\\pageCorrector.json";
        using (StreamReader reader = new StreamReader(jsonPath))
        {
            jsonString = reader.ReadToEnd();
        }
        JsonDocument doc = JsonDocument.Parse(jsonString);
        if (doc.RootElement.TryGetProperty("page", out JsonElement element))        //Validando número da página obtido do JSON
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

        for (int j = 0; j < 1; j++)             //Roda apenas uma vez
        {
            for (int i = 0; i < 100; i++)       //Com uma key, consigo fazer 100 requisições
            {
                JArray pageResults = GetPerenualEdiblePage(pagecheckpoint, apiKeys[k], perenualClient);
                foreach (JObject plant in pageResults)      //Percorro a lista de tipo JSON Object e extraio as informações
                {
                    scientificName = (string)plant["scientific_name"][0];   //Pego o nome cientifico para fazer o update
                    using (var dbContext = new DBContext())     
                    {
                        var plantToChange = dbContext.plant_details.FirstOrDefault(p => p.scientific_name == scientificName);
                        if (plantToChange != null)
                        {
                            plantToChange.edible_fruit = true;          //Troco o dado que me conver
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
            if (k < apiKeys.Count) //Impede lista de chaves de estourar
            {
                j = -1;             //Reinicia o ciclo
                Console.WriteLine("Trocando para a chave " + apiKeys[k]);
            }
        }


    }
    
    //Devolve uma página contendo 30 plantas. Dados retornam como uma JSON array
    private static JArray GetPerenualEdiblePage(int pageNumber, string perenualKey, RestClient perenualClient)
    {
        var plantRequest = new RestRequest($"species-list?key={perenualKey}&edible=1&page={pageNumber}")    //Ultimo endpoint usado foi para corrigir atributos faltantes na coluna indoor
        {
            OnBeforeDeserialization = resp => { resp.ContentType = "application/json";}
        };
        var plantQueryResult = perenualClient.ExecuteGet(plantRequest);
        JObject jsonPlantResponse = JObject.Parse(plantQueryResult.Content);
        JArray dataArray = (JArray)jsonPlantResponse["data"];
        return dataArray;
    }
    
}