using System;
using System.Collections.Generic;
using CosmosQueryEditor.Settings;

namespace CosmosQueryEditor.DocumentDb
{
    public class LocalEmulatorDetectorUnit
    {
        public LocalEmulatorDetectorUnit(bool isRunnng, IEnumerable<Connection> connections)
        {
            if (connections == null)
                throw new ArgumentNullException(nameof(connections));

            IsRunnng = isRunnng;
            Connections = connections;
        }

        public IEnumerable<Connection> Connections { get; }

        public bool IsRunnng { get; }
    }
}