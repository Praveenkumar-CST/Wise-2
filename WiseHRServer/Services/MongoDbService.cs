using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;

namespace WiseHRServer.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            try
            {
                var connectionString = configuration["MongoDB:ConnectionString"];
                var databaseName = configuration["MongoDB:DatabaseName"];
                Console.WriteLine($"MongoDB Connection String: {connectionString}");
                Console.WriteLine($"MongoDB Database Name: {databaseName}");

                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ConnectTimeout = TimeSpan.FromSeconds(60); // Increase timeout to 60 seconds
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(60);
                var client = new MongoClient(settings);
                _database = client.GetDatabase(databaseName);

                // Test the connection
                var databases = client.ListDatabaseNames().ToList();
                Console.WriteLine("Connected to MongoDB. Databases: " + string.Join(", ", databases));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to MongoDB: " + ex.Message);
                throw;
            }
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}