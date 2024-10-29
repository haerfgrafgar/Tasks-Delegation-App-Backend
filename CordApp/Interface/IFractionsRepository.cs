using CordApp.Models;

namespace CordApp.Interface
{
    public interface IFractionsRepository
    {
        //          GET
        Task<FractionOfTime> GetCurrentFractionByUsername(string username);
        Task<List<FractionOfTime>?> GetDayHistoryByUsername(string username, DateTime day);

        //          POST
        Task<FractionOfTime> Start(int workId, string appUserId);
        Task<FractionOfTime> CreateFirstUserFraction(string appUserId);

        //          PUT
        Task<List<FractionOfTime>> Stop(int id, string appUserId);
        Task<FractionOfTime> StopWithoutCreatingIdle(int id);

        //          DELETE
    }
}
