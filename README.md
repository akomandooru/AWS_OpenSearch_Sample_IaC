# Goal

Practice creating infrastructure as code for a manually provisioned infrastructure AWS OpenSearch service application found at  https://docs.aws.amazon.com/opensearch-service/latest/developerguide/search-example.html
This example is built using AWS OpenSearch, Lambda, API Gateway, and IAM services.


# Approach

Use AWS CDK framework/tool to write infrastructure as code in C# and deployment; use CDK to deploy from the CLI. The cloud formation template code that is produced as part of this process can be used to deploy from the AWS console too. 

## Steps
1. create a aws free tier account if you haven't yet; note that the opensearch service used in this tutorial is not available as part of the free tier and you will be charged for it usage. 
2. install and configure aws cli
3. install cdk
4. download source for this project
5. under the resources directoy, run python package manager pip to install requests, requests_aws4auth libraries
6. use cdk synth or cdk deploy to collect the cloud fomration template; cdk deploy will deploy the infrastructure to your AWS cloud account

## Note
This project is in progress - more development is needed to put the opensearch service inside a VPC (creating a VPC, and a private subnet to put the opensearch service nodes is in commented lines of code)