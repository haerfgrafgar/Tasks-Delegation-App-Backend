using CordApp.Dtos.User;

namespace CordApp.Interface
{
    public interface IUserRepository
    {
        Task<List<GeneralSituationResponseDto>> GetAllUsersGeneralSituation();

        Task<int> GetUserWorksAmount(string userId);
    }
}
