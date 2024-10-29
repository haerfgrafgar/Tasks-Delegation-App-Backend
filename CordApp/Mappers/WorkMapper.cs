using CordApp.Dtos.Task;
using CordApp.Dtos.Work;
using CordApp.Models;
using System.Runtime.CompilerServices;

namespace CordApp.Mappers
{
    public static class WorkMapper
    {
        public static Work ToWorkFromCreateDto(this CreateWorkRequestDto WorkDto)
        {
            var brazilianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

            DateTime? dueDate = null;
            if (WorkDto.DueDate != null)
            {
                dueDate = TimeZoneInfo.ConvertTime(WorkDto.DueDate.Value, brazilianTimeZone);
            }



            return new Work
            {
                CordId = "",
                Description = WorkDto.Description,
                Title = WorkDto.Title,
                ExecId = "",
                Finished = false,
                Terminated = false,
                PreviousWorkVersionId = 0,
                WasLate = false,
                SecondsTaken = 0,
                InProgressNow = false,
                CreationDate = DateTime.Now,
                Priority = WorkDto.Priority,
                DueDate = dueDate,
                StartDate = TimeZoneInfo.ConvertTime(WorkDto.StartDate, brazilianTimeZone)
            };
        }

        public static CalendarWorkDto ToCalendarWorkDtoFromWork(this Work work)
        {
            DateTime start = work.StartDate ?? DateTime.Now;
            DateTime due = work.DueDate ?? DateTime.Now;

            return new CalendarWorkDto
            {
                event_id = work.Id,
                start = start,
                end = due,
                title = work.Title
            };
        }
    }
}
