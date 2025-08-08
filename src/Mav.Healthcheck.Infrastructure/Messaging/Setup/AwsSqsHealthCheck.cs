using Amazon.SQS;
using Amazon.SQS.Model;
using Mav.Healthcheck.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mav.Healthcheck.Infrastructure.Messaging.Setup;

public class AwsSqsHealthCheck(IAmazonSQS sqsClient, ServiceBusReceiverConfiguration config) : IHealthCheck
{
    private readonly IAmazonSQS _sqsClient = sqsClient ?? throw new ArgumentNullException(nameof(sqsClient));
    private readonly ServiceBusReceiverConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queueResponse = await _sqsClient.GetQueueUrlAsync(_config.DefaultQueueName, cancellationToken);

            if (queueResponse.HttpStatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrWhiteSpace(queueResponse.QueueUrl))
            {
                return HealthCheckResult.Degraded($"SQS queue '{_config.DefaultQueueName}' returned non-OK status or empty URL.");
            }

            var attributesResponse = await _sqsClient.GetQueueAttributesAsync(new GetQueueAttributesRequest
            {
                QueueUrl = queueResponse.QueueUrl,
                AttributeNames = [QueueAttributeName.QueueArn]
            }, cancellationToken);

            if (attributesResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return HealthCheckResult.Degraded($"SQS queue '{_config.DefaultQueueName}' attributes fetch returned non-OK status.");
            }

            var queueArn = attributesResponse.Attributes.GetValueOrDefault(QueueAttributeName.QueueArn);
            if (string.IsNullOrWhiteSpace(queueArn))
            {
                return HealthCheckResult.Degraded($"SQS queue '{_config.DefaultQueueName}' is missing ARN attribute.");
            }

            return HealthCheckResult.Healthy($"SQS queue '{_config.DefaultQueueName}' is reachable. ARN: {queueArn}");
        }
        catch (QueueDoesNotExistException ex)
        {
            return HealthCheckResult.Unhealthy($"SQS queue '{_config.DefaultQueueName}' does not exist.", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Error accessing SQS queue '{_config.DefaultQueueName}'.", ex);
        }
    }
}
