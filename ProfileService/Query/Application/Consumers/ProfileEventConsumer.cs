using Infrastructure.Api.Messaging;
using Infrastructure.Api.Mapping;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ProfileService.Query.Domain;
using ProfileService.Query.Infrastructure;
using System.Text.Json;

namespace ProfileService.Query.Application.Consumers;

public class ProfileEventConsumer : IEventHandler
{
    private readonly ReadDbContext _readDb;
    private readonly ILogger<ProfileEventConsumer> _logger;

    public string EventType => "ProfileService.Command.Domain.Events.ProfileCreatedEvent";

    public ProfileEventConsumer(ReadDbContext readDb, ILogger<ProfileEventConsumer> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    public async Task HandleAsync(JsonElement payload)
    {
        if (payload.TryGetProperty("DeletedAt", out _))
        {
            await HandleDeletedAsync(payload);
            return;
        }

        await UpsertAsync(payload);
    }

    private async Task UpsertAsync(JsonElement payload)
    {
        try
        {
            var model = JsonElementMapper.Map<ProfileReadModel>(payload, static (source, readModel) =>
            {
                readModel.Id = source.GetRequiredGuid("ProfileId");
            });

            await _readDb.Profiles.ReplaceOneAsync(x => x.Id == model.Id, model, new ReplaceOptions { IsUpsert = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling profile upsert event.");
        }
    }

    private async Task HandleDeletedAsync(JsonElement payload)
    {
        try
        {
            var profileId = payload.GetProperty("ProfileId").GetGuid();
            await _readDb.Profiles.DeleteOneAsync(x => x.Id == profileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling profile delete event.");
        }
    }
}
