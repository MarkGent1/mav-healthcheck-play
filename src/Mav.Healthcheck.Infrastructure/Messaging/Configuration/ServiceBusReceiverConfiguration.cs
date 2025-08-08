namespace Mav.Healthcheck.Infrastructure.Messaging.Configuration;

public record ServiceBusReceiverConfiguration
{
    public required string DefaultQueueName { get; set; }
}
