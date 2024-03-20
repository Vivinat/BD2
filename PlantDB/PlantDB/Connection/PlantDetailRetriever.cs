using RestSharp;

namespace PlantDB.Connection;

public class PlantDetailRetriever
{
    public void RetrievePlantDetails(int plantId)
    {
        var client = new RestClient("https://perenual.com/api/");
        var request = new RestRequest($"https://perenual.com/api/species/details/{plantId}?key=sk-oldE65f73f5451cc54763")
        {
            OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
        };
        var queryResult = client.ExecuteGet(request);
    }
}