using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TimeService.Query.Domain;

[BsonIgnoreExtraElements]
public class TimesheetReadModel
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

    [BsonElement("periodStart")]
    public DateTime PeriodStart { get; set; }

    [BsonElement("periodEnd")]
    public DateTime PeriodEnd { get; set; }

    [BsonElement("totalHours")]
    public decimal TotalHours { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("submittedAt")]
    public DateTime? SubmittedAt { get; set; }

    [BsonElement("approvedAt")]
    public DateTime? ApprovedAt { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
