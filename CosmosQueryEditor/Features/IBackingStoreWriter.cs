using Newtonsoft.Json.Linq;

namespace CosmosQueryEditor.Features
{
    public interface IBackingStoreWriter
    {
        void WriteToBackingStore(object tabContentViewModel, JToken into);
    }
}