using RestSharp;
using System;

namespace PlantDB.Connection;

public static class PlantListRetriever
{
    public static void GetPlantList()
    {
        var client = new RestClient("https://perenual.com/api/");
        var request = new RestRequest("species-list?key=sk-oldE65f73f5451cc54763&page=1")
            {
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };
        var queryResult = client.ExecuteGet(request);
        System.Console.WriteLine(queryResult.Content);    
    }
}