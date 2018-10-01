using CosmosManager.Domain;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IStoredProcedureExecuteContext
    {
        Task<TResult> ExecuteStoredProcedureAsync<TResult>(params dynamic[] procedureParams) where TResult : class;
        Task<TResult> ExecuteStoredProcedureAsync<TResult>(RequestOptions options, params dynamic[] procedureParams) where TResult : class;
    }
}