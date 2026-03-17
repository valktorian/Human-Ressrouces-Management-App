namespace TimeService.Query.Domain;

public class HolidayReadModel
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
