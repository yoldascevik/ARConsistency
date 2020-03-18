using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ARConsistency.ContractResolver
{
    internal class SuppressItemTypeNameContractResolver : DefaultContractResolver
    {
        static SuppressItemTypeNameContractResolver()
            => Instance = new SuppressItemTypeNameContractResolver();

        internal static SuppressItemTypeNameContractResolver Instance { get; }

        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);
            if (contract is JsonContainerContract containerContract)
            {
                if (containerContract.ItemTypeNameHandling == null)
                    containerContract.ItemTypeNameHandling = TypeNameHandling.None;
            }
            return contract;
        }
    }
}
