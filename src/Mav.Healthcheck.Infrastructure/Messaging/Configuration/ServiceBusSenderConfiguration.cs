namespace Mav.Healthcheck.Infrastructure.Messaging.Configuration;

public record ServiceBusSenderConfiguration
{
    public required string DefaultTopicName { get; set; }
}
