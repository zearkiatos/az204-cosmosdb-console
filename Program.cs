using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Configuration;
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
            details = itemId  // Add the details property that matches the partition key path
        };

        var response = await container.CreateItemAsync(item, new PartitionKey(item.details));
        Console.WriteLine($"Item created with id: {response.Resource.id}");
    }
}