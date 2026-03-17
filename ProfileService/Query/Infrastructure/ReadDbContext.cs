using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ProfileService.Query.Domain;

namespace ProfileService.Query.Infrastructure;

public class ReadDbContext
{
    private readonly IMongoDatabase _database;

    public ReadDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ReadDatabase")
            ?? "mongodb://root:root@localhost:27017/admin?authSource=admin";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("profile_read");

        try
        {
            Profiles.Indexes.CreateOne(new CreateIndexModel<ProfileReadModel>(Builders<ProfileReadModel>.IndexKeys.Ascending(x => x.EmployeeNumber), new CreateIndexOptions { Unique = true }));
            Profiles.Indexes.CreateOne(new CreateIndexModel<ProfileReadModel>(Builders<ProfileReadModel>.IndexKeys.Ascending(x => x.WorkEmail), new CreateIndexOptions { Unique = true }));
            Profiles.Indexes.CreateOne(new CreateIndexModel<ProfileReadModel>(Builders<ProfileReadModel>.IndexKeys.Ascending(x => x.AccountId), new CreateIndexOptions { Unique = true, Sparse = true }));
        }
        catch (MongoCommandException)
        {
        }
    }

    public IMongoCollection<ProfileReadModel> Profiles => _database.GetCollection<ProfileReadModel>("profiles");
}
