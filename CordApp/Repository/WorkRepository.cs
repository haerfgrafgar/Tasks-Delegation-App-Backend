using CordApp.Data;
using CordApp.Dtos.Work;
using CordApp.Interface;
using CordApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CordApp.Repository
{
    public class WorkRepository : IWorkRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public WorkRepository(ApplicationDBContext dBContext, UserManager<AppUser> userManager, IServiceProvider serviceProvider)
        {
            _dbContext = dBContext;
            _userManager = userManager;
            _serviceProvider = serviceProvider;
        }

        //--------------------------------------------------- GET ---------------------------------------------------//

        public async Task<List<Work>> GetAllAsync()
        {
            return await _dbContext.Work.ToListAsync();
        }

        public async Task<List<Work>> GetAllFinishedByCordIdAsync(string cordId)
        {
            return await _dbContext.Work
                .Where(work => work.CordId == cordId &&
                work.Finished && 
                !work.Terminated && 
                work.Title != "Ocioso").ToListAsync();
        }

        public async Task<List<Work>> GetAllNotFinishedStartedByExecIdAsync(string userId)
        {
            return await _dbContext.Work
                .Where(work => work.ExecId == userId &&
                !work.Finished &&
                work.StartDate <= DateTime.Now &&
                work.Title != "Ocioso").ToListAsync();
        }

        public async Task<Work?> GetByIdAsync(int id)
        {
            return await _dbContext.Work.FindAsync(id);
        }

        public async Task<Work?> GetSelfInProgressWorkAsync(string userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return null;

            return await GetByIdAsync(user.WorkInProgressId);
        }

        public async Task<Work?> GetInprogressWorkByUsername(string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == username);

            if (user == null)
                return null;

            return await GetByIdAsync(user.WorkInProgressId);
        }

        public async Task<List<Work>?> GetAllNotFinishedStartedByUsernameAsync(string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == username);
            if (user == null)
                return null;

            

            return await _dbContext.Work.Where(work => work.ExecId == user.Id
                && !work.Finished
                && work.Title != "Ocioso")
                .ToListAsync();
        }

        public async Task<List<Work>> GetAllNotFinishedStartedByCordIdAsync(string cordId)
        {
            return await _dbContext.Work
                .Where(work => work.CordId == cordId &&
                !work.Finished &&
                work.StartDate <= DateTime.Now &&
                work.Title != "Ocioso").ToListAsync();
        }

        public async Task<List<Work>> GetAllNotFinishedStartedAndFutureByExecIdAsync(string userId)
        {
            return await _dbContext.Work
                .Where(work => work.ExecId == userId &&
                !work.Finished &&
                work.Title != "Ocioso").ToListAsync();
        }

        public async Task<List<Work>> GetAllNotFinishedStartedAndFutureByCordIdAsync(string cordId)
        {
            return await _dbContext.Work
                .Where(work => work.CordId == cordId &&
                !work.Finished &&
                work.Title != "Ocioso").ToListAsync();
        }

        public async Task<List<Work>?> GetAllNotFinishedStartedAndFutureByUsernameAsync(string username, TimeRangeDto timeRange)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == username);

            if (user == null)
                return null;

            return await _dbContext.Work.Where(work => work.ExecId == user.Id
                && !work.Finished
                && work.StartDate >= timeRange.dateStart.Date
                && work.StartDate <= timeRange.dateEnd.Date
                && work.Title != "Ocioso")
                .ToListAsync();
        }

        //--------------------------------------------------- POST ---------------------------------------------------//

        public async Task<Work?> CreateAsync(Work work, string execUsername)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == execUsername);
            if (user == null)
                return null;

            work.ExecId = user.Id;

            await _dbContext.Work.AddAsync(work);
            await _dbContext.SaveChangesAsync();

            return work;
        }

        //--------------------------------------------------- PUT ---------------------------------------------------//

        public async Task<Work?> FinishWork(int id, string userId)
        {
            var work = await _dbContext.Work.FindAsync(id);

            if (work == null)
                return null;

            if (work.ExecId == userId || work.CordId == userId)
                work.Finished = true;

            if (work.InProgressNow)
            {
                var user = await _dbContext.Users.FindAsync(userId);
                //Should make an actual solution later, but honestly i won't.
                var _fractionsRepo = _serviceProvider.GetRequiredService<IFractionsRepository>();
                await _fractionsRepo.Stop(user.FractionInProgressId, userId);
            }

            if (work.DueDate != DateTime.MinValue)
                if (work.DueDate < DateTime.Now)
                    work.WasLate = true;

            await _dbContext.SaveChangesAsync();
            return work;
        }

        public async Task<Work?> CorrigirWork(int workId, string userId, CorrigirWorkDto corrigirWorkDto)
        {
            var work = await _dbContext.Work.FindAsync(workId);

            if (work == null)
                return null;

            if (work.CordId != userId || !work.Finished || work.Terminated)
                return null;

            work.Terminated = true;
            work.Approved = corrigirWorkDto.Aprovado;
            work.CordResponse = corrigirWorkDto.Motivo;

            await _dbContext.SaveChangesAsync();
            return work;
        }

        

        //--------------------------------------------------- DELETE --------------------------------------------------//




    }
}
