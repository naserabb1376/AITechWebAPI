namespace AITechWebAPI.ViewModels
{
    public class ParentStudentSelectionVM
    {
        public long ParentId { get; set; }
        public string ParentName { get; set; } = "";
        public string ParentContactNumber { get; set; } = "";
        public long StudentDetailsId { get; set; }
        public long StudentUserId { get; set; }
        public string StudentFullName { get; set; } = "";
        public string? IdentificationCode { get; set; }
        public string? NationalCode { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}
