using CordApp.Data;
using CordApp.Dtos.User;
using CordApp.Interface;
using CordApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CordApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(ApplicationDBContext dBContext, UserManager<AppUser> userManager)
        {
            _dbContext = dBContext;
            _userManager = userManager;
        }
        public async Task<List<GeneralSituationResponseDto>> GetAllUsersGeneralSituation()
        {
            var users = await _userManager.Users.ToListAsync();
            var response = new List<GeneralSituationResponseDto>();

            Work thisUserWork;
            FractionOfTime thisUserFraction;
            GeneralSituationResponseDto thisUserGeneralSituation;

            foreach (var user in users)
            {
                thisUserWork = await _dbContext.Work.FindAsync(user.WorkInProgressId);
                thisUserFraction = await _dbContext.FractionOfTime.FindAsync(user.FractionInProgressId);
                var currentTime = (int)(DateTime.Now - thisUserFraction.Begin).TotalSeconds;

                thisUserGeneralSituation = new GeneralSituationResponseDto
                {
                    Username = user.UserName,
                    WorkTitle = thisUserWork.Title,
                    WorksAmount = await GetUserWorksAmount(user.Id),
                    CurrentTimeInTask = currentTime,
                    TotalTimeInTask = thisUserWork.SecondsTaken +currentTime,
                    DueDate = thisUserWork.DueDate,
                };

                response.Add(thisUserGeneralSituation);
            }

            return response;
        }

        public async Task<int> GetUserWorksAmount(string userId)
        {
            var worksAmount = await _dbContext.Work
                .Where(work => work.ExecId == userId && work.Finished == false && work.Title != "Ocioso")
                .CountAsync();

            return worksAmount;
        }
    }
}
