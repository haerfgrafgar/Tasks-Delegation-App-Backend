using CordApp.Data;
using CordApp.Dtos.Task;
using CordApp.Interface;
using CordApp.Mappers;
using CordApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CordApp.Repository
{
    public class FractionsRepository : IFractionsRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWorkRepository _workRepo;

        public FractionsRepository(ApplicationDBContext _dBcontext, UserManager<AppUser> userManager, IWorkRepository workRepo)
        {
            _dbContext = _dBcontext;
            _userManager = userManager;
            _workRepo = workRepo;
        }

        public async Task<FractionOfTime> GetCurrentFractionByUsername(string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == username);

            return await _dbContext.FractionOfTime.FindAsync(user.FractionInProgressId);
        }

        public async Task<FractionOfTime> CreateFirstUserFraction(string appUserId)
        {
            var currentUser = await _dbContext.Users.FindAsync(appUserId);
            var fractionModel = new FractionOfTime
            {
                WorkId = 1,
                Begin = DateTime.Now,
                End = DateTime.Now,
                AppUserId = appUserId
            };

            var newFractionEntityEntry = await _dbContext.FractionOfTime.AddAsync(fractionModel);
            var newFraction = newFractionEntityEntry.Entity;

            await _dbContext.SaveChangesAsync();

            currentUser.FractionInProgressId = newFraction.Id;

            await _dbContext.SaveChangesAsync();

            await Stop(fractionModel.Id, appUserId);

            return fractionModel;
        }

        public async Task<FractionOfTime> Start(int workId, string appUserId)
        {
            var currentUser = await _dbContext.Users.FindAsync(appUserId);
            var oldFraction = await _dbContext.FractionOfTime.FindAsync(currentUser.FractionInProgressId);

            if (oldFraction != null)
            {
                if (oldFraction.End == DateTime.MinValue)
                {
                    await StopWithoutCreatingIdle(oldFraction.Id);
                }
            }
                
            var fractionModel = new FractionOfTime{
                WorkId = workId,
                Begin = DateTime.Now,
                AppUserId = appUserId
            };

            var newFractionEntityEntry = await _dbContext.FractionOfTime.AddAsync(fractionModel);
            var newFraction = newFractionEntityEntry.Entity;

            var currentWork = await _dbContext.Work.FindAsync(currentUser.WorkInProgressId);
            currentWork.InProgressNow = false;

            currentUser.WorkInProgressId = workId;

            await _dbContext.SaveChangesAsync();

            currentUser.FractionInProgressId = newFraction.Id;

            currentWork = await _dbContext.Work.FindAsync(currentUser.WorkInProgressId);
            currentWork.InProgressNow = true;

            await _dbContext.SaveChangesAsync();

            return fractionModel;
        }

        public async Task<List<FractionOfTime>> Stop(int fractionId, string appUserId)
        {
            var currentUser = await _dbContext.Users.FindAsync(appUserId);
            var todayIdleWork = await _dbContext.Work.FindAsync(currentUser.TodayIdleWorkId);

            if (todayIdleWork.CreationDate.Date != DateTime.Today)
            {
                todayIdleWork.Terminated = true;
                var newWorkDto = new CreateWorkRequestDto
                {
                    Title = "Ocioso",
                    Description = "Esta task calcula a quantidade de tempo gasta em task nenhuma.",
                    ExecUsername = currentUser.UserName,
                    DueDate = null,
                };
                var newWork = newWorkDto.ToWorkFromCreateDto();
                newWork.ExecId = appUserId;
                newWork.CordId = appUserId;

                todayIdleWork = await _workRepo.CreateAsync(newWork, currentUser.UserName);
                currentUser.TodayIdleWorkId = todayIdleWork.Id;
            }

            var fractionModel = await _dbContext.FractionOfTime.FindAsync(fractionId);
            var workModel = await _dbContext.Work.FindAsync(fractionModel.WorkId);

            fractionModel.End = DateTime.Now;

            workModel.SecondsTaken += (int)(fractionModel.End - fractionModel.Begin).TotalSeconds;

            await _dbContext.SaveChangesAsync();

            var newFraction = await Start(currentUser.TodayIdleWorkId, appUserId);

            return [fractionModel, newFraction];
        }

        public async Task<FractionOfTime> StopWithoutCreatingIdle(int id)
        {
            var fractionModel = await _dbContext.FractionOfTime.FindAsync(id);
            var workModel = await _dbContext.Work.FindAsync(fractionModel.WorkId);

            fractionModel.End = DateTime.Now;

            workModel.SecondsTaken += (int)(fractionModel.End - fractionModel.Begin).TotalSeconds;

            await _dbContext.SaveChangesAsync();

            return fractionModel;
        }

        public async Task<List<FractionOfTime>?> GetDayHistoryByUsername(string username, DateTime day)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == username);
            if (user == null)
                return null;

            var fractions = await _dbContext.FractionOfTime
                .Where(frac => frac.AppUserId == user.Id && frac.Begin.Date == day.Date)
                .OrderBy(frac => frac.Begin)
                .ToListAsync();

            if (fractions.Any())
            {
                var lastFraction = fractions.Last();
                if (lastFraction.End == DateTime.MinValue)
                    lastFraction.End = DateTime.Now;
            }

            return fractions;
        }
    }
}
