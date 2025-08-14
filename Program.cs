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
            // Inicializar el cliente de Cosmos DB.
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

            // Crear la base de datos y el contenedor.
            await CreateDatabaseAsync();
            await CreateContainerAsync();

            // Crear un documento.
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
        Console.WriteLine($"Base de datos '{database.Id}' creada.");
    }

    private static async Task CreateContainerAsync()
    {
        // Crear un nuevo contenedor en la base de datos si no existe.
        container = await database.CreateContainerIfNotExistsAsync(containerId, "/id");
        Console.WriteLine($"Container '{container.Id}' creado.");
    }

    private static async Task AddItemsToContainerAsync()
    {
        // Crear un documento.
        var item = new
        {
            id = Guid.NewGuid().ToString(),
            nomprod = "techo"
        };

        // Insertar el documento en el contenedor.
        var response = await container.CreateItemAsync(item, new PartitionKey(item.id));
        Console.WriteLine($"Item creado con id: {response.Resource.id}");
    }
}
}