#!/bin/bash
export AWS_REGION=eu-north-1
export AWS_DEFAULT_REGION=eu-north-1
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test

set -e

echo "Bootstrapping SNS/SQS setup..."

# Create SNS Topics
topic_arn=$(awslocal sns create-topic \
  --name mav-dev-animal-events \
  --output text \
  --query 'TopicArn')

echo "SNS Topic created: $topic_arn"

# Create SQS Queue
queue_url=$(awslocal sqs create-queue \
  --queue-name mav-dev-animals \
  --output text \
  --query 'QueueUrl')

echo "SQS Queue created: $queue_url"

# Get the SQS Queue ARN
queue_arn=$(awslocal sqs get-queue-attributes \
  --queue-url "$queue_url" \
  --attribute-name QueueArn \
  --output text \
  --query 'Attributes.QueueArn')

echo "SQS Queue ARN: $queue_arn"

# Construct the policy JSON inline with escaped quotes
policy_json=$(cat <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": "*",
      "Action": "sqs:SendMessage",
      "Resource": "$queue_arn",
      "Condition": {
        "ArnEquals": {
          "aws:SourceArn": "$topic_arn"
        }
      }
    }
  ]
}
EOF
)

# Set SQS policy
awslocal sqs set-queue-attributes \
  --queue-url "$queue_url" \
  --attributes "{\"Policy\": \"$(
    echo "$policy_json" | jq -c
  )\"}"

# Subscribe the Queue to the Topic
awslocal sns subscribe \
  --topic-arn "$topic_arn" \
  --protocol sqs \
  --notification-endpoint "$queue_arn"

echo "Subscription complete."

echo "Waiting for SNS to be ready..."
awslocal sns list-topics

echo "Bootstrapping S3 setup..."

# Check if bucket already exists
existing_bucket=$(awslocal s3api list-buckets \
  --query "Buckets[?Name=='mav-dev-bucket'].Name" \
  --output text)

if [ "$existing_bucket" == "mav-dev-bucket" ]; then
  echo "S3 bucket already exists: mav-dev-bucket"
else
  awslocal s3api create-bucket --bucket mav-dev-bucket --region eu-north-1 \
    --create-bucket-configuration LocationConstraint=eu-north-1
  echo "S3 bucket created: mav-dev-bucket"
fi

# Optional: Add a test object
echo "Adding test object to S3 bucket..."
echo "Hello from LocalStack!" > /tmp/test-object.txt
awslocal s3 cp /tmp/test-object.txt s3://mav-dev-bucket/test-object.txt

# Optional: List contents
echo "Listing contents of mav-dev-bucket:"
awslocal s3 ls s3://mav-dev-bucket/

# Check if external bucket already exists
existing_bucket=$(awslocal s3api list-buckets \
  --query "Buckets[?Name=='mav-external-dev-bucket'].Name" \
  --output text)

if [ "$existing_bucket" == "mav-external-dev-bucket" ]; then
  echo "S3 bucket already exists: mav-external-dev-bucket"
else
  awslocal s3api create-bucket --bucket mav-external-dev-bucket --region eu-north-1 \
    --create-bucket-configuration LocationConstraint=eu-north-1
  echo "S3 bucket created: mav-external-dev-bucket"
fi
