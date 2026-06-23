namespace WorkForceHub.Gateway.Configuration;

public class DownstreamOptions
{
    public const string Section = "Downstream";

    public ServiceEndpoint AccountCommand { get; set; } = new();
    public ServiceEndpoint AccountQuery { get; set; } = new();
    public ServiceEndpoint ProfileCommand { get; set; } = new();
    public ServiceEndpoint ProfileQuery { get; set; } = new();
    public ServiceEndpoint TimeCommand { get; set; } = new();
    public ServiceEndpoint TimeQuery { get; set; } = new();
    public ServiceEndpoint EvolutionCommand { get; set; } = new();
    public ServiceEndpoint EvolutionQuery { get; set; } = new();
    public ServiceEndpoint Media { get; set; } = new();
}
