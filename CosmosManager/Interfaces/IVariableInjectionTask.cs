using System.Collections.Generic;

namespace CosmosManager.Interfaces
{
    public interface IVariableInjectionTask
    {
        string InjectVariables(string query, Dictionary<string, IReadOnlyCollection<object>> variables);
    }
}