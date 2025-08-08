using Amazon.SimpleNotificationService;
using Mav.Healthcheck.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mav.Healthcheck.Infrastructure.Messaging.Setup;

public class AwsSnsHealthCheck(IAmazonSimpleNotificationService snsClient,
    ServiceBusSenderConfiguration serviceBusSenderConfiguration) : IHealthCheck
{
    private readonly IAmazonSimpleNotificationService _snsClient = snsClient ?? throw new ArgumentNullException(nameof(snsClient));
    private readonly ServiceBusSenderConfiguration _serviceBusSenderConfiguration = serviceBusSenderConfiguration ?? throw new ArgumentNullException(nameof(serviceBusSenderConfiguration));

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var allTopics = await _snsClient.ListTopicsAsync(cancellationToken);
            var topic = allTopics.Topics.FirstOrDefault(x => x.TopicArn.EndsWith(_serviceBusSenderConfiguration.DefaultTopicName, StringComparison.InvariantCultureIgnoreCase));

            if (topic != null)
            {
                var topicResponse = await _snsClient.GetTopicAttributesAsync(topic.TopicArn, cancellationToken);

                if (topicResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return HealthCheckResult.Healthy("SNS is reachable.");
                }

                return HealthCheckResult.Degraded("SNS returned non-OK status.");
            }

            throw new Exception("SNS topic not found");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Error accessing SNS.", ex);
        }
    }
}
