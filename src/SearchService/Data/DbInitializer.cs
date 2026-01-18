using MongoDB.Bson;
using MongoDB.Driver;
using SearchService.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            // this is basically doing
            // var client = new MongoClient("<connection string>");
            //var myDB = mongoClient.GetDatabase("test_db");
            var mongoConn = app.Configuration.GetConnectionString("MongoDBConnectionString");
            var mongoSettings = MongoClientSettings.FromConnectionString(mongoConn);
            var mongoClient = new MongoClient(mongoSettings);
            var mongoDatabase = mongoClient.GetDatabase("SearchDB");

            // Ensure collection exists and create text index on Make, Model, Color
            var itemCollection = mongoDatabase.GetCollection<Item>("Items");

            // Build a combined text index for the three fields
            var indexKeys = Builders<Item>.IndexKeys
                .Text(i => i.Make)
                .Text(i => i.Model)
                .Text(i => i.Color);

            var indexModel = new CreateIndexModel<Item>(indexKeys);
            await itemCollection.Indexes.CreateOneAsync(indexModel);
            Console.WriteLine($"mongo connection is DONE, {mongoDatabase.ListCollectionNames()}");


            var count = await itemCollection.CountDocumentsAsync(FilterDefinition<Item>.Empty);

            if (count == 0)
            {
                Console.WriteLine("No Data, seeding data ");
                var itemData = await File.ReadAllTextAsync("Data/auctions.json");
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                // convert jsopn to dotnet based data
                var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);
                //var items = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<Item>>(itemData);

                await itemCollection.InsertManyAsync(items);

                Console.WriteLine("Data Seeded");
            }
            else
            {
                Console.WriteLine("Data already exists, no seeding needed");
            }

        }
    }
}
