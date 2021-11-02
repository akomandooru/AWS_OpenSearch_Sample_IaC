using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.S3;
using System.Collections.Generic;
using Amazon.CDK.AWS.OpenSearchService;
using Amazon.CDK.AWS.EC2;

namespace OpenSearchApp
{
    public class OpenSearchService : Construct
    {
        public OpenSearchService(Construct scope, string id) : base(scope, id)
        {
            /*
            //create a vpc to use for our opensearch service nodes
            var vpc = new Vpc(this,"osvpc",new VpcProps { 
                Cidr = "10.0.0.0/16",
                MaxAzs = 1,
                NatGateways = 1,
                SubnetConfiguration = new ISubnetConfiguration[]
                {
                    new SubnetConfiguration
                    {
                        CidrMask = 24,
                        SubnetType = SubnetType.PUBLIC,
                        Name = "publicsub"
                    },
                    new SubnetConfiguration
                    {
                        CidrMask = 24,
                        SubnetType = SubnetType.PRIVATE,
                        Name = "privatesub"
                    }
                }
            });
            */
            //create a new domain for movies on the opensearch service
            var domain = new Domain(this,"MoviesDomain",new DomainProps{
                DomainName = "movies",
                Capacity = new CapacityConfig{
                    DataNodeInstanceType = "t3.small.search",
                    DataNodes = 1
                },
                Version = EngineVersion.OPENSEARCH_1_0,
                //Vpc = vpc
            });
            /*
            var serviceLinkedRole = new Amazon.CDK.CfnResource(this, "os-svc-lnk-role", new  Amazon.CDK.CfnResourceProps {
                Type = "AWS::IAM::ServiceLinkedRole",
                Properties = new Dictionary<string,object>{
                    ["AWSServiceName"] = "es.amazonaws.com",
                    ["Description"] = "Role for ES to access resources in my VPC"
                }
            });
            domain.Node.AddDependency(serviceLinkedRole);
            */
            //create a new function for searching and adding movies
            var handler = new Function(this, "OpenSearchHandler", new FunctionProps
            {                
                Runtime = Runtime.PYTHON_3_9,
                Code = Code.FromAsset("resources"),
                Handler = "search.main",
                Environment = new Dictionary<string,string>{
                    ["ENDPOINT"] = domain.DomainEndpoint,
                    ["REGION"] = "us-east-2"
                }
            });
            //grant read and write for the function on the domain as the domain is not public
            domain.GrantReadWrite(handler);
            //create a new rest api and integrate 
            var api = new RestApi(this, "MoviesOpenSearch-API", new RestApiProps
            {
                RestApiName = "Open Search Movie Service",
                Description = "Movie search service."
            });

            var getMoviesIntegration = new LambdaIntegration(handler, new LambdaIntegrationOptions
            {
                RequestTemplates = new Dictionary<string, string>
                {
                    ["application/json"] = "{ \"statusCode\": \"200\" }"
                }
            });
            
            api.Root.AddMethod("GET", getMoviesIntegration);
            api.Root.AddMethod("POST", getMoviesIntegration);

        }
    }
}