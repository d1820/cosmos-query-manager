using CosmosManager.Domain;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{

    public interface ICosmosDocument
    {
        string id { get; set; }
        string PartitionKey { get; set; }
    }
}