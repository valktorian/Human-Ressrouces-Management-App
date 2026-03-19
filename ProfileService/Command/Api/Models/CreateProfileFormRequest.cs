namespace ProfileService.Command.Api.Models;

public class CreateProfileFormRequest
{
    public string? Payload { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}
