# MAV HealthCheck Playground

## 1. Introduction

### 1.1 Purpose of this Repository

The intention for this repository is to act as a personal 'Playground' to demonstrate how to use the .Net HealthCheck features.

Health checks have been added for:
 - MongoDb
 - AWS SNS
 - AWS SQS
 - AWS S3

The `AWS S3` implementation uses a factory to allow for multiple clients to be registered by name.

**Note**. Please run the API using the Docker Compose configuration which is set up to run using `localstack` in the container to test AWS connectivity. A MongoDb instance should also be available (in this example it is set up assuming an instance running on the local development machine).

## 2. Examples

### 2.1. Successful response

```
{
  "status": "Healthy",
  "results": {
    "mongodb": {
      "status": "Healthy",
      "description": "MongoDB is reachable.",
      "data": {}
    },
    "aws_sns": {
      "status": "Healthy",
      "description": "SNS topic \u0027mav-dev-animal-events\u0027 is reachable.",
      "data": {
        "TopicArn": "arn:aws:sns:eu-north-1:000000000000:mav-dev-animal-events"
      }
    },
    "aws_sqs": {
      "status": "Healthy",
      "description": "SQS queue \u0027mav-dev-animals\u0027 is reachable. ARN: arn:aws:sqs:eu-north-1:000000000000:mav-dev-animals",
      "data": {}
    },
    "aws_s3": {
      "status": "Healthy",
      "description": "All S3 buckets are reachable",
      "data": {
        "ClientA": {
          "Bucket": "mav-dev-bucket",
          "Status": "Healthy",
          "Owner": "webfile"
        },
        "ClientB": {
          "Bucket": "mav-external-dev-bucket",
          "Status": "Healthy",
          "Owner": "webfile"
        }
      }
    }
  }
}
```

### 2.2. Unsuccessful response

```
{
  "status": "Unhealthy",
  "results": {
    "mongodb": {
      "status": "Healthy",
      "description": "MongoDB is reachable.",
      "data": {}
    },
    "aws_sns": {
      "status": "Unhealthy",
      "description": "SNS topic \u0027mav-dev-animal-eventsX\u0027 not found.",
      "data": {}
    },
    "aws_sqs": {
      "status": "Unhealthy",
      "description": "SQS queue \u0027mav-dev-animalsX\u0027 does not exist.",
      "data": {}
    },
    "aws_s3": {
      "status": "Unhealthy",
      "description": "Some S3 buckets failed: ClientA, ClientB",
      "data": {
        "ClientA": {
          "Bucket": "mav-dev-bucketX",
          "Status": "Unhealthy (Bucket not found)",
          "Exception": "The specified bucket does not exist"
        },
        "ClientB": {
          "Bucket": "mav-external-dev-bucketX",
          "Status": "Unhealthy (Exception)",
          "Exception": "Connection refused (localstack:4567)"
        }
      }
    }
  }
}
```