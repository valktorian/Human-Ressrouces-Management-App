using MongoDB.Bson.Serialization.Attributes;

namespace TimeService.Query.Domain;

[BsonIgnoreExtraElements]
public class LeaveBalanceReadModel
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("accountId")]
    public Guid? AccountId { get; set; }

    [BsonElement("employeeId")]
    public Guid EmployeeId { get; set; }

    [BsonElement("leaveType")]
    public string LeaveType { get; set; } = string.Empty;

    [BsonElement("available")]
    public decimal Available { get; set; }

    [BsonElement("used")]
    public decimal Used { get; set; }

    [BsonElement("pending")]
    public decimal Pending { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
