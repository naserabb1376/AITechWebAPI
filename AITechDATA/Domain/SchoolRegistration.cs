namespace AITechDATA.Domain
{
    public class SchoolRegistration
    {
        public long ID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; } = true;

        public long UserId { get; set; }
        public User User { get; set; }

        public long StudentDetailsId { get; set; }
        public StudentDetails StudentDetails { get; set; }

        public long FatherParentId { get; set; }
        public Parent FatherParent { get; set; }

        public long MotherParentId { get; set; }
        public Parent MotherParent { get; set; }

        public string TenantKey { get; set; }
        public string FormType { get; set; }
        public string CurrentSchoolName { get; set; }
        public string TargetGrade { get; set; }
        public int? SeatNumber { get; set; }
        public string RegistrationStatus { get; set; } = "Submitted";
    }
}
