using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TimeService.Query.Domain;

namespace TimeService.Query.Infrastructure;

public class ReadDbContext
{
    private readonly IMongoDatabase _database;

    public ReadDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ReadDatabase")
            ?? "mongodb://root:root@localhost:27017/admin?authSource=admin";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("time_read");

        CreateIndexes();
    }

    public IMongoCollection<TimeEntryReadModel> TimeEntries => _database.GetCollection<TimeEntryReadModel>("timeEntries");
    public IMongoCollection<TimesheetReadModel> Timesheets => _database.GetCollection<TimesheetReadModel>("timesheets");
    public IMongoCollection<LeaveRequestReadModel> LeaveRequests => _database.GetCollection<LeaveRequestReadModel>("leaveRequests");
    public IMongoCollection<LeaveBalanceReadModel> LeaveBalances => _database.GetCollection<LeaveBalanceReadModel>("leaveBalances");

    private void CreateIndexes()
    {
        try
        {
            TimeEntries.Indexes.CreateMany(
            [
                new CreateIndexModel<TimeEntryReadModel>(Builders<TimeEntryReadModel>.IndexKeys.Ascending(x => x.EmployeeId)),
                new CreateIndexModel<TimeEntryReadModel>(Builders<TimeEntryReadModel>.IndexKeys.Ascending(x => x.AccountId)),
                new CreateIndexModel<TimeEntryReadModel>(Builders<TimeEntryReadModel>.IndexKeys.Descending(x => x.WorkDate))
            ]);

            Timesheets.Indexes.CreateMany(
            [
                new CreateIndexModel<TimesheetReadModel>(Builders<TimesheetReadModel>.IndexKeys.Ascending(x => x.EmployeeId)),
                new CreateIndexModel<TimesheetReadModel>(Builders<TimesheetReadModel>.IndexKeys.Ascending(x => x.AccountId)),
                new CreateIndexModel<TimesheetReadModel>(Builders<TimesheetReadModel>.IndexKeys.Ascending(x => x.Status))
            ]);

            LeaveRequests.Indexes.CreateMany(
            [
                new CreateIndexModel<LeaveRequestReadModel>(Builders<LeaveRequestReadModel>.IndexKeys.Ascending(x => x.EmployeeId)),
                new CreateIndexModel<LeaveRequestReadModel>(Builders<LeaveRequestReadModel>.IndexKeys.Ascending(x => x.AccountId)),
                new CreateIndexModel<LeaveRequestReadModel>(Builders<LeaveRequestReadModel>.IndexKeys.Ascending(x => x.Status))
            ]);

            LeaveBalances.Indexes.CreateMany(
            [
                new CreateIndexModel<LeaveBalanceReadModel>(Builders<LeaveBalanceReadModel>.IndexKeys.Ascending(x => x.EmployeeId)),
                new CreateIndexModel<LeaveBalanceReadModel>(Builders<LeaveBalanceReadModel>.IndexKeys.Ascending(x => x.AccountId)),
                new CreateIndexModel<LeaveBalanceReadModel>(
                    Builders<LeaveBalanceReadModel>.IndexKeys
                        .Ascending(x => x.EmployeeId)
                        .Ascending(x => x.LeaveType),
                    new CreateIndexOptions { Unique = true })
            ]);
        }
        catch (MongoCommandException)
        {
        }
    }
}
