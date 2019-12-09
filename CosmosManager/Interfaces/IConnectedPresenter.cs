using CosmosManager.Domain;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IConnectedPresenter
    {
        Connection SelectedConnection { get; set; }

        void SetConnections(List<Connection> connections);
    }
}