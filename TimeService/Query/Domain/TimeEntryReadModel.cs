using MongoDB.Bson.Serialization.Attributes;

namespace TimeService.Query.Domain;

[BsonIgnoreExtraElements]
public class TimeEntryReadModel
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("accountId")]
    public Guid? AccountId { get; set; }

    [BsonElement("employeeId")]
    public Guid EmployeeId { get; set; }

    [BsonElement("workDate")]
    public DateTime WorkDate { get; set; }

    [BsonElement("startTime")]
    public string StartTime { get; set; } = string.Empty;

    [BsonElement("endTime")]
    public string EndTime { get; set; } = string.Empty;

    [BsonElement("hours")]
    public decimal Hours { get; set; }

    [BsonElement("projectCode")]
    public string ProjectCode { get; set; } = string.Empty;

    [BsonElement("taskCode")]
    public string TaskCode { get; set; } = string.Empty;

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
