using System;

namespace CosmosQueryEditor.Settings
{
    public interface IQueryFileService
    {
        string GetFileName(Guid fileId);
    }
}