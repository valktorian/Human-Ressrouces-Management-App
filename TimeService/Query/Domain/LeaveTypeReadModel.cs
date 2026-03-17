namespace TimeService.Query.Domain;

public class LeaveTypeReadModel
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
}
