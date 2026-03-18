using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TimeService.Query.Domain;

[BsonIgnoreExtraElements]
public class LeaveBalanceReadModel
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }

    [BsonElement("accountId")]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid? AccountId { get; set; }

    [BsonElement("employeeId")]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
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
