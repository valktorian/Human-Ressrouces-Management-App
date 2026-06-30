using AccountService.Query.Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace AccountService.Query.Infrastructure;

public class ReadDbContext
{
    private readonly IMongoDatabase _database;

    public ReadDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ReadDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'ReadDatabase' is required.");
        }

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("account_read");

        try
        {
            var indexKeys = Builders<AccountReadModel>.IndexKeys.Ascending(x => x.Email);
            Accounts.Indexes.CreateOne(new CreateIndexModel<AccountReadModel>(indexKeys, new CreateIndexOptions { Unique = true }));
        }
        catch (MongoCommandException)
        {
            // Existing duplicate legacy data should not prevent the query API from starting.
        }
    }

    public IMongoCollection<AccountReadModel> Accounts =>
        _database.GetCollection<AccountReadModel>("accounts");
}
