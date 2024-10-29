using CordApp.Dtos.Work;
using CordApp.Models;

namespace CordApp.Interface
{
    public interface IWorkRepository
    {
        //          GET
        Task<List<Work>> GetAllAsync();
        Task<List<Work>> GetAllNotFinishedStartedByExecIdAsync(string userId);
        Task<List<Work>> GetAllNotFinishedStartedByCordIdAsync(string cordId);
        Task<List<Work>?> GetAllNotFinishedStartedByUsernameAsync(string username);
        Task<List<Work>> GetAllNotFinishedStartedAndFutureByExecIdAsync(string userId);
        Task<List<Work>> GetAllNotFinishedStartedAndFutureByCordIdAsync(string cordId);
        Task<List<Work>?> GetAllNotFinishedStartedAndFutureByUsernameAsync(string username, TimeRangeDto timeRange);
        Task<List<Work>> GetAllFinishedByCordIdAsync(string cordId);
        Task<Work?> GetByIdAsync(int id);
        Task<Work?> GetSelfInProgressWorkAsync(string userId);
        Task<Work?> GetInprogressWorkByUsername(string username);

        //          POST
        Task<Work?> CreateAsync(Work task, string execUsername);

        //          PUT
        Task<Work?> FinishWork(int id, string userId);
        Task<Work?> CorrigirWork(int workId, string userId, CorrigirWorkDto corrigirWorkDto);

        //          DELETE
    }
}
