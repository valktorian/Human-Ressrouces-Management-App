using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TimeService.Query.Domain;

[BsonIgnoreExtraElements]
public class LeaveRequestReadModel
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

    [BsonElement("startDate")]
    public DateTime StartDate { get; set; }

    [BsonElement("endDate")]
    public DateTime EndDate { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("reason")]
    public string? Reason { get; set; }

    [BsonElement("submittedAt")]
    public DateTime? SubmittedAt { get; set; }

    [BsonElement("decisionAt")]
    public DateTime? DecisionAt { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
