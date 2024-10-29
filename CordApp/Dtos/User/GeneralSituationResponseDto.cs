namespace CordApp.Dtos.User
{
    public class GeneralSituationResponseDto
    {
        public string Username { get; set; }
        public string WorkTitle { get; set; }
        public int WorksAmount { get; set; }
        public int TotalTimeInTask { get; set; }
        public int CurrentTimeInTask { get; set; }
        public DateTime? DueDate { get; set; }

    }
}
