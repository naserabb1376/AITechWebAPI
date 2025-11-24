using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class MinutesVM : BaseVM
    {
        public string MinutesSubject { get; set; }
        public string MinutesDescription { get; set; }
        public long MeetingId { get; set; }
        public string MeetingTitle { get; set; }
        public string MeetingOrganizer { get; set; }

    }
}
