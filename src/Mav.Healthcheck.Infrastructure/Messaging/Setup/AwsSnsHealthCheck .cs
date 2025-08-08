using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Mav.Healthcheck.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mav.Healthcheck.Infrastructure.Messaging.Setup;

public class AwsSnsHealthCheck(IAmazonSimpleNotificationService snsClient, ServiceBusSenderConfiguration config) : IHealthCheck
{
    private readonly IAmazonSimpleNotificationService _snsClient = snsClient ?? throw new ArgumentNullException(nameof(snsClient));
    private readonly ServiceBusSenderConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var listResponse = await _snsClient.ListTopicsAsync(cancellationToken);

            if (listResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return HealthCheckResult.Degraded("SNS ListTopics returned non-OK status.");
            }

            var topic = listResponse.Topics
                .FirstOrDefault(t => t.TopicArn.EndsWith(_config.DefaultTopicName, StringComparison.InvariantCultureIgnoreCase));

            if (topic == null || string.IsNullOrWhiteSpace(topic.TopicArn))
            {
                return HealthCheckResult.Unhealthy($"SNS topic '{_config.DefaultTopicName}' not found.");
            }

            var attrResponse = await _snsClient.GetTopicAttributesAsync(topic.TopicArn, cancellationToken);

            if (attrResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return HealthCheckResult.Degraded($"SNS topic '{_config.DefaultTopicName}' attributes fetch returned non-OK status.");
            }

            return HealthCheckResult.Healthy($"SNS topic '{_config.DefaultTopicName}' is reachable.", new Dictionary<string, object>
            {
                ["TopicArn"] = topic.TopicArn
            });
        }
        catch (NotFoundException ex)
        {
            return HealthCheckResult.Unhealthy($"SNS topic '{_config.DefaultTopicName}' does not exist.", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Error accessing SNS topic '{_config.DefaultTopicName}'.", ex);
        }
    }
}
