using CordApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CordApp.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<Work> Work { get; set; }
        public DbSet<FractionOfTime> FractionOfTime { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Lots of writes on this table, index may not be a good ideia
            //builder.Entity<FractionOfTime>().HasIndex(f => f.AppUserId);

            builder.Entity<Work>().HasIndex(w => w.ExecId);
            builder.Entity<Work>().HasIndex(w => w.CordId);

            builder.Entity<Work>().HasData(
                    new Work
                    {
                        Id = 1,
                        Approved = false,
                        CordId = "",
                        CreationDate = DateTime.MinValue,
                        Title = "Matriz",
                        DeletedByExec = null,
                        Description = "Matriz",
                        DueDate = DateTime.MinValue,
                        ExecId = "",
                        ExecResponse = "",
                        Finished = false,
                        InProgressNow = false,
                        PreviousWorkVersionId = 1,
                        Priority = 1,
                        SecondsTaken = 1,
                        StartDate = DateTime.MinValue,
                        Terminated = true,
                        WasLate = false,
                    }
                );

            List <IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Projetista",
                    NormalizedName = "PROJETISTA"
                },
                new IdentityRole
                {
                    Name = "Desenhista",
                    NormalizedName = "DESENHISTA"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
