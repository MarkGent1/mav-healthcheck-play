using Amazon.SQS;
using Amazon.SQS.Model;
using Mav.Healthcheck.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mav.Healthcheck.Infrastructure.Messaging.Setup;

public class AwsSqsHealthCheck(IAmazonSQS sqsClient,
    ServiceBusReceiverConfiguration serviceBusReceiverConfiguration) : IHealthCheck
{
    private readonly IAmazonSQS _sqsClient = sqsClient ?? throw new ArgumentNullException(nameof(sqsClient));
    private readonly ServiceBusReceiverConfiguration _serviceBusReceiverConfiguration = serviceBusReceiverConfiguration ?? throw new ArgumentNullException(nameof(serviceBusReceiverConfiguration));

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queueResponse = await _sqsClient.GetQueueUrlAsync(_serviceBusReceiverConfiguration.DefaultQueueName, cancellationToken);
            var queueUrl = queueResponse.QueueUrl;

            var queueAttributesResponse = await _sqsClient.GetQueueAttributesAsync(new GetQueueAttributesRequest
            {
                QueueUrl = queueUrl,
                AttributeNames = ["QueueArn"]
            }, cancellationToken);

            if (queueResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return HealthCheckResult.Healthy("SQS is reachable.");
            }

            return HealthCheckResult.Degraded("SQS returned non-OK status.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Error accessing SQS.", ex);
        }
    }
}
