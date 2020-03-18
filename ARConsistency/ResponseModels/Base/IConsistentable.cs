using ARConsistency.ResponseModels.Base;

namespace ARConsistency.ResponseModels.Base
{
    internal interface IConsistentable
    {
        ConsistentApiResponse GetConsistentApiResponse();
    }
}
