using ARConsistency.ResponseModels.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace ARConsistency.Helpers
{
    internal static class JsonHelper
    {
        internal static string ConvertToJsonString(object rawJSON)
            => JsonConvert.SerializeObject(rawJSON);

        internal static string ConvertResponseToJsonString(ConsistentApiResponse response, ResponseOptions options)
            => JsonConvert.SerializeObject(response, Formatting.Indented, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                NullValueHandling = options.IgnoreNullValue ? NullValueHandling.Ignore : NullValueHandling.Include,
                ContractResolver = options.UseCamelCaseNaming
                    ? new CamelCasePropertyNamesContractResolver()
                    : new DefaultContractResolver()
            });

        internal static ConsistentApiResponse GetConsistentApiResponseFromJsonToken(JToken jsonToken)
            => new ConsistentApiResponse(jsonToken.ToObject<object>());

    }
}
