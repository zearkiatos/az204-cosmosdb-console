using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.Cosmos;
using Configuration;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
public static class Program
{
    private static readonly string EndpointUri = AppConfiguration.CosmosDbBaseEndpointUrl;
    private static readonly string PrimaryKey = AppConfiguration.CosmosDbPrimaryKey;
    private static CosmosClient cosmosClient = null!;
    private static Database database = null!;
    private static Container container = null!;
    private static readonly string databaseId = AppConfiguration.CosmosDbDatabaseId;
    private static readonly string containerId = AppConfiguration.CosmosDbContainerId;

    static async Task Main(string[] args)
    {
        try
        {
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

            await CreateDatabaseAsync();
            await CreateContainerAsync();

            await AddItemsToContainerAsync();

            await AddItemsToContainerFromJsonAsync("data.json");
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Cosmos DB Error: {ex.Message}");
        }
        finally
        {
            if (cosmosClient != null)
            {
                cosmosClient.Dispose();
            }
        }
    }

    private static async Task CreateDatabaseAsync()
    {
        // Crear una nueva base de datos si no existe.
        database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        Console.WriteLine($"Database '{database.Id}' created.");
    }

    private static async Task CreateContainerAsync()
    {
        container = await database.CreateContainerIfNotExistsAsync(containerId, "/details");
        Console.WriteLine($"Container '{container.Id}' created.");
    }

    private static async Task AddItemsToContainerAsync()
    {
        var itemId = Guid.NewGuid().ToString();
        var item = new
        {
            id = itemId,
            product_name = "roof",
            details = itemId
        };

        var response = await container.CreateItemAsync(item, new PartitionKey(item.details));
        Console.WriteLine($"Item created with id: {response.Resource.id}");
    }

    private static async Task AddItemsToContainerFromJsonAsync(string jsonFilePath)
    {
        await LoadJsonAsync(jsonFilePath);
        Console.WriteLine("Upload Json completed.");
    }

    private static async Task LoadJsonAsync(string jsonFilePath)
    {
        string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
        JArray jsonArray = JArray.Parse(jsonContent);

        foreach (JObject jsonObject in jsonArray)
        {
            await container.CreateItemAsync(jsonObject, new PartitionKey(jsonObject["details"].ToString()));
            Console.WriteLine($"Object details inserted with id {jsonObject["id"]}");
        }
    }
}