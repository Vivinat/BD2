using System;
using PlantDB.Connection;

namespace PlantDB // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PlantListRetriever.GetPlantList();  //Por PlantListRetriever ser static, pode-se chama-la sem instanciar
        } 
    }
}