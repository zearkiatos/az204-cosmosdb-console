using DotNetEnv;

namespace Configuration
{
    public static class AppConfiguration
    {
        static AppConfiguration()
        {
            // Load .env file when the class is first accessed
            Env.Load();
        }

        public static string CosmosDbBaseEndpointUrl => 
            Environment.GetEnvironmentVariable("COSMOSDB_BASE_ENDPOINT_URL") 
            ?? throw new InvalidOperationException("COSMOSDB_BASE_ENDPOINT_URL environment variable is not set");

        public static string CosmosDbPrimaryKey => 
            Environment.GetEnvironmentVariable("COSMOSDB_PRIMARY_KEY") 
            ?? throw new InvalidOperationException("COSMOSDB_PRIMARY_KEY environment variable is not set");

        public static string CosmosDbDatabaseId => 
            Environment.GetEnvironmentVariable("COSMOSDB_DATABASE_ID") 
            ?? throw new InvalidOperationException("COSMOSDB_DATABASE_ID environment variable is not set");

        public static string CosmosDbContainerId => 
            Environment.GetEnvironmentVariable("COSMOSDB_CONTAINER_ID") 
            ?? throw new InvalidOperationException("COSMOSDB_CONTAINER_ID environment variable is not set");


        // Optional: Validate that all required configuration is present
        public static void ValidateConfiguration()
        {
            try
            {
                _ = CosmosDbBaseEndpointUrl;
                _ = CosmosDbPrimaryKey;
                _ = CosmosDbDatabaseId;
                _ = CosmosDbContainerId;
                Console.WriteLine("✓ Configuration validation passed");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"✗ Configuration validation failed: {ex.Message}");
                throw;
            }
        }
    }
}