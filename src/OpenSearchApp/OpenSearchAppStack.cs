using Amazon.CDK;

namespace OpenSearchApp
{
    public class OpenSearchAppStack : Stack
    {
        internal OpenSearchAppStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here
            new OpenSearchService(this,"Movies");
        }
    }
}
